using System;
using System.Web;

namespace VTT
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Xóa sạch dữ liệu Session phía server
            Session.Clear();
            Session.Abandon();

            // Ép trình duyệt loại bỏ Cookie Session cũ ngay lập tức,
            // đảm bảo request kế tiếp chắc chắn nhận 1 Session hoàn toàn mới, không còn dính dữ liệu cũ
            HttpCookie sessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            sessionCookie.Expires = DateTime.Now.AddDays(-1);
            sessionCookie.Path = "/";
            Response.Cookies.Add(sessionCookie);

            Response.Redirect("~/login.aspx", true);
        }
    }
}