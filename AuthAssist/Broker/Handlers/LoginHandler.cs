using AuthAssist.Providers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class LoginHandler(ILocalProvider localProvider) : IRequestHandler
    {
        private readonly ILocalProvider _localProvider = localProvider;

        public HttpMethod Method => HttpMethod.Post;

        public string Endpoint => "login";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            var authResult = await _localProvider.AuthenticateUser(context);
            await _localProvider.Login(context, authResult);
            return true;
        }
    }
}
