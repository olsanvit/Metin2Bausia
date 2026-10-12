namespace Metin2Bausia.Web.Data;

/// <summary>Server ze seznamu Metin2 private serverů (tabulka KnowledgeServers).</summary>
public class KnowledgeServerRow
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = "";
    public string Style { get; set; } = "";
    public int? MaxLevel { get; set; }
    public string? ShortDescription { get; set; }
    public string? OnlineText { get; set; }
    public int? CommentsCount { get; set; }
    public string ListingUrl { get; set; } = "";
    public string DetailUrl { get; set; } = "";
    public DateTime FirstSeenUtc { get; set; }
    public DateTime LastSeenUtc { get; set; }
    public bool IsActive { get; set; }
    public string SourceName { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
}

/// <summary>Monstrum z wiki (tabulka KnowledgeMonsters).</summary>
public class KnowledgeMonsterRow
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = "";
    public string NameUrl { get; set; } = "";
    public string Element { get; set; } = "";
    public string Resistance { get; set; } = "";
    public string Race { get; set; } = "";
    public string Level { get; set; } = "";
    public string Stage { get; set; } = "";
    public string Location { get; set; } = "";
    public string Exp { get; set; } = "";
    public int? MobVnum { get; set; }
    public string? MobName { get; set; }
    public Guid? MobGuid { get; set; }
    public string SourceName { get; set; } = "";
    public DateTime UpdatedAt { get; set; }

    public int? LevelNumber => KnowledgeText.ParseLevel(Level);
}

/// <summary>Metin kámen z wiki (tabulka KnowledgeStones).</summary>
public class KnowledgeStoneRow
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = "";
    public string NameUrl { get; set; } = "";
    public string Spawn { get; set; } = "";
    public string SkillBook { get; set; } = "";
    public string Spirit { get; set; } = "";
    public string Soul { get; set; } = "";
    public string Level { get; set; } = "";
    public int? MobVnum { get; set; }
    public string? MobName { get; set; }
    public Guid? MobGuid { get; set; }
    public string SourceName { get; set; } = "";
    public DateTime UpdatedAt { get; set; }

    public int? LevelNumber => KnowledgeText.ParseLevel(Level);
}

public static class KnowledgeText
{
    /// <summary>
    /// Level je z wiki text („120“, „90-95“, prázdný) — pro řazení a filtr stačí první číslo.
    /// </summary>
    public static int? ParseLevel(string? level)
    {
        if (string.IsNullOrWhiteSpace(level)) return null;
        var digits = new string(level.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) ? n : null;
    }

    /// <summary>Wiki značí „ano“ odškrtnutím; prázdná buňka znamená ne.</summary>
    public static bool IsChecked(string? value) => !string.IsNullOrWhiteSpace(value);

    /// <summary>Odkazy přicházejí ze stahování webu — do href pustit jen http(s), ne javascript: apod.</summary>
    public static string? SafeUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps)
            ? u.AbsoluteUri : null;
}
