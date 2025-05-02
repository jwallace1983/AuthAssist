using AuthAssist.Broker;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthAssist
{
    public class Settings
    {
        public string Endpoint { get; set; } = "/api/auth";

        public string RedirectToLogin { get; set; } = null;

        public string RedirectToAccessDenied { get; set; } = null;

        public bool RequireHttps { get; set; } = true;

        internal TimeSpan CookieDuration { get; set; }

        internal bool CookieSlidingExpiration { get; set; }

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Configure auth handler
        public void UseAuthHandler<T>() where T : IAuthHandler
            => this.AuthHandlerType = typeof(T);
        internal Type AuthHandlerType { get; private set; } = typeof(DefaultAuthHandler);

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

        public void UseNotFoundHandler(Func<HttpContext, Task> handler)
            => this.HandleNotFound = handler;
        internal Func<HttpContext, Task> HandleNotFound { get; private set; } = BrokerService.ShowNotFound;

        public void UseErrorHandler(Func<HttpContext, Exception, Task> handler)
            => this.HandleError = handler;
        internal Func<HttpContext, Exception, Task> HandleError { get; private set; } = BrokerService.ShowError;
    }
}
