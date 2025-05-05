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
    settings.Google.ClientId = builder.Configuration["SocialAuth:Google:ClientId"];
    settings.Google.ClientSecret = builder.Configuration["SocialAuth:Google:ClientSecret"];
    settings.Microsoft.ClientId = builder.Configuration["SocialAuth:Microsoft:ClientId"];
    settings.Microsoft.ClientSecret = builder.Configuration["SocialAuth:Microsoft:ClientSecret"];
    settings.EnableTokenEndpoint = true;
});
builder.Services.AddControllers();
var app = builder.Build();

// Run the app
app.UseAuthAssist();
app.MapControllers();
app.Run();
