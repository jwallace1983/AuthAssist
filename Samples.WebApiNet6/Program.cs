using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Samples.WebApiNet6.Auth;

// Build the app
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthAssist(settings =>
{
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
});
builder.Services.AddControllers();
var app = builder.Build();

// Run the app
app.UseAuthAssist();
app.MapControllers();
app.Run();
