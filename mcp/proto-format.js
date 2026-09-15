// ============================================================
// Formátování proto souborů pro TMP4 herní server.
//
// Proč vlastní modul: formát je poziční a symbolický (33 sloupců u itemů,
// 71 u mobů, hodnoty typu ITEM_WEAPON / WEAPON_SWORD / "ANTI_DROP | ANTI_GIVE").
// Špatně poskládaný soubor rozbije start game serveru, takže funkce musí jít
// otestovat bez databáze — proto jsou čisté a oddělené od server.js.
//
// Referencí je server/src/gamefiles/conf/{item_proto,mob_proto}.txt.
// ============================================================

// ── item_proto.txt ────────────────────────────────────────────────────────────

export const ITEM_PROTO_HEADER = [
  "VNUM", "NAME", "ITEM_TYPE", "SUB_TYPE", "SIZE", "ANTI_FLAG", "FLAG", "ITEM_WEAR", "IMMUNE",
  "GOLD", "SHOP_BUY_PRICE", "REFINE", "REFINESET", "MAGIC_PCT",
  "LIMIT_TYPE0", "LIMIT_VALUE0", "LIMIT_TYPE1", "LIMIT_VALUE1",
  "ADDON_TYPE0", "ADDON_VALUE0", "ADDON_TYPE1", "ADDON_VALUE1", "ADDON_TYPE2", "ADDON_VALUE2",
  "VALUE0", "VALUE1", "VALUE2", "VALUE3", "VALUE4", "VALUE5",
  "Specular", "SOCKET", "ATTU_ADDON",
];

// Hodnoty z admin UI (dropdown v ManageItemDetail) → symbolika souboru.
// Části zbroje sdílejí ITEM_ARMOR a liší se až v SUB_TYPE.
const ITEM_TYPE_MAP = {
  weapon: "ITEM_WEAPON", bow: "ITEM_WEAPON", arrow: "ITEM_WEAPON",
  armor: "ITEM_ARMOR", helmet: "ITEM_ARMOR", boots: "ITEM_ARMOR", shield: "ITEM_ARMOR",
  necklace: "ITEM_ARMOR", bracelet: "ITEM_ARMOR", ring: "ITEM_ARMOR",
  belt: "ITEM_BELT",
  potion: "ITEM_USE", use: "ITEM_USE",
  quest: "ITEM_QUEST", unique: "ITEM_UNIQUE", special: "ITEM_SPECIAL",
  material: "ITEM_MATERIAL", metin: "ITEM_METIN", costume: "ITEM_COSTUME",
  giftbox: "ITEM_GIFTBOX", skillbook: "ITEM_SKILLBOOK", none: "ITEM_NONE",
};

// Když SUB_TYPE v DB chybí, odvodí se z typu — jinak by zbroj neměla slot
const ITEM_SUBTYPE_MAP = {
  weapon: "WEAPON_SWORD", bow: "WEAPON_BOW", arrow: "WEAPON_ARROW",
  armor: "ARMOR_BODY", helmet: "ARMOR_HEAD", boots: "ARMOR_FOOTS",
  shield: "ARMOR_SHIELD", necklace: "ARMOR_NECK", bracelet: "ARMOR_WRIST",
  ring: "ARMOR_EAR",
};

// ITEM_WEAR určuje, do kterého slotu se item obléká
const ITEM_WEAR_MAP = {
  weapon: "WEAR_WEAPON", bow: "WEAR_WEAPON", arrow: "WEAR_ARROW",
  armor: "WEAR_BODY", helmet: "WEAR_HEAD", boots: "WEAR_FOOTS",
  shield: "WEAR_SHIELD", necklace: "WEAR_NECK", bracelet: "WEAR_WRIST",
  ring: "WEAR_EAR", unique: "WEAR_UNIQUE",
};

/** Hodnota, kterou už agent uložil v symbolické podobě, se nechává být. */
function symbolic(value, map, fallback) {
  if (value === null || value === undefined || value === "") return fallback;
  const raw = String(value).trim();
  if (/^[A-Z][A-Z0-9_]*$/.test(raw)) return raw;   // už symbolické (ITEM_WEAPON)
  return map[raw.toLowerCase()] ?? fallback;
}

/** Prázdný seznam flagů se v souboru zapisuje jako NONE, ne jako prázdné pole. */
function flagList(value, fallback = "NONE") {
  if (value === null || value === undefined) return fallback;
  const raw = String(value).trim();
  if (raw === "" || raw === "0") return fallback;
  return raw;
}

function num(value, fallback = 0) {
  const n = Number(value);
  return Number.isFinite(n) ? n : fallback;
}

/**
 * Metadata z DB (jsonb) — node-pg ho vrací jako objekt, ale ručně vložený řádek může mít text.
 */
function meta(r) {
  const m = r?.Metadata;
  if (!m) return {};
  if (typeof m === "string") { try { return JSON.parse(m); } catch { return {}; } }
  return m;
}

