using System;
using System.Collections.Generic;
using System.Globalization;

namespace VTT
{
    public partial class child1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string GetDisplayName()
        {
            string displayName = "Quản trị viên";
            if (Session["HoTen"] != null && !string.IsNullOrWhiteSpace(Session["HoTen"].ToString()))
            {
                displayName = Session["HoTen"].ToString();
            }
            else if (Session["Username"] != null && !string.IsNullOrWhiteSpace(Session["Username"].ToString()))
            {
                displayName = Session["Username"].ToString();
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
                    displayName = "Quản trị viên";
                }
            }

            return displayName;
        }

        protected string GetGreetingText()
        {
            return $"Xin chào, {GetDisplayName()}! 👋";
        }

        protected string GetCurrentDateText()
        {
            var culture = new CultureInfo("vi-VN");
            var now = DateTime.Now;
            var dayName = culture.DateTimeFormat.GetDayName(now.DayOfWeek);
            dayName = char.ToUpper(dayName[0]) + dayName.Substring(1);
            return $"{dayName}, {now:dd/MM/yyyy HH:mm:ss}";
        }
    }
}