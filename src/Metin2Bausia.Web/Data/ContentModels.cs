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
    public string? DropItemsJson { get; set; }
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
    public string? SpawnMobsJson { get; set; }
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

// ── Obsah importovaný z herních souborů (migrace 05) ────────────────────────

/// <summary>jsonb sloupce přichází jako text; parsování na jednom místě, ať se stránky nemusí starat o chyby formátu.</summary>
public static class JsonContent
{
    public static readonly System.Text.Json.JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static List<T> ParseList<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try { return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json, Options) ?? []; }
        catch (System.Text.Json.JsonException) { return []; }   // poškozený jsonb nesmí shodit celou stránku
    }
}

public class QuestListRow
{
    public Guid Guid { get; set; }
    public string? FileName { get; set; }
    public string QuestName { get; set; } = "";
    public string ContentStatus { get; set; } = "";
    public bool IsEnabled { get; set; }
    public int ScriptLength { get; set; }
    public string? Encoding { get; set; }
}

public class QuestDetailRow : QuestListRow
{
    public string? LocaleName { get; set; }
    public string? QuestType { get; set; }
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
    public int? StartNpcVnum { get; set; }
    public string? LuaScript { get; set; }
    public string? Description { get; set; }
    public string? DataOrigin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GroupRow
{
    public Guid Guid { get; set; }
    public string GroupType { get; set; } = "";
    public int GroupVnum { get; set; }
    public string? Name { get; set; }
    public int? LeaderVnum { get; set; }
    public string? GroupKind { get; set; }
    public string? EntriesJson { get; set; }
    public bool HasDuplicates { get; set; }

    private List<List<string>>? _entries;
    /// <summary>Řádky skupiny ze souboru — tvar se liší podle typu, proto zůstávají jako text.</summary>
    public List<List<string>> Entries => _entries ??= JsonContent.ParseList<List<string>>(EntriesJson);
}

/// <summary>Skupina dropů moba (mob_drop_item.txt).</summary>
public class DropGroupJson
{
    public string? Type { get; set; }
    public string? Group { get; set; }
    public string? KillDrop { get; set; }
    public string? LevelLimit { get; set; }
    public List<DropItemJson> Items { get; set; } = [];
}

public class DropItemJson
{
    public int? Vnum { get; set; }
    /// <summary>Typy kill/limit odkazují item jménem z proto — vnum pak může chybět.</summary>
    public string? ItemName { get; set; }
    public int Count { get; set; }
    public double Pct { get; set; }
    public string? Extra { get; set; }
}

/// <summary>Jeden řádek spawnu mapy (regen/boss/stone/npc.txt).</summary>
public class SpawnJson
{
    public string? File { get; set; }
    public string? Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int RangeX { get; set; }
    public int RangeY { get; set; }
    public int Z { get; set; }
    public int Dir { get; set; }
    public string? Time { get; set; }
    public int Pct { get; set; }
    public int Count { get; set; }
    public int Vnum { get; set; }
}

/// <summary>Odkaz na entitu podle vnum — pro dohledání jména v dropech a spawnech.</summary>
public class NameRef
{
    public Guid Guid { get; set; }
    public int Vnum { get; set; }
    public string? GroupType { get; set; }
    public string? Label { get; set; }
}
