using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Broker
{
    public interface IRequestHandler
    {
        string Endpoint { get; }

        HttpMethod Method { get; }

        Task<bool> ProcessRequest(HttpContext context);
    }
}
