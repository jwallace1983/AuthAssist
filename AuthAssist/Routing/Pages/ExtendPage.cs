using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class ExtendPage(ILocalProvider localProvider) : IEndpoint
    {
        private readonly ILocalProvider _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Uri => "extend";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await _localProvider.GetAuthenticatedUser(context.User);
            await _localProvider.Login(context, authResult);
            return true;
        }
    }
}
