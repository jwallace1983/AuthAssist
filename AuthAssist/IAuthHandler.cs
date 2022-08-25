using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAssist
{
    public interface IAuthHandler
    {
        Task<AuthResult> AuthenticateUser(AuthRequest request);

        Task AppendClaims(string user, List<Claim> claims);
    }
}
