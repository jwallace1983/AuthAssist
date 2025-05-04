using AuthAssist;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Samples.WebApiDemo.Auth
{
    public class AuthHandler : IAuthHandler
    {
        private static readonly string[] _validUsers = [ "user", "admin" ];

        public Task AppendClaims(AuthResult authResult)
        {
            const string _adminUser = "admin";
            if (_adminUser.Equals(authResult.Username, StringComparison.OrdinalIgnoreCase))
                authResult.Claims[Policies.ADMIN] = string.Empty;
            return Task.CompletedTask;
        }

        public Task<bool> VerifyUser(ClaimsPrincipal user) => Task.FromResult(true);

        public Task<AuthResult> AuthenticateUser(AuthRequest request)
        {
            return _validUsers.Contains(request.Username, StringComparer.OrdinalIgnoreCase)
                ? Task.FromResult(AuthResult.FromUsername(request.Username))
                : Task.FromResult(AuthResult.FromError("auth.failed"));
        }
    }
}
