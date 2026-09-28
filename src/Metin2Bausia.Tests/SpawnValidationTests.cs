using Metin2Bausia.Web.Data;

namespace Metin2Bausia.Tests;

/// <summary>Pravidla spawnů mapy — bez DB i bez Blazoru.</summary>
public class SpawnValidationTests
{
    private static SpawnJson Spawn(string type = "m", int vnum = 101, int count = 1) =>
        new() { File = "regen.txt", Type = type, Vnum = vnum, Count = count, Time = "5m" };

    [Theory]
    [InlineData("m")]
    [InlineData("ma")]
    [InlineData("s")]
    public void MobSpawnySeRozpoznaji(string type) => Assert.True(SpawnValidation.IsMobSpawn(type));

    [Theory]
    [InlineData("g")]
    [InlineData("ga")]
    [InlineData("r")]
    [InlineData("e")]
    public void OstatniTypyNejsouMobSpawn(string type) => Assert.False(SpawnValidation.IsMobSpawn(type));

    [Fact]
    public void TypEJeVyjimkaBezCile() => Assert.False(SpawnValidation.HasTarget("e"));

    [Fact]
    public void PlatnySpawnProjde()
        => Assert.Equal(SpawnValidation.Problem.None, SpawnValidation.FirstProblem([Spawn()]));

    [Fact]
    public void SpawnBezVnumuNeprojde()
        => Assert.Equal(SpawnValidation.Problem.BadVnum, SpawnValidation.FirstProblem([Spawn(vnum: 0)]));

    [Fact]
    public void VyjimkaBezVnumuProjde()
    {
        // Typ `e` označuje oblast, kde se nespawnuje nic — vnum u něj nedává smysl
        Assert.Equal(SpawnValidation.Problem.None, SpawnValidation.FirstProblem([Spawn("e", vnum: 0)]));
    }

    [Fact]
    public void PocetMusiBytAsponJedna()
        => Assert.Equal(SpawnValidation.Problem.BadCount, SpawnValidation.FirstProblem([Spawn(count: 0)]));

    [Fact]
    public void MobVnumySeOdduplikujiABeroouJenMobSpawny()
    {
        var spawns = new List<SpawnJson>
        {
            Spawn("m", 101), Spawn("ma", 101), Spawn("s", 102),
            Spawn("g", 900), Spawn("e", 0),
        };
        Assert.Equal([101, 102], SpawnValidation.MobVnums(spawns));
    }

    [Fact]
    public void SkupinySeRozdeliPodleTypu()
    {
        var spawns = new List<SpawnJson>
        {
            Spawn("g", 900), Spawn("ga", 901), Spawn("r", 500), Spawn("m", 101),
        };
        var (groups, groupGroups) = SpawnValidation.GroupVnums(spawns);
        Assert.Equal([900, 901], groups);
        Assert.Equal([500], groupGroups);
    }
}
