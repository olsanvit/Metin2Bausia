// ============================================================
// Parsery herních souborů TMP4 → řádky ve tvaru tabulek Metin2Bausia DB.
//
// Proč čisté funkce bez DB: generátor SQL i round-trip test musí pracovat
// s úplně stejnou podobou dat, jakou uloží Postgres. Kdyby měl test vlastní
// parsování, ověřoval by něco jiného, než co se reálně importuje.
//
// Kódování souborů (ověřeno na server/src/gamefiles):
//   item_proto.txt / mob_proto.txt — NAME v EUC-KR, zbytek ASCII
//   item_names_cz.txt / mob_names_cz.txt — CP1250
//   *.quest — část UTF-8, část EUC-KR (korejské komentáře)
// ============================================================

import fs from "fs";
import path from "path";

const krDecoder = new TextDecoder("euc-kr");
const cp1250Decoder = new TextDecoder("windows-1250");
const utf8Strict = new TextDecoder("utf-8", { fatal: true });

const readLatin1 = (p) => fs.readFileSync(p, "latin1");
const splitLines = (text) => text.replace(/\r/g, "").split("\n");

/** Latin1 řetězec s původními bajty → čitelný text; ASCII projde beze změny. */
export function decodeKr(latin1) {
  return /[\x80-\xff]/.test(latin1) ? krDecoder.decode(Buffer.from(latin1, "latin1")) : latin1;
}

/** NormalizedName: malá písmena bez diakritiky — stejné pravidlo, jaké má agent v promptu. */
export function normalizeName(text) {
  return String(text ?? "").normalize("NFD").replace(/[̀-ͯ]/g, "").toLowerCase().trim() || null;
}

function toInt(raw, column, vnum) {
  if (raw === "") return 0;   // prázdný sloupec server čte jako 0 (atoi)
  const n = Number.parseInt(raw, 10);
  if (!Number.isFinite(n) || String(n) !== raw.replace(/^\+/, "").replace(/^(-?)0+(?=\d)/, "$1")) {
    throw new Error(`Nečíselná hodnota "${raw}" ve sloupci ${column} (vnum ${vnum})`);
  }
  return n;
}

// ── Sloupce proto souborů → sloupce DB (pořadí = pořadí v souboru) ────────────
// Typ: vnum | name | int | numeric | text

export const ITEM_COLUMNS = [
  ["Vnum", "vnum"], ["Name", "name"], ["ItemType", "text"], ["SubType", "text"], ["Size", "int"],
  ["AntiFlags", "text"], ["Flags", "text"], ["WearFlags", "text"], ["ImmuneFlags", "text"],
  ["Gold", "int"], ["Buy", "int"], ["Refine", "int"], ["RefineSet", "int"], ["MagicPct", "int"],
  ["LimitType0", "text"], ["LimitValue0", "int"], ["LimitType1", "text"], ["LimitValue1", "int"],
  ["ApplyType0", "text"], ["ApplyValue0", "int"], ["ApplyType1", "text"], ["ApplyValue1", "int"],
  ["ApplyType2", "text"], ["ApplyValue2", "int"],
  ["Value0", "int"], ["Value1", "int"], ["Value2", "int"], ["Value3", "int"], ["Value4", "int"], ["Value5", "int"],
  ["Specular", "int"], ["Socket", "int"], ["AttuAddon", "int"],
];

export const MOB_COLUMNS = [
  ["Vnum", "vnum"], ["Name", "name"], ["Rank", "text"], ["MobType", "text"], ["BattleType", "text"],
  ["Level", "int"], ["Size", "int"], ["AiFlag", "text"], ["MountCapacity", "int"],
  ["RaceFlag", "text"], ["ImmuneFlags", "text"], ["Empire", "int"], ["Folder", "text"], ["OnClick", "int"],
  ["St", "int"], ["Dx", "int"], ["Ht", "int"], ["Iq", "int"], ["DamageMin", "int"], ["DamageMax", "int"],
  ["MaxHp", "int"], ["RegenCycle", "int"], ["RegenPercent", "int"], ["GoldMin", "int"], ["GoldMax", "int"],
  ["Exp", "int"], ["Def", "int"], ["AttackSpeed", "int"], ["MoveSpeed", "int"],
  ["AggressiveHpPct", "int"], ["AggressiveSight", "int"], ["AttackRange", "int"],
  ["DropItemVnum", "int"], ["ResurrectionVnum", "int"],
  ["EnchantCurse", "int"], ["EnchantSlow", "int"], ["EnchantPoison", "int"],
  ["EnchantStun", "int"], ["EnchantCritical", "int"], ["EnchantPenetrate", "int"],
  ["ResistSword", "int"], ["ResistTwohand", "int"], ["ResistDagger", "int"], ["ResistBell", "int"],
  ["ResistFan", "int"], ["ResistBow", "int"], ["ResistFire", "int"], ["ResistElect", "int"],
  ["ResistMagic", "int"], ["ResistWind", "int"], ["ResistPoison", "int"],
  ["DamMultiply", "numeric"], ["Summon", "int"], ["DrainSp", "int"], ["MobColor", "int"], ["PolymorphItem", "int"],
  ["SkillLevel0", "int"], ["SkillVnum0", "int"], ["SkillLevel1", "int"], ["SkillVnum1", "int"],
  ["SkillLevel2", "int"], ["SkillVnum2", "int"], ["SkillLevel3", "int"], ["SkillVnum3", "int"],
  ["SkillLevel4", "int"], ["SkillVnum4", "int"],
  ["SpBerserk", "int"], ["SpStoneskin", "int"], ["SpGodspeed", "int"], ["SpDeathblow", "int"], ["SpRevive", "int"],
];

