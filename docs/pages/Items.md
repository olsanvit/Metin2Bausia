# Items.razor
Route: /items
Popis: Fronta itemů, které AI agent nasbíral (`ContentStatus = 'pending'`) a čekají na schválení/zamítnutí.

## Hotovo ✅
- Tabulka max 200 pending itemů seřazená podle `ConfidenceScore` a data
- Barevný badge confidence (≥80 / ≥50 / <50), odkaz na zdroj
- Tlačítka ✓ / ✗ na řádku, blokace během zpracování (`_processing`)
- Po schválení zápis do `ApprovalLog`, řádek zmizí z tabulky
- **Opraveno 2026-09-12:** `ApprovedAt/ApprovedBy` místo neexistujících `ApprovalAt/ApprovalBy`; `ItemType` jako text přes `ContentLabels`; zamítnutí se zapisuje do `ApprovalLog`.

## Chybí / Rozpracováno ⚠️
- **Schválení padá:** UPDATE zapisuje `"ApprovalAt"` a `"ApprovalBy"`, schéma má `"ApprovedAt"` a `"ApprovedBy"` → `column does not exist`.
- **Načtení padá:** `PendingItem.ItemType` je `int?`, sloupec je `text` → Dapper výjimka → stránka ukáže chybu DB.
- Zamítnutí nezapisuje `ApprovalLog` (schválení ano) — nekonzistentní audit.
- Důvod zamítnutí natvrdo `manual_reject`, nelze zadat.
- Žádné potvrzení akce, žádné undo.
- `ApprovalBy` natvrdo `'admin'` místo jména přihlášeného uživatele.

## Návrhy na vylepšení 💡
- Opravit názvy sloupců + typ `ItemType` na `string`
- Hromadné akce: checkboxy + „Schválit vybrané", „Schválit vše s confidence ≥ X"
- Rozbalovací řádek s detailem (limity, bonusy, value0–5) bez odchodu ze stránky
- Dialog s důvodem zamítnutí (výběr + volný text) → `RejectedReason` + `ApprovalLog.Reason`
- Klávesové zkratky (A = schválit, R = zamítnout, ↑/↓ = pohyb)

## Brainstorming poznámky
- Pokud vnum už existuje jako approved, ukázat diff (co agent mění oproti schválené verzi)
- Náhled ikony z `ContentImages` (EntityTable = 'Items')
- Sloučit s Mobs.razor do jedné univerzální fronty `/approve?entity=Items|Mobs|Maps|Skills|Quests|Systems` — dnes jsou to dvě kopie stejného kódu a Maps/Skills/Quests/Systems frontu nemají vůbec
- Po schválení nabídnout rovnou „a zapnout ve hře" (IsEnabled) — dnes jsou to dva kroky na dvou stránkách
