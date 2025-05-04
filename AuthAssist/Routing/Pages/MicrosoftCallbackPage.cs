using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class MicrosoftCallbackPage(IMicrosoftProvider microsoftProvider, ILocalProvider localProvider) : IEndpoint
    {
        private readonly IMicrosoftProvider _microsoftProvider = microsoftProvider;

        private readonly ILocalProvider _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Uri => $"{_microsoftProvider.Endpoint}/callback";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await _microsoftProvider.AuthenticateUser(context);
            await _localProvider.Login(context, authResult);
            return true;
        }
    }
}