/**
 * NAME v proto souborech je interní (v originále korejsky v EUC-KR). Importér ukládá původní
 * bajty do Metadata.protoName jako latin1 text, aby export vrátil soubor bajt po bajtu.
 * U nových položek bez originálu se jméno zjednoduší na ASCII — soubor se zapisuje v latin1
 * a server ho používá jen do logů, zobrazované jméno bere z item_names/mob_names.
 */
function protoName(r) {
  const m = meta(r);
  if (typeof m.protoName === "string") return m.protoName;
  return String(r.Name ?? "").normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^\x20-\x7e]/g, "?");
}

/** Rozsah vnum ("110000~110099") se do integer sloupce nevejde — drží se v Metadata.vnumRange. */
function protoVnum(r) {
  const m = meta(r);
  if (typeof m.vnumRange === "string") return m.vnumRange;
  if (/~/.test(String(r.Vnum ?? ""))) return String(r.Vnum);
  return num(r.Vnum);
}

/** Jeden řádek item_proto.txt (33 sloupců oddělených tabulátorem). */
export function buildItemProtoLine(r) {
  const type = symbolic(r.ItemType, ITEM_TYPE_MAP, "ITEM_NONE");
  const key = String(r.ItemType ?? "").toLowerCase();

  const subType = r.SubType !== null && r.SubType !== undefined && String(r.SubType) !== ""
    ? String(r.SubType)
    : (ITEM_SUBTYPE_MAP[key] ?? "0");

  const wear = flagList(r.WearFlags, ITEM_WEAR_MAP[key] ?? "NONE");

  return [
    protoVnum(r),
    // Do proto jde interní název; lokalizovaný patří do item_names.txt, ne sem
    protoName(r),
    type,
    subType,
    num(r.Size, 1),
    flagList(r.AntiFlags),
    flagList(r.Flags),
    wear,
    flagList(r.ImmuneFlags),
    num(r.Gold),
    num(r.Buy),
    num(r.Refine),
    num(r.RefineSet),
    num(r.MagicPct),
    r.LimitType0 || "LIMIT_NONE", num(r.LimitValue0),
    r.LimitType1 || "LIMIT_NONE", num(r.LimitValue1),
    r.ApplyType0 || "APPLY_NONE", num(r.ApplyValue0),
    r.ApplyType1 || "APPLY_NONE", num(r.ApplyValue1),
    r.ApplyType2 || "APPLY_NONE", num(r.ApplyValue2),
    num(r.Value0), num(r.Value1), num(r.Value2), num(r.Value3), num(r.Value4), num(r.Value5),
    num(r.Specular),
    num(r.Socket),
    num(r.AttuAddon),
  ].join("\t");
}

// ── mob_proto.txt ─────────────────────────────────────────────────────────────

export const MOB_PROTO_HEADER = [
  "VNUM", "NAME", "RANK", "TYPE", "BATTLE_TYPE", "LEVEL", "SIZE", "AI_FLAG", "MOUNT_CAPACITY",
  "RACE_FLAG", "IMMUNE_FLAG", "EMPIRE", "FOLDER", "ON_CLICK",
  "ST", "DX", "HT", "IQ", "DAMAGE_MIN", "DAMAGE_MAX", "MAX_HP", "REGEN_CYCLE", "REGEN_PERCENT",
  "GOLD_MIN", "GOLD_MAX", "EXP", "DEF", "ATTACK_SPEED", "MOVE_SPEED",
  "AGGRESSIVE_HP_PCT", "AGGRESSIVE_SIGHT", "ATTACK_RANGE", "DROP_ITEM", "RESURRECTION_VNUM",
  "ENCHANT_CURSE", "ENCHANT_SLOW", "ENCHANT_POISON", "ENCHANT_STUN", "ENCHANT_CRITICAL", "ENCHANT_PENETRATE",
  "RESIST_SWORD", "RESIST_TWOHAND", "RESIST_DAGGER", "RESIST_BELL", "RESIST_FAN", "RESIST_BOW",
  "RESIST_FIRE", "RESIST_ELECT", "RESIST_MAGIC", "RESIST_WIND", "RESIST_POISON",
  "DAM_MULTIPLY", "SUMMON", "DRAIN_SP", "MOB_COLOR", "POLYMORPH_ITEM",
  "SKILL_LEVEL0", "SKILL_VNUM0", "SKILL_LEVEL1", "SKILL_VNUM1", "SKILL_LEVEL2", "SKILL_VNUM2",
  "SKILL_LEVEL3", "SKILL_VNUM3", "SKILL_LEVEL4", "SKILL_VNUM4",
  "SP_BERSERK", "SP_STONESKIN", "SP_GODSPEED", "SP_DEATHBLOW", "SP_REVIVE",
];

const MOB_RANK_MAP = {
  pawn: "PAWN", super_pawn: "S_PAWN", s_pawn: "S_PAWN",
  knight: "KNIGHT", super_knight: "S_KNIGHT", s_knight: "S_KNIGHT",
  boss: "BOSS", king: "KING",
};

