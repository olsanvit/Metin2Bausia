// ============================================================
// Round-trip test: herní soubory → řádky DB → export → porovnání s originálem.
//
// Proč: nasazení špatně poskládaného proto souboru rozbije start game serveru.
// Test ověřuje celý řetěz importér + exportér na reálných datech, ne na vzorku.
//
// Herní data jsou v .gitignore (proprietární), takže bez nich se test přeskočí.
// Spuštění: node --test tools/gamefiles-import/roundtrip.test.mjs
// ============================================================

import { test } from "node:test";
import assert from "node:assert/strict";
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import {
  ITEM_COLUMNS, MOB_COLUMNS, parseProto, parseNamesRows, parseGroupFile,
  attachMobDrops, toContentGroups, dedupeFirstWins, parseMaps, readMapAllow, parseQuests,
} from "./parsers.mjs";
import { buildItemProtoLine, buildMobProtoLine, buildNamesFile, ITEM_PROTO_HEADER, MOB_PROTO_HEADER } from "../../mcp/proto-format.js";

const repoRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "../..");
const conf = path.join(repoRoot, "server/src/gamefiles/conf");
const data = path.join(repoRoot, "server/src/gamefiles/data");
const skip = !fs.existsSync(path.join(conf, "item_proto.txt")) && "herní data nejsou k dispozici";

/** Napodobí uložení do Postgresu a zpětné načtení přes node-pg (jsonb → objekt). */
const viaDb = (row) => JSON.parse(JSON.stringify(row));

/** Prázdný sloupec a "0" server čte stejně (atoi) — referenční soubor v tom sám není konzistentní. */
const same = (a, b) => (a === "" ? "0" : a) === (b === "" ? "0" : b);

function compareProto(file, columns, build, header) {
  const original = fs.readFileSync(path.join(conf, file), "latin1").replace(/\r/g, "").split("\n").filter((l) => l !== "");
  assert.equal(original[0].split("\t").length, header.length, "počet sloupců hlavičky");
  const rows = parseProto(path.join(conf, file), columns).map(viaDb);
  assert.equal(rows.length, original.length - 1, "počet řádků");

  const diffs = {};
  rows.forEach((row, i) => {
    const out = build(row).split("\t");
    const src = original[i + 1].split("\t");
    assert.equal(out.length, src.length, `počet sloupců, vnum ${src[0]}`);
    out.forEach((v, c) => { if (!same(v, src[c])) diffs[header[c]] = (diffs[header[c]] ?? 0) + 1; });
  });
  assert.deepEqual(diffs, {}, `rozdíly po sloupcích v ${file}`);
  return rows;
}

test("item_proto.txt projde importem a exportem beze změny", { skip }, () => {
  const rows = compareProto("item_proto.txt", ITEM_COLUMNS, buildItemProtoLine, ITEM_PROTO_HEADER);
  const vnums = rows.map((r) => r.Vnum);
  assert.equal(new Set(vnums).size, vnums.length, "Vnum musí být jedinečný (UNIQUE v DB)");
});

test("mob_proto.txt projde importem a exportem beze změny", { skip }, () => {
  const rows = compareProto("mob_proto.txt", MOB_COLUMNS, buildMobProtoLine, MOB_PROTO_HEADER);
  const vnums = rows.map((r) => r.Vnum);
  assert.equal(new Set(vnums).size, vnums.length, "Vnum musí být jedinečný (UNIQUE v DB)");
});

for (const file of ["item_names_cz.txt", "mob_names_cz.txt"]) {
  test(`${file} se vyexportuje bajt po bajtu (CP1250)`, { skip }, () => {
    const original = fs.readFileSync(path.join(conf, file));
    const rebuilt = buildNamesFile(parseNamesRows(path.join(conf, file)).map(viaDb));
    assert.ok(rebuilt.equals(original), `${file}: ${rebuilt.length} vs ${original.length} bajtů`);
  });
}

