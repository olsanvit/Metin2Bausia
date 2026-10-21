using Dapper;
using FluentAssertions;
using Npgsql;

namespace Metin2Bausia.Tests;

/// <summary>
/// Migrace 10: KnowledgeItems musí unést upsert rutiny podle "NameUrl" (stejný vzor jako KnowledgeMonsters).
/// </summary>
[Collection(AdminDbCollection.Name)]
public class KnowledgeItemsMigrationTests
{
    [Fact]
    public async Task Upsert_ByNameUrl_UpdatesInsteadOfDuplicating()
    {
        await using var conn = new NpgsqlConnection(AdminAppFactory.ConnectionString);
        var url = $"https://wiki.example/Item_{Guid.NewGuid():N}";
        const string upsert = @"
            INSERT INTO ""KnowledgeItems"" (""Name"", ""NameUrl"", ""Category"", ""Level"", ""Stats"", ""Metadata"")
            VALUES (@name, @url, 'Weapon', @level, '{""atk"": 10}'::jsonb, '{""import"": {""revid"": 1}}'::jsonb)
            ON CONFLICT (""NameUrl"") DO UPDATE SET ""Name"" = EXCLUDED.""Name"", ""Level"" = EXCLUDED.""Level"", ""UpdatedAt"" = now()";

        await conn.ExecuteAsync(upsert, new { name = "Meč", url, level = "1" });
        await conn.ExecuteAsync(upsert, new { name = "Meč+1", url, level = "5" });

        var rows = (await conn.QueryAsync<(string Name, string Level, int? ItemVnum, string SourceName)>(
            @"SELECT ""Name"", ""Level"", ""ItemVnum"", ""SourceName"" FROM ""KnowledgeItems"" WHERE ""NameUrl"" = @url", new { url })).ToList();
        rows.Should().ContainSingle();
        rows[0].Should().Be(("Meč+1", "5", null, "metin2-wiki"));
    }
}
