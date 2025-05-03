using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class LoginHandler(Settings settings, IAuthHandler authHandler) : IRequestHandler
    {
        public HttpMethod Method { get; } = HttpMethod.Post;

        public string Endpoint { get; } = $"{settings.Prefix}/login";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authRequest = await JsonSerializer.DeserializeAsync<AuthRequest>(
                context.Request.Body, settings.JsonSerializerOptions);
            var authResult = await authHandler.AuthenticateUser(authRequest);
            if (authResult.IsSuccess)
                await this.SignInUser(context, authResult);
            else
                authResult.Error = "user.invalid";
            await context.Response.WriteAsJsonAsync(authResult, settings.JsonSerializerOptions);
            return true;
        }

        public async Task SignInUser(HttpContext context, AuthResult authResult)
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, authResult.Username) };
            await authHandler.AppendClaims(authResult.Username, claims);
            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            authResult.ExpiresUtc = DateTime.UtcNow.Add(settings.CookieDuration);
            await context.SignInAsync(context.User);
            authResult.Claims = claims.ToDictionary(claim => claim.Type, claim => claim.Value);
        }
    }
}
