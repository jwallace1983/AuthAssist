using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAssist.Services.Local
{
    public interface ILocalAuthService
    {
        Task<AuthResult> AuthenticateUser(HttpContext context);
        
        Task<AuthResult> GetAuthenticatedUser(ClaimsPrincipal user);
        
        Task Login(HttpContext context, AuthResult authResult);

        Task Logout(HttpContext context);
    }
}