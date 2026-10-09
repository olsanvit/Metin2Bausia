using System.Net;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Metin2Bausia.Tests;

/// <summary>
/// Tlačítko Google musí poslat POST na handler Google, ne na přihlášení heslem.
/// 9. 10. 2026 v produkci odeslalo prázdné heslo — asp-page-handler se bez tag helperů ignoroval.
/// </summary>
public class GoogleLoginFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GoogleLoginFlowTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:Metin2Bausia",
                Environment.GetEnvironmentVariable("ConnectionStrings__Metin2Bausia")
                ?? AdminAppFactory.ConnectionString);
            builder.UseSetting("Admin:Password", Guid.NewGuid().ToString("N"));
            // Zástupné hodnoty skládané za běhu — jen aby se Google zaregistroval, nic se neověřuje
            builder.UseSetting("Authentication:Google:ClientId", "test-" + "client.apps.googleusercontent.com");
            builder.UseSetting("Authentication:Google:ClientSecret", "test-" + Guid.NewGuid().ToString("N"));
        }).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task GoogleButton_PostsToGoogleHandler_AndRedirectsToGoogle()
    {
        var html = await _client.GetStringAsync("/account/login?ReturnUrl=%2Fmanage%2Fitems");

        var form = Regex.Match(html, "<form method=\"post\" action=\"([^\"]*)\">\\s*<input name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        form.Success.Should().BeTrue("formulář Google musí mít explicitní action a antiforgery token");
        var action = WebUtility.HtmlDecode(form.Groups[1].Value);
        action.Should().Contain("handler=Google").And.Contain("returnUrl=%2Fmanage%2Fitems");

        var response = await _client.PostAsync(action, new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = form.Groups[2].Value
        }));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.Host.Should().Be("accounts.google.com");
    }
}