test("české názvy se napárují na itemy a moby", { skip }, () => {
  const items = parseProto(path.join(conf, "item_proto.txt"), ITEM_COLUMNS, path.join(conf, "item_names_cz.txt"));
  const mobs = parseProto(path.join(conf, "mob_proto.txt"), MOB_COLUMNS, path.join(conf, "mob_names_cz.txt"));
  assert.equal(items.find((r) => r.Vnum === 10).LocaleName, "Meč+0");
  assert.equal(mobs.find((r) => r.Vnum === 101).LocaleName, "Divoký pes");
  assert.ok(items.filter((r) => r.LocaleName).length / items.length > 0.95, "aspoň 95 % itemů má český název");
});

test("dropy se připojí k existujícím mobům", { skip }, () => {
  const items = parseProto(path.join(conf, "item_proto.txt"), ITEM_COLUMNS);
  const mobs = parseProto(path.join(conf, "mob_proto.txt"), MOB_COLUMNS);
  const groups = parseGroupFile(path.join(data, "mob_drop_item.txt"));
  const { unresolvedNames } = attachMobDrops(mobs, groups, items);
  // Typy kill/limit odkazují jménem; nedohledané jméno server taky ignoruje, ale mělo by jich být málo
  assert.ok(unresolvedNames.length <= 10, `nedohledaná jména itemů: ${unresolvedNames.join(", ")}`);
  const withDrops = mobs.filter((m) => m.DropItems.length);
  assert.ok(withDrops.length > 300, `mobů s dropy: ${withDrops.length}`);
  for (const m of withDrops) for (const d of m.DropItems) for (const it of d.items) {
    assert.ok((Number.isInteger(it.vnum) || typeof it.itemName === "string") && Number.isFinite(it.pct), `drop moba ${m.Vnum}: ${JSON.stringify(it)}`);
  }
});

test("mapy, questy a skupiny mají jedinečné klíče pro upsert", { skip }, () => {
  const allow = readMapAllow([path.join(repoRoot, "server/docker/docker-compose.yml"), path.join(repoRoot, "server/game/conf.cfg")]);
  assert.ok(allow.size >= 40, `map v GAME_MAP_ALLOW napříč jádry: ${allow.size}`);
  const maps = parseMaps(path.join(data, "map"), allow);
  assert.equal(new Set(maps.map((m) => m.MapIndex)).size, maps.length, "MapIndex");
  assert.ok(maps.some((m) => m.IsEnabled), "aspoň jedna mapa je v MAP_ALLOW");
  assert.ok(maps.reduce((a, m) => a + m.SpawnMobs.length, 0) > 10000, "spawny");

  const quests = parseQuests(path.join(data, "quest"));
  assert.equal(new Set(quests.map((x) => x.FileName)).size, quests.length, "Quests.FileName");

  const { rows: groups, duplicates } = dedupeFirstWins([
    ...toContentGroups("mob_group", parseGroupFile(path.join(data, "group.txt"))),
    ...toContentGroups("mob_group_group", parseGroupFile(path.join(data, "group_group.txt"))),
    ...toContentGroups("special_item_group", parseGroupFile(path.join(data, "special_item_group.txt"))),
  ]);
  // Server bere první výskyt (std::map::insert) — tyhle čtyři duplicity jsou ve zdrojových datech
  assert.deepEqual(duplicates.sort(), ["mob_group:2409", "mob_group:2506", "mob_group_group:2706", "special_item_group:10003"]);
  assert.equal(groups.find((g) => g.GroupType === "special_item_group" && g.GroupVnum === 10003).Entries[0][1], "72007", "platí první výskyt");
  const keys = groups.map((g) => `${g.GroupType}:${g.GroupVnum}`);
  const dup = keys.filter((k, i) => keys.indexOf(k) !== i);
  assert.deepEqual([...new Set(dup)], [], "ContentGroups (GroupType, GroupVnum)");
});
