# Metin2Bausia

Správa Metin2 private serveru: Blazor admin nad PostgreSQL, MCP server pro AI agenta
a herní server (The Old Metin2 Project). Repo je **veřejné** — žádné heslo, token ani
herní data do commitu.

## Struktura

```
src/Metin2Bausia.Web/      Blazor Server admin (.NET 10, Dapper, 15 stránek)
src/Metin2Bausia.Patcher/  WinForms patcher klienta (net9.0-windows)
src/Metin2Bausia.Tests/    smoke testy (NENÍ v .sln)
src/SharedServices/        git submodul (ThemeService, lokalizace, UI komponenty)
mcp/                       MT2 MCP server (Node) + proto-format.js (formát proto souborů)
tools/gamefiles-import/    import herních dat do DB + round-trip test
database/postgres/         migrace 01–06 (schéma management DB)
database/import/generated/ vygenerované importní SQL — v .gitignore, NECOMMITOVAT
server/                    herní server: src/ (upstream), deploy/ (upstream), docker/ (náš compose)
docs/pages/                dokumentace všech 15 stránek
```

## Databáze

| Prostředí | Připojení |
|---|---|
| Produkce | sdílený kontejner `pg16` na QNAPu (síť appnet, PostgreSQL 16), databáze `Metin2Bausia`, vlastní role `metin2bausia`; přístup přes `docker exec pg16 psql -U roundnet -d Metin2Bausia` |
| Lokálně | PostgreSQL 18 (Homebrew), port **5442**, data v `~/.local/share/metin2bausia-pg18` |

Start lokální databáze (bez `LC_ALL` postmaster na macOS spadne):

```bash
export LC_ALL=en_US.UTF-8
/opt/homebrew/opt/postgresql@18/bin/pg_ctl -D ~/.local/share/metin2bausia-pg18 \
  -l ~/.local/share/metin2bausia-pg18/server.log -o "-p 5442 -k /tmp -c listen_addresses=localhost" -w start
```

Naplnění daty: migrace `database/postgres/0{1,2,3,4,5,6}_*.sql`, pak
`node tools/gamefiles-import/import.mjs` a vygenerované SQL z `database/import/generated/`.

## Nasazení

- Produkční kontejner **`metin2bausia`** = `aspnet:10.0` + publish složka
  `/share/Public/BlazorMetin2Bausia/publish`, port 5023. **Ne** compose z repa.
- Deploy: `~/deploy-to-qnap.sh metin2bausia` (rsync + restart kontejneru).
- **Projekt je záměrně vypnutý** (`restart=no`), dokud ho uživatel nespustí. Deploy skript kontejner restartuje — po ověření `/health` ho zase zastavit.
- `appsettings.Production.json` je na QNAPu zdroj pravdy (`KEEP_REMOTE_PROD_CFG=1`), necommituje se.

## Pravidla

- Odpovídat česky; komentovat PROČ, ne co.
- **Žádné git příkazy bez výslovného pokynu** (výjimka: `git status` a `git log -5` na startu session).
- **Jeden build na Macu najednou** (8 GB RAM): před `dotnet build/publish/test` ověřit
  `ps -axo comm=,args= | awk '$1 ~ /(^|\/)dotnet$/' | grep -E ' (build|publish|test)( |$)| ef '`
  a čekat, dokud něco běží.
- Před deployem: `df -h /` a lock `/tmp/claude-deploy.lock` (mladší 30 min = nedeployovat).
- Před commitem: `git submodule update --remote src/SharedServices`, ověřit build, push na `main`.

## Pasti, které stály čas

- **Interaktivita Blazoru:** `App.razor` musí načítat `blazor.web.js` a `Program.cs` volat
  `app.MapStaticAssets()`; bez něj vrací `_framework/blazor.web.js` 404. `@rendermode InteractiveServer`
  patří na stránku, ne na layout.
- **`.gitignore` a velikost písmen:** `data/` na macOS (`core.ignorecase=true`) ignorovalo i složku
  `Data/` a `ContentLabels.cs` chyběl v commitu — CI padalo na `CS0103`. Ignorovat konkrétní soubor.
- **Proto soubory:** `item_proto.txt` má 33 sloupců, `mob_proto.txt` 71, hodnoty jsou symbolické
  (`ITEM_WEAPON`, `WEAPON_SWORD`, `PAWN`, `AGGR,BERSERK`). Jméno je v EUC-KR (drží se v
  `Metadata.protoName`), názvy v `item_names_*.txt` v CP1250. Skládání řádku je v `mcp/proto-format.js`,
  ověřuje ho `node --test tools/gamefiles-import/roundtrip.test.mjs`.
- **Duplicity ve zdrojových datech:** server bere první výskyt (`std::map::insert`) — import taky.
- **Dropy typu `kill`/`limit`** odkazují item jménem, ne vnumem.
- **Opakovaný import** přepisuje jen řádky `DataOrigin='imported'`; uložení v detailu je přepne na `manual`.
  `IsEnabled` import převezme, jen když admin řádek ručně nepřepnul (`IsEnabledLocked`).
- **Testy stránek** potřebují DB: `ConnectionStrings__Metin2Bausia="Host=localhost;Port=5442;Database=m2b_test;Username=postgres" dotnet test src/Metin2Bausia.Tests/`.
  Prázdnou DB naplní migracemi sám; do existující `m2b_test` je novou migraci nutné nahrát ručně.
- **Node 26** nebere u `node --test` adresář — zadávat soubor `tools/gamefiles-import/roundtrip.test.mjs`.
