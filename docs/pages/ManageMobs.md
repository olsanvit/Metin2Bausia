# ManageMobs.razor
Route: /manage/mobs
Popis: Správa mobů, NPC, metinů a bossů — zapínání/vypínání ve hře, filtrování podle kategorie.

## Hotovo ✅
- Tabulka až 5000 záznamů, zapnuté nahoře
- Taby Vše / Monstři / NPC / Metiny / Bossové
- Hledání (název, vnum), filtr zapnuto/vypnuto
- Přepínač s optimistickým UI a rollbackem, odkaz na detail
- Heuristika pro metiny: vnum 8001–8999
- **Opraveno 2026-09-12:** `MobType`/`Rank` jako text; taby filtrují textové hodnoty (dřív byly prázdné); hledání i podle lokalizovaného názvu.

## Chybí / Rozpracováno ⚠️
- `ManageMob.MobType` a `Rank` jsou `int?`, sloupce `text` (`monster|npc|boss|...`, `pawn|knight|boss|king`) → načtení padá.
- Taby filtrují `MobType == 0/1/2` a `Rank >= 4` → i po opravě typu by byly prázdné (kromě vnum heuristiky pro metiny).
- `MobTypeName` / `RankName` / badge mapují čísla.
- Hledání neprohledává `LocaleName`, chybí filtr statusu (ManageItems ho má).
- Zapnutí nemá efekt na hru (export ignoruje `IsEnabled`).

## Návrhy na vylepšení 💡
- Přepsat typy a taby na textové hodnoty
- Filtr podle levelového rozsahu
- Sloupec „na kolika mapách se spawnuje" (z `Maps.SpawnMobs`)
- Hromadné zapnutí celé kategorie (např. všechny metiny do levelu 40)

## Brainstorming poznámky
- Varování, když je mob zapnutý, ale žádná zapnutá mapa ho nespawnuje (mrtvý obsah)
- Varování, když mob dropuje vypnutý / neschválený item
- Pro bossy zvláštní pohled s respawn časy (`RegenCycle`)
