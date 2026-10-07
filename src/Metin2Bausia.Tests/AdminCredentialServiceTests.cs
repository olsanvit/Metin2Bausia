using Metin2Bausia.Web.Services;

namespace Metin2Bausia.Tests;

/// <summary>
/// Repo je veřejné — výchozí heslo admina nesmí existovat. Chybějící nebo zástupná hodnota
/// musí shodit start, ne tiše vytvořit účet se známým heslem.
/// </summary>
public class AdminCredentialServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("changeme_set_in_production")]
    [InlineData("CHANGEME")]
    [InlineData("YOUR_ADMIN_PASSWORD")]
    public void BezSkutecnehoHeslaStartSelze(string? configured)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => AdminCredentialService.RequireInitialPassword(configured));
        Assert.Contains("Admin:Password", ex.Message);
    }

    [Fact]
    public void SkutecneHesloProjde()
    {
        var value = Guid.NewGuid().ToString("N");
        Assert.Equal(value, AdminCredentialService.RequireInitialPassword(value));
    }
}
