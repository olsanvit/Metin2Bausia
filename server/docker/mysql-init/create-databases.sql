-- ============================================================
-- Metin2Bausia — založení databází při prvním startu MySQL.
-- Převzato z upstream deploye (assets/db-init/create-databases.sql).
-- Jen prázdné databáze: tabulky vytváří a migruje služba web.
-- ============================================================
CREATE DATABASE account CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE common CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE log CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE player CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE website CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
