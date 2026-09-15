-- ============================================================
-- Metin2Bausia — migrace 05: obsah importovaný z herních souborů serveru
--
-- Proč: import z server/src/gamefiles musí jít pouštět opakovaně (idempotentně).
-- Items/Mobs/Maps mají jedinečný Vnum/MapIndex, Quests a skupiny žádný klíč
-- neměly — upsert by bez něj vytvářel duplicity.
-- ============================================================

-- ── Quests ────────────────────────────────────────────────────────────────────
-- Klíčem je soubor, ne název: 6 názvů questů (např. deviltower_zone) existuje
-- ve dvou souborech, unikátní index na QuestName by import shodil.
ALTER TABLE "Quests" ADD COLUMN IF NOT EXISTS "FileName" text;
CREATE UNIQUE INDEX IF NOT EXISTS ux_quests_filename ON "Quests" ("FileName");

-- ── Mobs ──────────────────────────────────────────────────────────────────────
-- DAM_MULTIPLY má v mob_proto.txt až 4 desetinná místa (např. 1.0625);
-- numeric(6,2) z migrace 04 by hodnotu zaokrouhlil a export by nevrátil originál.
ALTER TABLE "Mobs" ALTER COLUMN "DamMultiply" TYPE numeric(8,4);

-- ── ContentGroups ─────────────────────────────────────────────────────────────
-- Skupiny, které nemají vlastní entitu: group.txt (skupiny mobů pro spawn),
-- group_group.txt (skupiny skupin), special_item_group.txt (truhly, bonusy).
-- Položky se drží jako jsonb, protože každý typ skupiny má jiný tvar řádku.
CREATE TABLE IF NOT EXISTS "ContentGroups" (
    "Guid"          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"     timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"     timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"     boolean NOT NULL DEFAULT false,
    "Emoji"         text NOT NULL DEFAULT '',
    "Colors"        text NOT NULL DEFAULT '',

    "GroupType"     text NOT NULL,          -- mob_group | mob_group_group | special_item_group
    "GroupVnum"     integer NOT NULL,
    "Name"          text,                   -- původní název skupiny (v souboru korejsky)
    "LeaderVnum"    integer,                -- jen mob_group
    "GroupKind"     text,                   -- special_item_group: Pct | Quest …
    "Entries"       jsonb NOT NULL DEFAULT '[]',

    "ContentStatus" text NOT NULL DEFAULT 'approved',
    "IsEnabled"     boolean NOT NULL DEFAULT true,
    "SourceName"    text,
    "DataOrigin"    text DEFAULT 'imported',
    "Metadata"      jsonb DEFAULT '{}'
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_contentgroups_type_vnum ON "ContentGroups" ("GroupType", "GroupVnum");
