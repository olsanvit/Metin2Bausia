# Mobs.razor
Route: /mobs
Popis: Fronta mobů / NPC / metinů nasbíraných agentem, čekajících na schválení.

## Hotovo ✅
- Tabulka max 200 pending mobů (vnum, jméno, level, rank, zdroj, confidence)
- ✓ / ✗ na řádku, zápis do `ApprovalLog` při schválení
- Převod ranku na text (`RankLabel`)
- **Opraveno 2026-09-12:** `ApprovedAt/ApprovedBy`; `Rank` jako text přes `ContentLabels`; zamítnutí se zapisuje do `ApprovalLog`.

## Chybí / Rozpracováno ⚠️
- **Schválení padá:** stejná chyba jako Items — `ApprovalAt`/`ApprovalBy` místo `ApprovedAt`/`ApprovedBy`.
- **Načtení padá:** `PendingMob.Rank` je `int?`, sloupec je `text` (`pawn|knight|boss|king`) → výjimka. `RankLabel()` mapuje čísla, takže i po opravě typu by ukazoval „?".
- Zamítnutí nezapisuje `ApprovalLog`.
- Chybí sloupec typu (monster/npc/metin), odkaz na zdroj (`SourceUrl` se nenačítá), `_processing` se nevizualizuje (Items má šedý řádek, Mobs ne).

## Návrhy na vylepšení 💡
- Oprava sloupců a typů, `RankLabel` přepsat na textové hodnoty
- Zobrazit HP/EXP/level vedle sebe — pro rozhodnutí o schválení mobů je to podstatnější než confidence
- Hromadné akce stejně jako u itemů

## Brainstorming poznámky
- Kandidát na sloučení s Items.razor do jedné generické fronty (viz Items.md)
- U mobů zobrazit i počet položek v `DropItems` a jestli všechny dropnuté vnum existují mezi schválenými itemy (jinak mob po exportu dropuje neexistující item)
