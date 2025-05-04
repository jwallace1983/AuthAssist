using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class MicrosoftPage(IMicrosoftProvider microsoftProvider) : IEndpoint
    {
        private readonly IMicrosoftProvider _microsoftProvider = microsoftProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Uri => _microsoftProvider.Endpoint;

        public Task<bool> ProcessRequest(HttpContext context)
            => Task.FromResult(_microsoftProvider.RedirectToLogin(context));
    }
}
