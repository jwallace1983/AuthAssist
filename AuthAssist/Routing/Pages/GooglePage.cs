using AuthAssist.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class GooglePage(IAuthFacade authFacade) : IEndpoint
    {
        public HttpMethod Method => HttpMethod.Get;

        public string Uri => authFacade.Google.Endpoint;

        public Task<bool> ProcessRequest(HttpContext context)
            => Task.FromResult(authFacade.Google.RedirectToLogin(context));
    }
}
