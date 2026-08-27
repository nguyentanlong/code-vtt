using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using VTT.libs;

namespace VTT.libs
{
    public class BasePage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["TaiKhoanID"] == null)
            {
                string currentUrl = Server.UrlEncode(Request.RawUrl);
                Response.Redirect($"~/login.aspx?returnUrl={currentUrl}", true);
            }
        }

        public static bool IsAuthenticated()
        {
            var context = HttpContext.Current;
            return context != null && context.Session != null && context.Session["TaiKhoanID"] != null;
        }

        public static long GetCurrentUserId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["TaiKhoanID"] != null)
            {
                return Convert.ToInt64(context.Session["TaiKhoanID"]);
            }
            return 0;
        }

        public static long GetCurrentCongTyId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["CongTyID"] != null && context.Session["CongTyID"] != DBNull.Value)
            {
                return Convert.ToInt64(context.Session["CongTyID"]);
            }
            return 0;
        }

        public static long GetCurrentPhongBanId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["PhongBanID"] != null && context.Session["PhongBanID"] != DBNull.Value)
            {
                return Convert.ToInt64(context.Session["PhongBanID"]);
            }
            return 0;
        }

        public static long GetCurrentChiNhanhId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["ChiNhanhID"] != null && context.Session["ChiNhanhID"] != DBNull.Value)
            {
                return Convert.ToInt64(context.Session["ChiNhanhID"]);
            }
            return 0;
        }

        /// <summary>
        /// Lấy phạm vi (DataScope) mà tài khoản hiện tại được cấp cho 1 Chức năng trên 1 Trang,
        /// KHÔNG so khớp với bản ghi cụ thể nào — dùng để lọc câu truy vấn danh sách (GetList)
        /// trước khi trả dữ liệu về client, tránh lộ dữ liệu ngoài phạm vi rồi chỉ ẩn nút ở giao diện.
        /// </summary>
        /// <returns>"CONGTY" / "CHINHANH" / "PHONGBAN" nếu được phép; null nếu bị từ chối (DENY)</returns>
        protected static string GetPermissionScope(string maTrang, string maChucNang)
        {
            long taiKhoanId = GetCurrentUserId();
            if (taiKhoanId == 0) return null;

            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object>
            {
                { "@TaiKhoanID", taiKhoanId },
                { "@MaTrang", maTrang },
                { "@MaChucNang", maChucNang }
            };

            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_KiemTraQuyen", pars);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow dr = ds.Tables[0].Rows[0];
            string giaTri = dr["GiaTri"] == DBNull.Value ? "DENY" : dr["GiaTri"].ToString();
            if (giaTri != "ALLOW") return null;

            return dr["DataScope"] == DBNull.Value ? null : dr["DataScope"].ToString();
        }

        /// <summary>
        /// Kiểm tra tài khoản hiện tại có được phép thực hiện 1 Chức năng lên 1 bản ghi cụ thể không,
        /// dựa trên phạm vi (DataScope) so khớp với Phòng ban/Chi nhánh của bản ghi đích.
        /// </summary>
        protected static bool CheckPermission(string maTrang, string maChucNang, long targetPhongBanId = 0, long targetChiNhanhId = 0)
        {
            string dataScope = GetPermissionScope(maTrang, maChucNang);
            if (dataScope == null) return false;

            switch (dataScope)
            {
                case "CONGTY":
                    return true;

                case "CHINHANH":
                    return targetChiNhanhId != 0 && targetChiNhanhId == GetCurrentChiNhanhId();

                case "PHONGBAN":
                    return targetPhongBanId != 0 && targetPhongBanId == GetCurrentPhongBanId();

                default:
                    return false;
            }
        }
    }
}