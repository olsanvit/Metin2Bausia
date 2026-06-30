using Metin2Bausia.Web.Services;
using MercenariesAndBeasts.Infrastructure.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;
using SharedServices;
using SharedServices.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Auth — cookie, single admin ──────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization();
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
builder.Services.AddGlobalErrorNotifications();
builder.Services.AddSimpleLocalization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapMabCultureEndpoint();
app.MapRazorPages();   // Login / Logout Razor Pages

app.MapRazorComponents<Metin2Bausia.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
