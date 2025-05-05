using Microsoft.AspNetCore.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Security.Claims;

namespace AuthAssist.Services.OpenId
{
    public abstract class OpenIdAuthServiceBase : IOpenIdAuthService
    {
        private static readonly HttpClient _httpClient = new();

        public abstract string Endpoint { get; }

        public abstract string ClientId { get; }

        public abstract string ClientSecret { get; }

        public abstract string AuthUrl { get; }

        public abstract string TokenUrl { get; }

        public abstract string UserInfoUrl { get; }

        public abstract Settings Settings { get; }

        public abstract IAuthHandler AuthHandler { get; }

        public abstract AuthTypes AuthType { get; }

        public bool RedirectToLogin(HttpContext context)
        {
            if (string.IsNullOrEmpty(ClientId))
                return false; // Not configured
            context.Response.Cookies.Append(Settings.COOKIE_RETURN_URL,
                context.Request.Query.TryGetValue("returnUrl", out var returnUrl) ? returnUrl : "/");
            var nonce = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(Settings.COOKIE_NONCE, nonce);
            var callbackUrl = GetCallbackUrl(context);
            var url = new StringBuilder(AuthUrl)
                .Append("?include_granted_scopes=true")
                .Append("&response_type=code")
                .Append("&scope=" + Uri.EscapeDataString("openid profile email"))
                .Append("&state=" + nonce)
                .Append("&redirect_uri=" + Uri.EscapeDataString(callbackUrl))
                .Append("&client_id=" + ClientId)
                .ToString();
            context.Response.Redirect(url, false);
            return true;
        }

        public string GetCallbackUrl(HttpContext context)
            => new StringBuilder()
            .Append($"http{(context.Request.IsHttps ? "s" : string.Empty)}://{context.Request.Host}")
            .Append($"{Settings.Prefix}/{Endpoint}/callback")
            .ToString();

        public async Task<AuthResult> AuthenticateUser(HttpContext context)
        {
            // Validate the provided code and state
            if (context.Request.Query.TryGetValue("code", out var code) == false)
                return AuthResult.FromError("auth.code.invalid");
            else if (context.Request.Query.TryGetValue("state", out var state) == false
                || context.Request.Cookies.TryGetValue(Settings.COOKIE_NONCE, out var nonce) == false)
                return AuthResult.FromError("auth.nonce.invalid");
            else if (nonce.Equals(state.ToString()) == false)
                return AuthResult.FromError("auth.nonce.mismatch");

            // Exchange code for id/acccess tokens
            var token = await GetToken(code, GetCallbackUrl(context));
            if (string.IsNullOrEmpty(token.IdToken))
                return AuthResult.FromError("auth.token.invalid");

            // Get user info
            var userInfo = await GetUserInfo(token.AccessToken);
            if (userInfo == null)
                return AuthResult.FromError("auth.userinfo.invalid");

            // Add claim for authentication source
            var result = AuthResult.FromUserInfo(userInfo);
            result.AuthType = this.AuthType;
            return result;
        }

        public async Task<TokenResponse> GetToken(string code, string callbackUrl)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl);
            request.Headers.Add("Accept", "application/json");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "code", code },
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "redirect_uri", callbackUrl },
                { "grant_type", "authorization_code" }
            });
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
                return null; // No response
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }

        public async Task<UserInfoResponse> GetUserInfo(string token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, UserInfoUrl);
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
                return null; // No response
            return await response.Content.ReadFromJsonAsync<UserInfoResponse>();
        }

    }
}
