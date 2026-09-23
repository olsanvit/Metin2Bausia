# Dokumentace stránek — Metin2Bausia.Web

Jedna MD per routovatelná `.razor` stránka (`@page`). Stav k 2026-09-11, vzniklo čtením kódu — aplikace nebyla spuštěna.

## Opraveno 2026-09-12

- **Interaktivita:** `App.razor` načítá `blazor.web.js`, každá stránka má `@rendermode InteractiveServer` (layout zůstává SSR), `Program.cs` doplněn o `app.MapStaticAssets()` — bez něj vracel `_framework/blazor.web.js` 404. Přibyl `Properties/launchSettings.json` (profil `http`, Development, port 5099).
- **Bootstrap:** `App.razor` načítá Bootswatch přes `ThemeService` + `data-bs-theme`.
- **Model vs. schéma:** `ItemType`/`MobType`/`Rank` jako text, `ApprovedAt/ApprovedBy`, `ManualReviewQueue.Status`, `AgentRunReports` na skutečné sloupce. Popisky sjednoceny do `Data/ContentLabels.cs`.
- **Tiché chyby:** seznamové stránky chybu DB zobrazí místo `catch { }`.
- `LangSwitcher` v NavMenu se konečně renderuje (chyběl `@using`).
- **Audit:** `ApprovedBy` i `ApprovalLog.Operator` berou jméno přihlášeného uživatele; detailní stránky razítkují `ApprovedAt`/`RejectedAt` a zapisují do `ApprovalLog`.
- **Heslo admina** se ukládá jako PBKDF2 hash (migrace ze starého plaintextu proběhne při prvním přihlášení).
- **Export do hry:** `export_item_proto_txt` / `export_mob_proto_txt` respektují `IsEnabled` a generují **skutečný formát** — 33 sloupců u itemů, 71 u mobů, symbolické hodnoty (`ITEM_WEAPON`, `WEAPON_SWORD`, `PAWN`, `AGGR,BERSERK`). Skládání řádku je v `mcp/proto-format.js`, migrace schématu v `database/postgres/04_proto_fields.sql`.
  Ověřeno round-tripem proti reálným souborům v `server/src/gamefiles/conf/`: 5743 itemů a 1334 mobů projde beze změny.

Ověřeno buildem (0 chyb, 0 varování) a během aplikace na localhost:5099 — **ne proti živé databázi** (lokálně není Docker ani psql, port 5442 na QNAPu odsud nedostupný) a **ne proklikáním v adminu** (vyžaduje přihlášení).

## Průřezové problémy (platí pro všechny stránky)

- ✅ **Tiché chyby** (09-15): stránky dědí `Components/AdminPageBase` (přes `Pages/_Imports.razor`); `ShowError(ex)` / `LogError(ex)` zapíšou výjimku do logu se jménem stránky a operace. Prázdný `catch { }` v ItemsBrowse odstraněn.
- ✅ **Toast** (09-15): komponenta `Components/Shared/ToastMessage` — úspěch zmizí po 4 s, chyba zůstává do zavření (tlačítko ×). Zobrazí se jen pro nové `ShowOk`/`ShowError` (počítadlo), takže psaní do filtrů hlášku neoživí. Items a Mobs místo `alert` používají stejný toast.
- ✅ **Audit:** `SetStatusAsync` v detailech zapisuje `ApprovedAt/ApprovedBy/RejectedAt` a `ApprovalLog`.
- ✅ **Export do hry:** MCP export proto má filtr `onlyEnabled`.

## Stránky

