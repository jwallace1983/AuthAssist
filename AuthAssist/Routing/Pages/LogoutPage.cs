using AuthAssist.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class LogoutPage(ILocalProvider localProvider) : IEndpoint
    {
        private readonly ILocalProvider _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Uri => "logout";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            await _localProvider.Logout(context);
            return true;
        }
    }
}
