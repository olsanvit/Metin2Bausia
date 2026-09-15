# Herní server Metin2Bausia

Server stojí na **The Old Metin2 Project**: zdrojáky serveru z roku 2014 v `server/src`
a upstream deployment v `server/deploy` (obojí v `.gitignore`, do veřejného repa se
nedává). Upstream v README uvádí, že komerční provoz bez licence je nelegální.

## Co kde je

| Cesta | Obsah |
|---|---|
| `server/src/` | upstream zdrojáky + `Dockerfile` (build přes vcpkg) |
| `server/src/gamefiles/conf/` | `item_proto.txt`, `mob_proto.txt`, názvy v 15 jazycích |
| `server/src/gamefiles/data/` | mapy, spawny, dropy, skupiny, questy |
| `server/deploy/` | upstream compose — referenční předloha |
| `server/docker/` | náš compose odvozený od upstreamu |
| `tools/gamefiles-import/` | import herních dat do management DB (admin) |

`server/share/` je prázdná — data leží v `server/src/gamefiles/`.

## Architektura (podle upstreamu)

| Služba | Role | Port |
|---|---|---|
| `mysql` | MySQL 5.5 — `account`, `player`, `common`, `log`, `website` | `127.0.0.1:3366` |
| `web` | Laravel: web, item shop, autopatcher; **zakládá a migruje SQL schéma** | `127.0.0.1:8098` |
| `db` | DBCache — proto a názvy ze souborů, cache hráčů | interní 15000 |
| `auth` | přihlášení (`AUTH_SERVER: master`, hráčská DB = `account`) | `11000` |
| `ch1_first`, `ch1_game1`, `ch1_game2` | kanál 1 rozdělený do tří jader podle map | `13000`–`13002` |
| `game99` | speciální mapy (dungeony, válka říší) | `13099` |

Jádra se spouští až po webu, protože tabulky vzniknou teprve při jeho prvním startu.

## Tok obsahu

```
server/src/gamefiles  ──import──►  Metin2Bausia DB (admin)  ──export (MCP)──►  proto/*.txt
      (referenční data)              schvalování, zapínání          item_proto, mob_proto,
                                                                    item_names_cz, mob_names_cz
```

1. `node tools/gamefiles-import/import.mjs` → SQL do `database/import/generated/` (**NEcommitovat**, proprietární data).
2. SQL se pustí do `mt2-postgres` v jedné transakci (`psql --single-transaction`). Opakovaný import přepíše jen řádky s `DataOrigin='imported'` a nikdy nemění schválení ani zapnutí.
3. `node --test tools/gamefiles-import/roundtrip.test.mjs` ověří, že export vrátí originální soubory.

## Spuštění (zatím neověřeno — lokálně není Docker)

```bash
cp server/docker/.env.example server/docker/.env   # vyplnit heslo, veřejnou IP, web klíč
docker compose -f server/docker/docker-compose.yml build db
docker compose -f server/docker/docker-compose.yml up -d mysql web   # první start: web založí tabulky
docker compose -f server/docker/docker-compose.yml up -d
```

- **Jazyk:** `MT2_LOCALE=cz` přesměruje `item_names.txt` / `mob_names.txt` na české soubory (bind mount).
- **Data a logy:** `server/docker/storage/` (v `.gitignore`).
- **Kde běžet:** ne na QNAPu. Build přes vcpkg i běh 7 kontejnerů jsou náročné a QNAP opakovaně padá pod I/O zátěží.

## Otevřené body

- **Build image z `server/src` a start celého stacku nejsou ověřené** (lokálně není Docker).
- **Web image** `git.old-metin2.com/metin2/web` se stáhne až při `up`; bez něj nevznikne schéma.
- **Povolené mapy se liší:** runtime používá rozdělení mezi jádra z upstreamu (compose), import do adminu určuje `IsEnabled` podle `MAP_ALLOW` ze starého `server/game/conf.cfg`. Než admin začne mapy zapínat pro reálný server, je potřeba sjednotit zdroj.
- **Klient** (patcher v `src/Metin2Bausia.Patcher`, manifest v `patcher/server`) má zatím zástupné hodnoty.
