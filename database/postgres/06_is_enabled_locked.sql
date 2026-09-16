-- ============================================================
-- 06: příznak ručního zapnutí/vypnutí obsahu
--
-- Proč: opakovaný import přebírá IsEnabled ze serverových souborů (MAP_ALLOW, locale_list),
-- ale nesmí přepsat rozhodnutí admina. Admin přepínač nastaví IsEnabledLocked = TRUE
-- a import pak u takového řádku zapnutí ponechá.
-- ============================================================

ALTER TABLE "Items"         ADD COLUMN IF NOT EXISTS "IsEnabledLocked" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "Mobs"          ADD COLUMN IF NOT EXISTS "IsEnabledLocked" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "Maps"          ADD COLUMN IF NOT EXISTS "IsEnabledLocked" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "Quests"        ADD COLUMN IF NOT EXISTS "IsEnabledLocked" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "ContentGroups" ADD COLUMN IF NOT EXISTS "IsEnabledLocked" boolean NOT NULL DEFAULT FALSE;
