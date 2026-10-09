# TODO

Otevřené úkoly mimo hotový responzivní redesign (stav k 27. 9. 2026, `3024ab5`).
Ověřeno proti kódu, ne jen přepsáno ze starších poznámek.

## Pořadí podle priority (zadal Vítek 27. 9.)

Rozhodnutí z review 28. 9.: Google login vlastním lehkým OAuth (ne Identity),
sub-admini až s ním, export do hry ke stažení jako ZIP, MAP_ALLOW jen vygenerovat řádek,
kontejner zůstává vypnutý, testy beze změny (bez bUnit).

1. ~~Editor dropů a spawnů~~ — hotovo, čeká na build a ověření
2. ~~ThemePicker~~ — hotovo 6. 10.: přepnutí bez reloadu, volba per prohlížeč (localStorage)
3. Globální hledání
4. Export do hry
5. ~~Patcher `net9.0-windows` → `net10.0-windows`~~ — přepnuto, ale **CI Patcher nebuilduje**, změna není nikde ověřená

Ostatní položky níž jsou pod touto pětkou.

## 1. Editory obsahu

- [x] **Editor dropů moba** (`ManageMobDetail.razor`) — dropy se dnes jen zobrazují.
      Řádky: vnum s našeptáváním ze schválených itemů, počet, šance %, validace existence vnum.
      Pozor: typy `kill`/`limit` odkazují item **jménem**, ne vnumem.
- [x] **Editor spawnů mapy** (`ManageMapDetail.razor`) — stejná situace.
      Typ určuje význam vnumu: `m`/`ma`/`s` = mob, `g`/`ga` = skupina, `r` = skupina skupin.
- [ ] **Export do hry** — stránka, která ze schváleného obsahu vygeneruje `item_proto.txt`,
      `mob_proto.txt` a spol. a nabídne je **ke stažení jako ZIP** (nezapisuje na server).
      Skládání řádku už umí `mcp/proto-format.js`.
- [ ] **`MAP_ALLOW`** — zapnutí/vypnutí mapy dnes nemá na hru žádný dopad.
      Rozhodnuto: admin jen **vygeneruje řádek ke zkopírování** do `server/game/conf.cfg`, nic nepřepisuje sám.

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

- [x] **Patcher** přepnut na `net10.0-windows`. ⚠️ CI staví jen `Metin2Bausia.Tests` — Patcher se nepřekládá nikde,
      na macOS WinForms nejde. Chce to job na Windows runneru, jinak se případná chyba pozná až při ruční kompilaci.
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

## 7. Google login (rozpracováno 28. 9.)

- [x] `GoogleAuthOptions` + registrace v `Program.cs` jen se skutečnými klíči, whitelist e-mailů
- [x] Tlačítko na přihlašovací stránce, jednotkové testy povolených e-mailů
- [ ] Zaregistrovat redirect `https://bausia.vo2info.cz/signin-google` v Google Cloud Console
- [ ] Vyplnit `ClientId`/`ClientSecret` do `appsettings.Production.json` na QNAPu (necommitovat!)
- [ ] Ověřit celý tok proti skutečnému Googlu — bez klienta to nejde otestovat
