using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAssist.Providers
{
    public interface ILocalProvider
    {
        Task<AuthResult> AuthenticateUser(HttpContext context);
        
        Task<AuthResult> GetAuthenticatedUser(ClaimsPrincipal user);
        
        Task Login(HttpContext context, AuthResult authResult);
        Task Logout(HttpContext context);
    }
}