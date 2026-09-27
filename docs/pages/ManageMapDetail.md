# ManageMapDetail.razor
Route: /manage/maps/{Guid:guid}
Popis: Detail mapy — editace názvu/typu/popisu, rozměry a levelový rozsah, náhled, akce schválení a zapnutí.

## Hotovo ✅
- Schvalování zapisuje `ApprovedAt`/`ApprovedBy` i řádek do `ApprovalLog`
- Načtení záznamu (index, typ, rozměry, min/max level, popis, náhled, zdroj, skóre)
- Editace: název, lokalizovaný název, typ mapy (field/dungeon/pvp/guild/empire/boat), popis
- Náhledový obrázek (`PreviewImageUrl`), pokud existuje
- Karta rozměrů, Schválit / Zamítnout / Zapnout-Vypnout
- Spawny (`SpawnMobs`) po souborech s filtrem, limit 500 řádků; na mobilu karty místo sedmisloupcové tabulky
- Akce po ruce: na desktopu přilepené v pravém sloupci, pod 992 px lišta u spodního okraje, tlačítko Uložit značí neuložené změny
- „Zdroj dat" a „Kvalita dat" jako rozbalovátka (na mobilu sbalená, od 768 px vždy otevřená)

## Chybí / Rozpracováno ⚠️
- **Spawny se jen zobrazují, needitují** — editor spawnů chybí.
- `MapFiles`, `BaseX/BaseY`, `CellScale` chybí.
- Min/max level je v UI dvakrát (editační řádek readonly + karta rozměrů) a ani jednou nejde upravit.
- Zapnutí se nepropisuje do herního serveru (viz ManageMaps.md — `MAP_ALLOW` v `server/game/conf.cfg`).

## Návrhy na vylepšení 💡
- Editor spawnů: vnum moba (autocomplete ze schválených mobů) + počet + pozice/oblast
- Editovatelný levelový rozsah
- Seznam spawnovaných mobů s odkazem na jejich detail a upozorněním na vypnuté/neschválené

## Brainstorming poznámky
- Varování, když level mobů ve spawnu neodpovídá min/max levelu mapy
- Mapové soubory (`MapFiles`) — kontrola, jestli existují v `server/share/` přes MCP `list_files`
