using Dapper;
using FluentAssertions;
using Metin2Bausia.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Metin2Bausia.Tests;

/// <summary>
/// Správa sub-adminů proti skutečné DB: změna a audit vznikají spolu, super-admina ani sebe odebrat nejde.
/// Každý test používá vlastní e-mail, protože DB sdílí s ostatními třídami.
/// </summary>
[Collection(AdminDbCollection.Name)]
public class AdminUserServiceTests(AdminAppFactory factory)
{
    private readonly AdminUserService _service = factory.Services.GetRequiredService<AdminUserService>();

    private static string NewEmail() => $"sub-{Guid.NewGuid():N}@test.example";

    [Theory]
    [InlineData("  Someone@Example.COM ", "someone@example.com")]
    [InlineData("", null)]
    [InlineData("bez-zavinace", null)]
    [InlineData("a@b@c", null)]
    [InlineData("@example.com", null)]
    [InlineData("a b@example.com", null)]
    public void Normalize_TrimsLowercasesAndRejectsGarbage(string input, string? expected) =>
        AdminUserService.Normalize(input).Should().Be(expected);

    [Fact]
    public async Task Add_AllowsLoginAndWritesAudit()
    {
        var email = NewEmail();

        (await _service.AddAsync(email.ToUpperInvariant(), "tester")).Should().Be(AdminChange.Ok);

        (await _service.IsAllowedAsync(email)).Should().BeTrue();
        var row = (await _service.ListAsync()).Single(r => r.Email == email);
        row.IsSuperAdmin.Should().BeFalse();
        row.CreatedBy.Should().Be("tester");
        (await AuditAsync(row.Guid)).Should().Equal("admin_added");
    }

    [Fact]
    public async Task Add_Twice_ReportsAlreadyAdmin()
    {
        var email = NewEmail();
        await _service.AddAsync(email, "tester");

        (await _service.AddAsync(email, "tester")).Should().Be(AdminChange.AlreadyAdmin);
        (await _service.ListAsync()).Count(r => r.Email == email).Should().Be(1);
    }

    [Fact]
    public async Task Add_InvalidEmail_IsRejected() =>
        (await _service.AddAsync("nic", "tester")).Should().Be(AdminChange.InvalidEmail);

    [Fact]
    public async Task Remove_RevokesLogin_AndEmailCanBeAddedAgain()
    {
        var email = NewEmail();
        await _service.AddAsync(email, "tester");
        var guid = (await _service.ListAsync()).Single(r => r.Email == email).Guid;

        (await _service.RemoveAsync(guid, "tester", "jiny@test.example")).Should().Be(AdminChange.Ok);

        (await _service.IsAllowedAsync(email)).Should().BeFalse();
        (await AuditAsync(guid)).Should().Equal("admin_added", "admin_removed");
        (await _service.RemoveAsync(guid, "tester", null)).Should().Be(AdminChange.NotFound);
        (await _service.AddAsync(email, "tester")).Should().Be(AdminChange.Ok);
    }

    [Fact]
    public async Task Remove_Self_IsRefused()
    {
        var email = NewEmail();
        await _service.AddAsync(email, "tester");
        var guid = (await _service.ListAsync()).Single(r => r.Email == email).Guid;

        (await _service.RemoveAsync(guid, "Zobrazované jméno", email.ToUpperInvariant())).Should().Be(AdminChange.CannotRemoveSelf);
        (await _service.IsAllowedAsync(email)).Should().BeTrue();
    }

    [Fact]
    public async Task SuperAdmin_IsSeeded_AlwaysAllowed_AndCannotBeRemoved()
    {
        var super = (await _service.ListAsync()).Single(r => r.IsSuperAdmin);
        super.Email.Should().Be(AdminUserService.Normalize(_service.SuperAdminEmail));

        (await _service.IsAllowedAsync(_service.SuperAdminEmail)).Should().BeTrue();
        (await _service.RemoveAsync(super.Guid, "tester", "jiny@test.example")).Should().Be(AdminChange.CannotRemoveSuperAdmin);
    }

    [Fact]
    public async Task UnknownEmail_IsNotAllowed() =>
        (await _service.IsAllowedAsync(NewEmail())).Should().BeFalse();

    private static async Task<List<string>> AuditAsync(Guid guid)
    {
        await using var conn = new NpgsqlConnection(AdminAppFactory.ConnectionString);
        return (await conn.QueryAsync<string>(
            @"SELECT ""Action"" FROM ""ApprovalLog"" WHERE ""EntityTable"" = 'AdminUsers' AND ""EntityGuid"" = @guid
              ORDER BY ""CreatedAt"", ""Action""", new { guid })).ToList();
    }
}
