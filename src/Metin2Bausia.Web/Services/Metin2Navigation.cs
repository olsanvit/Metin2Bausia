using Microsoft.Extensions.Localization;
using SharedServices.Models.UI;

namespace Metin2Bausia.Web.Services;

/// <summary>
/// Položky společné navigace (AppSideNav / AppTopBar / AppBottomNav ze SharedServices, kaskáda z USM 13. 10.).
/// Komponenty jsou sdílené, tady je jen co a kam. Detailové stránky v menu nejsou — vedou na ně seznamy.
/// <para>„Znalosti“ = přehled, fronty ke schválení a informace z webu; „Admin“ = správa herního obsahu.
/// Celý web je dnes jen pro Admina, takže dělení je hlavně kvůli přehlednosti (rozhodl Vítek 13. 10.).</para>
/// </summary>
public sealed class Metin2Navigation(IStringLocalizer<SharedResources> S, IDbService db, ILogger<Metin2Navigation> log)
{
    public sealed record Model(IReadOnlyList<AppNavSection> Sections, IReadOnlyList<AppNavItem> Bottom);

    public async Task<Model> BuildAsync()
    {
        var (items, mobs, review) = await CountsAsync();

        var dashboard = new AppNavItem("dashboard", S["Nav2Dashboard"], "bi-speedometer2", "/", ExactMatch: true);
        var pendingItems = new AppNavItem("items-pending", S["Nav2ItemsPending"], "bi-box-seam", "/items", items);
        var pendingMobs = new AppNavItem("mobs-pending", S["Nav2MobsPending"], "bi-bug", "/mobs", mobs);
        var monsters = new AppNavItem("kn-monsters", S["Nav2KnMonsters"], "bi-journal-richtext", "/knowledge/monsters");

        var knowledge = new AppNavSection("knowledge", S["Nav2SecKnowledge"],
        [
            dashboard,
            pendingItems,
            pendingMobs,
            new("stats", S["Nav2Stats"], "bi-bar-chart", "/stats"),
            new("kn-servers", S["Nav2KnServers"], "bi-hdd-network", "/knowledge/servers"),
            monsters,
            new("kn-stones", S["Nav2KnStones"], "bi-gem", "/knowledge/stones"),
        ]);

        var admin = new AppNavSection("admin", S["Nav2SecAdmin"],
        [
            new("admin", S["Nav2Admin"], "bi-shield-lock", "/admin", ExactMatch: true),
            new("admin-access", S["Nav2AdminAccess"], "bi-person-lock", "/admin/access"),
            new("manage-items", S["Nav2ManageItems"], "bi-box", "/manage/items"),
            new("manage-mobs", S["Nav2ManageMobs"], "bi-bug-fill", "/manage/mobs"),
            new("manage-maps", S["Nav2ManageMaps"], "bi-map", "/manage/maps"),
            new("manage-quests", S["Nav2ManageQuests"], "bi-file-earmark-code", "/manage/quests"),
            new("manage-systems", S["Nav2ManageSystems"], "bi-gear", "/manage/systems"),
            new("manage-groups", S["Nav2ManageGroups"], "bi-collection", "/manage/groups"),
            new("reports", S["Nav2Reports"], "bi-journal-text", "/reports"),
            new("review", S["Nav2Review"], "bi-exclamation-triangle", "/review", review),
            new("kn-link", S["Nav2KnLink"], "bi-link-45deg", "/knowledge/link"),
        ], AdminOnly: true);

        // Spodní lišta na mobilu jen ze „Znalostí“ — admin správa je za tlačítkem Menu.
        // Krátké popisky: „Itemy ke schválení“ se na 390 px pod ikonu nevešly
        IReadOnlyList<AppNavItem> bottom =
        [
            dashboard,
            pendingItems with { Label = S["Nav2BottomItems"] },
            pendingMobs with { Label = S["Nav2BottomMobs"] },
            monsters with { Label = S["Nav2BottomMonsters"] },
        ];

        return new Model([knowledge, admin], bottom);
    }

    /// <summary>Admin sekce se otevře rozbalená, když je uživatel na některé z jejích stránek.</summary>
    public static bool IsAdminPath(string path) =>
        path.StartsWith("/manage", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase)
        || path is "/reports" or "/review" or "/knowledge/link";

    private async Task<(int Items, int Mobs, int Review)> CountsAsync()
    {
        try
        {
            var c = await db.QueryFirstOrDefaultAsync<(int, int, int)>(
                @"SELECT (SELECT COUNT(*)::int FROM ""Items"" WHERE ""ContentStatus"" = 'pending' AND ""IsDeleted"" = FALSE),
                         (SELECT COUNT(*)::int FROM ""Mobs""  WHERE ""ContentStatus"" = 'pending' AND ""IsDeleted"" = FALSE),
                         (SELECT COUNT(*)::int FROM ""ManualReviewQueue"" WHERE ""Status"" = 'pending' AND ""IsDeleted"" = FALSE)");
            return c;
        }
        catch (Exception ex)
        {
            // Nedostupná DB nesmí shodit menu — bez počtů je pořád použitelné
            log.LogWarning(ex, "Počty do navigace se nenačetly");
            return (0, 0, 0);
        }
    }
}
