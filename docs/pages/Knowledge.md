# Znalosti (Knowledge*.razor)
Route: `/knowledge/servers`, `/knowledge/monsters`, `/knowledge/stones` + detail `/{guid}`, párování `/knowledge/link`
Popis: Metin2 servery ze seznamů na webu, monstra a Metin kameny z wiki Gameforge.
Platforma: Web

## KnowledgeItems (migrace 10, 21. 10.)
Předměty z oficiální wiki pro kompletační rutinu IC (Trello ZRhWZajR, návrh Data Hubu `NAVRH-Metin2.md`).
Klíč `NameUrl` (upsert), `Category`, `Level` (text), `Stats` a `Metadata` jsonb (drops, raw infobox, import revid),
`ItemVnum` NULL — páruje Collector/operátor. Aplikace zatím jen počítá řádky na `/admin`; vlastní stránka přijde později.
Oprávnění pro zapisující roli rutiny migrace nedává (změna oprávnění jde přes souhlas).

## Proč samostatně
Data dřív plnil worker SimulateReal do DB SportReal (`MetinServers`, `MetinMonsters`, `MetinStones`).
Vítek 12. 10. 2026 rozhodl, že patří do Metin2Bausia. Nejsou to herní proto data (žádný vnum, anglická jména,
hodnoty jako text), proto vlastní tabulky `Knowledge*` (migrace `database/postgres/07_knowledge_tables.sql`),
ne sloupce v `Mobs`/`Items`.

## Hotovo ✅
- Seznamy s hledáním, filtry a řazením; na mobilu karty (`.list-cards`)
  - Servery: styl, aktivní/neaktivní, řazení podle jména, max. levelu, posledního výskytu.
    Vykreslí se prvních 300 (filtr hledá ve všech) — serverů jsou tisíce
  - Monstra: element, lokace, spárované/nespárované s naším mobem; řazení podle levelu (první číslo z textu)
  - Kameny: filtr podle dropu (kniha dovedností, duch, duše)
- Detaily jen ke čtení + odkaz na zdroj (jen http/https — `KnowledgeText.SafeUrl`)
- Monstra a kameny: ruční spárování s mobem přes vnum (`KnowledgeMobLink`), ověří, že mob existuje
- **Párování s moby** (`/knowledge/link`, rozhodl Vítek 13. 10.): seznam nespárovaných monster/kamenů,
  po výběru top 10 kandidátů z `Mobs` podle shody jména (slova bez závorek a diakritiky) a levelu
  (`KnowledgeLinking.Rank`). Kámen jde jen na mob typu `STONE`, monstrum naopak ne. Na Items se nepáruje —
  ve Znalostech žádné předměty nejsou
- Uložení i zrušení páru jde přes `KnowledgeLinkService` jedním příkazem spolu se záznamem do `ApprovalLog`
  (`Action` = `linked`/`unlinked`, `Operator` = přihlášený e-mail, `Reason` = starý → nový vnum)
- Lokalizace cs/en/de, sekce menu „Znalosti“
- Testy: vykreslení seznamů, detaily se vzorovými řádky, `KnowledgeTextTests`, `KnowledgeLinkingTests`
  (návrhy), `KnowledgeLinkServiceTests` (párování + audit proti DB)

## Rozpracováno / TODO 🔄
- Převod dat ze SportReal — další krok, noční dávka (pravidlo: produkce jen po dávkách)
- SimulateReal přepnout na zápis sem (upsert podle klíčů níže)
- Hromadné potvrzení návrhů (dnes se páruje po jednom)

## Zápis ze SimulateReal (upsert)
| Tabulka | Přirozený klíč (unikátní index) | Povinné sloupce |
|---|---|---|
| `KnowledgeServers` | `DetailUrl` | `Name`, `ListingUrl`, `DetailUrl` |
| `KnowledgeMonsters` | `NameUrl` | `Name`, `NameUrl` |
| `KnowledgeStones` | `NameUrl` | `Name`, `NameUrl` |

Ostatní textové sloupce mají výchozí `''`. `SourceGuid` = Guid řádku ze SportReal (jen pro převod),
`MobVnum` a `IsDeleted` worker nepřepisuje.
