using AuthAssist;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Samples.WebUI.Auth
{
    public class AuthService
    {
        private readonly static HttpClient _client = new();

        public async Task<AuthResult> Login(AuthRequest request)
        {
            try
            {
                var response = await _client.PostAsync("https://localhost:3000/api/auth/login", JsonContent.Create(request));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AuthResult>();
            }
            catch (Exception ex)
            {
                return new AuthResult { Error = ex.Message };
            }
        }

        public async Task<AuthResult> Logout()
        {
            try
            {
                var response = await _client.GetAsync("/api/auth/logout");
                response.EnsureSuccessStatusCode();
                return new AuthResult { Username = null };
            }
            catch (Exception ex)
            {
                return new AuthResult { Error = ex.Message };
            }
        }
    }
}
