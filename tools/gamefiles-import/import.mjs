#!/usr/bin/env node
// ============================================================
// Import herních dat TMP4 do Metin2Bausia DB — generuje SQL soubory.
//
// Proč SQL soubory místo přímého zápisu: produkční DB (mt2-postgres na QNAPu)
// je z Macu dostupná jen přes SSH, a výstup se tak dá zkontrolovat a pustit
// v jedné transakci (psql --single-transaction), případně nanečisto s ROLLBACK.
//
// Výstup obsahuje proprietární herní data → jde do database/import/generated/,
// který je v .gitignore. Repo je veřejné, vygenerované SQL se NESMÍ commitnout.
//
// Použití:
//   node tools/gamefiles-import/import.mjs [--gamefiles <dir>] [--out <dir>] [--locale cz]
// ============================================================

import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import {
  ITEM_COLUMNS, MOB_COLUMNS, parseProto, parseGroupFile, attachMobDrops,
  toContentGroups, dedupeFirstWins, parseMaps, readMapAllow, parseQuests, mapLevelRange,
} from "./parsers.mjs";

const repoRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "../..");

function arg(name, fallback) {
  const i = process.argv.indexOf(`--${name}`);
  return i > 0 && process.argv[i + 1] ? process.argv[i + 1] : fallback;
}

const gamefiles = path.resolve(arg("gamefiles", path.join(repoRoot, "server/src/gamefiles")));
const outDir = path.resolve(arg("out", path.join(repoRoot, "database/import/generated")));
const locale = arg("locale", "cz");
const conf = path.join(gamefiles, "conf");
const data = path.join(gamefiles, "data");

// ── SQL pomocníci ─────────────────────────────────────────────────────────────

const q = (ident) => `"${ident}"`;
const NOW = { __sql: "now()" };

