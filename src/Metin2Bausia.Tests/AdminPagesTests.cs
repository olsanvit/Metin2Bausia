using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Metin2Bausia.Tests;

/// <summary>
/// Stránky adminu vykreslené na serveru (prerender) s přihlášeným testovacím adminem.
/// Ověřuje dotazy, mapování Dapperu a markup — interaktivní uložení se tu nespouští.
/// </summary>
public class AdminPagesTests(AdminAppFactory factory) : IClassFixture<AdminAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Theory]
    [InlineData("/")]
    [InlineData("/items")]
    [InlineData("/mobs")]
    [InlineData("/items/browse")]
    [InlineData("/review")]
    [InlineData("/reports")]
    [InlineData("/stats")]
    [InlineData("/manage/items")]
    [InlineData("/manage/mobs")]
    [InlineData("/manage/maps")]
    [InlineData("/manage/systems")]
    [InlineData("/manage/quests")]
    [InlineData("/manage/groups")]
    public async Task AdminPage_RendersWithoutError(string url)
    {
        var html = await GetOkHtml(url);
        // Chyby DB se na stránkách ukazují jako toast-err nebo alert-danger
        html.Should().NotContain("toast-err", $"{url} zobrazila chybový toast");
        html.Should().NotContain("alert-danger", $"{url} zobrazila chybu");
    }

    [Fact]
    public async Task ItemDetail_ShowsEditableValues()
    {
        var html = await GetOkHtml($"/manage/items/{AdminAppFactory.ItemGuid}");

        html.Should().NotContain("toast-err");
        // SubType je v DB text — dřív ho model četl jako int a detail importovaného itemu spadl
        html.Should().Contain("Testovací meč");
        foreach (var value in new[] { "4242", "8484", "33", "11", "-5", "99" })
            html.Should().MatchRegex($"<input[^>]*type=\"number\"[^>]*value=\"{value}\"", $"hodnota {value} má být v editovatelném poli");
        html.Should().Contain("LEVEL").And.Contain("APPLY_STR");
        html.Should().Contain("<code>imported</code>");
    }

    [Fact]
    public async Task MobDetail_ShowsEditableStats()
    {
        var html = await GetOkHtml($"/manage/mobs/{AdminAppFactory.MobGuid}");

        html.Should().NotContain("toast-err");
        html.Should().Contain("Testovací vlk");
        foreach (var value in new[] { "17", "5151", "616", "21", "22", "23", "24", "105", "106", "30", "40", "55", "1777" })
            html.Should().MatchRegex($"<input[^>]*type=\"number\"[^>]*value=\"{value}\"", $"hodnota {value} má být v editovatelném poli");
        html.Should().NotMatchRegex("<input[^>]*readonly", "bojové statistiky už nejsou jen ke čtení");
    }

    [Theory]
    [InlineData("cs", "Správa itemů")]
    [InlineData("en", "Item management")]
    [InlineData("de", "Item-Verwaltung")]
    public async Task ManageItems_RendersInSelectedLanguage(string culture, string title)
    {
        // Stejná cookie, kterou nastavuje /set-culture z LangSwitcheru
        var html = await GetOkHtml("/manage/items", $".AspNetCore.Culture=c={culture}|uic={culture}");

        html.Should().Contain($"<html lang=\"{culture}\"");
        html.Should().Contain(title);
    }

    private async Task<string> GetOkHtml(string url, string? cookie = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (cookie != null) request.Headers.Add("Cookie", cookie);
        var response = await _client.SendAsync(request);
        ((int)response.StatusCode).Should().Be(200, $"GET {url}");
        // Blazor kóduje diakritiku v HTML (í → &#xED;) — porovnává se dekódovaný text
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }
}
