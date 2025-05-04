using AuthAssist.Services.Local;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class LogoutPage(ILocalAuthService localProvider) : IEndpoint
    {
        private readonly ILocalAuthService _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Uri => "logout";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            await _localProvider.Logout(context);
            return true;
        }
    }
}
