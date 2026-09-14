# Review.razor
Route: /review
Popis: Fronta konfliktů, které agent nedokázal vyřešit sám (`ManualReviewQueue`) — např. dva zdroje s rozdílnými daty pro stejné vnum.

## Hotovo ✅
- Tabulka nevyřešených položek (entita, důvod, zdroj A/B, datum)
- Tlačítko „Vyřešeno" odebere položku z fronty
- **Opraveno 2026-09-12:** místo neexistujících `Resolved/SourceA/SourceB` se pracuje se sloupcem `Status` (`pending` → `resolved`); chyba DB se zobrazí místo hlášky „fronta je prázdná".

## Chybí / Rozpracováno ⚠️
- **Stránka je nefunkční celá:** čte a zapisuje `"Resolved"`, `"SourceA"`, `"SourceB"` — v `ManualReviewQueue` nic z toho není. Tabulka má `EntityTable`, `EntityGuid`, `Reason`, `Status` (default `pending`), `Metadata` (jsonb).
- `catch { }` bez hlášky — uživatel vidí „Fronta je prázdná", i když dotaz spadl. To je zavádějící (tvrdí úspěch).
- „Vyřešeno" jen zavře položku, nic neřeší — nezmění dotčenou entitu, nezapíše důvod.
- Model `ManualReviewItem` chybí `Status`, `Metadata`.

## Návrhy na vylepšení 💡
- Přepsat na `Status` (`pending` → `resolved`/`dismissed`) a zdroje číst z `Metadata` (dohodnout s agentem formát, např. `{sourceA:{...}, sourceB:{...}}`)
- Zobrazit obě varianty dat vedle sebe s tlačítky „Použít A" / „Použít B" / „Ručně"
- Odkaz na detail dotčené entity (`/manage/{table}/{EntityGuid}`)

## Brainstorming poznámky
- Rozlišit typy důvodů (duplicitní vnum, konflikt hodnot, podezřelá hodnota, chybějící povinné pole) a pro každý jiné UI
- Po vyřešení zapsat do `ApprovalLog` akci `resolved`
- Počet v NavMenu jako badge (dnes jen na dashboardu)
