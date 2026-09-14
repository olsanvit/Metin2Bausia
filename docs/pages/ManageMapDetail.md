# ManageMapDetail.razor
Route: /manage/maps/{Guid:guid}
Popis: Detail mapy — editace názvu/typu/popisu, rozměry a levelový rozsah, náhled, akce schválení a zapnutí.

## Hotovo ✅
- Načtení záznamu (index, typ, rozměry, min/max level, popis, náhled, zdroj, skóre)
- Editace: název, lokalizovaný název, typ mapy (field/dungeon/pvp/guild/empire/boat), popis
- Náhledový obrázek (`PreviewImageUrl`), pokud existuje
- Karta rozměrů, Schválit / Zamítnout / Zapnout-Vypnout

## Chybí / Rozpracováno ⚠️
- **`SpawnMobs` (jsonb) se nezobrazuje** — co se na mapě spawnuje, je u mapy to hlavní.
- `MapFiles`, `BaseX/BaseY`, `CellScale` chybí.
- Min/max level je v UI dvakrát (editační řádek readonly + karta rozměrů) a ani jednou nejde upravit.
- Audit problém jako ostatní detaily (`SetStatusAsync` bez `ApprovedAt` a `ApprovalLog`).
- Zapnutí se nepropisuje do herního serveru (viz ManageMaps.md — `MAP_ALLOW` v `server/game/conf.cfg`).

## Návrhy na vylepšení 💡
- Editor spawnů: vnum moba (autocomplete ze schválených mobů) + počet + pozice/oblast
- Editovatelný levelový rozsah
- Seznam spawnovaných mobů s odkazem na jejich detail a upozorněním na vypnuté/neschválené

## Brainstorming poznámky
- Varování, když level mobů ve spawnu neodpovídá min/max levelu mapy
- Mapové soubory (`MapFiles`) — kontrola, jestli existují v `server/share/` přes MCP `list_files`