| Stránka | Route | Stav |
|---|---|---|
| [Index](Index.md) | `/` | ✅ opraveno 09-12, lokalizováno |
| [Items](Items.md) | `/items` | ✅ opraveno 09-12, lokalizováno |
| [Mobs](Mobs.md) | `/mobs` | ✅ opraveno 09-12, lokalizováno |
| [Review](Review.md) | `/review` | ✅ opraveno 09-12, lokalizováno |
| [Reports](Reports.md) | `/reports` | ✅ opraveno 09-12, lokalizováno |
| [ItemsBrowse](ItemsBrowse.md) | `/items/browse` | ✅ opraveno 09-12, lokalizováno; kandidát na sloučení s ManageItems |
| [StatsPage](StatsPage.md) | `/stats` | ✅ opraveno 09-12, lokalizováno |
| [ManageItems](ManageItems.md) | `/manage/items` | ✅ opraveno 09-12, lokalizováno |
| [ManageItemDetail](ManageItemDetail.md) | `/manage/items/{guid}` | ✅ editace hodnot 09-16, lokalizováno |
| [ManageMobs](ManageMobs.md) | `/manage/mobs` | ✅ opraveno 09-12, lokalizováno |
| [ManageMobDetail](ManageMobDetail.md) | `/manage/mobs/{guid}` | ✅ dropy 09-15, editace statistik 09-16, lokalizováno |
| [ManageMaps](ManageMaps.md) | `/manage/maps` | ✅ lokalizováno |
| [ManageMapDetail](ManageMapDetail.md) | `/manage/maps/{guid}` | ✅ spawny 09-15, lokalizováno |
| [ManageSystems](ManageSystems.md) | `/manage/systems` | ✅ lokalizováno |
| [ManageSystemDetail](ManageSystemDetail.md) | `/manage/systems/{guid}` | ✅ lokalizováno, chybí Files |
| [ManageQuests](ManageQuests.md) | `/manage/quests` | 🆕 09-15, lokalizováno |
| [ManageQuestDetail](ManageQuestDetail.md) | `/manage/quests/{guid}` | 🆕 09-15, lokalizováno |
| [ManageGroups](ManageGroups.md) | `/manage/groups` | 🆕 09-15, lokalizováno |

Mimo `.razor`: `Pages/Account/Login|ChangePassword|Logout.cshtml` (Razor Pages, cookie auth, heslo v plaintextu v `data/admin-creds.json`).

## Obsah a herní server (2026-09-15)

- **Import herních dat** (`tools/gamefiles-import/`): z `server/src/gamefiles` generuje SQL do `database/import/generated/` (v `.gitignore`, proprietární data). Obsah: 5743 itemů a 1334 mobů s českými názvy, dropy u 372 mobů, 65 map (17 v `MAP_ALLOW`, 28 933 spawnů), 284 questů (238 kompilovaných), 1298 skupin. Migrace `05_imported_content.sql` přidává `Quests.FileName`, tabulku `ContentGroups` a přesnost `DamMultiply`.
- **Round-trip test** `node --test tools/gamefiles-import/roundtrip.test.mjs`: 7/7 — proto soubory i české názvy se vyexportují zpět beze změny.
- **Admin** zobrazuje symbolické hodnoty z proto (`ITEM_WEAPON`, `STONE`, `S_PAWN`) česky přes `ContentLabels`; tab Metiny bere i `STONE`.
- **MCP export** zapisuje proto v původním kódování (latin1 + `Metadata.protoName`) a nový nástroj `export_names_txt` vytváří `item_names_<locale>.txt` / `mob_names_<locale>.txt` v CP1250.
- **Herní server**: `server/docker` odvozen od upstream deploye (`server/deploy`, v `.gitignore`): MySQL 5.5, web (Laravel, zakládá SQL schéma), db, auth, 3 jádra kanálu 1 a game99; image se builduje z `server/src`, názvy česky. Runbook v `server/README.md`. Build a start neověřeny (lokálně není Docker).
- **Neaplikováno na živou DB:** migrace 05 ani import — QNAP je v RAID resyncu.

## Lokální prostředí a úklid (2026-09-15)

- **Lokální PostgreSQL 18** (Homebrew, port 5442, data v `~/.local/share/metin2bausia-pg18`): migrace 01–05 i celý import proběhly za 2 s. Export z databáze se shoduje s originálními herními soubory bajt po bajtu (5743 itemů, 1334 mobů, oboje názvy); opakovaný import je idempotentní. Postup je v `CLAUDE.md`.
- **`CLAUDE.md`** nově popisuje strukturu, databáze, nasazení, pravidla a pasti (rendermode, MapStaticAssets, `.gitignore` a velikost písmen, formáty proto souborů).
- **`deploy.sh` z repa odstraněn** — nasazoval compose stack, který na QNAPu vůbec neběží. Platí `~/deploy-to-qnap.sh metin2bausia` a `server/README.md`.
- **Povolené mapy** bere import z `server/docker/docker-compose.yml` (rozdělení map mezi jádra), starý `server/game/conf.cfg` zůstává jako záloha.

## Lokalizace (2026-09-15)

