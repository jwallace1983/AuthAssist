using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class GooglePage(IGoogleProvider googleProvider) : IEndpoint
    {
        private readonly IGoogleProvider _googleProvider = googleProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Uri => _googleProvider.Endpoint;

        public Task<bool> ProcessRequest(HttpContext context)
            => Task.FromResult(_googleProvider.RedirectToLogin(context));
    }
}
