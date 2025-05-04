using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class GoogleHandler(IGoogleProvider googleProvider) : IRequestHandler
    {
        private readonly IGoogleProvider _googleProvider = googleProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Endpoint => "google";

        public Task<bool> ProcessRequest(HttpContext context)
            => Task.FromResult(_googleProvider.RedirectToLogin(context, this.Endpoint));
    }
}