/** Literál pro Postgres (standard_conforming_strings = on → zpětné lomítko je obyčejný znak). */
function lit(v) {
  if (v === null || v === undefined) return "NULL";
  if (typeof v === "object" && v.__sql) return v.__sql;
  if (typeof v === "number") {
    if (!Number.isFinite(v)) throw new Error(`Neplatné číslo: ${v}`);
    return String(v);
  }
  if (typeof v === "boolean") return v ? "TRUE" : "FALSE";
  const text = typeof v === "object" ? JSON.stringify(v) : String(v);
  const escaped = text.replace(/\0/g, "").replace(/'/g, "''");
  return typeof v === "object" ? `'${escaped}'::jsonb` : `'${escaped}'`;
}

/**
 * Upsert po dávkách. Při opakovaném importu se přepisují jen řádky, které
 * pocházejí z importu (DataOrigin='imported') — ruční úpravy a obsah od agenta
 * zůstanou. Stav schválení se nepřepisuje nikdy; zapnutí (IsEnabled) se převezme
 * ze serverových souborů, pokud ho admin ručně nepřepnul (IsEnabledLocked).
 */
function upsert(table, keyCols, rows, protectedCols) {
  if (rows.length === 0) return `-- ${table}: žádné řádky\n`;
  const cols = [...new Set(rows.flatMap((r) => Object.keys(r)))];
  const updateCols = cols.filter((c) => !keyCols.includes(c) && !protectedCols.includes(c));
  const chunks = [];
  for (let i = 0; i < rows.length; i += 250) {
    const values = rows.slice(i, i + 250)
      .map((r) => `(${cols.map((c) => lit(r[c])).join(", ")})`)
      .join(",\n");
    chunks.push(
      `INSERT INTO ${q(table)} (${cols.map(q).join(", ")}) VALUES\n${values}\n` +
      `ON CONFLICT (${keyCols.map(q).join(", ")}) DO UPDATE SET\n  ` +
      updateCols.map((c) => c === "IsEnabled"
        ? `"IsEnabled" = CASE WHEN ${q(table)}."IsEnabledLocked" THEN ${q(table)}."IsEnabled" ELSE EXCLUDED."IsEnabled" END`
        : `${q(c)} = EXCLUDED.${q(c)}`).join(",\n  ") +
      `,\n  "UpdatedAt" = now()\nWHERE ${q(table)}."DataOrigin" = 'imported';`,
    );
  }
  return chunks.join("\n\n") + "\n";
}

// Pole společná všem schvalovaným entitám (Items/Mobs/Maps/Quests)
const approvedFields = (isEnabled = true) => ({
  ContentStatus: "approved",
  IsEnabled: isEnabled,
  DataOrigin: "imported",
  SourceName: "TMP4 gamefiles",
  ApprovedAt: NOW,
  ApprovedBy: "import",
  ConfidenceScore: 100,
  FinalScore: 100,
});
const PROTECTED = ["ContentStatus", "ApprovedAt", "ApprovedBy"];

function writeSql(file, title, body) {
  const header = `-- ${title}\n-- Vygenerováno ${new Date().toISOString()} z ${path.relative(repoRoot, gamefiles)}\n` +
    `-- Obsahuje proprietární herní data — NECOMMITOVAT.\n\n`;
  fs.writeFileSync(path.join(outDir, file), header + body);
}

// ── Import ────────────────────────────────────────────────────────────────────

fs.mkdirSync(outDir, { recursive: true });
const summary = {};

const items = parseProto(path.join(conf, "item_proto.txt"), ITEM_COLUMNS, path.join(conf, `item_names_${locale}.txt`));
writeSql("01_items.sql", "Items z item_proto.txt", upsert("Items", ["Vnum"], items.map((r) => ({ ...r, ...approvedFields() })), PROTECTED));
summary.items = { rows: items.length, withLocaleName: items.filter((r) => r.LocaleName).length };

const mobs = parseProto(path.join(conf, "mob_proto.txt"), MOB_COLUMNS, path.join(conf, `mob_names_${locale}.txt`));
const drops = attachMobDrops(mobs, parseGroupFile(path.join(data, "mob_drop_item.txt")), items);
writeSql("02_mobs.sql", "Mobs z mob_proto.txt + mob_drop_item.txt", upsert("Mobs", ["Vnum"], mobs.map((r) => ({ ...r, ...approvedFields() })), PROTECTED));
summary.mobs = {
  rows: mobs.length,
  withLocaleName: mobs.filter((r) => r.LocaleName).length,
  withDrops: mobs.filter((r) => r.DropItems.length).length,
  dropGroupsWithoutMob: drops.orphans,
  dropItemNamesUnresolved: drops.unresolvedNames.length,
};

const mapAllow = readMapAllow([
  path.join(repoRoot, "server/docker/docker-compose.yml"),
  path.join(repoRoot, "server/game/conf.cfg"),
]);
const mobLevels = new Map(mobs.filter((m) => m.MobType === "MONSTER" || m.MobType === "STONE").map((m) => [m.Vnum, m.Level]));
const mobGroups = parseGroupFile(path.join(data, "group.txt"));
const mobGroupGroups = parseGroupFile(path.join(data, "group_group.txt"));
const mapNames = JSON.parse(fs.readFileSync(new URL("./map-names-cz.json", import.meta.url), "utf8"));
const maps = parseMaps(path.join(data, "map"), mapAllow).map((m) => {
  const [localeName, verified] = mapNames[m.MapIndex] ?? [null, false];
  return {
    ...m, ...mapLevelRange(m.SpawnMobs, mobLevels, mobGroups, mobGroupGroups),
    LocaleName: localeName,
    Metadata: { ...m.Metadata, localeNameVerified: localeName ? verified : null },
  };
});
writeSql("03_maps.sql", "Maps z data/map", upsert("Maps", ["MapIndex"], maps.map((r) => ({ ...r, ...approvedFields(r.IsEnabled) })), PROTECTED));
summary.maps = { rows: maps.length, mapAllowZdroj: mapAllow.size ? "compose/conf" : "prázdné", enabled: maps.filter((m) => m.IsEnabled).length, spawns: maps.reduce((a, m) => a + m.SpawnMobs.length, 0), withLevels: maps.filter((m) => m.MaxLevel > 0).length, withLocaleName: maps.filter((m) => m.LocaleName).length };

const quests = parseQuests(path.join(data, "quest"));
writeSql("04_quests.sql", "Quests z data/quest", upsert("Quests", ["FileName"], quests.map((r) => ({ ...r, ...approvedFields(r.IsEnabled) })), PROTECTED));
summary.quests = { rows: quests.length, enabled: quests.filter((x) => x.IsEnabled).length };

// Duplicity ve zdrojových souborech: platí první výskyt, stejně jako na serveru (viz dedupeFirstWins)
const { rows: groupRows, duplicates: groupDuplicates } = dedupeFirstWins([
  ...toContentGroups("mob_group", parseGroupFile(path.join(data, "group.txt")), "group.txt"),
  ...toContentGroups("mob_group_group", parseGroupFile(path.join(data, "group_group.txt")), "group_group.txt"),
  ...toContentGroups("special_item_group", parseGroupFile(path.join(data, "special_item_group.txt")), "special_item_group.txt"),
]);
const groups = groupRows.map((g) => ({ ...g, ContentStatus: "approved", IsEnabled: true, DataOrigin: "imported" }));
writeSql("05_groups.sql", "ContentGroups z group/group_group/special_item_group", upsert("ContentGroups", ["GroupType", "GroupVnum"], groups, ["ContentStatus"]));
summary.groups = {
  ...Object.fromEntries(["mob_group", "mob_group_group", "special_item_group"].map((t) => [t, groups.filter((g) => g.GroupType === t).length])),
  duplicatesIgnored: groupDuplicates,
};

writeSql("99_verify.sql", "Kontrolní počty po importu",
  `SELECT 'Items' AS tabulka, count(*) FROM "Items" WHERE "DataOrigin" = 'imported'\n` +
  `UNION ALL SELECT 'Mobs', count(*) FROM "Mobs" WHERE "DataOrigin" = 'imported'\n` +
  `UNION ALL SELECT 'Maps', count(*) FROM "Maps" WHERE "DataOrigin" = 'imported'\n` +
  `UNION ALL SELECT 'Quests', count(*) FROM "Quests" WHERE "DataOrigin" = 'imported'\n` +
  `UNION ALL SELECT 'ContentGroups', count(*) FROM "ContentGroups" WHERE "DataOrigin" = 'imported';\n`);

fs.writeFileSync(path.join(outDir, "summary.json"), JSON.stringify(summary, null, 2) + "\n");
console.log(JSON.stringify(summary, null, 2));
for (const f of fs.readdirSync(outDir).sort()) {
  console.log(`  ${f.padEnd(16)} ${(fs.statSync(path.join(outDir, f)).size / 1024 / 1024).toFixed(2)} MB`);
}
