using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthAssist.Routing
{
    public class RouterService : IRouterService
    {
        private readonly Settings _settings;
        private readonly Dictionary<string, IEndpoint> _routes = new(StringComparer.OrdinalIgnoreCase);

        public RouterService(Settings options, IEnumerable<IEndpoint> handlers)
        {
            _settings = options;
            foreach (var handler in handlers)
            {
                var key = GetKey(handler.Method, $"{_settings.Prefix}/{handler.Uri}");
                _routes[key] = handler;
            }
        }

        public static string GetKey<TMethod>(TMethod method, string endpoint)
            => $"{method}|{endpoint}";

        public bool TryGetHandler(HttpContext context, out IEndpoint handler)
        {
            handler = null;
            if (_settings.RequireHttps && !context.Request.IsHttps)
                return false; // Guard: Invalid https request
            string key = GetKey(context.Request.Method, context.Request.Path.Value);
            return _routes.TryGetValue(key, out handler);
        }

        public async Task Process(HttpContext context, Func<Task> next)
        {
            if (this.TryGetHandler(context, out var handler))
                await handler.ProcessRequest(context);
            else
                await next(); // No handler matched, so proceed in pipeline
        }
    }
}
