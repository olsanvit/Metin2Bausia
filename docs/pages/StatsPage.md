# StatsPage.razor
Route: /stats
Popis: Souhrnné statistiky management DB — počty entit, rozpad itemů podle typu, mobů podle ranku, rozložení confidence a levelů.

## Hotovo ✅
- 4 souhrnné karty (Items, Mobs, Maps, Systems) včetně počtu schválených/zapnutých
- 20 COUNT dotazů paralelně přes `Task.WhenAll` (commit 5165b44) — každý dotaz má vlastní spojení, takže je to bezpečné
- Tabulky s progress bary: itemy podle typu (top 15), mobové podle ranku
- Pásma confidence itemů a levelů mobů
- **Opraveno 2026-09-12:** `ItemType`/`Rank` jako text přes sdílený `ContentLabels`; `Systems` se počítají s filtrem `IsDeleted`; chyba DB se zobrazí.

## Chybí / Rozpracováno ⚠️
- `TypeCount.ItemType` a `RankCount.Rank` jsou `int?`, sloupce jsou `text` → mapování padá; celé načtení je v jednom `try` s `catch { }`, takže spadne celá stránka do nul.
- `ItemTypeName()` / `MobRankName()` mapují čísla — neodpovídá textovým hodnotám v DB a navíc se liší od mapování v ManageItems (3 = Helma vs. 3 = Prsten).
- `Systems` count bez `IsDeleted = FALSE` (ostatní ho mají).
- Chybí Skills, Quests, ContentImages.
- 8 COUNT dotazů na pásma by šlo nahradit jedním `GROUP BY CASE ...`.

## Návrhy na vylepšení 💡
- Opravit typy na `string`, jednotné mapování názvů do sdíleného helperu (jeden zdroj pravdy pro všechny stránky)
- Přidat Skills/Quests/obrázky
- Pokrytí ikon: kolik schválených itemů/mobů nemá `ContentImages`

## Brainstorming poznámky
- Vývoj v čase: schváleno/zamítnuto za týden (z `ApprovalLog`)
- Kvalita podle zdroje: průměrná confidence a míra zamítnutí per `SourceName` → podklad pro vypnutí špatných zdrojů
- Díry ve vnum rozsazích (např. chybějící +0…+9 u zbraní)
- Grafy místo progress barů, pokud přibude knihovna
