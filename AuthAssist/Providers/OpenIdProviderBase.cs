using Microsoft.AspNetCore.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using AuthAssist.Providers.Models;

namespace AuthAssist.Providers
{
    public abstract class OpenIdProviderBase
    {
        private static readonly HttpClient _httpClient = new();

        public abstract string ClientId { get; }

        public abstract string ClientSecret { get; }

        public abstract string AuthUrl { get; }

        public abstract GoogleSettings ClientSettings { get; }

        public abstract Settings Settings { get; }

        public abstract IAuthHandler AuthHandler { get; }

        public async Task<AuthResult> AuthenticateUser(HttpContext context)
        {
            var authRequest = await JsonSerializer.DeserializeAsync<AuthRequest>(
                context.Request.Body, this.Settings.JsonSerializerOptions);
            var authResult = await this.AuthHandler.AuthenticateUser(authRequest);
            if (authResult.IsSuccess == false)
                authResult.Error = "user.invalid";
            return authResult;
        }

        public bool RedirectToLogin(HttpContext context, string endpoint)
        {
            if (string.IsNullOrEmpty(this.ClientId))
                return false; // Not configured
            var callbackUrl = GetCallbackUrl(context, endpoint);
            var url = new StringBuilder(this.AuthUrl)
                .Append("?include_granted_scopes=true")
                .Append("&response_type=code")
                .Append("&scope=profile")
                .Append("&state=" + Guid.NewGuid().ToString())
                .Append("&redirect_uri=" + Uri.EscapeDataString(callbackUrl))
                .Append("&client_id=" + this.ClientId)
                .ToString();
            context.Response.Redirect(url, false);
            return true;
        }

        public static string GetCallbackUrl(HttpContext context, string endpoint)
            => new StringBuilder("http")
            .Append(context.Request.IsHttps ? "s" : string.Empty)
            .Append("://")
            .Append(context.Request.Host)
            .Append(endpoint)
            .Append("/callback")
            .ToString();

    }
}
