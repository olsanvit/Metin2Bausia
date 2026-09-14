# Reports.razor
Route: /reports
Popis: Historie posledních 50 běhů AI agenta (`AgentRunReports`).

## Hotovo ✅
- Tabulka: agent, readiness, status, spuštění, trvání, počty items/mobs/obrázků, zkrácené highlights
- Výpočet trvání z časů začátku/konce, zkrácení highlights na 80 znaků
- **Opraveno 2026-09-12:** model přemapován na skutečné sloupce; trvání se počítá z `DurationMs`; přibyl sloupec režimu běhu; chyba DB se zobrazí.

## Chybí / Rozpracováno ⚠️
- Stejný nesoulad modelu jako Dashboard: `RunStatus`, `RunRunAt`, `RunFinishedAt`, `ItemsNew`, `MobsNew`, `ImagesDownloaded` v tabulce nejsou → většina sloupců prázdná, status vždy červený. Použitelné je jen `AgentName`, `ReadinessStatus`, `Highlights`.
- Trvání se dá vzít rovnou z `DurationMs`.
- `Errors` (jsonb), `Notes`, `BlockerCategory`, `PromptVersion`/`McpVersion` se nezobrazují — přitom jsou pro diagnostiku to nejcennější.
- `catch { }` → při chybě „Zatím žádné agent runy" (zavádějící).
- Žádný detail runu, filtr ani stránkování.

## Návrhy na vylepšení 💡
- Přemapovat model na skutečné sloupce
- Rozklik řádku → detail (errors, notes, blocker, verze promptu/skills/MCP)
- Filtr: agent, úspěch/neúspěch, období
- Zvýraznit změnu `PromptVersion` mezi runy (kdy se upgradoval prompt)

## Brainstorming poznámky
- Graf `EntitiesInserted` a `DurationMs` v čase — odhalí degradaci zdrojů
- Propojit s `AgentSchedules` → „měl běžet v X, neběžel" (missed run)
- Seskupit podle `BlockerCategory` — nejčastější příčiny selhání
- Dashboard a Reports sdílejí model i dotaz → vytáhnout do jedné služby, ať se oprava dělá jednou
