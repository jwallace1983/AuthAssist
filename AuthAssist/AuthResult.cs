﻿using System;
using System.Collections.Generic;

namespace AuthAssist
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }

        public string Username { get; set; }

        public Dictionary<string, string> Claims { get; set; }

        public DateTime? ExpiresUtc { get; set; }
        
        public bool? SlidingExpiration { get; set; }

        public string Error { get; set; }
    }
}
