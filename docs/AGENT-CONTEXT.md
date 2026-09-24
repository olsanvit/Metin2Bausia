# Metin2Bausia — Agent Context

> Vstupní bod pro každou agent session. Přečti jako první, pak CLAUDE.md v kořeni projektu.

---

## Co je projekt

Správa a provoz Metin2 private serveru (TMP4). Blazor Server admin aplikace pro review a schvalování obsahu (items, moby, mapy, questy) sbíraného AI agentem, s exportem do herních proto souborů.

**Repo:** `github.com/olsanvit/Metin2Bausia`  
**Větev:** `main`  
**Deploy:** QNAP NAS, kontejner `metin2bausia` (aspnet:10.0 + publish složka), port 5023 — záměrně vypnutý

---

## Stav projektu (2026-09-23)

### Co funguje
- 18 admin stránek — Items, Mobs, Maps, Quests, Systems, Groups, Review, Reports, Stats
- Cookie auth — single admin, PBKDF2 hash, MustChangePassword flag
- Approve/reject workflow s zápisem do ApprovalLog
- IsEnabled toggle + IsEnabledLocked (ochrana před přepsáním importem)
- Export item_proto.txt a mob_proto.txt (správný formát, ověřen round-tripem 5743 items + 1334 mobů)
- Lokalizace cs/en/de, 299 klíčů
- MCP server (kontejner `mt2-mcp`, port 3000 dle docker-compose.yml, 73 nástrojů) pro AI agent Collector
- Import herních dat: `tools/gamefiles-import/import.mjs`
- Testy: 20 (13 stránek, 2 detaily, 3 jazyky, 2 smoke) — potřebují PostgreSQL, viz CLAUDE.md
- CI: GitHub Actions na push/PR

### Produkce (2026-09-24)
- DB přesunuta na sdílený `pg16` (mt2-postgres byl smazán), vlastní role `metin2bausia`
- Migrace 01–06 nahrány, import hotový: 5743 items, 1334 mobů, 65 map, 284 questů, 1298 skupin
- Deploy proběhl, `/health` vrací 200 Healthy; kontejner je zase zastavený (`restart=no`)

### TODO
- ManageSystemDetail — sekce Files není implementována
- `/items/browse` — kandidát na sloučení s ManageItems

---

## Workflow

1. AI agent (Collector) sbírá data → `ContentStatus='pending'`
2. Admin schvaluje/zamítá v Blazor UI → `ContentStatus='approved'`
3. MCP export: `export_item_proto_txt` / `export_mob_proto_txt`
4. Operátor kopíruje proto soubory do `server/share/` a restartuje herní server

---

## Struktura projektu

```
src/
  Metin2Bausia.Web/     Blazor Server (.NET 10, Dapper)
    Components/Pages/   18 Razor stránek (@page) + Layout, Shared/ToastMessage
    Components/         AdminPageBase (bázová třída s ShowOk/ShowError/LogError)
    Pages/Account/      Login, Logout, ChangePassword (Razor Pages, ne Blazor)
    Services/           DbService, AdminCredentialService
mcp/                    Node.js MCP server (73 nástrojů)
server/                 TMP4 C++ herní server + Docker stack
database/
  postgres/             Migrace 01-06 (schéma management DB)
  mysql/                MySQL schémata herní DB
tools/
  gamefiles-import/     import.mjs — z gamefiles do DB
```

---

## Klíčová architektura

- **DB:** PostgreSQL — lokálně 18 (port 5442), produkce sdílený `pg16` na QNAPu; MySQL 5.5 (herní DB v Docker)
- **ORM:** Dapper (ne EF Core) přes `IDbService` / `DbService`
- **Auth:** Cookie auth — single admin, žádné role, whitelist
- **SharedServices:** ThemeService, ConnectionStateService, IStringLocalizer
- **AdminPageBase:** `ShowOk()`, `ShowError(ex)`, `LogError()` pro toast notifikace

---

## Tabulky (PostgreSQL `Metin2Bausia`)

**Content:** Items, Mobs, Maps, Skills, Quests, Systems, ContentImages  
**Workflow:** ApprovalLog, ManualReviewQueue, ProtoFiles  
**Agent:** AgentRunReports, AgentSchedules, AgentPromptCache, DiscoveryQueue, SourceCircuitBreaker  
**Infrastruktura:** AuditLog, ServerConfig, ContentGroups  

DataOrigin: `ai_collected | manual | imported`  
ContentStatus: `pending | approved | rejected | needs_review`

---

## Co číst na začátku session

1. **CLAUDE.md** — pravidla, jeden build najednou (8GB RAM)
2. **`docs/pages/<Stránka>.md`** — stav konkrétní stránky
3. **`database/postgres/`** — migrace pro pochopení schématu

---

## Lokální DB start

```bash
export LC_ALL=en_US.UTF-8
/opt/homebrew/opt/postgresql@18/bin/pg_ctl -D ~/.local/share/metin2bausia-pg18 \
  -l ~/.local/share/metin2bausia-pg18/server.log \
  -o "-p 5442 -k /tmp -c listen_addresses=localhost" -w start
```

Port 5442, DB `Metin2Bausia`, user `postgres`

---

## Deploy

```bash
~/deploy-to-qnap.sh metin2bausia
```

Kontejner záměrně vypnutý (`restart=no`), dokud ho uživatel nespustí.

---

## Responzivita (2026-09-23)

Scrolluje jen `.content`; `.page` má výšku okna. Pod 768 px je sidebar vysouvací menu (checkbox `#nav-toggle`, layout je SSR). Tabulky jsou v `table-responsive`. Detaily v `docs/pages/README.md`.
