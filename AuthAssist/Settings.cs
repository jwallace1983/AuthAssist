using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using AuthAssist.Services.Google;
using AuthAssist.Services.Microsoft;

namespace AuthAssist
{
    public class Settings
    {
        internal const string COOKIE_RETURN_URL = "return_url";

        internal const string COOKIE_NONCE = "nonce";

        public GoogleSettings GoogleIdp { get; private set; } = new();

        public MicrosoftSettings MicrosoftIdp { get; private set; } = new();

        public string Prefix { get; set; } = "/api/auth";

        public string RedirectToLogin { get; set; } = null;

        public string RedirectToAccessDenied { get; set; } = null;

        public string RedirectToLoginError { get; set; } = null;

        public string DefaultReturnUrl { get; set; } = "/";

        public bool RequireHttps { get; set; } = true;

        internal TimeSpan CookieDuration { get; set; }

        internal bool CookieSlidingExpiration { get; set; }

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        // Configure auth policies
        public void UseAuthPolicies(Action<AuthorizationOptions> applyAuthPolicies)
            => this.ApplyAuthPolicies = applyAuthPolicies;
        internal Action<AuthorizationOptions> ApplyAuthPolicies { get; private set; } = options => { };

        // Configure cookie options
        public void UseCookieOptions(Action<CookieAuthenticationOptions> applyCookieOptions)
            => this.ApplyCookieOptions = applyCookieOptions;
        internal Action<CookieAuthenticationOptions> ApplyCookieOptions { get; private set; } = options => { };

        // Configure cookie policy
        public CookiePolicyOptions CookiePolicyOptions { get; set; } = new CookiePolicyOptions
        {
            HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
        };


    }
}
