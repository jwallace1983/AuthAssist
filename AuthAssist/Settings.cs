using Microsoft.AspNetCore.Authorization;
using System;

namespace AuthAssist
{
    public class Settings
    {
        public string Endpoint { get; set; } = "/api/auth";

        public bool RequireHttps { get; set; } = true;

        public void UseAuthHandler<T>() where T : IAuthHandler
            => this.AuthHandlerType = typeof(T);

        internal Type AuthHandlerType { get; private set; } = typeof(DefaultAuthHandler);

        public delegate void ApplyAuthPolicyDelegate(AuthorizationOptions options);

        public void UseAuthPolicies(ApplyAuthPolicyDelegate applyAuthPolicies)
            => this.ApplyAuthPolicies = applyAuthPolicies;

        internal ApplyAuthPolicyDelegate ApplyAuthPolicies { get; private set; } = options => { };
    }
}
