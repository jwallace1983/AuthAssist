using AuthAssist;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Samples.WebUI.Auth
{
    public class AuthHandler : IAuthHandler
    {
        private static readonly string[] _validUsers = [ "user", "admin" ];

        public Task AppendClaims(string username, List<Claim> claims)
        {
            const string _adminUser = "admin";
            if (_adminUser.Equals(username, StringComparison.OrdinalIgnoreCase))
                claims.Add(new Claim(Policies.ADMIN, string.Empty));
            return Task.CompletedTask;
        }

        public Task<bool> VerifyUser(ClaimsPrincipal user) => Task.FromResult(true);

        public Task<AuthResult> AuthenticateUser(AuthRequest request)
        {
            return _validUsers.Contains(request.Username, StringComparer.OrdinalIgnoreCase)
                ? Task.FromResult(new AuthResult { Username = request.Username })
                : Task.FromResult(new AuthResult { Error = "auth.failed" });
        }
    }
}
