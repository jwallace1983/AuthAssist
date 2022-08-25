using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class LoginHandler : IRequestHandler
    {
        private readonly IServiceProvider _services;
        private readonly string _endpoint;
        private readonly Settings _settings;
        private static readonly string[] _methods = new string[] { HttpMethods.Post };
        private static readonly JsonSerializerOptions _jsonOptions = new()
        { 
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public LoginHandler(Settings settings, IServiceProvider services)
        {
            _services = services;
            _endpoint = $"{settings.Endpoint}/login";
            _settings = settings;
        }

        public Task<bool> CanHandle(HttpContext context)
        {
            return Task.FromResult(_methods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase)
                && _endpoint.Equals(context.Request.Path.Value, StringComparison.OrdinalIgnoreCase));
        }

        public async Task ProcessRequest(HttpContext context)
        {
            try
            {
                var authHandler = _services.GetService<IAuthHandler>();
                if (authHandler == null)
                    throw new ApplicationException("auth.config.error");
                var authRequest = await JsonSerializer.DeserializeAsync<AuthRequest>(context.Request.Body, _jsonOptions);
                var authResult = await authHandler.AuthenticateUser(authRequest);
                if (authResult.IsSuccess)
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, authResult.Username) };
                    await authHandler.AppendClaims(authResult.Username, claims);
                    context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                    authResult.ExpiresUtc = DateTime.UtcNow.Add(_settings.CookieDuration);
                    authResult.SlidingExpiration = _settings.CookieSlidingExpiration;
                    await context.SignInAsync(context.User);
                    authResult.Claims = claims.ToDictionary(claim => claim.Type, claim => claim.Value);
                }
                await ReturnSuccess(context.Response, authResult);
            }
            catch (Exception)
            {
                await ReturnError(context.Response);
            }
        }

        public static async Task ReturnSuccess(HttpResponse response, AuthResult authResult)
        {
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(authResult, _jsonOptions);
        }

        public static async Task ReturnError(HttpResponse response)
        {
            response.StatusCode = 501;
            await response.WriteAsync("app.error");
        }
    }
}
