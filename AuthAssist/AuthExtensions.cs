using AuthAssist;
using AuthAssist.Broker;
using AuthAssist.Broker.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using System;
using System.Security.Claims;

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
                .AddTransient(typeof(IAuthHandler), settings.AuthHandlerType)
                .AddSingleton(settings);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
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
                });
            services.AddAuthorization(options => settings.ApplyAuthPolicies(options));
            return services;
        }

        public static IApplicationBuilder UseAuthAssist(this IApplicationBuilder app)
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
            });
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
