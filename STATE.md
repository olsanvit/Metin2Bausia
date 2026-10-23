# STATE — Metin2Bausia

Aktualizováno 23. 10. 2026 (session Metin2 Bausia).

## Rozdělané
- **KnowledgeItems (Trello ZRhWZajR):** migrace `database/postgres/10_knowledge_items.sql` + počet na `/admin` v commitu 6888f71, win-build OK.
  **23. 10. 16:07 nasazeno:** záloha pre10-20261023-1607, migrace 10 + GRANT OK, deploy gate exit 0, /health 200. Proklik /admin neproběhl — deploy odhlásí (viz níže).
  Původní plán: migrace na produkci 22. 10. v 16:00 (ROZHODNUTÍ VÍTKA 21. 10. 7:20, dávka APP): nejdřív pg_dump, pak migrace jako `SET ROLE metin2bausia`, pak deploy přes gate. Nic neimportovat — plní kompletační rutina IC.
  GRANT pro metin2bausia_simulatereal je v migraci (ROZHODNUTÍ VÍTKA 21. 10.).
- **Průchod stránek (Trello eW8ZH9Sj):** dávky 1–2 nasazené a proklikané; 163f58a (ikona Statistiky) čeká na deploy APP; dávka 3 (systémy z configu serveru) až na pokyn APP.

## Poslední rozhodnutí Vítka
- 21. 10.: Metin2 data z wiki do Knowledge*, ne do Mobs/Items (Vnum povinný pro export); migrace KnowledgeItems schválena.
- 17. 10.: stav serveru TCP pingem (GameServer:Endpoints), historie dávek v BatchRuns (migrace 09), agent zatím neexistuje.

## Známý problém
- Každý deploy odhlásí uživatele: DataProtection klíče jsou v `/root/.aspnet/DataProtection-Keys` uvnitř kontejneru (log warning), nový kontejner = neplatné cookie → Chrome pro proklik skončí na Google loginu. Oprava = `PersistKeysToFileSystem` do mountu `/app/data` (návrh pro APP).

## Další krok (ROZHODNUTÍ VÍTKA 23. 10.)
Po resetu usage (po 26. 10. 12:00): `PersistKeysToFileSystem` do `/app/data` + deploy → Vítek se jednou přihlásí → proklik `/admin`. Do té doby nic.

## Po restartu
Čekat na pokyn APP. Pokud je 22. 10. po 16:00 a migrace 10 neproběhla: záloha → migrace 10 → deploy gate → proklik `/admin`.
Noční/dávkové skripty: `database/import/generated/` (gitignored; night-lib.sh s pojistkami IC).
