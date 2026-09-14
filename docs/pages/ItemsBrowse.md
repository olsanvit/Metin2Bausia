# ItemsBrowse.razor
Route: /items/browse
Popis: Read-only katalog schválených itemů s vyhledáváním a filtry.

## Hotovo ✅
- Počet schválených itemů v nadpisu
- Hledání v názvu / lokalizovaném názvu / přesném vnum (`@bind:event="oninput"`)
- Filtr typu (dropdown generovaný z dat) a filtr zapnuto/vypnuto
- LIMIT 5000 z DB, zobrazeno prvních 500 + upozornění „upřesněte filtr"
- Vypnuté itemy průhledné
- **Opraveno 2026-09-12:** `ItemType` jako text (filtr typů i badge přes `ContentLabels`).

## Chybí / Rozpracováno ⚠️
- `BrowseItem.ItemType` je `int?`, sloupec je `text` → načtení padá; `catch { }` pak ukáže „Žádné schválené itemy" (zavádějící).
- `ApplyFilter()` je prázdná metoda (filtrování už dělá computed property) — mrtvý kód.
- Žádný odkaz na detail itemu.
- `Filtered` se v šabloně vyhodnocuje 3× (Count, Take, Count) — při 5000 řádcích zbytečné.

## Návrhy na vylepšení 💡
- **Sloučit s ManageItems.razor** — zobrazuje totéž, jen bez přepínače. Buď smazat, nebo z ManageItems udělat jedinou stránku s filtrem statusu.
- Pokud zůstane: klik na řádek → `/manage/items/{guid}`, ikona itemu, řazení podle sloupců
- `Virtualize` místo `Take(500)`

## Brainstorming poznámky
- Mohla by se stát veřejným „wiki" katalogem pro hráče (bez auth, jen `IsEnabled = true`) — to je jediný důvod, proč ji držet odděleně od ManageItems
- Export do CSV
