-- ============================================================
-- Metin2Bausia — MySQL inicializace: vytvoření všech DB
-- Spouští se automaticky při prvním startu mt2-mysql kontejneru
-- ============================================================

CREATE DATABASE IF NOT EXISTS account  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS player   CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS common   CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS log      CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Oprávnění pro root přes síť (docker interní)
GRANT ALL PRIVILEGES ON account.* TO 'root'@'%' IDENTIFIED BY 'bausia_root_secret';
GRANT ALL PRIVILEGES ON player.*  TO 'root'@'%' IDENTIFIED BY 'bausia_root_secret';
GRANT ALL PRIVILEGES ON common.*  TO 'root'@'%' IDENTIFIED BY 'bausia_root_secret';
GRANT ALL PRIVILEGES ON log.*     TO 'root'@'%' IDENTIFIED BY 'bausia_root_secret';

FLUSH PRIVILEGES;