- Všech 18 stránek, `NavMenu` i `MainLayout` jdou přes `@S["Klic"]` (`IStringLocalizer<SharedResources>` z `_Imports.razor`); texty v `Resources/SharedResources.resx` (cs) a `.en.resx`, 294 klíčů.
- Nové klíče přidávat přes `python3 tools/i18n/resx.py klice.json` (`{"Klic": ["česky", "english"]}`) — drží oba soubory v souladu.
- Úvody s `<strong>` jsou v resx jako HTML a vykreslují se přes `MarkupString`.
- Build 0 chyb. Němčina doplněna 09-16 (viz níže).

## Editace hodnot a testy stránek (2026-09-16)

- **Detail itemu** edituje ceny, váhu, velikost, hodnoty limitů a bonusů a `value0`–`value5`. Symbolické typy (`LEVEL`, `APPLY_STR`) zůstávají jen ke čtení — server je čte jako konstanty.
- **Detail moba** edituje level, HP, EXP, útok, obranu, rychlosti, gold a agresivní dohled.
- **Ochrana před importem:** uložení přepne `DataOrigin` z `imported` na `manual`; opakovaný import přepisuje jen řádky `imported`, ruční úpravy tedy zůstanou. Detail zobrazuje původ dat a upozornění.
- **Opravené chyby:** `DetailItem.SubType` byl `int?`, v DB je `text` (`WEAPON_SWORD`) — detail importovaného itemu by nešel načíst. Statistiky padaly, jakmile byla v DB data (`COUNT(*)` je `bigint`, záznamy mají `int`).
- **Testy** (`src/Metin2Bausia.Tests/AdminPagesTests.cs`): všech 13 stránek adminu a oba detaily se vykreslí s testovacím přihlášeným adminem (`AdminAppFactory`, schéma autentizace jen v testech). Prázdnou DB si test sám naplní migracemi a vzorkem. Lokálně: `ConnectionStrings__Metin2Bausia="Host=localhost;Port=5442;Database=m2b_test;Username=postgres" dotnet test src/Metin2Bausia.Tests/`.

## Němčina a zapínání při importu (2026-09-16)

- **Němčina:** `Resources/SharedResources.de.resx` (298 klíčů), `Program.cs` rozšiřuje podporované jazyky na cs/en/de (SharedServices zná jen cs/en a sdílí ho víc projektů). `<html lang>` se řídí zvoleným jazykem. Test `ManageItems_RendersInSelectedLanguage` ověřuje všechny tři jazyky přes cookie z `/set-culture`. `tools/i18n/resx.py` přijímá třetí hodnotu (de); `null` nechá stávající překlad.
- **IsEnabled při opakovaném importu:** migrace `06_is_enabled_locked.sql` přidává `IsEnabledLocked` (Items, Mobs, Maps, Quests, ContentGroups). Přepínač v adminu řádek zamkne; import převezme zapnutí ze serverových souborů (`MAP_ALLOW`, `locale_list`) jen u nezamčených řádků. Ověřeno na lokální DB: zamčený vypnutý item zůstal vypnutý, nezamčený se znovu zapnul.
- **Produkce:** migrace 06 se musí nahrát spolu s 05 před importem na QNAP.

## Responzivita a scrollování (2026-09-23)

- **Scrolluje jen obsah.** `.page` má výšku okna (`100dvh`), `main` je sloupcový flex a `.content` jediný `overflow: auto`. Dřív `min-height: 100vh` nechalo růst celou stránku, takže se sidebar i horní lišta odsouvaly pryč.
- **Sidebar** má vlastní `overflow-y` (dlouhé menu na nízkém okně) a `main` `min-width: 0`, aby široká tabulka neroztáhla layout přes okno.
- **Mobil pod 768 px:** sidebar je vysouvací menu s tlačítkem ☰ v horní liště, ovládané skrytým checkboxem `#nav-toggle` (layout je SSR, `@onclick` tu nefunguje). Klik mimo menu ho zavře; po enhanced navigaci ho zavírá skript v `App.razor`.
- **Tabulky:** všech 15 tabulek je v `table-responsive`, posouvají se vodorovně samy, ne obsah. Toolbary v ManageMaps a ManageSystems mají `flex-wrap`.
- **Ověřeno v prohlížeči** na vykreslených stránkách (15 stránek × 1280/768/375 px): `body` se neposouvá svisle ani vodorovně, `.content` scrolluje svisle, horní lišta zůstává nahoře, sidebar je na mobilu zasunutý a otevírá i zavírá se. Přihlašovací stránka (vlastní styly, `app.css` nenačítá) je na 375 px bez přetečení.
