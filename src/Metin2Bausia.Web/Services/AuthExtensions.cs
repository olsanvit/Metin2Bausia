using Microsoft.AspNetCore.Components.Authorization;

namespace Metin2Bausia.Web.Services;

/// <summary>
/// Pomůcka pro audit — dřív se do ApprovalLog i ApprovedBy psalo natvrdo 'admin',
/// takže z historie nešlo poznat, kdo změnu provedl.
/// </summary>
public static class AuthExtensions
{
    /// <summary>Jméno přihlášeného operátora; fallback 'admin' pro případ, že by kaskáda chyběla.</summary>
    public static async Task<string> OperatorNameAsync(this Task<AuthenticationState>? authState)
    {
        if (authState is null) return "admin";
        var state = await authState;
        var name = state.User.Identity?.Name;
        return string.IsNullOrWhiteSpace(name) ? "admin" : name;
    }

    public static async Task<string?> EmailAsync(this Task<AuthenticationState>? authState) =>
        authState is null ? null
            : (await authState).User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
}
