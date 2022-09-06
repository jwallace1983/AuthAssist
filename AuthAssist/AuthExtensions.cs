using AuthAssist;
using AuthAssist.Broker;
using AuthAssist.Broker.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthAssist(this IServiceCollection services, Action<Settings> applySettings = null)
        {
            var settings = new Settings();
            applySettings?.Invoke(settings);
            services
                .AddSingleton<IBrokerService, BrokerService>()
                .AddTransient<IRequestHandler, LoginHandler>()
                .AddTransient<IRequestHandler, LogoutHandler>()
                .AddTransient<IRequestHandler, ExtendHandler>()
                .AddTransient(typeof(IAuthHandler), settings.AuthHandlerType)
                .AddSingleton(settings);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = false;
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return System.Threading.Tasks.Task.CompletedTask;
                    };
                    options.Events.OnRedirectToLogin = context =>
                    {
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
                var broker = app.ApplicationServices.GetService<IBrokerService>();
                await broker.Process(context, next);
            });
            return app;
        }
    }
}
