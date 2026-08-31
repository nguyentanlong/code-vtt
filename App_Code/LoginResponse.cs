using System;

namespace VTT
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
        public System.Collections.Generic.List<object> ThietBiList { get; set; } // Mới: danh sách thiết bị khi bị chặn
    }
}
