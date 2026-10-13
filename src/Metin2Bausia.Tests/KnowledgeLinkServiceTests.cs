using Dapper;
using FluentAssertions;
using Metin2Bausia.Web.Data;
using Metin2Bausia.Web.Services;
using Npgsql;

namespace Metin2Bausia.Tests;

/// <summary>
/// Párování proti skutečné DB — ověřuje, že změna a audit vznikají spolu a že kámen jde jen na mob STONE.
/// Každý test si zakládá vlastní řádky, protože ostatní třídy testů seedují sdílená data souběžně.
/// </summary>
[Collection(AdminDbCollection.Name)]
public class KnowledgeLinkServiceTests
{
    private const int StoneVnum = 990013;
    private readonly KnowledgeLinkService _service = new(new DbService(AdminAppFactory.ConnectionString));

    [Fact]
    public async Task Link_SetsVnumAndWritesAudit()
    {
        var guid = await NewRowAsync("KnowledgeMonsters");

        var (result, mob) = await _service.LinkAsync(KnowledgeKind.Monster, guid, AdminAppFactory.MobVnum, "tester@test");

        result.Should().Be(LinkResult.Ok);
        mob!.Vnum.Should().Be(AdminAppFactory.MobVnum);
        (await VnumAsync("KnowledgeMonsters", guid)).Should().Be(AdminAppFactory.MobVnum);
        var log = await AuditAsync(guid);
        log.Should().ContainSingle();
        log[0].Action.Should().Be("linked");
        log[0].Operator.Should().Be("tester@test");
        log[0].NewStatus.Should().Be(AdminAppFactory.MobVnum.ToString());
    }

    [Fact]
    public async Task Unlink_ClearsVnumAndWritesAudit()
    {
        var guid = await NewRowAsync("KnowledgeMonsters", AdminAppFactory.MobVnum);

        (await _service.UnlinkAsync(KnowledgeKind.Monster, guid, "tester@test")).Should().Be(LinkResult.Ok);

        (await VnumAsync("KnowledgeMonsters", guid)).Should().BeNull();
        var log = await AuditAsync(guid);
        log.Should().ContainSingle().Which.Action.Should().Be("unlinked");
        log[0].PrevStatus.Should().Be(AdminAppFactory.MobVnum.ToString());
    }

    [Fact]
    public async Task Unlink_WhenNotLinked_DoesNothing()
    {
        var guid = await NewRowAsync("KnowledgeMonsters");

        (await _service.UnlinkAsync(KnowledgeKind.Monster, guid, "tester@test")).Should().Be(LinkResult.RowNotFound);
        (await AuditAsync(guid)).Should().BeEmpty();
    }

    [Fact]
    public async Task Stone_RejectsNonStoneMob_AndAcceptsStone()
    {
        await EnsureStoneMobAsync();
        var guid = await NewRowAsync("KnowledgeStones");

        (await _service.LinkAsync(KnowledgeKind.Stone, guid, AdminAppFactory.MobVnum, "t")).Result.Should().Be(LinkResult.WrongMobType);
        (await VnumAsync("KnowledgeStones", guid)).Should().BeNull();
        (await AuditAsync(guid)).Should().BeEmpty();

        (await _service.LinkAsync(KnowledgeKind.Stone, guid, StoneVnum, "t")).Result.Should().Be(LinkResult.Ok);
        (await VnumAsync("KnowledgeStones", guid)).Should().Be(StoneVnum);
    }

    [Theory]
    [InlineData(0, LinkResult.InvalidVnum)]
    [InlineData(-3, LinkResult.InvalidVnum)]
    [InlineData(987654321, LinkResult.MobNotFound)]
    public async Task Link_InvalidInput_ChangesNothing(int vnum, LinkResult expected)
    {
        var guid = await NewRowAsync("KnowledgeMonsters");

        (await _service.LinkAsync(KnowledgeKind.Monster, guid, vnum, "t")).Result.Should().Be(expected);
        (await AuditAsync(guid)).Should().BeEmpty();
    }

    [Fact]
    public async Task Link_UnknownRow_ReturnsRowNotFound() =>
        (await _service.LinkAsync(KnowledgeKind.Monster, Guid.NewGuid(), AdminAppFactory.MobVnum, "t"))
            .Result.Should().Be(LinkResult.RowNotFound);

    private static async Task<Guid> NewRowAsync(string table, int? mobVnum = null)
    {
        var guid = Guid.NewGuid();
        await using var conn = new NpgsqlConnection(AdminAppFactory.ConnectionString);
        await conn.ExecuteAsync(
            $@"INSERT INTO ""{table}"" (""Guid"", ""Name"", ""NameUrl"", ""Level"", ""MobVnum"")
               VALUES (@guid, 'Link test', 'https://example.test/link/' || @guid, '10', @mobVnum)",
            new { guid, mobVnum });
        return guid;
    }

    private static async Task<int?> VnumAsync(string table, Guid guid)
    {
        await using var conn = new NpgsqlConnection(AdminAppFactory.ConnectionString);
        return await conn.QuerySingleAsync<int?>($@"SELECT ""MobVnum"" FROM ""{table}"" WHERE ""Guid"" = @guid", new { guid });
    }

    private static async Task<List<AuditRow>> AuditAsync(Guid guid)
    {
        await using var conn = new NpgsqlConnection(AdminAppFactory.ConnectionString);
        return (await conn.QueryAsync<AuditRow>(
            @"SELECT ""Action"", ""Operator"", ""PrevStatus"", ""NewStatus"" FROM ""ApprovalLog"" WHERE ""EntityGuid"" = @guid",
            new { guid })).ToList();
    }

    private static async Task EnsureStoneMobAsync()
    {
        await using var conn = new NpgsqlConnection(AdminAppFactory.ConnectionString);
        await conn.ExecuteAsync(
            @"INSERT INTO ""Mobs"" (""Vnum"", ""Name"", ""MobType"", ""Level"", ""ContentStatus"")
              VALUES (@v, 'test_stone', 'STONE', 10, 'approved') ON CONFLICT (""Vnum"") DO NOTHING",
            new { v = StoneVnum });
    }

    private sealed class AuditRow
    {
        public string Action { get; set; } = "";
        public string? Operator { get; set; }
        public string? PrevStatus { get; set; }
        public string? NewStatus { get; set; }
    }
}
