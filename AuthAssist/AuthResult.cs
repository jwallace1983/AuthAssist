using AuthAssist.Services.OpenId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace AuthAssist
{
    public class AuthResult
    {
        public static AuthResult FromError(string error)
            => new() { Error = error, Claims = null };

        public static AuthResult FromUsername(string username)
            => new()
            {
                Username = username,
                Claims = {
                    { ClaimTypes.Name, username },
                },
            };

        public static AuthResult FromUserInfo(UserInfoResponse userInfo)
            => new()
            {
                Username = userInfo.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Claims = {
                    { ClaimTypes.Name, userInfo.Email },
                    { ClaimTypes.GivenName, userInfo.FirstName },
                    { ClaimTypes.Surname, userInfo.LastName },
                    { ClaimTypes.Email, userInfo.Email },
                },
            };

        public static AuthResult FromPrincipal(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated 
                ? new()
                {
                    Username = user.Identity.Name,
                    FirstName = user.FindFirst(ClaimTypes.GivenName)?.Value,
                    LastName = user.FindFirst(ClaimTypes.Surname)?.Value,
                    Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value),
                }
                : AuthResult.FromError("user.invalid");
        }

        public bool IsSuccess => string.IsNullOrEmpty(this.Error);

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Dictionary<string, string> Claims { get; set; } = [];

        public DateTime? ExpiresUtc { get; set; }

        public string Error { get; set; }

        [JsonIgnore]
        public AuthTypes AuthType { get; set; } = AuthTypes.None;

        public string AuthTypeName => this.AuthType.ToString().ToUpper();
    }
}
