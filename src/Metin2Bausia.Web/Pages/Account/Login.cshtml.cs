using System.Security.Claims;
using Metin2Bausia.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metin2Bausia.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly AdminCredentialService _creds;

    public string? Error { get; private set; }
    public string? Email { get; private set; }

    public LoginModel(AdminCredentialService creds) => _creds = creds;

    public void OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            Response.Redirect("/");
    }

    public async Task<IActionResult> OnPostAsync(
        string email, string password, string? returnUrl = null)
    {
        Email = email;

        if (!string.Equals(email, _creds.AdminEmail, StringComparison.OrdinalIgnoreCase)
            || !_creds.VerifyPassword(password))
        {
            Error = "Nesprávný e-mail nebo heslo.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,  _creds.AdminEmail),
            new(ClaimTypes.Email, _creds.AdminEmail),
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

        // Povinná změna hesla při prvním přihlášení
        if (_creds.MustChangePassword)
            return Redirect("/account/change-password");

        var target = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        return Redirect(target);
    }
}
