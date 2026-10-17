-- ============================================================
-- 09: historie nočních dávek (BatchRuns)
--
-- Proč: dávky (levely map, názvy, questy, převod znalostí) běží skripty mimo aplikaci a jejich
-- výsledek zůstával jen v logu na Macu. Přehled teď ukáže poslední běhy a chyby (Vítek 17. 10.).
-- Zápis dělá skript na začátku (Result = running) a na konci (ok|check|fail|blocked).
-- ============================================================

CREATE TABLE IF NOT EXISTS "BatchRuns" (
    "Guid"        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"   timestamptz NOT NULL DEFAULT now(),
    "Name"        text NOT NULL,                       -- např. maps-quests, knowledge-transfer
    "StartedAt"   timestamptz NOT NULL DEFAULT now(),
    "FinishedAt"  timestamptz,
    "Result"      text NOT NULL DEFAULT 'running',     -- running|ok|check|fail|blocked
    "Summary"     text,                                -- poslední řádky výstupu, bez hesel
    "BackupFile"  text
);

CREATE INDEX IF NOT EXISTS "IX_BatchRuns_StartedAt" ON "BatchRuns" ("StartedAt" DESC);
