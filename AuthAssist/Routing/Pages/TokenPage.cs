using AuthAssist.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class TokenPage(IAuthFacade authFacade) : IEndpoint
    {
        public HttpMethod Method => HttpMethod.Get;

        public string Uri => "token";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            if (authFacade.Settings.EnableTokenEndpoint == false)
                return false; // Token not enabled
            await context.Response.WriteAsJsonAsync(AuthResult.FromPrincipal(context.User));
            return true; // Current token
        }
    }
}
