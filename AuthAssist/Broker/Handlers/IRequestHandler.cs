using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AuthAssist.Broker.Handlers
{
    public interface IRequestHandler
    {
        Task<bool> CanHandle(HttpContext context);

        Task ProcessRequest(HttpContext context);
    }
}
