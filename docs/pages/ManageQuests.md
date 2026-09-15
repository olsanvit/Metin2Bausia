# ManageQuests.razor
Route: /manage/quests
Popis: Seznam questů importovaných z `server/src/gamefiles/data/quest` se zapínáním. Zapnutý quest odpovídá tomu, že ho server kompiluje (`locale_list`).
Platforma: Web

## Hotovo ✅
- Tabulka 284 questů: soubor, název questu, kódování zdroje (UTF-8 / EUC-KR), velikost skriptu, status
- Přepínač `IsEnabled` s okamžitým zápisem a vrácením při chybě
- Hledání v názvu i souboru, filtr zapnuté/vypnuté, počet zobrazených
- Skript se do seznamu nenačítá (některé mají přes 100 kB), jen jeho délka
- Texty přes `IStringLocalizer` (cs, en)

## Chybí / Rozpracováno ⚠️
- Přepnutí se nezapisuje do `ApprovalLog`
- Export zpět do `locale_list` pro server zatím neexistuje — přepínač je jen evidence
- Klíčem je `FileName`: 6 názvů questů je ve dvou souborech, v seznamu tak mohou být dva řádky se stejným názvem

## Návrhy na vylepšení 💡
- Export `locale_list` ze zapnutých questů přes MCP (jako proto soubory)
- Filtr podle kódování a upozornění na EUC-KR soubory, které server čte jinak
- Řazení podle velikosti skriptu

## Brainstorming poznámky
- Dvojice souborů se stejným názvem questu (např. `deviltower_zone`) by si zasloužila varování — server nahraje obě verze
- Zapnutí questu, který v `locale_list` nebyl, nemusí po kompilaci fungovat (závislosti na `questlib.lua`)
