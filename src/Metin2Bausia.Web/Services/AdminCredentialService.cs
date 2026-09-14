using System.Security.Cryptography;
using System.Text.Json;

namespace Metin2Bausia.Web.Services;

/// <summary>
/// Spravuje admin heslo a příznak MustChangePassword v souboru data/admin-creds.json.
/// Email se čte z appsettings.json (Admin:Email), heslo se spravuje tímto servisem.
/// Heslo se ukládá jako PBKDF2 hash — dřív leželo na disku v plaintextu a kdokoliv s přístupem
/// k souboru (nebo k zálohám) ho rovnou přečetl.
/// </summary>
public class AdminCredentialService
{
    private readonly string _dataPath;
    private readonly IConfiguration _config;

    private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    // OWASP doporučení pro PBKDF2-HMAC-SHA256; drží se i na slabším hardwaru pod ~100 ms
    private const int Iterations = 210_000;
    private const int SaltBytes  = 16;
    private const int HashBytes  = 32;
    private const string HashPrefix = "pbkdf2";

    public AdminCredentialService(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        var dataDir = Path.Combine(env.ContentRootPath, "data");
        Directory.CreateDirectory(dataDir);
        _dataPath = Path.Combine(dataDir, "admin-creds.json");
    }

    public string AdminEmail => _config["Admin:Email"] ?? "olsanskyvitek@gmail.com";

    // ── Načtení stavu ─────────────────────────────────────────────────────

    private AdminCreds LoadOrCreate()
    {
        if (!File.Exists(_dataPath))
        {
            // První spuštění — heslo z appsettings rovnou zahashovat a vynutit změnu
            var initial = new AdminCreds
            {
                PasswordHash       = Hash(_config["Admin:Password"] ?? "Admin@123"),
                MustChangePassword = true
            };
            Save(initial);
            return initial;
        }

        try
        {
            var json = File.ReadAllText(_dataPath);
            return JsonSerializer.Deserialize<AdminCreds>(json) ?? new AdminCreds();
        }
        catch
        {
            return new AdminCreds();
        }
    }

    private void Save(AdminCreds creds)
        => File.WriteAllText(_dataPath, JsonSerializer.Serialize(creds, _json));

    // ── Hashování ─────────────────────────────────────────────────────────

    private static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashBytes);
        return $"{HashPrefix}:{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyHash(string stored, string password)
    {
        var parts = stored.Split(':');
        if (parts.Length != 4 || parts[0] != HashPrefix) return false;
        if (!int.TryParse(parts[1], out var iterations)) return false;

        byte[] salt, expected;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expected = Convert.FromBase64String(parts[3]);
        }
        catch { return false; }

        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        // Porovnání v konstantním čase — prosté == prozrazuje délku shodné předpony časováním
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    // ── Veřejné API ───────────────────────────────────────────────────────

    public bool MustChangePassword => LoadOrCreate().MustChangePassword;

    public bool VerifyPassword(string password)
    {
        var creds = LoadOrCreate();

        if (!string.IsNullOrEmpty(creds.PasswordHash))
            return VerifyHash(creds.PasswordHash, password);

        // Migrace starých souborů s plaintextem — při prvním úspěšném přihlášení se heslo zahashuje
        if (!string.IsNullOrEmpty(creds.Password))
        {
            var ok = CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(creds.Password),
                System.Text.Encoding.UTF8.GetBytes(password));
            if (ok)
            {
                creds.PasswordHash = Hash(password);
                creds.Password = null;
                Save(creds);
            }
            return ok;
        }

        return false;
    }

    /// <summary>Nastaví nové heslo a zruší příznak MustChangePassword.</summary>
    public void ChangePassword(string newPassword)
    {
        var creds = LoadOrCreate();
        creds.PasswordHash       = Hash(newPassword);
        creds.Password           = null;
        creds.MustChangePassword = false;
        Save(creds);
    }

    // ── Model ─────────────────────────────────────────────────────────────

    private class AdminCreds
    {
        /// <summary>Formát "pbkdf2:iterace:salt:hash" (base64).</summary>
        public string? PasswordHash       { get; set; }
        /// <summary>Pozůstatek po starém formátu — čte se jen kvůli migraci, nikdy se nezapisuje.</summary>
        public string? Password           { get; set; }
        public bool    MustChangePassword { get; set; } = true;
    }
}
