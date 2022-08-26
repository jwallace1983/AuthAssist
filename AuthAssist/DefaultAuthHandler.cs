using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAssist
{
    public class DefaultAuthHandler : IAuthHandler
    {
        public virtual Task<AuthResult> AuthenticateUser(AuthRequest request)
            => Task.FromResult(new AuthResult() { Error = "not.implemented" });

        public virtual Task AppendClaims(string user, List<Claim> claims) => Task.CompletedTask;

        public virtual Task<bool> VerifyUser(ClaimsPrincipal user) => Task.FromResult(false);
    }
}
