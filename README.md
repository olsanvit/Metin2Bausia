# Metin2Bausia

Kompletní infrastruktura pro správu a provoz Metin2 private serveru (TMP4).

## Architektura

```
┌─────────────────────────────────────────────────────────────┐
│  HERNÍ SERVER (server/docker/docker-compose.yml)            │
│                                                             │
│  mt2-mysql ──► mt2-db ──► mt2-auth  :11002  (přihlášení)  │
│                       └──► mt2-game  :13000  (hra CH1)     │
│  mt2-phpmyadmin       :8099  (MySQL admin, pouze lokálně)  │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  MANAGEMENT STACK (docker-compose.yml v root)               │
│                                                             │
│  mt2-postgres  :5442  (PostgreSQL — Metin2Bausia DB)        │
│  mt2-mcp       :3030  → mcp.vo2info.cz/MT2/  (AI agent)   │
│  mt2-admin     :8081  (Blazor admin — schvalování obsahu)  │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  AI AGENT                                                    │
│  Metin2BausiaCollector — sbírá itemy/moby/mapy z webu      │
│  Ukládá jako pending → operátor schvaluje v Blazor admin   │
│  Schválené itemy → export do item_proto.txt / mob_proto.txt │
└─────────────────────────────────────────────────────────────┘
```

## Databáze

| Databáze | Typ | Obsah |
|----------|-----|-------|
| `account` | MySQL | Herní účty, GM list |
| `player` | MySQL | Postavy, inventáře, guildy |
| `common` | MySQL | item_proto, mob_proto, skill_proto |
| `log` | MySQL | Logy přihlášení, GM akcí, obchodů |
| `Metin2Bausia` | PostgreSQL | Management: Items, Mobs, Maps, Skills, ContentImages, AgentRunReports... |

## Rychlý start

### 1. Management stack (PostgreSQL + MCP + Admin)

```bash
cp .env.example .env          # nastav POSTGRES_PASSWORD
docker compose up -d
```

Přístupy:
- MT2 MCP API: http://localhost:3030 → veřejně: https://mcp.vo2info.cz/MT2/
- Blazor Admin: http://localhost:8081

### 2. Herní server (TMP4)

```bash
# Zkopíruj TMP4 source a herní data
cp -r /cesta/k/tmp4/* server/src/
cp -r /cesta/k/share/* server/share/

# Uprav PUBLIC_IP a hesla
nano server/game/conf.cfg          # nastav PUBLIC_IP
cp server/docker/.env.example server/docker/.env  # nastav MYSQL_ROOT_PASSWORD

# Build a spuštění
cd server/docker
./build.sh
docker compose up -d
```

Porty herního serveru:
- Auth: `:11002` (klienti)
- Game CH1: `:13000` (klienti)
- MySQL (lokální admin): `:3366`
- phpMyAdmin: `http://localhost:8099`

### 3. Deploy na QNAP

```bash
./deploy.sh    # nahraje soubory a spustí management stack na QNAP
```

## Adresářová struktura

```
Metin2Bausia/
├── agent/                          # AI agent prompty
│   ├── Metin2BausiaCollectorPrompt.txt
│   └── Metin2BausiaCollectorPromptSkills.txt
├── database/
│   ├── mysql/                      # MySQL schémata (account, player, common, log)
│   └── postgres/                   # PostgreSQL schéma (management DB)
├── mcp/                            # MT2 MCP server (Node.js)
│   ├── server.js
│   ├── Dockerfile
│   └── package.json
├── nginx/
│   └── mt2-mcp.conf               # nginx proxy konfigurace
├── packs/                          # .epk/.eix pack soubory (binárky v .gitignore)
├── proto/                          # item_proto.txt, mob_proto.txt (z DB exportu)
├── server/
│   ├── auth/conf.cfg              # Auth server konfigurace
│   ├── db/conf.cfg                # DB server konfigurace
│   ├── game/conf.cfg              # Game server CH1 konfigurace
│   ├── docker/
│   │   ├── Dockerfile             # Build TMP4 server image
│   │   ├── docker-compose.yml     # Herní server stack
│   │   ├── entrypoint.sh          # Spustí db|auth|game
│   │   ├── build.sh               # Build helper
│   │   └── mysql-init/            # MySQL init SQL skripty
│   ├── src/                       # TMP4 source (v .gitignore — zkopírovat ručně)
│   └── share/                     # Herní data: maps, locale, scripts (v .gitignore)
├── src/
│   └── Metin2Bausia.Web/          # Blazor Server admin aplikace
├── docker-compose.yml              # Management stack
├── deploy.sh                       # QNAP deployment skript
└── .env.example                    # Vzor pro .env
```

## Workflow obsahu

```
Agent collector
    │  sbírá z webu → ContentStatus='pending'
    ▼
PostgreSQL (Metin2Bausia)
    │  Items/Mobs/Maps/Skills tabulky
    ▼
Blazor Admin (mt2-admin :8081)
    │  operátor schválí/zamítne každý záznam
    ▼
MT2 MCP export
    │  export_item_proto_txt() → /app/mt2files/proto/item_proto.txt
    │  export_mob_proto_txt()  → /app/mt2files/proto/mob_proto.txt
    ▼
Herní server
    │  operátor zkopíruje proto soubory do server/share/
    │  restart game serveru
    ▼
Hráči vidí nové itemy/moby ve hře
```