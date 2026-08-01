using System;
using System.Web;
using System.Web.UI;

namespace VTT.libs
{
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
    }
}