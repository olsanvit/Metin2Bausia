# ManageGroups.razor
Route: /manage/groups
Popis: Skupiny bez vlastní entity — skupiny mobů pro spawny (`group.txt`), skupiny skupin (`group_group.txt`) a skupiny předmětů (`special_item_group.txt`).
Platforma: Web

## Hotovo ✅
- Taby podle typu s počty (802 / 211 / 285)
- Vnum, název, vůdce skupiny mobů, druh skupiny předmětů, počet položek
- Rozbalení řádku kliknutím ukáže položky tak, jak jsou v souboru
- Varování ⚠ u skupin, které byly ve zdroji dvakrát (platí první výskyt, druhý je v metadatech)
- Hledání v názvu a přesném vnum, limit 500 řádků
- Texty přes `IStringLocalizer` (cs, en)

## Chybí / Rozpracováno ⚠️
- Položky se zobrazují jako surové sloupce souboru, bez dohledání jmen mobů a itemů
- Skupiny nejde upravovat ani zapínat
- Názvy skupin jsou v originále korejsky, u položek skupin mobů je jméno moba německy (tak je to ve zdroji)

## Návrhy na vylepšení 💡
- U skupin mobů dohledat moby podle vnum a odkazovat na jejich detail
- U skupin předmětů dohledat itemy a zobrazit šance jako procenta
- Zpětná vazba: v detailu mapy odkaz na skupinu ze spawnu

## Brainstorming poznámky
- `special_item_group` pohání truhly a bonusové předměty — editor by dával smysl pro eventy
- Duplicitní skupiny (např. `special_item_group` 10003) jsou pravděpodobně chyba ve zdrojových datech
