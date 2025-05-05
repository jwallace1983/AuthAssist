using AuthAssist.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class LogoutPage(IAuthFacade authFacade) : IEndpoint
    {
        public HttpMethod Method => HttpMethod.Get;

        public string Uri => "logout";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            await authFacade.Local.Logout(context);
            return true;
        }
    }
}
