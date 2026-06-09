namespace Metin2Bausia.Web.Data;

public class PendingItem
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public int? ItemType { get; set; }
    public string ContentStatus { get; set; } = "pending";
    public string? SourceName { get; set; }
    public string? SourceUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ConfidenceScore { get; set; }
}

public class PendingMob
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public int? Level { get; set; }
    public int? Rank { get; set; }
    public string ContentStatus { get; set; } = "pending";
    public string? SourceName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ConfidenceScore { get; set; }
}

public class AgentRunReport
{
    public Guid Guid { get; set; }
    public string AgentType { get; set; } = "";
    public string AgentName { get; set; } = "";
    public string? ReadinessStatus { get; set; }
    public string? RunStatus { get; set; }
    public DateTime? RunRunAt { get; set; }
    public DateTime? RunFinishedAt { get; set; }
    public int? ItemsNew { get; set; }
    public int? MobsNew { get; set; }
    public int? ImagesDownloaded { get; set; }
    public string? Highlights { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ManualReviewItem
{
    public Guid Guid { get; set; }
    public string? EntityTable { get; set; }
    public Guid? EntityGuid { get; set; }
    public string? Reason { get; set; }
    public string? SourceA { get; set; }
    public string? SourceB { get; set; }
    public bool Resolved { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── Manage stránky (IsEnabled toggle) ──────────────────────
public class ManageItem
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public int? ItemType { get; set; }
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
    public string? SourceName { get; set; }
    public string? SourceUrl { get; set; }
}

public class ManageMob
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public int? Level { get; set; }
    public int? MobType { get; set; }
    public int? Rank { get; set; }
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
    public string? SourceName { get; set; }
}

public class ManageMap
{
    public Guid Guid { get; set; }
    public int? MapIndex { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
}

public class ManageSystem
{
    public Guid Guid { get; set; }
    public string SystemName { get; set; } = "";
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
}
