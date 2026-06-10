using Metin2Bausia.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metin2Bausia.Web.Pages.Account;

[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly AdminCredentialService _creds;

    public string? Error   { get; private set; }
    public string? Success { get; private set; }
    public bool    IsMandatory { get; private set; }

    public ChangePasswordModel(AdminCredentialService creds) => _creds = creds;

    public void OnGet()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            Response.Redirect("/account/login");
            return;
        }
        IsMandatory = _creds.MustChangePassword;
    }

    public IActionResult OnPost(
        string? currentPassword,
        string  newPassword,
        string  confirmPassword)
    {
        IsMandatory = _creds.MustChangePassword;

        if (newPassword != confirmPassword)
        {
            Error = "Nová hesla se neshodují.";
            return Page();
        }

        if (newPassword.Length < 6)
        {
            Error = "Heslo musí mít alespoň 6 znaků.";
            return Page();
        }

        if (!IsMandatory)
        {
            // Standardní změna — ověř aktuální heslo
            if (string.IsNullOrWhiteSpace(currentPassword) || !_creds.VerifyPassword(currentPassword))
            {
                Error = "Aktuální heslo je nesprávné.";
                return Page();
            }
        }

        _creds.ChangePassword(newPassword);

        Success = "Heslo bylo změněno. Přesměrovávám…";
        // Krátké zobrazení úspěchu, pak přesměruj na hlavní stránku
        Response.Headers.Append("Refresh", "1;url=/");
        return Page();
    }
}
