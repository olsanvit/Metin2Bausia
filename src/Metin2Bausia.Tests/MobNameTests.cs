using FluentAssertions;
using Metin2Bausia.Web.Data;

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
