-- ============================================================
-- Role pro worker SimulateReal: zápis do tabulek Znalostí (migrace 07)
--
-- Proč vlastní role: worker běží mimo QNAP a nemá dostat práva na herní data (Items, Mobs…).
-- SELECT je nutný pro INSERT … ON CONFLICT DO UPDATE (čte konfliktní řádek), DELETE záměrně ne —
-- mazání je IsDeleted = true přes UPDATE.
--
-- Heslo tu NENÍ (repo je veřejné). Roli vytvoří Vítek, heslo nastaví z Bitwardenu
-- (položka „Metin2Bausia DB – SimulateReal worker“). Spouští se jako roundnet (superuser).
-- Skript je idempotentní — opakované spuštění jen znovu nastaví práva.
-- ============================================================

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'metin2bausia_simulatereal') THEN
        CREATE ROLE metin2bausia_simulatereal LOGIN NOINHERIT NOCREATEDB NOCREATEROLE CONNECTION LIMIT 5;
    END IF;
END $$;

REVOKE ALL ON DATABASE "Metin2Bausia" FROM metin2bausia_simulatereal;
GRANT CONNECT ON DATABASE "Metin2Bausia" TO metin2bausia_simulatereal;
GRANT USAGE ON SCHEMA public TO metin2bausia_simulatereal;

GRANT SELECT, INSERT, UPDATE ON "KnowledgeServers", "KnowledgeMonsters", "KnowledgeStones", "KnowledgeItems"
    TO metin2bausia_simulatereal;
