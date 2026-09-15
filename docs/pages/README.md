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
| [Index](Index.md) | `/` | ✅ opraveno 09-12 |
| [Items](Items.md) | `/items` | ✅ opraveno 09-12 |
| [Mobs](Mobs.md) | `/mobs` | ✅ opraveno 09-12 |
| [Review](Review.md) | `/review` | ✅ opraveno 09-12 |
| [Reports](Reports.md) | `/reports` | ✅ opraveno 09-12 |
| [ItemsBrowse](ItemsBrowse.md) | `/items/browse` | ✅ opraveno 09-12; kandidát na sloučení s ManageItems |
| [StatsPage](StatsPage.md) | `/stats` | ✅ opraveno 09-12 |
| [ManageItems](ManageItems.md) | `/manage/items` | ✅ opraveno 09-12 |
| [ManageItemDetail](ManageItemDetail.md) | `/manage/items/{guid}` | ✅ typy OK, chybí editace hodnot |
| [ManageMobs](ManageMobs.md) | `/manage/mobs` | ✅ opraveno 09-12 |
| [ManageMobDetail](ManageMobDetail.md) | `/manage/mobs/{guid}` | ✅ typy OK, chybí DropItems |
| [ManageMaps](ManageMaps.md) | `/manage/maps` | ✅ typy OK |
| [ManageMapDetail](ManageMapDetail.md) | `/manage/maps/{guid}` | ✅ typy OK, chybí SpawnMobs |
| [ManageSystems](ManageSystems.md) | `/manage/systems` | ✅ typy OK |
| [ManageSystemDetail](ManageSystemDetail.md) | `/manage/systems/{guid}` | ✅ typy OK, chybí Files |

Mimo `.razor`: `Pages/Account/Login|ChangePassword|Logout.cshtml` (Razor Pages, cookie auth, heslo v plaintextu v `data/admin-creds.json`).

## Obsah a herní server (2026-09-15)

- **Import herních dat** (`tools/gamefiles-import/`): z `server/src/gamefiles` generuje SQL do `database/import/generated/` (v `.gitignore`, proprietární data). Obsah: 5743 itemů a 1334 mobů s českými názvy, dropy u 372 mobů, 65 map (17 v `MAP_ALLOW`, 28 933 spawnů), 284 questů (238 kompilovaných), 1298 skupin. Migrace `05_imported_content.sql` přidává `Quests.FileName`, tabulku `ContentGroups` a přesnost `DamMultiply`.
- **Round-trip test** `node --test tools/gamefiles-import/roundtrip.test.mjs`: 7/7 — proto soubory i české názvy se vyexportují zpět beze změny.
- **Admin** zobrazuje symbolické hodnoty z proto (`ITEM_WEAPON`, `STONE`, `S_PAWN`) česky přes `ContentLabels`; tab Metiny bere i `STONE`.
- **MCP export** zapisuje proto v původním kódování (latin1 + `Metadata.protoName`) a nový nástroj `export_names_txt` vytváří `item_names_<locale>.txt` / `mob_names_<locale>.txt` v CP1250.
- **Herní server**: `server/docker` odvozen od upstream deploye (`server/deploy`, v `.gitignore`): MySQL 5.5, web (Laravel, zakládá SQL schéma), db, auth, 3 jádra kanálu 1 a game99; image se builduje z `server/src`, názvy česky. Runbook v `server/README.md`. Build a start neověřeny (lokálně není Docker).
- **Neaplikováno na živou DB:** migrace 05 ani import — QNAP je v RAID resyncu.
