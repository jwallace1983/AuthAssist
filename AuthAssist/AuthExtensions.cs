using AuthAssist;
using AuthAssist.Routing;
using AuthAssist.Routing.Pages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using System;
using AuthAssist.Services.Google;
using AuthAssist.Services.Local;
using AuthAssist.Services.Microsoft;
using AuthAssist.Services;

// Exception: Namespace does not match folder structure
#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthAssist<TAuthHandlerType>(
            this IServiceCollection services,
            Action<Settings> applySettings = null)
            where TAuthHandlerType : IAuthHandler
        {
            var settings = new Settings();
            applySettings?.Invoke(settings);
            services
                // User-provided auth handler
                .AddTransient(typeof(IAuthHandler), typeof(TAuthHandlerType))

                // Routing
                .AddSingleton<IRouterService, RouterService>()
                .AddTransient<IEndpoint, LoginPage>()
                .AddTransient<IEndpoint, LogoutPage>()
                .AddTransient<IEndpoint, ExtendPage>()
                .AddTransient<IEndpoint, GooglePage>()
                .AddTransient<IEndpoint, GoogleCallbackPage>()
                .AddTransient<IEndpoint, MicrosoftPage>()
                .AddTransient<IEndpoint, MicrosoftCallbackPage>()

                // Services
                .AddTransient<ILocalAuthService, LocalAuthService>()
                .AddTransient<IGoogleAuthService, GoogleAuthService>()
                .AddTransient<IMicrosoftAuthService, MicrosofAuthServive>()
                .AddTransient<IAuthFacade, AuthFacade>()

                // Configuration
                .AddSingleton(settings);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = false;
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        if (settings.RedirectToAccessDenied != null)
                            context.Response.Redirect(settings.RedirectToAccessDenied);
                        else
                            context.Response.StatusCode = 403;
                        return System.Threading.Tasks.Task.CompletedTask;
                    };
                    options.Events.OnRedirectToLogin = context =>
                    {
                        if (settings.RedirectToLogin != null)
                            context.Response.Redirect(settings.RedirectToLogin);
                        else
                            context.Response.StatusCode = 401;
                        return System.Threading.Tasks.Task.CompletedTask;
                    };
                    settings.ApplyCookieOptions?.Invoke(options);
                    settings.CookieDuration = options.ExpireTimeSpan;
                    settings.CookieSlidingExpiration = options.SlidingExpiration;
                });
            services.AddAuthorization(options => settings.ApplyAuthPolicies(options));
            return services;
        }

        public static IApplicationBuilder UseAuthAssist(this IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetService<Settings>();
            if (settings?.CookiePolicyOptions != null)
                app.UseCookiePolicy(settings.CookiePolicyOptions);
            app.UseAuthentication();
            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                var broker = app.ApplicationServices.GetService<IRouterService>();
                await broker.Process(context, next);
            });
            return app;
        }
    }
}
