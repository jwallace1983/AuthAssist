using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class LogoutHandler(Settings settings) : IRequestHandler
    {
        private readonly string _endpoint = $"{settings.Endpoint}/logout";
        private static readonly string[] _methods = [ HttpMethods.Post, HttpMethods.Get ];

        public Task<bool> CanHandle(HttpContext context)
        {
            return Task.FromResult(_methods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase)
                && _endpoint.Equals(context.Request.Path.Value, StringComparison.OrdinalIgnoreCase));
        }

        public async Task ProcessRequest(HttpContext context)
        {
            await context.SignOutAsync();
            context.Response.StatusCode = 204;
        }
    }
}
