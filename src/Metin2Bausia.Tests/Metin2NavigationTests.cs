using FluentAssertions;
using Metin2Bausia.Web.Services;

namespace Metin2Bausia.Tests;

public class Metin2NavigationTests
{
    [Theory]
    [InlineData("/manage/items", true)]
    [InlineData("/manage/mobs/7e57a000-0000-4000-8000-000000000002", true)]
    [InlineData("/reports", true)]
    [InlineData("/review", true)]
    [InlineData("/knowledge/link", true)]
    [InlineData("/knowledge/monsters", false)]
    [InlineData("/", false)]
    [InlineData("/items", false)]
    public void IsAdminPath_OpensAdminSectionOnlyForAdminPages(string path, bool expected) =>
        Metin2Navigation.IsAdminPath(path).Should().Be(expected);
}
