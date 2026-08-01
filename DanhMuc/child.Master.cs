using System;
using System.Collections.Generic;
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
                if (Session["USER_SESSION"] == null && Session["NhanVienID"] == null)
                {
                    string returnUrl = Server.UrlEncode(Request.RawUrl);
                    Response.Redirect($"~/Login.aspx?returnUrl={returnUrl}", true);
                }
            }
        }

        protected string GetDisplayName()
        {
            string displayName = "GĐ VTT";
            if (Session["Username"] != null && !string.IsNullOrWhiteSpace(Session["Username"].ToString()))
            {
                displayName = Session["Username"].ToString();
            }
            else if (Session["HoTen"] != null && !string.IsNullOrWhiteSpace(Session["HoTen"].ToString()))
            {
                displayName = Session["HoTen"].ToString();
            }
            else if (Session["Pending_Username"] != null && !string.IsNullOrWhiteSpace(Session["Pending_Username"].ToString()))
            {
                displayName = Session["Pending_Username"].ToString();
            }
            else if (Session["TaiKhoanID"] != null)
            {
                try
                {
                    var db = new VTT.libs.ConnectServer();
                    var ds = db.ExecuteDataset("SELECT TOP 1 Username FROM dbo.DMTaiKhoan WHERE TaiKhoanID = @TaiKhoanID", new Dictionary<string, object>
                    {
                        { "@TaiKhoanID", Session["TaiKhoanID"] }
                    });
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        displayName = ds.Tables[0].Rows[0]["Username"].ToString();
                    }
                }
                catch
                {
                    displayName = "GĐ VTT";
                }
            }

            return HttpUtility.HtmlEncode(displayName);
        }

        protected string GetChatGreeting()
        {
            return $"Xin chào {GetDisplayName()}! Tôi có thể giúp gì cho bạn hôm nay?";
        }

        protected string GetUserInitials()
        {
            var displayName = GetDisplayName();
            if (string.IsNullOrWhiteSpace(displayName) || displayName == "GĐ VTT")
            {
                return "GD";
            }

            var parts = displayName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
            }

            return string.Concat(parts[0][0], parts[parts.Length - 1][0]).ToUpperInvariant();
        }
    }
}