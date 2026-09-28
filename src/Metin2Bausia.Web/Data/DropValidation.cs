namespace Metin2Bausia.Web.Data;

/// <summary>
/// Kontroly drop tabulky nezávislé na databázi.
/// Proč mimo komponentu: pravidla plynou z formátu mob_drop_item.txt a musí jít otestovat
/// bez Blazoru i bez DB. Existenci vnumu ověřuje až stránka, ta jediná má přístup k itemům.
/// </summary>
public static class DropValidation
{
    /// <summary>Druh problému; text hlášky skládá stránka, aby zůstal lokalizovaný.</summary>
    public enum Problem { None, MissingItem, BadCount, BadChance }

    /// <summary>První nalezený problém a skupina, ve které je. Prázdné skupiny jsou v pořádku.</summary>
    public static (Problem Problem, string Group) FirstProblem(IEnumerable<DropGroupJson> groups)
    {
        foreach (var g in groups)
        {
            var label = string.IsNullOrWhiteSpace(g.Group) ? "?" : g.Group;
            foreach (var row in g.Items)
            {
                // Server potřebuje buď vnum, nebo jméno z proto — jinak řádek nikam neodkazuje
                if (row.Vnum is null && string.IsNullOrWhiteSpace(row.ItemName)) return (Problem.MissingItem, label);
                if (row.Count < 1) return (Problem.BadCount, label);
                if (row.Pct <= 0 || row.Pct > 100) return (Problem.BadChance, label);
            }
        }
        return (Problem.None, "");
    }

    /// <summary>Vnumy, na které se drop odkazuje — vstup pro ověření, že takové itemy existují.</summary>
    public static int[] ReferencedVnums(IEnumerable<DropGroupJson> groups) =>
        groups.SelectMany(g => g.Items)
              .Where(i => i.Vnum.HasValue)
              .Select(i => i.Vnum!.Value)
              .Distinct()
              .ToArray();
}
