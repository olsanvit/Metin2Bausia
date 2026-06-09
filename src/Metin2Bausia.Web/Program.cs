using Metin2Bausia.Web.Services;
using SharedServices.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DB service
builder.Services.AddSingleton<IDbService>(sp =>
    new DbService(builder.Configuration.GetConnectionString("Metin2Bausia")
        ?? throw new InvalidOperationException("Metin2Bausia connection string missing")));

// SharedServices
builder.Services.AddSingleton<ThemeService>(_ => new ThemeService(builder.Configuration));
builder.Services.AddSingleton<ConnectionStateService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Server.Circuits.CircuitHandler, AppCircuitHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Metin2Bausia.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
