using System;
using System.Web;
using System.Web.UI;

namespace VTT.libs
{
    public class AccessScope
    {
        public bool IsFullAccess { get; set; }  // true = Admin/IT, CRUD toàn hệ thống
        public long PhongBanID { get; set; }    // phòng ban của tài khoản hiện tại
    }
    public class BasePage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Kiểm tra Session chính thức (TaiKhoanID)
            if (Session["TaiKhoanID"] == null)
            {
                // Nếu là request trang bình thường (GET page), chuyển hướng về Login
                string currentUrl = Server.UrlEncode(Request.RawUrl);
                Response.Redirect($"~/login.aspx?returnUrl={currentUrl}", true);
            }
        }

        /// <summary>
        /// Hàm tiện ích dùng cho các static [WebMethod] kiểm tra quyền truy cập AJAX
        /// </summary>
        public static bool IsAuthenticated()
        {
            var context = HttpContext.Current;
            return context != null && context.Session != null && context.Session["TaiKhoanID"] != null;
        }

        /// <summary>
        /// Lấy TaiKhoanID của User hiện tại đang đăng nhập
        /// </summary>
        public static long GetCurrentUserId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["TaiKhoanID"] != null)
            {
                return Convert.ToInt64(context.Session["TaiKhoanID"]);
            }
            return 0;
        }

        /// <summary>
        /// Lấy CongTyID của User hiện tại đang đăng nhập
        /// </summary>
        public static long GetCurrentCongTyId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["CongTyID"] != null && context.Session["CongTyID"] != DBNull.Value)
            {
                return Convert.ToInt64(context.Session["CongTyID"]);
            }
            return 0;
        }
// Long thêm
private static readonly string[] FullAccessRoleCodes = { "ADMIN", "IT" };

    protected static AccessScope GetCurrentAccessScope()
    {
        var maVaiTro = System.Web.HttpContext.Current.Session["MaVaiTro"] as string ?? "";
        var phongBanObj = System.Web.HttpContext.Current.Session["PhongBanID"];

        return new AccessScope
        {
            IsFullAccess = Array.Exists(FullAccessRoleCodes, code => code.Equals(maVaiTro, StringComparison.OrdinalIgnoreCase)),
            PhongBanID = phongBanObj != null ? Convert.ToInt64(phongBanObj) : 0
        };
    }

    /// <summary>
    /// Kiểm tra tài khoản hiện tại có được SỬA/XÓA (CRUD) dữ liệu thuộc 1 Phòng ban cụ thể không.
    /// Admin/IT luôn được phép. Người khác chỉ được phép nếu đúng Phòng ban của mình.
    /// </summary>
    protected static bool CanEdit(long targetPhongBanId)
    {
        var scope = GetCurrentAccessScope();
        return scope.IsFullAccess || scope.PhongBanID == targetPhongBanId;
    }

    }
}