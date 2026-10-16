# AdminIndex.razor + AdminAccess.razor
Route: /admin, /admin/access (role Admin)
Popis: Rozcestník administrace a správa správců (kdo se smí přihlásit přes Google).

## Hotovo ✅
- /admin: počty položek, mobů, map, questů, systémů, čekajících na schválení, běhů agenta a správců; odkazy na přístup, reporty a kontrolu
- /admin/access: seznam aktivních správců, přidání e-mailem, odebrání s potvrzením
- Zdroj pravdy je tabulka `AdminUsers` (migrace 08); odebrání = `RemovedAt`, každá změna jde do `ApprovalLog` (`admin_added` / `admin_removed`)
- Super-admin (`Admin:Email`) se doplní při startu aplikace, nejde odebrat a smí vždy; sebe odebrat nejde
- E-maily z `Authentication:Google:AllowedEmails` se do tabulky přenesou jen jednou (dokud v ní není žádný sub-admin)
- Bez migrace 08 platí původní seznam z appsettings a stránka zobrazí vysvětlení — deploy kódu nezamkne správce venku

## Chybí / Rozpracováno ⚠️
- Historie odebraných správců se v UI nezobrazuje (je jen v `ApprovalLog`)

## Návrhy na vylepšení 💡
- Poslední přihlášení u každého správce
