namespace Metin2Bausia.Web.Data;

public class PendingItem
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public string? ItemType { get; set; }
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
    public string? Rank { get; set; }
    public string ContentStatus { get; set; } = "pending";
    public string? SourceName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ConfidenceScore { get; set; }
}

public class AgentRunReport
{
    public Guid Guid { get; set; }
    public string? AgentName { get; set; }
    public string? RunMode { get; set; }
    public bool? Success { get; set; }
    public string? PromptVersion { get; set; }
    public string? SkillsVersion { get; set; }
    public string? McpVersion { get; set; }
    public string? DbStatus { get; set; }
    public string? ReadinessStatus { get; set; }
    public int? DurationMs { get; set; }
    public int? EntitiesProcessed { get; set; }
    public int? EntitiesInserted { get; set; }
    public int? EntitiesUpdated { get; set; }
    public int? EntitiesFailed { get; set; }
    public string? BlockerCategory { get; set; }
    public string? Highlights { get; set; }   // jsonb pole v DB — čteme jako text a zobrazujeme zkráceně
    public string? Errors { get; set; }       // jsonb pole v DB
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ManualReviewItem
{
    public Guid Guid { get; set; }
    public string? EntityTable { get; set; }
    public Guid? EntityGuid { get; set; }
    public string? Reason { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── Manage stránky (IsEnabled toggle) ──────────────────────
public class ManageItem
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public string? ItemType { get; set; }
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
    public string? MobType { get; set; }
    public string? Rank { get; set; }
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

// ── Detail stránky (full data) ──────────────────────────────
public class DetailItem
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public string? ItemType { get; set; }
    public int? SubType { get; set; }
    public int? Weight { get; set; }
    public int? Size { get; set; }
    public long? Gold { get; set; }
    public long? Buy { get; set; }
    public string? LimitType0 { get; set; } public int? LimitValue0 { get; set; }
    public string? LimitType1 { get; set; } public int? LimitValue1 { get; set; }
    public string? ApplyType0 { get; set; } public int? ApplyValue0 { get; set; }
    public string? ApplyType1 { get; set; } public int? ApplyValue1 { get; set; }
    public string? ApplyType2 { get; set; } public int? ApplyValue2 { get; set; }
    public long? Value0 { get; set; }
    public long? Value1 { get; set; }
    public long? Value2 { get; set; }
    public long? Value3 { get; set; }
    public long? Value4 { get; set; }
    public long? Value5 { get; set; }
    public string? Description { get; set; }
    public string? SourceName { get; set; }
    public string? SourceUrl { get; set; }
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
    public decimal? FinalScore { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DetailMob
{
    public Guid Guid { get; set; }
    public int? Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public string? MobType { get; set; }
    public string? Rank { get; set; }
    public string? BattleType { get; set; }
    public int? Level { get; set; }
    public long? MaxHp { get; set; }
    public long? Exp { get; set; }
    public long? GoldMin { get; set; }
    public long? GoldMax { get; set; }
    public int? GoldDropRate { get; set; }
    public int? AttackSpeed { get; set; }
    public int? MoveSpeed { get; set; }
    public int? Atk { get; set; }
    public int? MagicAtk { get; set; }
    public int? Def { get; set; }
    public int? MagicDef { get; set; }
    public int? AggressiveSight { get; set; }
    public int? AttackRange { get; set; }
    public string? Description { get; set; }
    public string? SourceName { get; set; }
    public string? SourceUrl { get; set; }
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
    public decimal? FinalScore { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DetailMap
{
    public Guid Guid { get; set; }
    public int? MapIndex { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public string? MapType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
    public string? Description { get; set; }
    public string? PreviewImageUrl { get; set; }
    public string? SourceName { get; set; }
    public string? SourceUrl { get; set; }
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
    public decimal? FinalScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DetailSystem
{
    public Guid Guid { get; set; }
    public string SystemName { get; set; } = "";
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Implementation { get; set; }
    public string? Complexity { get; set; }
    public string? SourceServer { get; set; }
    public string? SourceName { get; set; }
    public string? SourceUrl { get; set; }
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
    public decimal? FinalScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
