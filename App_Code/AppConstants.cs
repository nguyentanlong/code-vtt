using System;
namespace VTT.libs
{
    public static class Trang
    {
        public const string NV = "NHANVIEN";
        public const string PB = "PHONGBAN";
        public const string DA = "DUAN";
        public const string GD = "GIAIDOAN";
        public const string QT = "QUYTRINH";
        public const string BP = "BOPHAN";
        public const string TK = "TAIKHOAN";
        // Thêm dần khi refactor các trang tiếp theo: GiaiDoan, QuyTrinh, BoPhan...
    }

    /// <summary>
    /// Hằng số MaChucNang — khớp đúng cột ChucNang.MaChucNang đã seed trong DB.
    /// </summary>
    public static class ChucNang
    {
        public const string R = "XEM";
        public const string I = "THEM";
        public const string U = "SUA";
        public const string D = "XOA";
        public const string ALL = "ALL";
    }
}