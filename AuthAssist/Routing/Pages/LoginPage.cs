using AuthAssist.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class LoginPage(IAuthFacade authFacade) : IEndpoint
    {
        public HttpMethod Method => HttpMethod.Post;

        public string Uri => "login";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await authFacade.Local.AuthenticateUser(context);
            await authFacade.Local.Login(context, authResult);
            return true;
        }
    }
}
