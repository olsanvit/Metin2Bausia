using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Metin2Bausia.Tests;

/// <summary>
/// Stránky adminu vykreslené na serveru (prerender) s přihlášeným testovacím adminem.
/// Ověřuje dotazy, mapování Dapperu a markup — interaktivní uložení se tu nespouští.
/// </summary>
[Collection(AdminDbCollection.Name)]
public class AdminPagesTests(AdminAppFactory factory)
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
    [InlineData("/knowledge/servers")]
    [InlineData("/knowledge/monsters")]
    [InlineData("/knowledge/stones")]
    [InlineData("/knowledge/link")]
    [InlineData("/knowledge/link?kind=stone")]
    [InlineData("/admin")]
    [InlineData("/admin/access")]
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

    [Fact]
    public async Task Layout_UsesSharedNavigation()
    {
        var html = await GetOkHtml("/");

        html.Should().Contain("app-sidenav").And.Contain("app-bottomnav").And.Contain("app-topbar");
        html.Should().Contain("href=\"/knowledge/servers\"").And.Contain("href=\"/stats\"");

        // Admin sekce je sbalená (details), detaily v menu nejsou
        var admin = html[html.IndexOf("<details class=\"app-sidenav-section\"", StringComparison.Ordinal)..];
        admin.Should().Contain("href=\"/manage/items\"").And.Contain("href=\"/knowledge/link\"");
        html.Should().NotContain("href=\"/manage/items/");

        // Spodní lišta (herní menu) nevede do adminu
        var bottomStart = html.IndexOf("class=\"app-bottomnav\"", StringComparison.Ordinal);
        var bottom = html[bottomStart..html.IndexOf("</nav>", bottomStart, StringComparison.Ordinal)];
        bottom.Should().NotContain("/manage").And.NotContain("/reports").And.NotContain("/review");
    }

    [Fact]
    public async Task Layout_OpensAdminSectionOnAdminPage()
    {
        var html = await GetOkHtml("/manage/maps");
        html.Should().MatchRegex("<details class=\"app-sidenav-section\"[^>]*open");
    }

    [Fact]
    public async Task KnowledgeDetails_ShowSeededRows()
    {
        var server = await GetOkHtml($"/knowledge/servers/{AdminAppFactory.ServerGuid}");
        server.Should().Contain("Testovací server").And.Contain("Oldschool").And.Contain("https://example.test/server/1");

        // Monstrum je spárované s testovacím mobem — detail ukazuje odkaz na něj
        var monster = await GetOkHtml($"/knowledge/monsters/{AdminAppFactory.MonsterGuid}");
        monster.Should().Contain("Test Wolf (Ice)").And.Contain("Seungryong Valley")
            .And.Contain($"/manage/mobs/{AdminAppFactory.MobGuid}").And.Contain("Testovací vlk");

        var stone = await GetOkHtml($"/knowledge/stones/{AdminAppFactory.StoneGuid}");
        stone.Should().Contain("Test Metin").And.Contain("Joan, Pyungmoo");

        foreach (var html in new[] { server, monster, stone })
            html.Should().NotContain("alert-danger").And.NotContain("toast-err");
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

    [Theory]
    [InlineData("/login", "/account/login")]
    [InlineData("/register", "/account/login")]
    [InlineData("/forgot-password", "/account/login")]
    [InlineData("/access-pending", "/account/login")]
    [InlineData("/change-password", "/account/change-password")]
    public async Task OldAccountRoutes_RedirectToOwnLogin(string url, string target)
    {
        var response = await _client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.OriginalString.Should().Be(target);
    }

    [Theory]
    [InlineData("/tahle-stranka-neexistuje")]
    [InlineData("/sys/neexistuje")]
    public async Task UnknownRoute_Returns404WithAppPage(string url)
    {
        var response = await _client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await response.Content.ReadAsStringAsync()).Should().Contain("bi-signpost-split");
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
