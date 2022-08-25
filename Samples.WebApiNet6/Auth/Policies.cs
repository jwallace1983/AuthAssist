using Microsoft.AspNetCore.Authorization;

namespace Samples.WebApiNet6.Auth
{
    public static class Policies
    {
        public const string ADMIN = "admin";

        public static void ApplyPolicies(AuthorizationOptions options)
        {
            options.AddPolicy(ADMIN, builder => builder.RequireClaim(ADMIN));
        }
    }
}
