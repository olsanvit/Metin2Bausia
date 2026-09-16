using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Metin2Bausia.Tests;

/// <summary>
/// Aplikace s přihlášeným testovacím adminem a databází se schématem a vzorovými daty.
/// Proč: admin je celý za [Authorize] a heslo do testů nepatří — testovací schéma autentizace
/// existuje jen tady, produkční Program.cs zůstává beze změny.
/// </summary>
public sealed class AdminAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public static readonly Guid ItemGuid = Guid.Parse("7e57a000-0000-4000-8000-000000000001");
    public static readonly Guid MobGuid = Guid.Parse("7e57a000-0000-4000-8000-000000000002");
    public const int ItemVnum = 990001;
    public const int MobVnum = 990002;

    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("ConnectionStrings__Metin2Bausia")
        ?? "Host=localhost;Port=54321;Database=ci_test_db;Username=postgres;Password=postgres";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Metin2Bausia", ConnectionString);
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = TestAuthHandler.SchemeName;
                    o.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    public async Task InitializeAsync()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        // CI startuje s prázdnou databází — schéma se nahraje z migrací, lokální DB se jen doplní vzorkem
        await using (var check = new NpgsqlCommand("SELECT to_regclass('\"Items\"') IS NOT NULL", conn))
        {
            if (!(bool)(await check.ExecuteScalarAsync())!)
            {
                foreach (var file in Directory.GetFiles(FindMigrationsDir(), "0*.sql").Order())
                {
                    await using var migrate = new NpgsqlCommand(await File.ReadAllTextAsync(file), conn);
                    await migrate.ExecuteNonQueryAsync();
                }
            }
        }

        const string seed = """
            DELETE FROM "Items" WHERE "Guid" = @ig OR "Vnum" = @iv;
            DELETE FROM "Mobs"  WHERE "Guid" = @mg OR "Vnum" = @mv;
            INSERT INTO "Items" ("Guid", "Vnum", "Name", "LocaleName", "ItemType", "SubType",
                                 "Gold", "Buy", "Weight", "Size",
                                 "LimitType0", "LimitValue0", "ApplyType0", "ApplyValue0",
                                 "Value0", "Value5", "ContentStatus", "IsEnabled", "DataOrigin")
            VALUES (@ig, @iv, 'test_sword', 'Testovací meč', 'ITEM_WEAPON', 'WEAPON_SWORD',
                    4242, 8484, 7, 2,
                    'LEVEL', 33, 'APPLY_STR', 11,
                    -5, 99, 'approved', TRUE, 'imported');
            INSERT INTO "Mobs" ("Guid", "Vnum", "Name", "LocaleName", "MobType", "Rank",
                                "Level", "MaxHp", "Exp", "Atk", "Def", "MagicAtk", "MagicDef",
                                "AttackSpeed", "MoveSpeed", "GoldMin", "GoldMax", "GoldDropRate", "AggressiveSight",
                                "ContentStatus", "IsEnabled", "DataOrigin")
            VALUES (@mg, @mv, 'test_wolf', 'Testovací vlk', 'MONSTER', 'PAWN',
                    17, 5151, 616, 21, 22, 23, 24,
                    105, 106, 30, 40, 55, 1777,
                    'approved', TRUE, 'imported');
            """;
        await using var cmd = new NpgsqlCommand(seed, conn);
        cmd.Parameters.AddWithValue("ig", ItemGuid);
        cmd.Parameters.AddWithValue("iv", ItemVnum);
        cmd.Parameters.AddWithValue("mg", MobGuid);
        cmd.Parameters.AddWithValue("mv", MobVnum);
        await cmd.ExecuteNonQueryAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;

    private static string FindMigrationsDir()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "database", "postgres");
            if (Directory.Exists(candidate)) return candidate;
        }
        throw new DirectoryNotFoundException("database/postgres nenalezeno nad " + AppContext.BaseDirectory);
    }
}

/// <summary>Každý požadavek je přihlášený testovací admin.</summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "test-admin")], SchemeName);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
    }
}
