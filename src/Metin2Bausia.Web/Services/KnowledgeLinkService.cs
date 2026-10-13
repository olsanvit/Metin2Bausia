using Metin2Bausia.Web.Data;

namespace Metin2Bausia.Web.Services;

public enum LinkResult { Ok, RowNotFound, MobNotFound, WrongMobType, InvalidVnum }

/// <summary>
/// Spárování záznamu ze Znalostí s mobem. Změna i záznam do ApprovalLog jdou jedním příkazem (CTE),
/// takže párování bez auditu nemůže vzniknout ani při chybě uprostřed.
/// </summary>
public class KnowledgeLinkService(IDbService db)
{
    public async Task<(LinkResult Result, MobCandidate? Mob)> LinkAsync(KnowledgeKind kind, Guid rowGuid, int vnum, string op)
    {
        if (vnum <= 0) return (LinkResult.InvalidVnum, null);

        var mob = await db.QueryFirstOrDefaultAsync<MobCandidate>(
            @"SELECT ""Guid"", ""Vnum"", ""Name"", ""LocaleName"", ""MobType"", ""Level""
              FROM ""Mobs"" WHERE ""Vnum"" = @vnum AND ""IsDeleted"" = FALSE", new { vnum });
        if (mob == null) return (LinkResult.MobNotFound, null);
        if (!KnowledgeLinking.IsAllowed(kind, mob.MobType)) return (LinkResult.WrongMobType, mob);

        var table = KnowledgeLinking.TableOf(kind);
        var changed = await db.ExecuteAsync(
            $@"WITH old AS (SELECT ""Guid"", ""Name"", ""MobVnum"" FROM ""{table}"" WHERE ""Guid"" = @g AND ""IsDeleted"" = FALSE),
                    upd AS (UPDATE ""{table}"" k SET ""MobVnum"" = @vnum, ""UpdatedAt"" = NOW()
                            FROM old WHERE k.""Guid"" = old.""Guid"" RETURNING k.""Guid"")
               INSERT INTO ""ApprovalLog"" (""EntityTable"", ""EntityGuid"", ""EntityVnum"", ""EntityName"", ""Action"",
                                            ""Operator"", ""Reason"", ""PrevStatus"", ""NewStatus"")
               SELECT @table, old.""Guid"", @vnum, old.""Name"", 'linked', @op,
                      'MobVnum ' || coalesce(old.""MobVnum""::text, '—') || ' → ' || @vnum,
                      old.""MobVnum""::text, @vnum::text
               FROM old JOIN upd ON upd.""Guid"" = old.""Guid""",
            new { g = rowGuid, vnum, op, table });
        return changed == 0 ? (LinkResult.RowNotFound, mob) : (LinkResult.Ok, mob);
    }

    public async Task<LinkResult> UnlinkAsync(KnowledgeKind kind, Guid rowGuid, string op)
    {
        var table = KnowledgeLinking.TableOf(kind);
        var changed = await db.ExecuteAsync(
            $@"WITH old AS (SELECT ""Guid"", ""Name"", ""MobVnum"" FROM ""{table}""
                            WHERE ""Guid"" = @g AND ""IsDeleted"" = FALSE AND ""MobVnum"" IS NOT NULL),
                    upd AS (UPDATE ""{table}"" k SET ""MobVnum"" = NULL, ""UpdatedAt"" = NOW()
                            FROM old WHERE k.""Guid"" = old.""Guid"" RETURNING k.""Guid"")
               INSERT INTO ""ApprovalLog"" (""EntityTable"", ""EntityGuid"", ""EntityVnum"", ""EntityName"", ""Action"",
                                            ""Operator"", ""Reason"", ""PrevStatus"", ""NewStatus"")
               SELECT @table, old.""Guid"", old.""MobVnum"", old.""Name"", 'unlinked', @op,
                      'MobVnum ' || old.""MobVnum"" || ' → —', old.""MobVnum""::text, NULL
               FROM old JOIN upd ON upd.""Guid"" = old.""Guid""",
            new { g = rowGuid, op, table });
        return changed == 0 ? LinkResult.RowNotFound : LinkResult.Ok;
    }
}
