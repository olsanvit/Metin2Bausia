# ManageSystems.razor
Route: /manage/systems
Popis: Přehled herních systémů (enchant, alchymie, crafting, pet, mount…) ve formě karet se zapínáním.

## Hotovo ✅
- Karty systémů (název → detail, kategorie, zkrácený popis na 100 znaků)
- Přepínač zapnutí na kartě s rollbackem, zelený/šedý okraj podle stavu
- Hledání v názvu, filtr zapnuto/vypnuto
- Typy modelu odpovídají schématu

## Chybí / Rozpracováno ⚠️
- Chybí filtr podle kategorie a složitosti, badge statusu (pending/approved) na kartě.
- „Zapnuto" u systému nic nedělá — systém vyžaduje implementaci v C++ zdrojích serveru, přepínač je jen evidence.
- Zkracování popisu přes `[..Math.Min(...)]` přímo v šabloně — funguje, ale čitelněji v helperu.

## Návrhy na vylepšení 💡
- Místo zapnuto/vypnuto workflow stavů: nápad → schváleno → implementuje se → hotovo → aktivní
- Seskupení karet podle kategorie
- Badge složitosti (low/medium/high) přímo na kartě

## Brainstorming poznámky
- Kanban pohled pro implementaci systémů (sloupce podle stavu) — reálně je to backlog vývoje serveru
- Závislosti mezi systémy (pet systém potřebuje item typ X)
- Odkaz na CCR routinu „Metin2 PServer Feature Tracker", která systémy sbírá
