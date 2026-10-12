-- ============================================================
-- 07: Znalosti — Metin2 servery, monstra a kameny z webu
--
-- Proč: data dřív plnil worker SimulateReal do DB SportReal (MetinServers, MetinMonsters,
-- MetinStones). Vítek 12. 10. rozhodl, že patří sem. Jde o informace z wiki a seznamů
-- serverů, ne o herní proto data — proto samostatné tabulky, ne sloupce v Mobs/Items.
-- Hodnoty z wiki zůstávají textem jako ve zdroji (Level „120“, Exp „41“, odrážky „√“).
--
-- Upsert ze SimulateReal jde přes přirozený klíč (unikátní index):
--   KnowledgeServers  → "DetailUrl"
--   KnowledgeMonsters → "NameUrl"
--   KnowledgeStones   → "NameUrl"
-- "SourceGuid" drží Guid řádku ze SportReal, aby převod šel opakovat bez duplicit.
-- ============================================================

CREATE TABLE IF NOT EXISTS "KnowledgeServers" (
    "Guid"             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"        timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"        timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"        boolean NOT NULL DEFAULT false,

    "Name"             text NOT NULL,
    "Style"            text NOT NULL DEFAULT '',       -- Oldschool, Newschool, Unknown…
    "MaxLevel"         integer,
    "ShortDescription" text,
    "OnlineText"       text,                           -- „10 měsíců“ — text tak, jak ho web ukazuje
    "CommentsCount"    integer,
    "ListingUrl"       text NOT NULL,
    "DetailUrl"        text NOT NULL,
    "FirstSeenUtc"     timestamptz NOT NULL DEFAULT now(),
    "LastSeenUtc"      timestamptz NOT NULL DEFAULT now(),
    "IsActive"         boolean NOT NULL DEFAULT true,  -- při posledním stažení byl na seznamu

    "SourceName"       text NOT NULL DEFAULT 'SimulateReal',
    "SourceGuid"       uuid
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeServers_DetailUrl" ON "KnowledgeServers" ("DetailUrl");
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeServers_SourceGuid" ON "KnowledgeServers" ("SourceGuid");
CREATE INDEX IF NOT EXISTS "IX_KnowledgeServers_Style" ON "KnowledgeServers" ("Style");

CREATE TABLE IF NOT EXISTS "KnowledgeMonsters" (
    "Guid"        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"   timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"   timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"   boolean NOT NULL DEFAULT false,

    "Name"        text NOT NULL,
    "NameUrl"     text NOT NULL,                   -- odkaz na stránku wiki
    "Element"     text NOT NULL DEFAULT '',
    "Resistance"  text NOT NULL DEFAULT '',
    "Race"        text NOT NULL DEFAULT '',
    "Level"       text NOT NULL DEFAULT '',
    "Stage"       text NOT NULL DEFAULT '',
    "Location"    text NOT NULL DEFAULT '',
    "Exp"         text NOT NULL DEFAULT '',

    -- Propojení s naším mobem — wiki jména jsou anglická, naše korejská/česká, páruje se ručně
    "MobVnum"     integer,

    "SourceName"  text NOT NULL DEFAULT 'SimulateReal',
    "SourceGuid"  uuid
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeMonsters_NameUrl" ON "KnowledgeMonsters" ("NameUrl");
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeMonsters_SourceGuid" ON "KnowledgeMonsters" ("SourceGuid");
CREATE INDEX IF NOT EXISTS "IX_KnowledgeMonsters_MobVnum" ON "KnowledgeMonsters" ("MobVnum");

CREATE TABLE IF NOT EXISTS "KnowledgeStones" (
    "Guid"        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"   timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"   timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"   boolean NOT NULL DEFAULT false,

    "Name"        text NOT NULL,
    "NameUrl"     text NOT NULL,
    "Spawn"       text NOT NULL DEFAULT '',        -- mapy, kde se kámen objevuje
    "SkillBook"   text NOT NULL DEFAULT '',        -- „√“ = padá kniha dovedností
    "Spirit"      text NOT NULL DEFAULT '',
    "Soul"        text NOT NULL DEFAULT '',
    "Level"       text NOT NULL DEFAULT '',

    "MobVnum"     integer,

    "SourceName"  text NOT NULL DEFAULT 'SimulateReal',
    "SourceGuid"  uuid
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeStones_NameUrl" ON "KnowledgeStones" ("NameUrl");
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeStones_SourceGuid" ON "KnowledgeStones" ("SourceGuid");
CREATE INDEX IF NOT EXISTS "IX_KnowledgeStones_MobVnum" ON "KnowledgeStones" ("MobVnum");
