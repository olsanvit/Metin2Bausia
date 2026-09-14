# ManageSystemDetail.razor
Route: /manage/systems/{Guid:guid}
Popis: Detail herního systému — editace metadat, zobrazení implementačních instrukcí, akce schválení a zapnutí.

## Hotovo ✅
- Editace: název, kategorie (12 hodnot), složitost (low/medium/high), zdrojový server, popis
- Karta „Implementace" (`Implementation` jako `<pre>`), karta zdroje
- Badge složitosti v hlavičce, Schválit / Zamítnout / Zapnout-Vypnout
- Typy modelu odpovídají schématu

## Chybí / Rozpracováno ⚠️
- **`Files` (jsonb seznam souborů ke změně) se nezobrazuje** — přitom je to podklad pro implementaci.
- `Implementation` je jen ke čtení.
- `ConfidenceScore` se nezobrazuje (ostatní detaily ho mají).
- Audit problém jako ostatní detaily.

## Návrhy na vylepšení 💡
- Seznam souborů z `Files` s tlačítkem „otevřít" přes MCP `read_file` (soubory serveru v `MT2_FILES_DIR`)
- Editovatelná implementace (markdown editor + náhled)
- Checklist implementačních kroků se stavem

## Brainstorming poznámky
- Propojení se `ServerConfig` — některé systémy jsou jen konfigurační přepínač (rates, PvP), ty by šly zapínat opravdu
- Odkaz na relevantní itemy/moby, které systém přináší
- Poznámky / komentáře k systému (diskuse před implementací)
