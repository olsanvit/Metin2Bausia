# ManageItemDetail.razor
Route: /manage/items/{Guid:guid}
Popis: Detail jednoho itemu — editace základních údajů, zobrazení cen/limitů/bonusů/hodnot, změna statusu a zapnutí.

## Hotovo ✅
- Načtení plného záznamu (vnum, typ, subtyp, váha, velikost, ceny, limity 0–1, apply 0–2, value 0–5, popis, zdroj, skóre)
- Editace: název, lokalizovaný název, typ (dropdown textových hodnot), popis → „Uložit změny"
- Karty limitů a bonusů (zobrazí se jen když existují), value0–5 v mřížce
- Akce: Schválit / Zamítnout / Zapnout-Vypnout, badge statusu a zapnutí v hlavičce
- `ItemType` je správně `string` (na rozdíl od list stránek)

## Chybí / Rozpracováno ⚠️
- `SetStatusAsync` nenastavuje `ApprovedAt/ApprovedBy/RejectedAt/RejectedReason` a nezapisuje `ApprovalLog` → schválení z detailu nemá audit.
- Value0–5, limity, bonusy, `Weight`, `Size`, `Gold`, `Buy` jsou jen ke čtení — přitom právě ty definují item ve hře.
- `SubType`, `AntiFlags`, `Flags`, `WearFlags`, `ImmuneFlags`, `IconFile`, `ModelFile`, `SummonMobVnum` se vůbec nezobrazují.
- Neuložené změny se při odchodu tiše ztratí.
- `@inject NavigationManager Nav` se nepoužívá.

## Návrhy na vylepšení 💡
- Editovatelné value/limit/apply s validací (číselné rozsahy, výběr apply typu ze seznamu)
- Náhled ikony z `ContentImages`
- Historie změn a schválení z `ApprovalLog` / `AuditLog`
- Navigace „předchozí / další" v rámci aktuálního filtru listu
- Náhled výsledného řádku `item_proto.txt` (jak item vyexportuje MCP)

## Brainstorming poznámky
- Flags jako checkboxy s názvy (ANTI_DROP, ANTI_SELL…) místo čísla
- „Klonovat item" pro rychlé vytvoření +1…+9 variant
- Seznam mobů, kteří item dropují (`Mobs.DropItems` obsahuje vnum)
- Ruční vytvoření itemu (`DataOrigin = 'manual'`) — dnes jde obsah přidat jen přes agenta
