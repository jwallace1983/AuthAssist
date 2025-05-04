using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class LoginPage(ILocalProvider localProvider) : IEndpoint
    {
        private readonly ILocalProvider _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Post;

        public string Uri => "login";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await _localProvider.AuthenticateUser(context);
            await _localProvider.Login(context, authResult);
            return true;
        }
    }
}
