using Microsoft.AspNetCore.Authorization;

namespace Samples.WebUI.Auth
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
