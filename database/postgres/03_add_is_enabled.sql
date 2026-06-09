-- ============================================================
-- Metin2Bausia — migrace 03: IsEnabled pro všechny entity
-- Defaultně FALSE — admin musí explicitně zapnout každou položku
-- ============================================================

ALTER TABLE "Items"   ADD COLUMN IF NOT EXISTS "IsEnabled" BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE "Mobs"    ADD COLUMN IF NOT EXISTS "IsEnabled" BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE "Maps"    ADD COLUMN IF NOT EXISTS "Maps_IsEnabled" BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE "Skills"  ADD COLUMN IF NOT EXISTS "IsEnabled" BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE "Quests"  ADD COLUMN IF NOT EXISTS "IsEnabled" BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE "Systems" ADD COLUMN IF NOT EXISTS "IsEnabled" BOOLEAN NOT NULL DEFAULT FALSE;

-- Opravit název u Maps (konzistentní)
ALTER TABLE "Maps" RENAME COLUMN "Maps_IsEnabled" TO "IsEnabled";

-- Index pro rychlé filtrování povolených položek
CREATE INDEX IF NOT EXISTS idx_items_is_enabled   ON "Items"   ("IsEnabled") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS idx_mobs_is_enabled    ON "Mobs"    ("IsEnabled") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS idx_maps_is_enabled    ON "Maps"    ("IsEnabled") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS idx_skills_is_enabled  ON "Skills"  ("IsEnabled") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS idx_systems_is_enabled ON "Systems" ("IsEnabled") WHERE "IsDeleted" = FALSE;
