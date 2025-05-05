using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthAssist.Routing
{
    public interface IEndpoint
    {
        string Uri { get; }

        HttpMethod Method { get; }

        Task<bool> ProcessRequest(HttpContext context);
    }
}
