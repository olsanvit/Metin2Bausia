using System.Globalization;
using System.Text;

namespace Metin2Bausia.Web.Data;

/// <summary>Mob, se kterým jde záznam ze Znalostí spárovat.</summary>
public class MobCandidate
{
    public Guid Guid { get; set; }
    public int Vnum { get; set; }
    public string Name { get; set; } = "";
    public string? LocaleName { get; set; }
    public string? MobType { get; set; }
    public int? Level { get; set; }

    public string Label => string.IsNullOrWhiteSpace(LocaleName) ? Name : LocaleName!;
}

public record ScoredCandidate(MobCandidate Mob, double Score, double NameScore, int? LevelDiff);

/// <summary>Nespárovaný záznam ze Znalostí (monstrum nebo kámen) pro stránku párování.</summary>
public class KnowledgeLinkRow
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = "";
    public string Level { get; set; } = "";
    public string? Extra { get; set; }

    public int? LevelNumber => KnowledgeText.ParseLevel(Level);
}

public enum KnowledgeKind { Monster, Stone }

public static class KnowledgeLinking
{
    public const string StoneMobType = "STONE";

    public static string TableOf(KnowledgeKind kind) => kind switch
    {
        KnowledgeKind.Monster => "KnowledgeMonsters",
        KnowledgeKind.Stone   => "KnowledgeStones",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    /// <summary>
    /// Kámen je ve hře mob typu STONE — jiný mob by byl chybné spárování. U monster se kameny naopak
    /// nenabízejí. Typ chybí jen u neúplně importovaných mobů, ty se nevylučují.
    /// </summary>
    public static bool IsAllowed(KnowledgeKind kind, string? mobType)
    {
        var isStone = string.Equals(mobType, StoneMobType, StringComparison.OrdinalIgnoreCase);
        return kind == KnowledgeKind.Stone ? isStone || string.IsNullOrEmpty(mobType) : !isStone;
    }

    /// <summary>
    /// Kandidáti seřazení podle shody jména a levelu. Wiki jména jsou anglická a naše většinou korejská
    /// nebo česká, takže jméno často nesedí vůbec — proto váží i level, který je na obou stranách číslem.
    /// </summary>
    public static List<ScoredCandidate> Rank(KnowledgeKind kind, string name, string? level,
        IEnumerable<MobCandidate> mobs, int take = 10)
    {
        var tokens = Tokens(name);
        var lvl = KnowledgeText.ParseLevel(level);
        return mobs
            .Where(m => IsAllowed(kind, m.MobType))
            .Select(m =>
            {
                var nameScore = Math.Max(Similarity(tokens, Tokens(m.Name)), Similarity(tokens, Tokens(m.LocaleName)));
                int? diff = lvl.HasValue && m.Level.HasValue ? Math.Abs(lvl.Value - m.Level.Value) : null;
                // Rozdíl 0 = 1, rozdíl 5 = 0,5; bez levelu na jedné straně neutrálních 0,25
                var levelScore = diff.HasValue ? 1.0 / (1.0 + diff.Value / 5.0) : 0.25;
                return new ScoredCandidate(m, 0.6 * nameScore + 0.4 * levelScore, nameScore, diff);
            })
            .Where(c => c.Score > 0)
            .OrderByDescending(c => c.Score)
            .ThenBy(c => c.Mob.Vnum)
            .Take(take)
            .ToList();
    }

    /// <summary>Jaccard nad slovy — pořadí ani přípona typu „(Ice)“ nehraje roli.</summary>
    public static double Similarity(HashSet<string> a, HashSet<string> b)
    {
        if (a.Count == 0 || b.Count == 0) return 0;
        var inter = a.Count(b.Contains);
        return (double)inter / (a.Count + b.Count - inter);
    }

    /// <summary>Slova jména bez diakritiky a závorek; „Serpent Soldier (Ice)“ → {serpent, soldier}.</summary>
    public static HashSet<string> Tokens(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return [];
        var withoutParens = new StringBuilder();
        var depth = 0;
        foreach (var c in name)
        {
            if (c == '(') depth++;
            else if (c == ')') depth = Math.Max(0, depth - 1);
            else if (depth == 0) withoutParens.Append(c);
        }
        var plain = new string(withoutParens.ToString().Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : ' ')
            .ToArray());
        return plain.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(t => t.Length > 1).ToHashSet();
    }
}
