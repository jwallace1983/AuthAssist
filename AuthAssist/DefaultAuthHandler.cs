using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAssist
{
    public class DefaultAuthHandler : IAuthHandler
    {
        public virtual Task<AuthResult> AuthenticateUser(AuthRequest request)
            => Task.FromResult(new AuthResult() { IsSuccess = false });

        public Task AppendClaims(string user, List<Claim> claims) => Task.CompletedTask;
    }
}
