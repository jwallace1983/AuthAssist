using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class LogoutHandler(Settings settings) : IRequestHandler
    {
        public HttpMethod Method { get; } = HttpMethod.Get;

        public string Endpoint { get; } = $"{settings.Prefix}/logout";

        public async Task<bool> ProcessRequest(HttpContext context)
        {
            await context.SignOutAsync();
            context.Response.StatusCode = 204;
            return true;
        }
    }
}