// ── Názvy (VNUM<TAB>LOCALE_NAME, CP1250) ──────────────────────────────────────

/** Řádky souboru názvů v původním pořadí — slouží i pro round-trip test exportu. */
export function parseNamesRows(file) {
  const text = cp1250Decoder.decode(fs.readFileSync(file));
  const rows = [];
  for (const line of splitLines(text).slice(1)) {
    if (line === "") continue;
    const tab = line.indexOf("\t");
    const key = tab < 0 ? line : line.slice(0, tab);
    const name = tab < 0 ? "" : line.slice(tab + 1);
    const row = { LocaleName: name, Metadata: {} };
    if (key.includes("~")) { row.Vnum = Number.parseInt(key, 10); row.Metadata.vnumRange = key; }
    else row.Vnum = toInt(key, "VNUM", key);
    rows.push(row);
  }
  return rows;
}

function namesMap(file) {
  const map = new Map();
  if (!file || !fs.existsSync(file)) return map;
  for (const r of parseNamesRows(file)) map.set(r.Metadata.vnumRange ?? String(r.Vnum), r.LocaleName);
  return map;
}

// ── item_proto.txt / mob_proto.txt ────────────────────────────────────────────

/**
 * Proto soubor → řádky DB. Původní bajty jména a rozsah vnum jdou do Metadata,
 * aby export (mcp/proto-format.js) vrátil soubor v původní podobě.
 */
export function parseProto(file, columns, namesFile) {
  const names = namesMap(namesFile);
  const lines = splitLines(readLatin1(file));
  const rows = [];
  for (const line of lines.slice(1)) {
    if (!line.trim()) continue;
    const f = line.split("\t");
    if (f.length !== columns.length) {
      throw new Error(`${path.basename(file)}: řádek vnum ${f[0]} má ${f.length} sloupců místo ${columns.length}`);
    }
    const row = { Metadata: { source: path.basename(file) } };
    columns.forEach(([col, type], i) => {
      const raw = f[i];
      switch (type) {
        case "vnum":
          if (raw.includes("~")) { row.Vnum = Number.parseInt(raw, 10); row.Metadata.vnumRange = raw; }
          else row.Vnum = toInt(raw, col, raw);
          break;
        case "name":
          row.Name = decodeKr(raw);
          row.Metadata.protoName = raw;
          break;
        case "int":
          row[col] = toInt(raw, col, f[0]);
          break;
        case "numeric":
          row[col] = raw === "" ? null : Number(raw);
          break;
        default:
          row[col] = raw === "" ? null : raw;
      }
    });
    row.LocaleName = names.get(row.Metadata.vnumRange ?? String(row.Vnum)) ?? null;
    row.NormalizedName = normalizeName(row.LocaleName ?? row.Name);
    rows.push(row);
  }
  return rows;
}

// ── Soubory se skupinami (Group jméno { klíč hodnota … řádky }) ────────────────

/** Obecný parser pro mob_drop_item, special_item_group, group, group_group. */
export function parseGroupFile(file) {
  const out = [];
  let cur = null;
  for (const rawLine of splitLines(readLatin1(file))) {
    const s = rawLine.trim();
    if (!s || s.startsWith("//")) continue;
    if (/^group\b/i.test(s)) {
      cur = { name: decodeKr(s.replace(/^group\s*/i, "")), kv: {}, rows: [] };
      out.push(cur);
      continue;
    }
    if (!cur || s === "{" || s === "}") continue;
    const parts = s.split(/\s+/);
    // Klíče jsou v souborech nekonzistentně velkými i malými písmeny (Mob / mob)
    if (/^\d+$/.test(parts[0])) cur.rows.push(parts.map(decodeKr));
    else cur.kv[parts[0].toLowerCase()] = parts.slice(1).map(decodeKr);
  }
  return out;
}

