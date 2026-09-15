# database/mysql — NEKOMPATIBILNÍ se současným serverem, nepoužívá se

Tahle schémata (`account`, `player`, `common`, `log`) vznikla pro jinou verzi serveru.
Server v `server/src` (The Old Metin2 Project) potřebuje tabulky, které tu nejsou —
například `player_index`, `affect`, `item_award`, `guild_grade`, `guild_comment`,
`marriage`, `monarch`, `myshop_pricelist`, `messenger_list` — a naopak nepoužívá
`offline_shop` ani `item_proto` / `mob_proto` (proto a názvy čte ze souborů).

Správné schéma vytváří a migruje služba **web** (Laravel image
`git.old-metin2.com/metin2/web`) při prvním startu — takhle to dělá upstream deployment
(`server/deploy`). Compose v `server/docker/` zakládá jen prázdné databáze
(`server/docker/mysql-init/create-databases.sql`). Viz `server/README.md`.
