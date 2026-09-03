using System;

namespace VTT.libs
{
    [Obsolete("Tạm thời, cần refactor trang dùng class này sang CheckPermission()")]
    public class AccessScope
    {
        public bool IsFullAccess { get; set; }
        public bool IsTongCtyAccess { get; set; }
        public bool IsChiNhanhAccess { get; set; }
        public long CongTyID { get; set; }
        public long PhongBanID { get; set; }
    }
}