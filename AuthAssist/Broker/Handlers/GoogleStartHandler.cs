using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public class GoogleStartHandler(Settings settings) : IRequestHandler
    {
        public HttpMethod Method { get; } = HttpMethod.Get;

        public string Endpoint { get; } = $"{settings.Prefix}/google/start";

        /*
         * https://accounts.google.com/o/oauth2/v2/auth?
  scope=https%3A//www.googleapis.com/auth/drive.metadata.readonly%20https%3A//www.googleapis.com/auth/calendar.readonly&
  include_granted_scopes=true&
  response_type=token&
  state=state_parameter_passthrough_value&
  redirect_uri=https%3A//oauth2.example.com/code&
  client_id=your_client_id
         */

        public Task<bool> ProcessRequest(HttpContext context)
        {
            if (settings.GoogleIdp == null)
                return Task.FromResult(false); // Guard: Google not configured
            var url = "https://google.com";
            context.Response.Redirect(url, false);
            return Task.FromResult(true);
        }
    }
}
