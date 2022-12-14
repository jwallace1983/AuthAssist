using AuthAssist.Broker.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthAssist.Broker
{
    public interface IBrokerService
    {
        Task Process(HttpContext context, Func<Task> next);
    }

    public class BrokerService : IBrokerService
    {
        private readonly Settings _settings;
        private readonly IEnumerable<IRequestHandler> _handlers;

        public BrokerService(Settings settings, IServiceProvider services)
        {
            _settings = settings;
            _handlers = services.GetServices<IRequestHandler>();
        }

        public async Task Process(HttpContext context, Func<Task> next)
        {
            if (!this.ValidateRequest(context.Request))
            {
                await next(); // Guard: do not process
                return;
            }
            try
            {
                // Use handler to process request
                foreach (var handler in _handlers)
                {
                    if (await handler.CanHandle(context))
                    {
                        await handler.ProcessRequest(context);
                        return; // Stop processing
                    }
                }

                // No handler matched, so show not found
                await _settings.HandleNotFound(context);
            }
            catch (Exception ex)
            {
                // Display the error message
                await _settings.HandleError(context, ex);
            }
        }

        public bool ValidateRequest(HttpRequest httpRequest)
        {
            return (!_settings.RequireHttps || httpRequest.IsHttps) // Require https if configured
                && httpRequest.Path.Value.StartsWith(_settings.Endpoint, StringComparison.OrdinalIgnoreCase); // Match path
        }

        public static Task ShowNotFound(HttpContext context)
        {
            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }

        public static async Task ShowError(HttpContext context, Exception ex)
        {
            await context.Response.WriteAsJsonAsync(new AuthResult
            {
                Error = ex is ApplicationException ? ex.Message : "app.error"
            });
        }
    }
}
