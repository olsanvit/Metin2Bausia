using Microsoft.Extensions.DependencyInjection.Extensions;
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
        o.Events.OnCreatingTicket = async ctx =>
        {
            var email = ctx.Identity?.FindFirst(ClaimTypes.Email)?.Value;
            // Správce spravuje admin na /admin/access (tabulka AdminUsers), ne appsettings
            var admins = ctx.HttpContext.RequestServices.GetRequiredService<AdminUserService>();
            if (!await admins.IsAllowedAsync(email))
                throw new AuthenticationFailureException($"Účet {email} nemá do adminu přístup.");

            // Role musí přidat i tahle cesta, jinak by Google admin neprošel [Authorize(Roles = "Admin")]
            ctx.Identity!.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
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
builder.Services.AddSingleton<AdminUserService>();

// ── Blazor ───────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── DB ───────────────────────────────────────────────────
builder.Services.AddSingleton<IDbService>(sp =>
    new DbService(builder.Configuration.GetConnectionString("Metin2Bausia")
        ?? throw new InvalidOperationException("Metin2Bausia connection string missing")));
builder.Services.AddSingleton<KnowledgeLinkService>();
// Scoped kvůli lokalizaci — popisky menu se skládají v jazyce aktuálního požadavku
builder.Services.AddScoped<Metin2Navigation>();

// ── SharedServices ────────────────────────────────────────
builder.Services.AddSingleton<ThemeService>(_ => new ThemeService(builder.Configuration));
builder.Services.AddSingleton<ConnectionStateService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Server.Circuits.CircuitHandler, AppCircuitHandler>();
builder.Services.AddScoped<SharedServices.Services.UiLibraryService>();
builder.Services.AddGlobalErrorNotifications();
// Závislosti diagnostických stránek /sys/* (Components/Pages/Sys/SysPage.razor)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SvgScannerService>();
builder.Services.AddScoped<DebugSnapshotService>();
builder.Services.TryAddSingleton<Microsoft.AspNetCore.Authentication.Cookies.ITicketStore, SharedServices.Services.Entra.MemoryTicketStore>();
builder.Services.TryAddSingleton<SharedServices.Services.Entra.IUserProfileStore, SharedServices.Services.Entra.MemoryUserProfileStore>();
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

// Fail fast: bez uloženého hesla i bez Admin:Password by admin vznikl až při prvním
// přihlášení a chyba by se ukázala uživateli. Takhle kontejner rovnou nenaběhne s jasnou hláškou.
_ = app.Services.GetRequiredService<AdminCredentialService>().MustChangePassword;
await app.Services.GetRequiredService<AdminUserService>().SeedAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Neznámá adresa → 404 se stránkou aplikace místo prázdné odpovědi
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
// MapStaticAssets servíruje framework assety (_framework/blazor.web.js) — bez něj vrací 404 a circuit se nenaváže
app.MapStaticAssets();
app.UseStaticFiles();   // kvůli vlastním souborům ve wwwroot (app.css)
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

// Obvyklé adresy účtu (ze SharedServices a starých odkazů) vedou na vlastní přihlášení — jinak 404
var accountRedirects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["/login"] = "/account/login", ["/register"] = "/account/login", ["/forgot-password"] = "/account/login",
    ["/reset-password"] = "/account/login", ["/access-pending"] = "/account/login", ["/logout"] = "/account/logout",
    ["/change-password"] = "/account/change-password",
};
app.Use(async (ctx, next) =>
{
    var path = ctx.Request.Path.Value?.TrimEnd('/') ?? "";
    if (accountRedirects.TryGetValue(path, out var target))
    {
        ctx.Response.Redirect(target + ctx.Request.QueryString);
        return;
    }
    // /sys/* jen pro admina: anonym jde na přihlášení i u neexistující podstránky, aby adresa nic neprozradila
    if (path.StartsWith("/sys", StringComparison.OrdinalIgnoreCase) && (path.Length == 4 || path[4] == '/')
        && ctx.User.Identity?.IsAuthenticated != true)
    {
        await ctx.ChallengeAsync();
        return;
    }
    await next();
});
app.UseAntiforgery();

app.MapHealthChecks("/health");
app.MapMabCultureEndpoint();
app.MapRazorPages();   // Login / Logout Razor Pages

app.MapRazorComponents<Metin2Bausia.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
