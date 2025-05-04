using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class GoogleCallbackHandler(IGoogleProvider googleProvider, ILocalProvider localProvider) : IRequestHandler
    {
        private readonly IGoogleProvider _googleProvider = googleProvider;

        private readonly ILocalProvider _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Endpoint => "google/callback";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await _googleProvider.AuthenticateUser(context);
            await _localProvider.Login(context, authResult);
            return true;
        }
    }
}
