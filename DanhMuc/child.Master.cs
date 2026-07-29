using System;
using System.Web;
using System.Web.UI;

namespace VTT.DanhMuc
{
    public partial class child : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra đăng nhập ở cấp MasterPage
                if (Session["USER_SESSION"] == null && Session["NhanVienID"] == null)
                {
                    string returnUrl = Server.UrlEncode(Request.RawUrl);
                    Response.Redirect($"~/Login.aspx?returnUrl={returnUrl}", true);
                }
            }
        }
    }
}