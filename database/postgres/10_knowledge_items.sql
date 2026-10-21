-- ============================================================
-- 10: Znalosti — předměty z oficiální Metin2 wiki (KnowledgeItems)
--
-- Proč: kompletační rutina IC (Data Hub návrh ~/Projects/DataHub/kompletace/NAVRH-Metin2.md, Trello ZRhWZajR)
-- plní data z wiki do vrstvy Knowledge*. Wiki neuvádí Vnum, proto předměty nepatří do „Items“
-- (proto vrstva, Vnum povinný pro export do item_proto). Rozhodnutí Vítka 21. 10.: migrace schválena.
--
-- Stejný vzor jako KnowledgeMonsters/Stones: hodnoty z wiki textem jako ve zdroji, upsert podle "NameUrl"
-- (URL stránky = stabilní klíč), nic se nemaže — zmizelá stránka dostane "IsDeleted" až po 2 bězích bez ní.
-- "Metadata": drops[], raw parametry infoboxu, import = {source, lang, pageid, revid, fetched_at};
-- změna řádku jen při novějším revid (hlídá rutina).
-- ============================================================

CREATE TABLE IF NOT EXISTS "KnowledgeItems" (
    "Guid"        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"   timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"   timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"   boolean NOT NULL DEFAULT false,

    "Name"        text NOT NULL,
    "NameUrl"     text NOT NULL,                   -- odkaz na stránku wiki
    "Category"    text NOT NULL DEFAULT '',        -- kategorie wiki (zbraň, zbroj, …)
    "Level"       text NOT NULL DEFAULT '',        -- text jako ve zdroji, číslo parsuje aplikace
    "Stats"       jsonb NOT NULL DEFAULT '{}',     -- bonusy a hodnoty z infoboxu

    -- Propojení s naším itemem — páruje Collector nebo operátor, z wiki Vnum nevyčteme
    "ItemVnum"    integer,

    "Metadata"    jsonb NOT NULL DEFAULT '{}',
    "SourceName"  text NOT NULL DEFAULT 'metin2-wiki',
    "SourceGuid"  uuid
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeItems_NameUrl" ON "KnowledgeItems" ("NameUrl");
CREATE UNIQUE INDEX IF NOT EXISTS "UX_KnowledgeItems_SourceGuid" ON "KnowledgeItems" ("SourceGuid");
CREATE INDEX IF NOT EXISTS "IX_KnowledgeItems_ItemVnum" ON "KnowledgeItems" ("ItemVnum");
CREATE INDEX IF NOT EXISTS "IX_KnowledgeItems_Category" ON "KnowledgeItems" ("Category");

-- Oprávnění pro zapisující roli rutiny se tu záměrně nedává — změna oprávnění jde přes souhlas Vítka
-- (roles/simulatereal_worker.sql nebo role, kterou IC pro kompletační rutinu určí).
