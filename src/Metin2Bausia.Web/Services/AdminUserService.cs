namespace Metin2Bausia.Web.Services;

public class AdminUserRow
{
    public Guid Guid { get; set; }
    public string Email { get; set; } = "";
    public bool IsSuperAdmin { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum AdminChange { Ok, InvalidEmail, AlreadyAdmin, NotFound, CannotRemoveSuperAdmin, CannotRemoveSelf }

/// <summary>
/// Kdo smí do adminu přes Google. Zdroj pravdy je tabulka AdminUsers (migrace 08);
/// dokud migrace na produkci neproběhne, platí původní seznam z appsettings — deploy kódu
/// tak nemůže zamknout správce venku.
/// </summary>
public class AdminUserService(IDbService db, GoogleAuthOptions google, AdminCredentialService credentials, ILogger<AdminUserService> log)
{
    // Stejný zdroj jako přihlášení heslem — super-admin je jeden pro obě cesty
    public string SuperAdminEmail => credentials.AdminEmail.Trim();

    public static string? Normalize(string? email)
    {
        var e = email?.Trim().ToLowerInvariant();
        // Stačí hrubá kontrola — skutečné ověření je přihlášení Googlem s tímhle e-mailem
        return string.IsNullOrEmpty(e) || e.Length > 254 || e.Count(c => c == '@') != 1
               || e.StartsWith('@') || e.EndsWith('@') || e.Any(char.IsWhiteSpace)
            ? null : e;
    }

    /// <summary>Při startu: super-admin a e-maily z appsettings se přenesou do tabulky (jen jednou).</summary>
    public async Task SeedAsync()
    {
        try
        {
            if (Normalize(SuperAdminEmail) is { } super)
                await db.ExecuteAsync(
                    @"INSERT INTO ""AdminUsers"" (""Email"", ""IsSuperAdmin"", ""CreatedBy"")
                      VALUES (@e, TRUE, 'seed') ON CONFLICT (""Email"") WHERE ""RemovedAt"" IS NULL
                      DO UPDATE SET ""IsSuperAdmin"" = TRUE", new { e = super });

            // Jen když tabulka nemá žádné sub-adminy — jinak by odebraný e-mail z configu ožil při restartu
            var any = await db.QueryFirstOrDefaultAsync<bool>(
                @"SELECT EXISTS (SELECT 1 FROM ""AdminUsers"" WHERE NOT ""IsSuperAdmin"")");
            if (!any)
                foreach (var e in google.AllowedEmails.Select(Normalize).OfType<string>().Distinct())
                    await db.ExecuteAsync(
                        @"INSERT INTO ""AdminUsers"" (""Email"", ""CreatedBy"") VALUES (@e, 'seed:appsettings')
                          ON CONFLICT (""Email"") WHERE ""RemovedAt"" IS NULL DO NOTHING", new { e });
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Seed AdminUsers přeskočen (chybí migrace 08?) — platí seznam z appsettings");
        }
    }

    public async Task<bool> IsAllowedAsync(string? email)
    {
        var e = Normalize(email);
        if (e == null) return false;
        if (e == Normalize(SuperAdminEmail)) return true;
        try
        {
            return await db.QueryFirstOrDefaultAsync<bool>(
                @"SELECT EXISTS (SELECT 1 FROM ""AdminUsers"" WHERE ""Email"" = @e AND ""RemovedAt"" IS NULL)", new { e });
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "AdminUsers nedostupná — ověřuji podle appsettings");
            return google.IsAllowed(e, SuperAdminEmail);
        }
    }

    public async Task<List<AdminUserRow>> ListAsync() =>
        (await db.QueryAsync<AdminUserRow>(
            @"SELECT ""Guid"", ""Email""::text AS ""Email"", ""IsSuperAdmin"", ""CreatedBy"", ""CreatedAt""
              FROM ""AdminUsers"" WHERE ""RemovedAt"" IS NULL
              ORDER BY ""IsSuperAdmin"" DESC, ""Email""")).ToList();

    public async Task<AdminChange> AddAsync(string? email, string op)
    {
        var e = Normalize(email);
        if (e == null) return AdminChange.InvalidEmail;
        // Insert + audit jedním příkazem; konflikt = e-mail už je aktivní správce
        var added = await db.ExecuteAsync(
            @"WITH ins AS (INSERT INTO ""AdminUsers"" (""Email"", ""CreatedBy"") VALUES (@e, @op)
                           ON CONFLICT (""Email"") WHERE ""RemovedAt"" IS NULL DO NOTHING RETURNING ""Guid"")
              INSERT INTO ""ApprovalLog"" (""EntityTable"", ""EntityGuid"", ""EntityName"", ""Action"", ""Operator"")
              SELECT 'AdminUsers', ""Guid"", @e, 'admin_added', @op FROM ins", new { e, op });
        return added == 0 ? AdminChange.AlreadyAdmin : AdminChange.Ok;
    }

    /// <param name="currentEmail">E-mail přihlášeného — jméno operátora je u Google účtu zobrazované jméno, ne e-mail</param>
    public async Task<AdminChange> RemoveAsync(Guid guid, string op, string? currentEmail)
    {
        var row = await db.QueryFirstOrDefaultAsync<AdminUserRow>(
            @"SELECT ""Guid"", ""Email""::text AS ""Email"", ""IsSuperAdmin"" FROM ""AdminUsers""
              WHERE ""Guid"" = @guid AND ""RemovedAt"" IS NULL", new { guid });
        if (row == null) return AdminChange.NotFound;
        if (row.IsSuperAdmin) return AdminChange.CannotRemoveSuperAdmin;
        if (row.Email == Normalize(currentEmail)) return AdminChange.CannotRemoveSelf;

        await db.ExecuteAsync(
            @"WITH upd AS (UPDATE ""AdminUsers"" SET ""RemovedAt"" = now(), ""RemovedBy"" = @op, ""UpdatedAt"" = now()
                           WHERE ""Guid"" = @guid AND ""RemovedAt"" IS NULL RETURNING ""Guid"", ""Email"")
              INSERT INTO ""ApprovalLog"" (""EntityTable"", ""EntityGuid"", ""EntityName"", ""Action"", ""Operator"")
              SELECT 'AdminUsers', ""Guid"", ""Email""::text, 'admin_removed', @op FROM upd", new { guid, op });
        return AdminChange.Ok;
    }
}
