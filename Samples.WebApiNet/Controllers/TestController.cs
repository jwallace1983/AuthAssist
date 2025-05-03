using AuthAssist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Samples.WebApiDemo.Controllers
{
    [ApiController, Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly static HttpClient _httpClient = new();

        [HttpGet("run")]
        public IActionResult Get()
        {
            return Ok(new
            {
                Message = "Welcome",
                LoginAsUser = "https://localhost:3000/test/loginAsUser",
                LoginAsAdmin = "https://localhost:3000/test/loginAsAdmin",
                LoginInvalid = "https://localhost:3000/test/loginInvalid",
                LoginGoogle = "https://localhost:3000/test/LoginGoogle",
                Logout = "https://localhost:3000/test/logout",
            });
        }

        [HttpGet("loginAsUser")]
        public async Task<IActionResult> LoginAsUser()
            => await this.Login("user", "Welcome123");

        [HttpGet("loginAsAdmin")]
        public async Task<IActionResult> LoginAsAdmin()
            => await this.Login("admin", "Welcome123");

        [HttpGet("loginInvalid")]
        public async Task<IActionResult> LoginInvalid()
            => await this.Login("fake", "user");

        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var request = new AuthRequest { Username = username, Password = password };
                var response = await _httpClient.PostAsJsonAsync("https://localhost:3000/api/auth/login", request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<AuthResult>();
                return Ok(new
                {
                    Link = "https://localhost:3000/test/run",
                    Result = result,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Link = "https://localhost:3000/test/run",
                    Error = ex.Message,
                });
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:3000/api/auth/logout");
                response.EnsureSuccessStatusCode();
                return Ok(new
                {
                    Link = "https://localhost:3000/test/run",
                    Result = "logout done",
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Link = "https://localhost:3000/test/run",
                    Error = ex.Message,
                });
            }
        }

        [HttpGet("loginGoogle")]
        public async Task<IActionResult> LoginGoogle()
        {
            return await Task.FromResult(Redirect("https://localhost:3000/api/auth/google/start"));
        }
    }
}
