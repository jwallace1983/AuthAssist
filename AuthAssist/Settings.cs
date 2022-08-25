using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using System;

namespace AuthAssist
{
    public class Settings
    {
        public string Endpoint { get; set; } = "/api/auth";

        public bool RequireHttps { get; set; } = true;

        internal TimeSpan CookieDuration { get; set; }

        internal bool CookieSlidingExpiration { get; set; }

        // Configure auth handler
        public void UseAuthHandler<T>() where T : IAuthHandler
            => this.AuthHandlerType = typeof(T);
        internal Type AuthHandlerType { get; private set; } = typeof(DefaultAuthHandler);

        // Configure auth policies
        public delegate void ApplyAuthPolicyDelegate(AuthorizationOptions options);
        public void UseAuthPolicies(ApplyAuthPolicyDelegate applyAuthPolicies)
            => this.ApplyAuthPolicies = applyAuthPolicies;
        internal ApplyAuthPolicyDelegate ApplyAuthPolicies { get; private set; } = options => { };

        // Configure cookie options
        public delegate void ApplyCookieOptionsDelegate(CookieAuthenticationOptions options);
        public void UseCookieOptions(ApplyCookieOptionsDelegate applyCookieOptions)
            => this.ApplyCookieOptions = applyCookieOptions;
        internal ApplyCookieOptionsDelegate ApplyCookieOptions { get; private set; } = options => { };

        // Configure cookie policy
        public CookiePolicyOptions CookiePolicyOptions { get; set; } = new CookiePolicyOptions
        {
            HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
        };
    }
}
