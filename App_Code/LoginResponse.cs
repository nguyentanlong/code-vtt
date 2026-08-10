using System;

namespace VTT
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}
