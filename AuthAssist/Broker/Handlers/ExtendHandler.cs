using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class ExtendHandler(Settings settings, IAuthHandler authHandler) : IRequestHandler
    {
        public HttpMethod Method { get; } = HttpMethod.Get;

        public string Endpoint { get; } = $"{settings.Prefix}/extend";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await this.GetAuthResult(context.User);
            if (authResult.IsSuccess)
                await this.ExtendSession(context, authResult);
            await context.Response.WriteAsJsonAsync(authResult, settings.JsonSerializerOptions);
            return true;
        }

        public async Task<AuthResult> GetAuthResult(ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated == false)
                return AuthResult.FromError("user.invalid");
            return await authHandler.VerifyUser(user)
                ? AuthResult.FromUsername(user.Identity.Name)
                : AuthResult.FromError("user.invalid");
        }

        public async Task ExtendSession(HttpContext context, AuthResult authResult)
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, authResult.Username) };
            await authHandler.AppendClaims(authResult.Username, claims);
            authResult.ExpiresUtc = DateTime.UtcNow.Add(settings.CookieDuration);
            authResult.Claims = claims.ToDictionary(claim => claim.Type, claim => claim.Value);
            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await context.SignInAsync(context.User);
        }

    }
}
