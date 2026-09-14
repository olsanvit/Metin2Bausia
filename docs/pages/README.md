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
