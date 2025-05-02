using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samples.WebApiDemo.Auth;

namespace Samples.WebApiDemo.Controllers
{
    [ApiController, Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet("public")]
        public string GetPublic() => "Public endpoint";

        [HttpGet("secure"), Authorize]
        public string GetSecure() => "Secure endpoint: " + User.Identity.Name;

        [HttpGet("admin"), Authorize(Policies.ADMIN)]
        public string GetClaim() => "Admin endpoint: " + User.Identity.Name;
    }
}