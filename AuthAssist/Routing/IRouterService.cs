using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AuthAssist.Routing
{
    public interface IRouterService
    {
        Task Process(HttpContext context, Func<Task> next);
    }
}
