# ManageMobDetail.razor
Route: /manage/mobs/{Guid:guid}
Popis: Detail moba — editace názvu/typu/ranku/popisu, bojové statistiky, gold drop, akce schválení a zapnutí.

## Hotovo ✅
- Načtení plného záznamu včetně bojových statistik
- Editace: název, lokalizovaný název, typ (monster/npc/metin/boss/chest), rank (pawn…king), popis
- Karta statistik: HP, EXP, útok/obrana (fyz./mag.), rychlosti; gold min–max, šance na gold, agresivní dohled
- Schválit / Zamítnout / Zapnout-Vypnout včetně razítka `ApprovedAt/ApprovedBy` a zápisu do `ApprovalLog`
- `MobType` a `Rank` správně `string`
- Editovatelné hodnoty (level, statistiky, gold, dohled) a indikátor neuložených změn u tlačítka Uložit
- Drop tabulka (`DropItems`) se zobrazuje po skupinách; na mobilu místo tabulky karty, na desktopu tabulka
- Akce po ruce: na desktopu přilepené v pravém sloupci, pod 992 px lišta u spodního okraje
- „Zdroj dat" a „Kvalita dat" jako rozbalovátka (na mobilu sbalená, od 768 px vždy otevřená)

## Chybí / Rozpracováno ⚠️
- **`DropItems` se jen zobrazuje, needituje** — editor dropů zatím chybí.
- `Resists` (jsonb), `ImmuneFlags`, `RegenCycle/Percent`, `ScalePercent`, `AggressiveHpPct`, `Size`, `BattleType`, `AttackRange` (načte se, nezobrazí) chybí v UI.
- Dropdown ranku nemá `super_pawn` (list stránka ho zná jako „S-Pěšák").

## Návrhy na vylepšení 💡
- Editor drop tabulky: řádky vnum (autocomplete ze schválených itemů) + počet + šance %, validace existence vnum
- Editor odolností (resists) jako mřížka procent
- Editovatelné statistiky s porovnáním proti průměru mobů stejného levelu (odhalí nesmyslná data od agenta)

## Brainstorming poznámky
- Seznam map, kde se mob spawnuje
- Kalkulačka „EXP/HP poměr" a „gold/min" pro balancování
- Náhled řádku `mob_proto.txt`
