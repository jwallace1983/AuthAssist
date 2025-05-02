using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class ExtendHandler(Settings settings, IServiceProvider services) : IRequestHandler
    {
        private readonly IServiceProvider _services = services;
        private readonly string _endpoint = $"{settings.Endpoint}/extend";
        private readonly Settings _settings = settings;
        private static readonly string[] _methods = [ HttpMethods.Get, HttpMethods.Post ];

        public Task<bool> CanHandle(HttpContext context)
        {
            return Task.FromResult(_methods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase)
                && _endpoint.Equals(context.Request.Path.Value, StringComparison.OrdinalIgnoreCase));
        }

        public async Task ProcessRequest(HttpContext context)
        {
            var authHandler = _services.GetService<IAuthHandler>();
            if (authHandler == null)
                throw new ApplicationException("auth.config.error");
            var authResult = await GetAuthResult(authHandler, context.User);
            if (authResult.IsSuccess)
            {
                var claims = new List<Claim> { new(ClaimTypes.Name, authResult.Username) };
                await authHandler.AppendClaims(authResult.Username, claims);
                context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                authResult.ExpiresUtc = DateTime.UtcNow.Add(_settings.CookieDuration);
                await context.SignInAsync(context.User);
                authResult.Claims = claims.ToDictionary(claim => claim.Type, claim => claim.Value);
            }
            await context.Response.WriteAsJsonAsync(authResult, _settings.JsonSerializerOptions);
        }

        public static async Task<AuthResult> GetAuthResult(IAuthHandler authHandler, ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated == false)
                return new AuthResult { Error = "user.invalid" };
            return await authHandler.VerifyUser(user)
                ? new AuthResult { Username = user.Identity.Name }
                : new AuthResult { Error = "user.invalid" };
        }
    }
}
