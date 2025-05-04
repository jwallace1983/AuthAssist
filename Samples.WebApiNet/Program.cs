using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Samples.WebApiDemo.Auth;

// Build the app
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthAssist<AuthHandler>(settings =>
{
    settings.UseAuthPolicies(Policies.ApplyPolicies);
    settings.UseCookieOptions(cookieOptions =>
    {
        cookieOptions.ExpireTimeSpan = System.TimeSpan.FromMinutes(5);
    });
    settings.GoogleIdp.ClientId = builder.Configuration["SocialAuth:Google:ClientId"];
    settings.GoogleIdp.ClientSecret = builder.Configuration["SocialAuth:Google:ClientSecret"];
});
builder.Services.AddControllers();
var app = builder.Build();

// Run the app
app.UseAuthAssist();
app.MapControllers();
app.Run();
