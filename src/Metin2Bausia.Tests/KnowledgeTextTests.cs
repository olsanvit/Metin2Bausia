using FluentAssertions;
using Metin2Bausia.Web.Data;

namespace Metin2Bausia.Tests;

public class KnowledgeTextTests
{
    [Theory]
    [InlineData("120", 120)]
    [InlineData("90-95", 90)]
    [InlineData("Lv. 35", 35)]
    [InlineData("", null)]
    [InlineData("?", null)]
    public void ParseLevel_TakesFirstNumber(string input, int? expected) =>
        KnowledgeText.ParseLevel(input).Should().Be(expected);

    [Theory]
    [InlineData("https://en-wiki.metin2.gameforge.com/index.php/Test", true)]
    [InlineData("http://metin2pserver.net/x", true)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("/relative/path", false)]
    [InlineData(null, false)]
    public void SafeUrl_AllowsOnlyHttp(string? url, bool allowed) =>
        (KnowledgeText.SafeUrl(url) != null).Should().Be(allowed);

    [Theory]
    [InlineData("√", true)]
    [InlineData(" ", false)]
    [InlineData("", false)]
    public void IsChecked_TreatsAnyMarkAsYes(string value, bool expected) =>
        KnowledgeText.IsChecked(value).Should().Be(expected);
}
