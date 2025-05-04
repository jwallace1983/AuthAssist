using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AuthAssist.Providers
{
    public interface IOpenIdProvider
    {
        public abstract string Endpoint { get; }

        bool RedirectToLogin(HttpContext context);

        Task<AuthResult> AuthenticateUser(HttpContext context);
    }
}