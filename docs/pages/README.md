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

- **Tiché chyby v detailních stránkách:** `catch (Exception ex) { _msg = ... }` bez logování.
- **Toast** (`.toast-msg`) nikdy nezmizí — chybí timer.
- **Audit:** `SetStatusAsync` v detailech nenastavuje `ApprovedAt/ApprovedBy/RejectedAt` a nezapisuje `ApprovalLog`.
- **Export do hry:** MCP `export_item_proto_txt` / `export_mob_proto_txt` ignoruje `IsEnabled` → přepínače na Manage stránkách nemají na hru vliv.

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
| [ManageItemDetail](ManageItemDetail.md) | `/manage/items/{guid}` | ✅ lokalizováno, chybí editace hodnot |
| [ManageMobs](ManageMobs.md) | `/manage/mobs` | ✅ opraveno 09-12, lokalizováno |
| [ManageMobDetail](ManageMobDetail.md) | `/manage/mobs/{guid}` | ✅ dropy 09-15, lokalizováno |
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
- Build 0 chyb; přepnutí jazyka v prohlížeči neověřeno (vyžaduje přihlášení). `LangSwitcher` nabízí i DE, ale `AddSimpleLocalization` zná jen cs/en.
