# Login.cshtml
Route: /account/login
Popis: Přihlášení do adminu — heslem, nebo přes Google.
Platforma: Web

## Hotovo ✅
- Přihlášení e-mailem a heslem; heslo se drží jako PBKDF2-HMAC-SHA256 hash (210 000 iterací)
  v `data/admin-creds.json`, e-mail v `Admin:Email`
- Vynucená změna hesla při prvním přihlášení (`MustChangePassword`)
- Přihlášení přes Google — tlačítko se zobrazí jen s nastaveným klientem
- Odhlášení (`/account/logout`), změna hesla (`/account/change-password`)

## Google login — jak je postavený
Sdílené `AddMabAuth` ze SharedServices se **nepoužívá**: stojí na ASP.NET Identity nad EF Core
(tabulky `AspNetUsers` a spol.), zatímco tenhle projekt jede celý na Dapperu a žádnou tabulku
uživatelů nemá. Místo toho:

- `Services/GoogleAuthOptions.cs` — konfigurace a seznam povolených e-mailů
- `Program.cs` registruje `AddGoogle` **jen když jsou nastavené skutečné klíče**; zástupné hodnoty
  (`YOUR_…`, `changeme…`) se berou jako nenastavené, jinak by tlačítko vedlo na `401 invalid_client`
- Po návratu od Googlu se ověří e-mail proti seznamu; kdo v něm není, skončí chybou a je přesměrován
  zpět na přihlášení. Kdo projde, dostane **stejnou admin cookie** jako po přihlášení heslem,
  včetně role `Admin`

Konfigurace (`appsettings.json` má jen zástupné hodnoty, skutečné patří do
`appsettings.Production.json` na QNAPu, který se **necommituje**):

```json
"Authentication": {
  "Google": {
    "ClientId": "…",
    "ClientSecret": "…",
    "AllowedEmails": ["olsansky575@gmail.com"]
  }
}
```

`Admin:Email` (olsanskyvitek@gmail.com) je povolený vždy, i když v `AllowedEmails` není. Druhý správcovský
účet olsansky575@gmail.com je v seznamu — oba mají plný přístup včetně role `Admin`.

Přihlášení přes Google je trvalé stejně jako heslem (7 dní), jinak by cookie zmizela se zavřením prohlížeče.

## Chybí / Rozpracováno ⚠️
- V Google Cloud Console musí být zaregistrovaný redirect `https://metin2bausia.vo2info.cz/signin-google`
  — bez toho se přihlášení nedokončí. Zatím nenastaveno.
- Celý tok přes Google není ověřený proti skutečnému Googlu (vyžaduje klienta a přihlášení uživatele);
  otestovaná je jen logika povolených e-mailů (`GoogleAuthOptionsTests`).
- Není stránka pro správu seznamu adminů — mění se v konfiguraci a vyžaduje restart kontejneru.

## Návrhy na vylepšení 💡
- Zápis přihlášení do `AuditLog` (kdo a kdy se přihlásil, jakou cestou)
- Omezení počtu pokusů o heslo
