using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthAssist.Providers
{
    public class LocalProvider(Settings settings, IAuthHandler authHandler) : ILocalProvider
    {
        private readonly Settings _settings = settings;

        private readonly IAuthHandler _authHandler = authHandler;

        public async Task<AuthResult> AuthenticateUser(HttpContext context)
        {
            var authRequest = await JsonSerializer.DeserializeAsync<AuthRequest>(
                context.Request.Body, _settings.JsonSerializerOptions);
            var authResult = await _authHandler.AuthenticateUser(authRequest);
            if (authResult.IsSuccess == false)
                authResult.Error = "user.invalid";
            return authResult;
        }

        public async Task<AuthResult> GetAuthenticatedUser(ClaimsPrincipal user)
        {
            if ((user?.Identity?.IsAuthenticated ?? false) == false)
                return AuthResult.FromError("user.invalid");
            else // Validate user
                return await _authHandler.VerifyUser(user)
                    ? AuthResult.FromUsername(user.Identity.Name)
                    : AuthResult.FromError("user.invalid");
        }

        public async Task Login(HttpContext context, AuthResult authResult)
        {
            if (authResult.IsSuccess)
                await this.LoadUser(context, authResult);
            await context.Response.WriteAsJsonAsync(authResult, _settings.JsonSerializerOptions);
        }

        public async Task LoadUser(HttpContext context, AuthResult authResult)
        {
            context.User = await this.CreatePrincipal(authResult.Username);
            authResult.ExpiresUtc = DateTime.UtcNow.Add(_settings.CookieDuration);
            authResult.Claims = context.User.Claims
                .ToDictionary(claim => claim.Type, claim => claim.Value);
            await context.SignInAsync(context.User);
        }
        public async Task<ClaimsPrincipal> CreatePrincipal(string username)
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, username) };
            await _authHandler.AppendClaims(username, claims);
            return new ClaimsPrincipal(new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme));
        }

        public async Task Logout(HttpContext context)
        {
            await context.SignOutAsync();
            context.Response.StatusCode = 204;
        }
    }
}
