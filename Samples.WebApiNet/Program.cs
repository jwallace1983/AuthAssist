using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Samples.WebApiDemo.Auth;

// Build the app
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthAssist(settings =>
{
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
    settings.UseCookieOptions(options =>
    {
        options.ExpireTimeSpan = System.TimeSpan.FromMinutes(5);
    });
});
builder.Services.AddControllers();
var app = builder.Build();

// Run the app
app.UseAuthAssist();
app.MapControllers();
app.Run();
