# ManageMobDetail.razor
Route: /manage/mobs/{Guid:guid}
Popis: Detail moba — editace názvu/typu/ranku/popisu, bojové statistiky, gold drop, akce schválení a zapnutí.

## Hotovo ✅
- Načtení plného záznamu včetně bojových statistik
- Editace: název, lokalizovaný název, typ (monster/npc/metin/boss/chest), rank (pawn…king), popis
- Karta statistik: HP, EXP, útok/obrana (fyz./mag.), rychlosti; gold min–max, šance na gold, agresivní dohled
- Schválit / Zamítnout / Zapnout-Vypnout
- `MobType` a `Rank` správně `string`

## Chybí / Rozpracováno ⚠️
- **`DropItems` (jsonb) se nezobrazuje ani needituje** — drop tabulka je pro private server klíčová.
- `Resists` (jsonb), `ImmuneFlags`, `RegenCycle/Percent`, `ScalePercent`, `AggressiveHpPct`, `Size`, `BattleType`, `AttackRange` (načte se, nezobrazí) chybí v UI.
- Level a všechny statistiky jen ke čtení.
- Dropdown ranku nemá `super_pawn` (list stránka ho zná jako „S-Pěšák").
- Stejný audit problém jako ItemDetail (`SetStatusAsync` bez `ApprovedAt` a `ApprovalLog`).

## Návrhy na vylepšení 💡
- Editor drop tabulky: řádky vnum (autocomplete ze schválených itemů) + počet + šance %, validace existence vnum
- Editor odolností (resists) jako mřížka procent
- Editovatelné statistiky s porovnáním proti průměru mobů stejného levelu (odhalí nesmyslná data od agenta)

## Brainstorming poznámky
- Seznam map, kde se mob spawnuje
- Kalkulačka „EXP/HP poměr" a „gold/min" pro balancování
- Náhled řádku `mob_proto.txt`
