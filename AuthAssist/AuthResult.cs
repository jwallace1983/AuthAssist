using System;
using System.Collections.Generic;

namespace AuthAssist
{
    public class AuthResult
    {
        public static AuthResult FromError(string error)
            => new() { Error = error };

        public static AuthResult FromUsername(string username)
            => new() { Username = username };

        public bool IsSuccess => string.IsNullOrEmpty(this.Error);

        public string Username { get; set; }

        public Dictionary<string, string> Claims { get; set; }

        public DateTime? ExpiresUtc { get; set; }

        public string Error { get; set; }
    }
}
