using System;
using System.Text;
using System.Web;

namespace VTT
{
    public partial class debug_session : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] keysToCheck = new string[]
            {
                "TaiKhoanID",
                "NhanVienID",
                "TenDangNhap",
                "HoTen",
                "Email",
                "CongTyID",
                "PhongBanID",
                "ChiNhanhID",
                "ChucVuID"
            };

            StringBuilder sb = new StringBuilder();

            foreach (var key in keysToCheck)
            {
                var val = Session[key];
                string displayVal = val == null
                    ? "<span class='null-val'>NULL (chưa được set)</span>"
                    : HttpUtility.HtmlEncode(val.ToString());

                sb.Append($"<tr><td>{key}</td><td>{displayVal}</td></tr>");
            }

            litSessionRows.Text = sb.ToString();
        }
    }
}