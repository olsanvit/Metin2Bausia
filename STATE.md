# STATE — Metin2Bausia

Aktualizováno 21. 10. 2026 (session Metin2 Bausia).

## Rozdělané
- **KnowledgeItems (Trello ZRhWZajR):** migrace `database/postgres/10_knowledge_items.sql` + počet na `/admin` v commitu 6888f71, win-build OK.
  Migrace na produkci **22. 10. v 16:00** (ROZHODNUTÍ VÍTKA 21. 10. 7:20, dávka APP): nejdřív pg_dump, pak migrace jako `SET ROLE metin2bausia`, pak deploy přes gate. Nic neimportovat — plní kompletační rutina IC.
  Otevřené: GRANT pro zapisující roli rutiny migrace nedává (změna oprávnění → souhlas přes IC).
- **Průchod stránek (Trello eW8ZH9Sj):** dávky 1–2 nasazené a proklikané; 163f58a (ikona Statistiky) čeká na deploy APP; dávka 3 (systémy z configu serveru) až na pokyn APP.

## Poslední rozhodnutí Vítka
- 21. 10.: Metin2 data z wiki do Knowledge*, ne do Mobs/Items (Vnum povinný pro export); migrace KnowledgeItems schválena.
- 17. 10.: stav serveru TCP pingem (GameServer:Endpoints), historie dávek v BatchRuns (migrace 09), agent zatím neexistuje.

## Po restartu
Čekat na pokyn APP. Pokud je 22. 10. po 16:00 a migrace 10 neproběhla: záloha → migrace 10 → deploy gate → proklik `/admin`.
Noční/dávkové skripty: `database/import/generated/` (gitignored; night-lib.sh s pojistkami IC).
