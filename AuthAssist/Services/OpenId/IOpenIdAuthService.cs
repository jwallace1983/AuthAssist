using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AuthAssist.Services.OpenId
{
    public interface IOpenIdAuthService
    {
        public abstract string Endpoint { get; }

        bool RedirectToLogin(HttpContext context);

        Task<AuthResult> AuthenticateUser(HttpContext context);
    }
}