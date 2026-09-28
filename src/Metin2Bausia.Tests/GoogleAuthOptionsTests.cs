using Metin2Bausia.Web.Services;

namespace Metin2Bausia.Tests;

/// <summary>
/// Kdo smí přes Google dovnitř. Testuje se zvlášť, protože chyba tady znamená
/// buď zavřený admin, nebo naopak otevřený komukoliv s účtem u Googlu.
/// </summary>
public class GoogleAuthOptionsTests
{
    private const string Admin = "olsanskyvitek@gmail.com";

    [Fact]
    public void BezKlicuNeniNastaveno()
        => Assert.False(new GoogleAuthOptions().IsConfigured);

    [Theory]
    [InlineData("YOUR_GOOGLE_CLIENT_ID")]
    [InlineData("your_google_client_id")]
    [InlineData("changeme")]
    [InlineData("  ")]
    public void ZastupneHodnotyNeprojdou(string value)
        => Assert.False(new GoogleAuthOptions { ClientId = value, ClientSecret = "skutecny-secret" }.IsConfigured);

    [Fact]
    public void SkutecneKliceProjdou()
        => Assert.True(new GoogleAuthOptions { ClientId = "123.apps.googleusercontent.com", ClientSecret = "abc" }.IsConfigured);

    [Fact]
    public void HlavniAdminProjdeIKdyzNeniVSeznamu()
        => Assert.True(new GoogleAuthOptions().IsAllowed(Admin, Admin));

    [Fact]
    public void CizekUcetNeprojde()
        => Assert.False(new GoogleAuthOptions().IsAllowed("kdokoliv@gmail.com", Admin));

    [Fact]
    public void DruhyUcetVitkaProjde()
    {
        // Oba účty jsou správcovské; whitelist je zároveň seznam adminů
        var options = new GoogleAuthOptions { AllowedEmails = ["olsansky575@gmail.com"] };
        Assert.True(options.IsAllowed("olsansky575@gmail.com", Admin));
        Assert.True(options.IsAllowed(Admin, Admin));
        Assert.False(options.IsAllowed("nekdo.jiny@gmail.com", Admin));
    }

    [Fact]
    public void SubAdminZeSeznamuProjde()
    {
        var options = new GoogleAuthOptions { AllowedEmails = ["parta@example.com"] };
        Assert.True(options.IsAllowed("parta@example.com", Admin));
    }

    [Theory]
    [InlineData("OLSANSKYVITEK@GMAIL.COM")]
    [InlineData("  olsanskyvitek@gmail.com  ")]
    public void PorovnaniIgnorujeVelikostAMezery(string email)
        => Assert.True(new GoogleAuthOptions().IsAllowed(email, Admin));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void PrazdnyEmailNeprojde(string? email)
        => Assert.False(new GoogleAuthOptions().IsAllowed(email, Admin));
}
