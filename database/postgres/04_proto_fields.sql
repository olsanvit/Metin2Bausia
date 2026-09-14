-- ============================================================
-- Metin2Bausia — migrace 04: pole potřebná pro export proto souborů
--
-- Proč: item_proto.txt má 33 sloupců a mob_proto.txt 71 (ověřeno proti
-- server/src/gamefiles/conf/). Hodnoty jsou SYMBOLICKÉ, ne číselné —
-- např. ITEM_WEAPON, WEAPON_SWORD, "ANTI_DROP | ANTI_GIVE", PAWN, MELEE,
-- "AGGR,BERSERK". Proto se flagy a subtypy drží jako text, ne jako bigint.
-- Sloupce, které ve schématu chyběly úplně, se doplňují s výchozí 0.
-- ============================================================

-- ── Items ─────────────────────────────────────────────────────────────────────
-- Flagy jsou v souboru seznamy oddělené " | " — číslo je neumí vyjádřit.
-- NULLIF(...,0) drží prázdnou hodnotu jako NULL místo textu '0'.
ALTER TABLE "Items" ALTER COLUMN "AntiFlags"   TYPE text USING NULLIF("AntiFlags", 0)::text;
ALTER TABLE "Items" ALTER COLUMN "Flags"       TYPE text USING NULLIF("Flags", 0)::text;
ALTER TABLE "Items" ALTER COLUMN "WearFlags"   TYPE text USING NULLIF("WearFlags", 0)::text;
ALTER TABLE "Items" ALTER COLUMN "ImmuneFlags" TYPE text USING NULLIF("ImmuneFlags", 0)::text;
ALTER TABLE "Items" ALTER COLUMN "AntiFlags"   DROP DEFAULT;
ALTER TABLE "Items" ALTER COLUMN "Flags"       DROP DEFAULT;
ALTER TABLE "Items" ALTER COLUMN "WearFlags"   DROP DEFAULT;
ALTER TABLE "Items" ALTER COLUMN "ImmuneFlags" DROP DEFAULT;

-- SUB_TYPE je symbolický (WEAPON_SWORD, ARMOR_BODY), v souboru "0" když žádný
ALTER TABLE "Items" ALTER COLUMN "SubType" TYPE text USING NULLIF("SubType", 0)::text;

-- Sloupce, které ve schématu chyběly (pozice 12, 13, 14, 31, 32, 33 v item_proto.txt)
ALTER TABLE "Items" ADD COLUMN IF NOT EXISTS "Refine"    integer NOT NULL DEFAULT 0;
ALTER TABLE "Items" ADD COLUMN IF NOT EXISTS "RefineSet" integer NOT NULL DEFAULT 0;
ALTER TABLE "Items" ADD COLUMN IF NOT EXISTS "MagicPct"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Items" ADD COLUMN IF NOT EXISTS "Specular"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Items" ADD COLUMN IF NOT EXISTS "Socket"    integer NOT NULL DEFAULT 0;
ALTER TABLE "Items" ADD COLUMN IF NOT EXISTS "AttuAddon" integer NOT NULL DEFAULT 0;

-- ── Mobs ──────────────────────────────────────────────────────────────────────
-- IMMUNE_FLAG je seznam oddělený čárkou (CURSE,TERROR)
ALTER TABLE "Mobs" ALTER COLUMN "ImmuneFlags" TYPE text USING NULLIF("ImmuneFlags", 0)::text;
ALTER TABLE "Mobs" ALTER COLUMN "ImmuneFlags" DROP DEFAULT;

-- Seznamy oddělené čárkou (AGGR,BERSERK / ANIMAL,ATT_WIND)
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "AiFlag"   text;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "RaceFlag" text;

ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "MountCapacity"    integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "Empire"           integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "Folder"           text;      -- název složky s modelem
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "OnClick"          integer NOT NULL DEFAULT 0;

-- Základní statistiky — server z nich počítá útok, proto nestačí Atk/MagicAtk
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "St" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "Dx" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "Ht" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "Iq" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "DamageMin" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "DamageMax" integer NOT NULL DEFAULT 0;

ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "DropItemVnum"     integer NOT NULL DEFAULT 0;  -- DROP_ITEM (mob_drop_item.txt group)
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResurrectionVnum" integer NOT NULL DEFAULT 0;

ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "EnchantCurse"     integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "EnchantSlow"      integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "EnchantPoison"    integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "EnchantStun"      integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "EnchantCritical"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "EnchantPenetrate" integer NOT NULL DEFAULT 0;

ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistSword"   integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistTwohand" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistDagger"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistBell"    integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistFan"     integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistBow"     integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistFire"    integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistElect"   integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistMagic"   integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistWind"    integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "ResistPoison"  integer NOT NULL DEFAULT 0;

ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "DamMultiply"   numeric(6,2) NOT NULL DEFAULT 1;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "Summon"        integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "DrainSp"       integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "MobColor"      integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "PolymorphItem" integer NOT NULL DEFAULT 0;

ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillLevel0" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillVnum0"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillLevel1" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillVnum1"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillLevel2" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillVnum2"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillLevel3" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillVnum3"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillLevel4" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SkillVnum4"  integer NOT NULL DEFAULT 0;

-- SP_* sloupce jsou v reálném souboru prázdné u běžných mobů, u bossů nesou hodnoty
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SpBerserk"   integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SpStoneskin" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SpGodspeed"  integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SpDeathblow" integer NOT NULL DEFAULT 0;
ALTER TABLE "Mobs" ADD COLUMN IF NOT EXISTS "SpRevive"    integer NOT NULL DEFAULT 0;
