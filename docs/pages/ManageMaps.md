# ManageMaps.razor
Route: /manage/maps
Popis: Správa map — zapínání/vypínání přístupnosti map.

## Hotovo ✅
- Tabulka všech map (zapnuté nahoře, pak map index) bez limitu
- Hledání v názvu, filtr zapnuto/vypnuto
- Přepínač s optimistickým UI a rollbackem, odkaz na detail
- Typy modelu odpovídají schématu — jediná list stránka bez typové chyby

## Chybí / Rozpracováno ⚠️
- Chybí sloupec typu mapy (`MapType`), filtr statusu, hledání podle indexu.
- Není fronta pending map (schvalovat lze jen z detailu po jedné).
- Vypnutí mapy nemá žádný mechanismus, jak se propíše do herního serveru (mapy nejsou v proto exportu; řídí se `server/game/conf.cfg` MAP_ALLOW).

## Návrhy na vylepšení 💡
- Sloupec typu + filtr (field / dungeon / pvp / guild / empire)
- Počet spawnovaných mobů (délka `SpawnMobs`)
- Náhledový obrázek jako miniatura

## Brainstorming poznámky
- Generování řádku `MAP_ALLOW` pro `conf.cfg` ze zapnutých map (přes MCP `update_file`) — tím by přepínač začal mít reálný dopad
- Rozdělení map mezi kanály/cores
- Mapový pohled (grid podle BaseX/BaseY) — atlas světa
