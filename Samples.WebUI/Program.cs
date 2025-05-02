using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samples.WebUI.Auth;
using Samples.WebUI.Components;

// Build the app
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<AuthService>();
builder.Services.AddAuthAssist(settings =>
{
    settings.RedirectToLogin = "/login";
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
    settings.UseCookieOptions(options =>
    {
        options.ExpireTimeSpan = System.TimeSpan.FromMinutes(5);
    });
});
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
var app = builder.Build();

// Run the app
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseAuthAssist();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
