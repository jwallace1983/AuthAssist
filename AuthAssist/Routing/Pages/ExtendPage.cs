using AuthAssist.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class ExtendPage(IAuthFacade authFacade) : IEndpoint
    {
        public HttpMethod Method => HttpMethod.Get;

        public string Uri => "extend";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await authFacade.Local.GetAuthenticatedUser(context.User);
            await authFacade.Local.Login(context, authResult);
            return true;
        }
    }
}
