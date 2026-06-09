using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metin2Bausia.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IConfiguration _config;

    public string? Error { get; private set; }
    public string? Email { get; private set; }

    public LoginModel(IConfiguration config) => _config = config;

    public void OnGet(string? returnUrl = null)
    {
        // Pokud je user už přihlášen, přesměrovat na hlavní stránku
        if (User.Identity?.IsAuthenticated == true)
            Response.Redirect("/");
    }

    public async Task<IActionResult> OnPostAsync(
        string email, string password, string? returnUrl = null)
    {
        Email = email;

        var adminEmail    = _config["Admin:Email"]    ?? "";
        var adminPassword = _config["Admin:Password"] ?? "";

        if (!string.Equals(email, adminEmail, StringComparison.OrdinalIgnoreCase)
            || password != adminPassword)
        {
            Error = "Nesprávný e-mail nebo heslo.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,  adminEmail),
            new(ClaimTypes.Email, adminEmail),
            new(ClaimTypes.Role,  "Admin"),
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc   = DateTimeOffset.UtcNow.AddDays(7)
            });

        var target = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        return Redirect(target);
    }
}
