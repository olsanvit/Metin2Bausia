namespace Metin2Bausia.Web.Data;

/// <summary>
/// Jediný zdroj pravdy pro překlad textových kódů z DB na české popisky a Bootstrap badge třídy.
/// Vzniklo proto, že každá stránka měla vlastní mapování — a navzájem si odporovala
/// (StatsPage: 3 = Helma, ManageItems: 3 = Prsten), navíc mapovala čísla, zatímco sloupce jsou text.
/// </summary>
public static class ContentLabels
{
    // Hodnoty odpovídají dropdownům v detailních stránkách a komentářům ve schématu (01_content_tables.sql)
    private static readonly Dictionary<string, string> ItemTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["weapon"] = "Zbraň",      ["armor"] = "Brnění",       ["ring"] = "Prsten",
        ["belt"] = "Pásek",        ["boots"] = "Boty",         ["necklace"] = "Náhrdelník",
        ["bracelet"] = "Náramek",  ["shield"] = "Štít",        ["bow"] = "Luk",
        ["arrow"] = "Šíp",         ["potion"] = "Lektvar",     ["quest"] = "Quest",
        ["unique"] = "Unikátní",   ["special"] = "Speciální",  ["helmet"] = "Helma",

        // Symbolické hodnoty z item_proto.txt — tak je ukládá import herních dat a agent.
        // Klíče se nesmí shodovat s malými písmeny výše: slovník ignoruje velikost písmen
        // a duplicitní klíč by shodil statický konstruktor, tedy každou stránku.
        ["ITEM_NONE"] = "—",                 ["ITEM_WEAPON"] = "Zbraň",        ["ITEM_ARMOR"] = "Zbroj",
        ["ITEM_BELT"] = "Pásek",             ["ITEM_USE"] = "Použitelné",      ["ITEM_QUEST"] = "Quest",
        ["ITEM_UNIQUE"] = "Unikátní",        ["ITEM_SPECIAL"] = "Speciální",   ["ITEM_MATERIAL"] = "Materiál",
        ["ITEM_METIN"] = "Metin kámen",      ["ITEM_COSTUME"] = "Kostým",      ["ITEM_GIFTBOX"] = "Dárková krabice",
        ["ITEM_SKILLBOOK"] = "Kniha dovedností", ["ITEM_SKILLFORGET"] = "Zapomnění dovednosti",
        ["ITEM_ELK"] = "Yang",               ["ITEM_DS"] = "Dračí kámen",      ["ITEM_FISH"] = "Ryba",
        ["ITEM_ROD"] = "Rybářský prut",      ["ITEM_PICK"] = "Krumpáč",        ["ITEM_RESOURCE"] = "Surovina",
        ["ITEM_CAMPFIRE"] = "Táborák",       ["ITEM_POLYMORPH"] = "Polymorf",  ["ITEM_BLEND"] = "Směs",
        ["ITEM_EXTRACT"] = "Extrakt",        ["ITEM_CONTAINER"] = "Kontejner", ["ITEM_LOTTERY"] = "Loterie",
        ["ITEM_TREASURE_BOX"] = "Truhla s pokladem", ["ITEM_TREASURE_KEY"] = "Klíč k truhle",
    };

    private static readonly Dictionary<string, string> MobTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["monster"] = "Příšera", ["npc"] = "NPC", ["metin"] = "Metin",
        ["boss"] = "Boss",       ["chest"] = "Truhla",
        // Symbolika z mob_proto.txt; MONSTER a NPC pokrývají klíče výše (slovník ignoruje velikost písmen)
        ["STONE"] = "Metin", ["WARP"] = "Portál", ["GOTO"] = "Přesun", ["DOOR"] = "Dveře", ["BUILDING"] = "Budova",
    };

    private static readonly Dictionary<string, string> Ranks = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pawn"] = "Pěšák",             ["super_pawn"] = "Super pěšák",
        ["knight"] = "Rytíř",           ["super_knight"] = "Super rytíř",
        ["boss"] = "Boss",              ["king"] = "Král",
        // mob_proto.txt používá zkratky S_ místo super_
        ["S_PAWN"] = "Super pěšák",     ["S_KNIGHT"] = "Super rytíř",
    };

    /// <summary>Neznámý kód vracíme tak, jak přišel z DB — agent může přinést hodnotu, kterou zatím neznáme.</summary>
    private static string Lookup(Dictionary<string, string> map, string? key)
        => string.IsNullOrWhiteSpace(key) ? "—" : map.TryGetValue(key, out var v) ? v : key;

    public static string ItemType(string? t) => Lookup(ItemTypes, t);
    public static string MobType(string? t) => Lookup(MobTypes, t);
    public static string Rank(string? r) => Lookup(Ranks, r);

    public static string MobTypeBadge(string? t) => t?.ToLowerInvariant() switch
    {
        "npc" => "bg-info text-dark",
        "metin" or "stone" => "bg-warning text-dark",
        "chest" => "bg-secondary",
        _ => "bg-danger",
    };

    public static string RankBadge(string? r) => r?.ToLowerInvariant() switch
    {
        "boss" => "bg-danger",
        "king" => "bg-dark",
        _ => "bg-secondary",
    };

    public static string StatusBadge(string? s) => s switch
    {
        "approved" => "bg-success",
        "pending" => "bg-warning text-dark",
        "rejected" => "bg-danger",
        "needs_review" => "bg-info text-dark",
        _ => "bg-secondary",
    };

    /// <summary>Bossy a krále poznáme podle ranku — používá se pro filtr „Bossové" v seznamu mobů.</summary>
    public static bool IsBossRank(string? r)
        => string.Equals(r, "boss", StringComparison.OrdinalIgnoreCase)
        || string.Equals(r, "king", StringComparison.OrdinalIgnoreCase);
}
