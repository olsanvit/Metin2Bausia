# ManageQuestDetail.razor
Route: /manage/quests/{Guid:guid}
Popis: Detail questu — metadata, levelový rozsah, startovní NPC a celý Lua skript jen ke čtení.
Platforma: Web

## Hotovo ✅
- Název, soubor, levelový rozsah a startovní NPC (pokud jsou vyplněné)
- Celý skript v posuvném `<pre>` s kódováním a velikostí
- Zapnutí / vypnutí questu, původ dat, datum vytvoření a aktualizace
- Texty přes `IStringLocalizer` (cs, en)

## Chybí / Rozpracováno ⚠️
- Skript nejde upravit — záměrně, questy kompiluje `qc` ze souborů serveru
- Import neplní `MinLevel`, `MaxLevel` ani `StartNpcVnum` (ve `.quest` souborech nejsou strukturovaně), sekce se tak většinou nezobrazí
- Chybí zvýraznění syntaxe Lua

## Návrhy na vylepšení 💡
- Vytáhnout ze skriptu `when <npc_vnum>.chat` a zobrazit odkazy na NPC
- Odkazy na itemy z volání `pc.give_item2(vnum)`
- Porovnání s verzí na serveru (diff), až bude export questů

## Brainstorming poznámky
- Texty questů odkazují do `locale.lua` (`gameforge.<quest>.<klíč>`) — bez něj skript neukazuje, co hráč uvidí
