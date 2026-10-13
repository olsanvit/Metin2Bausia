using FluentAssertions;
using Metin2Bausia.Web.Data;

namespace Metin2Bausia.Tests;

public class KnowledgeLinkingTests
{
    private static MobCandidate Mob(int vnum, string name, int? level, string type = "MONSTER", string? locale = null) =>
        new() { Guid = Guid.NewGuid(), Vnum = vnum, Name = name, LocaleName = locale, Level = level, MobType = type };

    [Fact]
    public void Tokens_DropParenthesesDiacriticsAndCase()
    {
        KnowledgeLinking.Tokens("Serpent Soldier (Ice)").Should().BeEquivalentTo(["serpent", "soldier"]);
        KnowledgeLinking.Tokens("Šedý Vlk").Should().BeEquivalentTo(["sedy", "vlk"]);
        KnowledgeLinking.Tokens(null).Should().BeEmpty();
    }

    [Theory]
    [InlineData(KnowledgeKind.Stone, "STONE", true)]
    [InlineData(KnowledgeKind.Stone, "MONSTER", false)]
    [InlineData(KnowledgeKind.Stone, null, true)]
    [InlineData(KnowledgeKind.Monster, "STONE", false)]
    [InlineData(KnowledgeKind.Monster, "MONSTER", true)]
    [InlineData(KnowledgeKind.Monster, "stone", false)]
    public void IsAllowed_KeepsStonesAndMonstersApart(KnowledgeKind kind, string? type, bool expected) =>
        KnowledgeLinking.IsAllowed(kind, type).Should().Be(expected);

    [Fact]
    public void Rank_PrefersNameMatchThenLevel()
    {
        var mobs = new[]
        {
            Mob(1, "Wild Dog", 5),
            Mob(2, "Serpent Soldier", 90),
            Mob(3, "Serpent Soldier", 125),
            Mob(4, "Serpent Metin", 125, "STONE"),
        };

        var ranked = KnowledgeLinking.Rank(KnowledgeKind.Monster, "Serpent Soldier (Ice)", "125", mobs);

        ranked.Select(c => c.Mob.Vnum).Should().StartWith([3, 2]);
        ranked.Should().NotContain(c => c.Mob.Vnum == 4, "kámen se monstru nenabízí");
        ranked[0].LevelDiff.Should().Be(0);
    }

    [Fact]
    public void Rank_UsesLevelWhenNamesDoNotMatch()
    {
        // Wiki je anglicky, naše jména česky — rozhoduje level
        var mobs = new[] { Mob(10, "Vlk", 40), Mob(11, "Medvěd", 12) };

        var ranked = KnowledgeLinking.Rank(KnowledgeKind.Monster, "Wolf", "12", mobs);

        ranked[0].Mob.Vnum.Should().Be(11);
    }

    [Fact]
    public void Rank_ForStonesOffersOnlyStones()
    {
        var mobs = new[] { Mob(20, "Metin of Combat", 10, "STONE"), Mob(21, "Metin of Combat", 10) };

        KnowledgeLinking.Rank(KnowledgeKind.Stone, "Metin of Combat", "10", mobs)
            .Should().ContainSingle().Which.Mob.Vnum.Should().Be(20);
    }

    [Fact]
    public void Rank_RespectsTake() =>
        KnowledgeLinking.Rank(KnowledgeKind.Monster, "x", "1",
            Enumerable.Range(1, 30).Select(i => Mob(i, "m" + i, i)), take: 5).Should().HaveCount(5);
}
