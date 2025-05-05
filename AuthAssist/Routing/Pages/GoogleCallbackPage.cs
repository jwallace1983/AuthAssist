using AuthAssist.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class GoogleCallbackPage(IAuthFacade authFacade) : IEndpoint
    {
        public HttpMethod Method => HttpMethod.Get;

        public string Uri => $"{authFacade.Google.Endpoint}/callback";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await authFacade.Google.AuthenticateUser(context);
            await authFacade.Local.Login(context, authResult);
            return true;
        }
    }
}
