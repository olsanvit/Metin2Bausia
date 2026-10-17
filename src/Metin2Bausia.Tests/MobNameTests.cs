using FluentAssertions;
using Metin2Bausia.Web.Data;
using Metin2Bausia.Web.Services;

namespace Metin2Bausia.Tests;

public class MobNameTests
{
    [Theory]
    [InlineData("??", "Divoký pes", "Divoký pes")]
    [InlineData("?? ????", "Modrý silný vlk", "Modrý silný vlk")]
    [InlineData("들개", "Divoký pes", "들개")]
    [InlineData("??", null, "??")]
    [InlineData("", "Vlk", "Vlk")]
    public void MobName_FallsBackToLocaleOnlyForLostKoreanName(string name, string? locale, string expected) =>
        ContentLabels.MobName(name, locale).Should().Be(expected);
}

public class GameServerStateTests
{
    [Fact]
    public void Evaluate_AllOpen_Running_SomeOpen_Partial_NoneOpen_Down_Empty_NotDeployed()
    {
        GameServerStatusService.Evaluate([new("auth", 11000, true), new("game", 13000, true)]).Should().Be(GameServerState.Running);
        GameServerStatusService.Evaluate([new("auth", 11000, true), new("game", 13000, false)]).Should().Be(GameServerState.Partial);
        GameServerStatusService.Evaluate([new("auth", 11000, false)]).Should().Be(GameServerState.Down);
        GameServerStatusService.Evaluate([]).Should().Be(GameServerState.NotDeployed);
    }
}