// Truhly jsou v proto běžné MONSTER — odlišuje je až chování, ne typ
const MOB_TYPE_MAP = {
  monster: "MONSTER", npc: "NPC", metin: "STONE", stone: "STONE",
  chest: "MONSTER", boss: "MONSTER",
  warp: "WARP", goto: "GOTO", door: "DOOR", building: "BUILDING",
};

const MOB_BATTLE_TYPE_MAP = {
  melee: "MELEE", range: "RANGE", magic: "MAGIC",
  special: "SPECIAL", power: "POWER", tanker: "TANKER",
};

/** Jeden řádek mob_proto.txt (71 sloupců oddělených tabulátorem). */
export function buildMobProtoLine(r) {
  // Seznamy AI/RACE/IMMUNE se oddělují čárkou a prázdná hodnota je prázdný sloupec (ne NONE)
  const list = (v) => (v === null || v === undefined || String(v) === "0" ? "" : String(v).trim());

  return [
    protoVnum(r),
    protoName(r),
    symbolic(r.Rank, MOB_RANK_MAP, "PAWN"),
    symbolic(r.MobType, MOB_TYPE_MAP, "MONSTER"),
    symbolic(r.BattleType, MOB_BATTLE_TYPE_MAP, "MELEE"),
    num(r.Level),
    num(r.Size),
    list(r.AiFlag),
    num(r.MountCapacity),
    list(r.RaceFlag),
    list(r.ImmuneFlags),
    num(r.Empire),
    r.Folder || "",
    num(r.OnClick),
    num(r.St), num(r.Dx), num(r.Ht), num(r.Iq),
    num(r.DamageMin), num(r.DamageMax),
    num(r.MaxHp),
    num(r.RegenCycle), num(r.RegenPercent),
    num(r.GoldMin), num(r.GoldMax),
    num(r.Exp),
    num(r.Def),
    num(r.AttackSpeed, 100), num(r.MoveSpeed, 100),
    num(r.AggressiveHpPct), num(r.AggressiveSight), num(r.AttackRange),
    num(r.DropItemVnum), num(r.ResurrectionVnum),
    num(r.EnchantCurse), num(r.EnchantSlow), num(r.EnchantPoison),
    num(r.EnchantStun), num(r.EnchantCritical), num(r.EnchantPenetrate),
    num(r.ResistSword), num(r.ResistTwohand), num(r.ResistDagger), num(r.ResistBell),
    num(r.ResistFan), num(r.ResistBow), num(r.ResistFire), num(r.ResistElect),
    num(r.ResistMagic), num(r.ResistWind), num(r.ResistPoison),
    (typeof meta(r).damMultiplyRaw === "string" ? meta(r).damMultiplyRaw : num(r.DamMultiply, 1)), num(r.Summon), num(r.DrainSp), num(r.MobColor), num(r.PolymorphItem),
    num(r.SkillLevel0), num(r.SkillVnum0), num(r.SkillLevel1), num(r.SkillVnum1),
    num(r.SkillLevel2), num(r.SkillVnum2), num(r.SkillLevel3), num(r.SkillVnum3),
    num(r.SkillLevel4), num(r.SkillVnum4),
    num(r.SpBerserk), num(r.SpStoneskin), num(r.SpGodspeed), num(r.SpDeathblow), num(r.SpRevive),
  ].join("\t");
}


// ── item_names / mob_names (VNUM<TAB>LOCALE_NAME) ─────────────────────────────

export const NAMES_HEADER = "VNUM\tLOCALE_NAME";

// Tabulka Unicode → bajt pro CP1250 se skládá z TextDecoderu, protože Node umí
// windows-1250 jen dekódovat. Obejde se tím závislost na iconv.
let cp1250Encode = null;
function cp1250Table() {
  if (cp1250Encode) return cp1250Encode;
  cp1250Encode = new Map();
  const dec = new TextDecoder("windows-1250");
  for (let b = 0x80; b <= 0xff; b++) {
    const ch = dec.decode(Uint8Array.of(b));
    if (ch !== "\ufffd") cp1250Encode.set(ch, b);
  }
  return cp1250Encode;
}

/** Text → Buffer v CP1250; znak, který v kódové stránce není, se nahradí "?". */
export function encodeCp1250(text) {
  const table = cp1250Table();
  const out = [];
  for (const ch of String(text)) {
    const code = ch.codePointAt(0);
    if (code < 0x80) out.push(code);
    else out.push(table.get(ch) ?? 0x3f);
  }
  return Buffer.from(out);
}

/**
 * Soubor názvů pro server (item_names_cz.txt / mob_names_cz.txt).
 * rows: [{ Vnum, LocaleName, Name, Metadata }] — pořadí se zachová, jak přišlo.
 */
export function buildNamesFile(rows) {
  const lines = [NAMES_HEADER];
  for (const r of rows) lines.push(`${protoVnum(r)}\t${r.LocaleName ?? r.Name ?? ""}`);
  return encodeCp1250(lines.join("\n") + "\n");
}
