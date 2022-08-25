using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samples.WebApiNet6.Auth;

namespace Samples.WebApiNet6.Controllers
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