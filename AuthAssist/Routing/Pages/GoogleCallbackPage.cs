using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing.Pages
{
    public class GoogleCallbackPage(IGoogleProvider googleProvider, ILocalProvider localProvider) : IEndpoint
    {
        private readonly IGoogleProvider _googleProvider = googleProvider;

        private readonly ILocalProvider _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Get;

        public string Uri => "google/callback";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await _googleProvider.AuthenticateUser(context);
            await _localProvider.Login(context, authResult);
            return true;
        }
    }
}
