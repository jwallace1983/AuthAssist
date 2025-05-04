using Microsoft.AspNetCore.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace AuthAssist.Providers
{
    public abstract class OpenIdProviderBase : IOpenIdProvider
    {
        private static readonly HttpClient _httpClient = new();

        public abstract string ClientId { get; }

        public abstract string ClientSecret { get; }

        public abstract string AuthUrl { get; }

        public abstract Settings Settings { get; }

        public abstract IAuthHandler AuthHandler { get; }

        public async Task<AuthResult> AuthenticateUser(HttpContext context)
        {
            await Task.CompletedTask; // TODO: REMOVE
            if (context.Request.Query.TryGetValue("code", out var code) == false)
                return AuthResult.FromError("auth.code.invalid");
            else if (context.Request.Query.TryGetValue("state", out var state) == false)
                return AuthResult.FromError("auth.state.invalid");
            return AuthResult.FromUsername("temp");
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

        public string GetCallbackUrl(HttpContext context, string endpoint)
            => new StringBuilder()
            .Append($"http{(context.Request.IsHttps ? "s" : string.Empty)}://{context.Request.Host}")
            .Append($"{this.Settings.Prefix}/{endpoint}/callback")
            .ToString();

    }
}
