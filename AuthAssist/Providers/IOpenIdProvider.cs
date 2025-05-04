using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AuthAssist.Providers
{
    public interface IOpenIdProvider
    {
        Task<AuthResult> AuthenticateUser(HttpContext context);

        bool RedirectToLogin(HttpContext context, string endpoint);
    }
}