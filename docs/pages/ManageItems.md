# ManageItems.razor
Route: /manage/items
Popis: Hlavní správa itemů — zapínání/vypínání itemů ve hře (`IsEnabled`), výchozí stav nových položek je vypnuto.

## Hotovo ✅
- Tabulka až 2000 itemů (řazení: zapnuté nahoře, pak vnum)
- Hledání (název, přesné vnum), filtr zapnuto/vypnuto, filtr statusu (výchozí `approved`)
- Přepínač `form-switch` na řádku s optimistickým UI a rollbackem při chybě
- Odkaz na detail `/manage/items/{guid}`, odkaz na zdroj
- **Opraveno 2026-09-12:** `ItemType` jako text přes `ContentLabels` (jednotné popisky napříč stránkami); hledání i podle lokalizovaného názvu.

## Chybí / Rozpracováno ⚠️
- `ManageItem.ItemType` je `int?`, sloupec `text` → načtení padá.
- `ItemTypeName()` mapuje čísla (1 = Zbraň, 3 = Prsten…) — neodpovídá DB ani StatsPage.
- Filtr statusu nemá volbu `rejected` / `needs_review`.
- Hledání neprohledává `LocaleName` (ItemsBrowse ano).
- **Zapnutí nemá efekt na hru:** export proto souborů v MCP `IsEnabled` nefiltruje.
- Lze zapnout i `pending`/`rejected` item — chybí pravidlo „zapnout lze jen approved".
- Toast nezmizí.

## Návrhy na vylepšení 💡
- Oprava typu + sdílené mapování typů
- Hromadné akce: vybrat → zapnout/vypnout/schválit
- Řazení kliknutím na hlavičku, `Virtualize` místo LIMIT 2000
- Zakázat přepínač u neschválených (nebo schválit + zapnout jedním krokem s potvrzením)

## Brainstorming poznámky
- Filtr „bez ikony", „bez lokalizace", podle vnum rozsahu
- Indikátor „změněno od posledního exportu" (porovnání `UpdatedAt` s `ProtoFiles.DeployedAt`)
- Tahle stránka + ItemsBrowse + Items (pending) = 3 pohledy na stejnou tabulku → zvážit jednu stránku s taby Pending / Schválené / Vše
