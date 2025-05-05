using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthAssist.Services.Local
{
    public class LocalAuthService(Settings settings, IAuthHandler authHandler) : ILocalAuthService
    {
        private readonly Settings _settings = settings;

        private readonly IAuthHandler _authHandler = authHandler;

        public async Task<AuthResult> AuthenticateUser(HttpContext context)
        {
            var authRequest = await JsonSerializer.DeserializeAsync<AuthRequest>(
                context.Request.Body, _settings.JsonSerializerOptions);
            var authResult = await _authHandler.AuthenticateUser(authRequest);
            if (authResult.IsSuccess)
                authResult.AuthType = AuthTypes.Local;
            else
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
                await LoadUser(context, authResult);
            if (authResult.AuthType == AuthTypes.Local)
                await context.Response.WriteAsJsonAsync(authResult, _settings.JsonSerializerOptions);
            else // Social logins must redirect
                context.Response.Redirect(this.GetRedirectUrl(context, authResult), false);
        }

        public async Task LoadUser(HttpContext context, AuthResult authResult)
        {
            context.User = await CreatePrincipal(authResult);
            authResult.ExpiresUtc = DateTime.UtcNow.Add(_settings.CookieDuration);
            await context.SignInAsync(context.User);
        }

        public async Task<ClaimsPrincipal> CreatePrincipal(AuthResult authResult)
        {
            await _authHandler.AppendClaims(authResult);
            var claims = authResult.Claims.Select(c => new Claim(c.Key, c.Value));
            return new ClaimsPrincipal(new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme));
        }

        public string GetRedirectUrl(HttpContext context, AuthResult authResult)
        {
            if (authResult.IsSuccess)
                return context.Request.Cookies.TryGetValue(Settings.COOKIE_RETURN_URL, out var returnUrl)
                    ? returnUrl : (_settings.DefaultReturnUrl ?? "/");
            else
                return _settings.RedirectToLoginError;
        }

        public async Task Logout(HttpContext context)
        {
            await context.SignOutAsync();
            context.Response.StatusCode = 204;
        }
    }
}
