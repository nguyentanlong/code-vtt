/*using System;
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
}*/
using System;
using System.Collections.Generic;
using System.Web;
using VTT.libs;

namespace VTT
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Thu hồi thiết bị hiện tại trong DB TRƯỚC KHI xóa Session (cần đọc Session lúc còn tồn tại)
            if (Session["TaiKhoanID"] != null && Session["DeviceID"] != null)
            {
                try
                {
                    long taiKhoanId = Convert.ToInt64(Session["TaiKhoanID"]);
                    string deviceId = Session["DeviceID"].ToString();

                    ConnectServer db = new ConnectServer();
                    var pars = new Dictionary<string, object>
                    {
                        { "@TaiKhoanID", taiKhoanId },
                        { "@DeviceID", deviceId }
                    };
                    db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_ThuHoiTheoDeviceId", pars);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Lỗi thu hồi thiết bị khi logout: " + ex.ToString());
                    // Không chặn logout dù lỗi ghi DB -> vẫn phải xóa Session bình thường
                }
            }

            Session.Clear();
            Session.Abandon();

            HttpCookie sessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            sessionCookie.Expires = DateTime.Now.AddDays(-1);
            sessionCookie.Path = "/";
            Response.Cookies.Add(sessionCookie);

            Response.Redirect("~/login.aspx", true);
        }
    }
}