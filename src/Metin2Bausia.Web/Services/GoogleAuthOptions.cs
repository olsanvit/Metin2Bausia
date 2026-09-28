namespace Metin2Bausia.Web.Services;

/// <summary>
/// Nastavení přihlášení přes Google.
///
/// Proč vlastní řešení a ne sdílené AddMabAuth: to stojí na ASP.NET Identity nad EF Core
/// (tabulky AspNetUsers a spol.), zatímco tenhle projekt jede celý na Dapperu a nemá
/// žádnou tabulku uživatelů. Pro jednoho až pár správců stačí seznam povolených e-mailů
/// v konfiguraci — kdo v něm je, dostane stejnou admin cookie jako po přihlášení heslem.
/// </summary>
public class GoogleAuthOptions
{
    public const string Section = "Authentication:Google";

    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }

    /// <summary>E-maily, které smí dovnitř. Prázdný seznam = pustí se jen Admin:Email.</summary>
    public string[] AllowedEmails { get; set; } = [];

    /// <summary>
    /// Zástupné hodnoty „YOUR_…" v appsettings nesmí projít jako nastavené — poskytovatel
    /// by se zaregistroval s neexistujícím klientem a Google by vracel 401 invalid_client.
    /// </summary>
    public bool IsConfigured =>
        IsRealValue(ClientId) && IsRealValue(ClientSecret);

    private static bool IsRealValue(string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && !value.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)
        && !value.StartsWith("changeme", StringComparison.OrdinalIgnoreCase);

    /// <summary>Povolené e-maily včetně hlavního admina, malými písmeny kvůli porovnání.</summary>
    public HashSet<string> AllowList(string adminEmail) =>
        new(AllowedEmails.Append(adminEmail)
                         .Where(e => !string.IsNullOrWhiteSpace(e))
                         .Select(e => e.Trim().ToLowerInvariant()),
            StringComparer.Ordinal);

    public bool IsAllowed(string? email, string adminEmail) =>
        !string.IsNullOrWhiteSpace(email) && AllowList(adminEmail).Contains(email.Trim().ToLowerInvariant());
}
