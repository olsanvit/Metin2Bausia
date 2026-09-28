using System.Globalization;
using Metin2Bausia.Web.Services;
using MercenariesAndBeasts.Infrastructure.Localization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SharedServices;
using SharedServices.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Auth — cookie, single admin ──────────────────────────
// Google se registruje jen s opravdovými klíči — se zástupnou hodnotou by tlačítko vedlo na 401
var googleAuth = builder.Configuration.GetSection(GoogleAuthOptions.Section).Get<GoogleAuthOptions>() ?? new();
builder.Services.AddSingleton(googleAuth);

var authBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath    = "/account/login";
        options.LogoutPath   = "/account/logout";
        options.AccessDeniedPath = "/account/login";
        options.ExpireTimeSpan   = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "mt2bausia_admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

if (googleAuth.IsConfigured)
{
    authBuilder.AddGoogle(o =>
    {
        o.ClientId     = googleAuth.ClientId!;
        o.ClientSecret = googleAuth.ClientSecret!;
        // Přihlášení končí ve stejné admin cookie jako přihlášení heslem — žádný druhý svět uživatelů
        o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.Events.OnCreatingTicket = ctx =>
        {
            var email = ctx.Identity?.FindFirst(ClaimTypes.Email)?.Value;
            var admin = ctx.HttpContext.RequestServices.GetRequiredService<AdminCredentialService>().AdminEmail;
            if (!googleAuth.IsAllowed(email, admin))
                throw new AuthenticationFailureException($"Účet {email} nemá do adminu přístup.");

            // Role musí přidat i tahle cesta, jinak by Google admin neprošel [Authorize(Roles = "Admin")]
            ctx.Identity!.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            return Task.CompletedTask;
        };
        o.Events.OnRemoteFailure = ctx =>
        {
            ctx.Response.Redirect("/account/login?error=google");
            ctx.HandleResponse();
            return Task.CompletedTask;
        };
    });
}

builder.Services.AddAuthorization();
// /health pinkne DB; connection string se tu jmenuje Metin2Bausia, ne výchozí DefaultConnection
builder.Services.AddSharedHealthChecks("Metin2Bausia");
builder.Services.AddRazorPages();   // pro Login Razor Page (POST + HttpContext)

// Admin přihlašovací údaje + MustChangePassword flag
builder.Services.AddSingleton<AdminCredentialService>();

// ── Blazor ───────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── DB ───────────────────────────────────────────────────
builder.Services.AddSingleton<IDbService>(sp =>
    new DbService(builder.Configuration.GetConnectionString("Metin2Bausia")
        ?? throw new InvalidOperationException("Metin2Bausia connection string missing")));

// ── SharedServices ────────────────────────────────────────
builder.Services.AddSingleton<ThemeService>(_ => new ThemeService(builder.Configuration));
builder.Services.AddSingleton<ConnectionStateService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Server.Circuits.CircuitHandler, AppCircuitHandler>();
builder.Services.AddScoped<SharedServices.Services.UiLibraryService>();
builder.Services.AddGlobalErrorNotifications();
builder.Services.AddSimpleLocalization();
// SharedServices podporuje jen cs/en a sdílí ho víc projektů; LangSwitcher nabízí i DE,
// proto se seznam jazyků rozšiřuje až tady (Configure běží po tom ze SharedServices)
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new[] { "cs", "en", "de" }.Select(CultureInfo.GetCultureInfo).ToList();
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// MapStaticAssets servíruje framework assety (_framework/blazor.web.js) — bez něj vrací 404 a circuit se nenaváže
app.MapStaticAssets();
app.UseStaticFiles();   // kvůli vlastním souborům ve wwwroot (app.css)
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapHealthChecks("/health");
app.MapMabCultureEndpoint();
app.MapRazorPages();   // Login / Logout Razor Pages

app.MapRazorComponents<Metin2Bausia.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
