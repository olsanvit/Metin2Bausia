# Index.razor
Route: /
Popis: Dashboard admina — rychlý přehled front ke schválení a stavu AI agenta (Metin2BausiaCollector).

## Hotovo ✅
- 4 klikací karty: pending itemy, pending mobové, schválené itemy, ruční review; barva karty podle toho, jestli je co řešit
- Banner posledního agent runu (OK/chyba, čas, počty nových entit)
- Tabulka posledních 5 runů z `AgentRunReports` + odkaz na `/reports`
- Odkaz na `/stats`
- **Opraveno 2026-09-12:** model `AgentRunReport` přemapován na skutečné sloupce (`Success`, `EntitiesInserted/Updated/Failed`, `CreatedAt`, `BlockerCategory`); počet ruční review čte `Status = 'pending'`; chyba DB se zobrazí místo tichého spolknutí.

## Chybí / Rozpracováno ⚠️
- Model `AgentRunReport` čte `RunStatus`, `RunRunAt`, `RunFinishedAt`, `ItemsNew`, `MobsNew`, `ImagesDownloaded`, `AgentType` — v tabulce `AgentRunReports` neexistují (jsou tam `Success`, `RunMode`, `DurationMs`, `EntitiesInserted/Updated/Failed`, `BlockerCategory`). Banner je proto vždy ❌ a čísla nulová.
- Počet ruční review čte `ManualReviewQueue."Resolved"` — sloupec neexistuje (je `Status`) → dotaz padá, karta ukáže 0.
- `Highlights` je `jsonb`, zobrazuje se jako syrový JSON.
- Chybí karty pro pending Maps / Skills / Quests / Systems / ContentImages.
- `catch { /* DB not yet available */ }` spolkne jakoukoli chybu, nejen nedostupnou DB.

## Návrhy na vylepšení 💡
- Přemapovat model na skutečné sloupce (`Success` → badge, `DurationMs` → trvání, `EntitiesInserted` → nové)
- Rozpis `Highlights` jako odrážky
- Karta „Nejstarší nevyřízená položka" (kolik dní čeká)
- Tlačítko „Exportovat proto" (volání MCP `export_item_proto_txt`/`export_mob_proto_txt`) s výsledkem
- Auto-refresh (timer 30–60 s) místo jednorázového načtení

## Brainstorming poznámky
- Sparkline velikosti pending fronty za 7/30 dní (z `CreatedAt`/`ApprovedAt`)
- Upozornění, když agent neběžel déle než `AgentSchedules.IntervalHours` (missed run)
- Stav circuit breakerů zdrojů (`SourceCircuitBreaker` s `State = open`)
- Stav herního serveru (auth :11002, game :13000) — ping/health, pokud bude dostupný