/**
 * Dropy z mob_drop_item.txt připojí k mobům jako DropItems (jsonb).
 *
 * Typy kill a limit neodkazují na item vnumem, ale interním jménem z item_proto
 * (NAME, např. "혈검+4") — server si ho dohledá sám. Jméno se proto zachová
 * a vnum se dohledá podle importovaných itemů, aby admin ukázal odkaz na item.
 *
 * Vrací { orphans: skupiny bez moba v mob_proto, unresolvedNames: jména bez itemu }.
 */
export function attachMobDrops(mobs, dropGroups, items = []) {
  const byVnum = new Map(mobs.map((m) => [m.Vnum, m]));
  const vnumByName = new Map();
  // Stejné jméno může mít víc itemů (varianty) — server bere první nalezený, stejně tak tady
  for (const it of items) if (it.Name && !vnumByName.has(it.Name)) vnumByName.set(it.Name, it.Vnum);

  let orphans = 0;
  const unresolvedNames = new Set();
  for (const g of dropGroups) {
    const mobVnum = Number.parseInt(g.kv.mob?.[0] ?? "", 10);
    const mob = byVnum.get(mobVnum);
    if (!mob) { orphans++; continue; }
    mob.DropItems ??= [];
    mob.DropItems.push({
      group: g.name,
      type: (g.kv.type?.[0] ?? "drop").toLowerCase(),
      killDrop: g.kv.kill_drop?.[0] ?? null,
      levelLimit: g.kv.level_limit?.[0] ?? null,
      // Řádek: pořadí, item (vnum nebo interní jméno), počet, šance (%), u některých typů další sloupec
      items: g.rows.map((r) => {
        const byNumber = /^\d+$/.test(r[1] ?? "");
        const vnum = byNumber ? Number.parseInt(r[1], 10) : (vnumByName.get(r[1]) ?? null);
        if (!byNumber && vnum === null) unresolvedNames.add(r[1]);
        return {
          vnum,
          ...(byNumber ? {} : { itemName: r[1] }),
          count: Number.parseInt(r[2], 10),
          pct: Number(r[3]),
          ...(r[4] !== undefined ? { extra: r[4] } : {}),
        };
      }),
    });
  }
  for (const m of mobs) m.DropItems ??= [];
  return { orphans, unresolvedNames: [...unresolvedNames] };
}

/** Skupiny bez vlastní entity → řádky tabulky ContentGroups. */
export function toContentGroups(groupType, groups, sourceName) {
  return groups
    .filter((g) => g.kv.vnum?.[0] !== undefined)
    .map((g) => {
      const leader = g.kv.leader ?? [];
      const leaderVnum = [...leader].reverse().find((v) => /^\d+$/.test(v));
      return {
        GroupType: groupType,
        GroupVnum: Number.parseInt(g.kv.vnum[0], 10),
        Name: g.name,
        LeaderVnum: leaderVnum !== undefined ? Number.parseInt(leaderVnum, 10) : null,
        GroupKind: g.kv.type?.[0] ?? null,
        Entries: g.rows,
        SourceName: sourceName,
        Metadata: { leader, extra: Object.fromEntries(Object.entries(g.kv).filter(([k]) => !["vnum", "leader", "type"].includes(k))) },
      };
    });
}

/**
 * Duplicitní skupiny ve zdrojových souborech (např. special_item_group 10003 je
 * v souboru dvakrát s různým obsahem). Server je načítá přes std::map::insert,
 * které existující klíč nepřepíše — platí tedy PRVNÍ výskyt. Stejně se chová import;
 * pozdější výskyty se neztratí, uloží se do Metadata.duplicatesIgnored.
 * Bez toho by upsert se dvěma stejnými klíči v jedné dávce shodil celou transakci.
 */
export function dedupeFirstWins(groups) {
  const kept = new Map();
  const duplicates = [];
  for (const g of groups) {
    const key = `${g.GroupType}:${g.GroupVnum}`;
    const first = kept.get(key);
    if (!first) { kept.set(key, g); continue; }
    first.Metadata.duplicatesIgnored ??= [];
    first.Metadata.duplicatesIgnored.push({ name: g.Name, leaderVnum: g.LeaderVnum, kind: g.GroupKind, entries: g.Entries });
    duplicates.push(key);
  }
  return { rows: [...kept.values()], duplicates };
}

// ── Mapy ──────────────────────────────────────────────────────────────────────

/**
 * Indexy map, které server opravdu obsluhuje → IsEnabled importované mapy.
 * Zdrojem je compose herního serveru: mapy jsou rozdělené mezi jádra (ch1_first,
 * ch1_game1, ch1_game2, game99), takže se berou všechny hodnoty GAME_MAP_ALLOW dohromady.
 * Starý server/game/conf.cfg zůstává jako záloha, když compose chybí.
 */
