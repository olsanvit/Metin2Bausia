using System.Text.Json;

namespace Metin2Bausia.Web.Services;

/// <summary>
/// Spravuje admin heslo a příznak MustChangePassword v souboru data/admin-creds.json.
/// Email se čte z appsettings.json (Admin:Email), heslo se spravuje tímto servisem.
/// </summary>
public class AdminCredentialService
{
    private readonly string _dataPath;
    private readonly IConfiguration _config;

    private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

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
            // První spuštění — použij heslo z appsettings, nastav MustChangePassword=true
            var initial = new AdminCreds
            {
                Password           = _config["Admin:Password"] ?? "Admin@123",
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

    // ── Veřejné API ───────────────────────────────────────────────────────

    public string GetPassword() => LoadOrCreate().Password;

    public bool MustChangePassword => LoadOrCreate().MustChangePassword;

    public bool VerifyPassword(string password)
        => GetPassword() == password;

    /// <summary>Nastaví nové heslo a zruší příznak MustChangePassword.</summary>
    public void ChangePassword(string newPassword)
    {
        var creds = LoadOrCreate();
        creds.Password           = newPassword;
        creds.MustChangePassword = false;
        Save(creds);
    }

    // ── Model ─────────────────────────────────────────────────────────────

    private class AdminCreds
    {
        public string Password           { get; set; } = "Admin@123";
        public bool   MustChangePassword { get; set; } = true;
    }
}
