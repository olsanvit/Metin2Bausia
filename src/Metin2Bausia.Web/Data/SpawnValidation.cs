namespace Metin2Bausia.Web.Data;

/// <summary>
/// Kontroly spawnů mapy nezávislé na databázi.
/// Proč mimo komponentu: stejně jako u dropů plynou pravidla z formátu herních souborů
/// (regen/boss/stone/npc.txt) a musí jít otestovat bez Blazoru i bez DB.
/// </summary>
public static class SpawnValidation
{
    public enum Problem { None, BadVnum, BadCount }

    /// <summary>Typ spawnu podle game/src/regen.cpp: m/ma/s míří na moba, g/ga/r na skupinu.</summary>
    public static bool IsMobSpawn(string? type) => type is "m" or "ma" or "s";

    /// <summary>Typ `e` je výjimka z oblasti — nemá cíl, takže se u něj vnum nekontroluje.</summary>
    public static bool HasTarget(string? type) => type is not "e";

    public static Problem FirstProblem(IEnumerable<SpawnJson> spawns)
    {
        foreach (var sp in spawns)
        {
            if (HasTarget(sp.Type) && sp.Vnum < 1) return Problem.BadVnum;
            if (sp.Count < 1) return Problem.BadCount;
        }
        return Problem.None;
    }

    /// <summary>Vnumy mobů, na které spawny míří — vstup pro ověření existence.</summary>
    public static int[] MobVnums(IEnumerable<SpawnJson> spawns) =>
        spawns.Where(s => IsMobSpawn(s.Type) && s.Vnum > 0).Select(s => s.Vnum).Distinct().ToArray();

    /// <summary>Vnumy skupin, rozdělené podle toho, do které tabulky skupin patří.</summary>
    public static (int[] Groups, int[] GroupGroups) GroupVnums(IEnumerable<SpawnJson> spawns)
    {
        var withTarget = spawns.Where(s => HasTarget(s.Type) && !IsMobSpawn(s.Type) && s.Vnum > 0).ToList();
        return (
            withTarget.Where(s => s.Type is "g" or "ga").Select(s => s.Vnum).Distinct().ToArray(),
            withTarget.Where(s => s.Type == "r").Select(s => s.Vnum).Distinct().ToArray());
    }
}