export function readMapAllow(...files) {
  const allow = new Set();
  for (const file of files.flat()) {
    if (!file || !fs.existsSync(file)) continue;
    const text = readLatin1(file);
    for (const m of text.matchAll(/^\s*(?:GAME_)?MAP_ALLOW\s*[:=]?\s*(.+)$/gm)) {
      for (const n of m[1].trim().split(/\s+/)) {
        const v = Number(n);
        if (Number.isInteger(v)) allow.add(v);
      }
    }
    if (allow.size) break;   // první soubor, který něco dal, vyhrává
  }
  return allow;
}

/** data/map/index + Setting.txt + spawn soubory každé mapy → řádky tabulky Maps. */
export function parseMaps(mapDir, mapAllow) {
  const index = splitLines(readLatin1(path.join(mapDir, "index")))
    .map((l) => l.trim().split(/\s+/))
    .filter((p) => p.length >= 2 && /^\d+$/.test(p[0]));

  return index.map(([idx, name]) => {
    const dir = path.join(mapDir, name);
    const files = fs.existsSync(dir) ? fs.readdirSync(dir).sort() : [];
    const setting = {};
    const settingFile = path.join(dir, "Setting.txt");
    if (fs.existsSync(settingFile)) {
      for (const l of splitLines(readLatin1(settingFile))) {
        const p = l.trim().split(/\s+/);
        if (p[0]) setting[p[0]] = p.slice(1);
      }
    }

    // Spawny: typ x y rozsahX rozsahY z směr čas šance počet vnum (viz game/src/regen.cpp read_line)
    const spawns = [];
    for (const f of files.filter((f) => /^(regen|boss|stone|npc)\w*\.txt$/i.test(f))) {
      for (const l of splitLines(readLatin1(path.join(dir, f)))) {
        const p = l.trim().split(/\s+/);
        if (p.length < 11 || p[0].startsWith("//")) continue;
        spawns.push({
          file: f, type: p[0], x: +p[1], y: +p[2], rangeX: +p[3], rangeY: +p[4],
          z: +p[5], dir: +p[6], time: p[7], pct: +p[8], count: +p[9], vnum: +p[10],
        });
      }
    }

    const mapIndex = Number(idx);
    return {
      MapIndex: mapIndex,
      Name: name,
      NormalizedName: normalizeName(name),
      // MapSize je v segmentech mapy (1 segment = 256 buněk × CellScale), ne v herních jednotkách
      Width: Number(setting.MapSize?.[0] ?? 0) || null,
      Height: Number(setting.MapSize?.[1] ?? 0) || null,
      BaseX: Number(setting.BasePosition?.[0] ?? 0) || null,
      BaseY: Number(setting.BasePosition?.[1] ?? 0) || null,
      CellScale: Number(setting.CellScale?.[0] ?? 0) || 25,
      SpawnMobs: spawns,
      MapFiles: files,
      IsEnabled: mapAllow.has(mapIndex),
      Metadata: {
        source: `data/map/${name}`,
        textureSet: setting.TextureSet?.[0] ?? null,
        environment: setting.Environment?.[0] ?? null,
        heightScale: setting.HeightScale?.[0] ?? null,
        viewRadius: setting.ViewRadius?.[0] ?? null,
      },
    };
  });
}

// ── Questy ────────────────────────────────────────────────────────────────────

/** .quest soubory → řádky tabulky Quests; zapnuté jsou ty, které server kompiluje (locale_list). */
export function parseQuests(questDir) {
  const listFile = path.join(questDir, "locale_list");
  const compiled = new Set(
    fs.existsSync(listFile) ? splitLines(readLatin1(listFile)).map((s) => s.trim()).filter(Boolean) : [],
  );
  return fs.readdirSync(questDir).filter((f) => f.endsWith(".quest")).sort().map((f) => {
    const buf = fs.readFileSync(path.join(questDir, f));
    let text; let encoding;
    try { text = utf8Strict.decode(buf); encoding = "utf-8"; }
    catch { text = krDecoder.decode(buf); encoding = "euc-kr"; }
    const m = text.match(/^\s*quest\s+([A-Za-z0-9_]+)\s+begin/m);
    const questName = m ? m[1] : f.replace(/\.quest$/, "");
    return {
      FileName: f,
      QuestName: questName,
      NormalizedName: normalizeName(questName),
      LuaScript: text.replace(/ /g, ""),
      IsEnabled: compiled.has(f),
      Metadata: { source: `data/quest/${f}`, encoding },
    };
  });
}
