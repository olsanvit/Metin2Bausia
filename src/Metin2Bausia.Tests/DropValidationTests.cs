using Metin2Bausia.Web.Data;

namespace Metin2Bausia.Tests;

/// <summary>Pravidla drop tabulky — bez DB i bez Blazoru, aby šla ověřit samostatně.</summary>
public class DropValidationTests
{
    private static DropGroupJson Group(params DropItemJson[] rows) =>
        new() { Type = "drop", Group = "skupina", Items = [.. rows] };

    private static DropItemJson Row(int? vnum = 1001, string? name = null, int count = 1, double pct = 10) =>
        new() { Vnum = vnum, ItemName = name, Count = count, Pct = pct };

    [Fact]
    public void PrazdnySeznamJeVPoradku()
        => Assert.Equal(DropValidation.Problem.None, DropValidation.FirstProblem([]).Problem);

    [Fact]
    public void SkupinaBezRadkuJeVPoradku()
        => Assert.Equal(DropValidation.Problem.None, DropValidation.FirstProblem([Group()]).Problem);

    [Fact]
    public void RadekSVnumemProjde()
        => Assert.Equal(DropValidation.Problem.None, DropValidation.FirstProblem([Group(Row())]).Problem);

    [Fact]
    public void RadekJenSeJmenemProjde()
    {
        // Typy kill/limit odkazují item jménem z proto, vnum u nich chybí legitimně
        var result = DropValidation.FirstProblem([Group(Row(vnum: null, name: "혈검+4"))]);
        Assert.Equal(DropValidation.Problem.None, result.Problem);
    }

    [Fact]
    public void RadekBezVnumuIJmenaNeprojde()
    {
        var (problem, group) = DropValidation.FirstProblem([Group(Row(vnum: null))]);
        Assert.Equal(DropValidation.Problem.MissingItem, problem);
        Assert.Equal("skupina", group);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void PocetMusiBytAsponJedna(int count)
        => Assert.Equal(DropValidation.Problem.BadCount,
                        DropValidation.FirstProblem([Group(Row(count: count))]).Problem);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100.1)]
    public void SanceMusiBytVRozsahu(double pct)
        => Assert.Equal(DropValidation.Problem.BadChance,
                        DropValidation.FirstProblem([Group(Row(pct: pct))]).Problem);

    [Fact]
    public void SanceStoProcentProjde()
        => Assert.Equal(DropValidation.Problem.None,
                        DropValidation.FirstProblem([Group(Row(pct: 100))]).Problem);

    [Fact]
    public void PrazdnyNazevSkupinySeVypiseJakoOtaznik()
    {
        var group = new DropGroupJson { Group = "  ", Items = [Row(vnum: null)] };
        Assert.Equal("?", DropValidation.FirstProblem([group]).Group);
    }

    [Fact]
    public void ReferencovaneVnumySeOdduplikujiAJmenaSeVynechaji()
    {
        var groups = new List<DropGroupJson>
        {
            Group(Row(vnum: 10), Row(vnum: 10), Row(vnum: null, name: "meč")),
            Group(Row(vnum: 20)),
        };
        Assert.Equal([10, 20], DropValidation.ReferencedVnums(groups));
    }

    [Fact]
    public void NajdeSePrvniProblemNeNejzavaznejsi()
    {
        // Pořadí hlášek musí odpovídat pořadí řádků, jinak admin opravuje jiný řádek, než mu stránka ukazuje
        var groups = new List<DropGroupJson>
        {
            Group(Row(count: 0)),
            Group(Row(vnum: null)),
        };
        Assert.Equal(DropValidation.Problem.BadCount, DropValidation.FirstProblem(groups).Problem);
    }
}
