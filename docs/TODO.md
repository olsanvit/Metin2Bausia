# TODO

Otevřené úkoly mimo hotový responzivní redesign (stav k 27. 9. 2026, `3024ab5`).
Ověřeno proti kódu, ne jen přepsáno ze starších poznámek.

## Pořadí podle priority (zadal Vítek 27. 9.)

1. Editor dropů a spawnů
2. ThemePicker — mění téma až po reloadu
3. Globální hledání
4. Export do hry
5. Patcher `net9.0-windows` → `net10.0-windows`

Ostatní položky níž jsou pod touto pětkou.

## 1. Editory obsahu

- [ ] **Editor dropů moba** (`ManageMobDetail.razor`) — dropy se dnes jen zobrazují.
      Řádky: vnum s našeptáváním ze schválených itemů, počet, šance %, validace existence vnum.
      Pozor: typy `kill`/`limit` odkazují item **jménem**, ne vnumem.
- [ ] **Editor spawnů mapy** (`ManageMapDetail.razor`) — stejná situace.
      Typ určuje význam vnumu: `m`/`ma`/`s` = mob, `g`/`ga` = skupina, `r` = skupina skupin.
- [ ] **Export do hry** — stránka, která z schváleného obsahu vygeneruje `item_proto.txt`,
      `mob_proto.txt` a spol. Skládání řádku už umí `mcp/proto-format.js`.
- [ ] **`MAP_ALLOW`** — zapnutí/vypnutí mapy dnes nemá na hru žádný dopad.
      Mapy nejsou v proto exportu, řídí se `server/game/conf.cfg`. Generovat ze zapnutých map.

## Chybějící pole v detailech

- [ ] **Mob**: `Resists` (jsonb), `ImmuneFlags`, `RegenCycle`/`RegenPercent`, `ScalePercent`,
      `AggressiveHpPct`, `Size`, `BattleType`, `AttackRange` (načte se, nezobrazí).
- [ ] **Mob**: dropdown ranku nemá `super_pawn`, přestože ho seznam zná jako „S-Pěšák".
- [ ] **Item**: `SubType`, `AntiFlags`, `Flags`, `WearFlags`, `ImmuneFlags`, `IconFile`,
      `ModelFile`, `SummonMobVnum` se vůbec nezobrazují.
- [ ] **Item**: flagy jako checkboxy s názvy (ANTI_DROP, ANTI_SELL…) místo čísla.
- [ ] **Mapa**: levelový rozsah je jen ke čtení a v UI dvakrát (editační řádek + karta rozměrů).
- [ ] **Systém**: sekce se soubory (`Files`) chybí úplně.

## UX

- [ ] **Odchod z rozepsaného formuláře** — tlačítko Uložit sice značí neuložené změny tečkou
      (`Snapshot()`/`IsDirty`), ale odchod ze stránky nic nehlídá a změny se tiše ztratí.
- [ ] **ThemePicker** — odkaz na CSS je v SSR části `App.razor`, takže se motiv projeví až po reloadu.
      Potřebuje Bootstrap JS nebo výměnu `href` z interaktivní komponenty.
- [ ] **Globální hledání** napříč itemy, moby, mapami a questy.
- [ ] **Zvýraznění syntaxe Lua** ve výpisu questu.
- [ ] **Navigace „předchozí / další"** v rámci aktuálního filtru seznamu.

## Import a data

- [ ] Import neplní u questů `MinLevel`, `MaxLevel` ani `StartNpcVnum` — ve `.quest` souborech
      nejsou strukturovaně. Sekce se proto většinou nezobrazí.
- [ ] Vytáhnout ze skriptu questu `when <npc_vnum>.chat` a `pc.give_item2(vnum)` a udělat z nich odkazy.

## Infrastruktura

- [ ] **Patcher** je pořád `net9.0-windows` (`src/Metin2Bausia.Patcher/*.csproj:5`), zbytek řešení `net10.0`.
- [ ] **QNAP**: zaseknuté procesy `docker restart qnap-game-mcp`.
- [ ] **QNAP**: kontejnery `selenium-debug` a `selenium-prod` jsou zastavené — rozhodnout,
      jestli mají zůstat vypnuté natrvalo.

## Neověřené

- [ ] **Skupiny** (`/manage/groups`) — rozbalení skupiny klikem není ověřené v prohlížeči.
      Statický export pro měření layoutu nemá běžící Blazor, takže `@onclick` v něm nefunguje.
      Sbalený stav na 375 px ověřený je (tabulka 343 px, nulový přesah).

## Hotovo (nezapomenout, že už to platí)

- Responzivní redesign všech 15 stránek — vzor `.list-table` / `.list-cards`, `.detail-actions`,
  `details.collapsible` v `wwwroot/app.css`.
- Audit schvalování (`ApprovedAt`/`ApprovedBy` + zápis do `ApprovalLog`) mají **všechny čtyři**
  detailní stránky. Starší poznámky v `docs/pages/*.md` tvrdily opak.
