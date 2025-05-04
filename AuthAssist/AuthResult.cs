using AuthAssist.Services.OpenId;
using System;
using System.Collections.Generic;
using System.Security.Claims;

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

        public bool IsSuccess => string.IsNullOrEmpty(this.Error);

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Dictionary<string, string> Claims { get; set; } = [];

        public DateTime? ExpiresUtc { get; set; }

        public string Error { get; set; }
    }
}
