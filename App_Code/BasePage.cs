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
        // protected override void OnInit(EventArgs e)
        // {
        //     base.OnInit(e);
        //     // Ngăn trình duyệt lưu cache trang này (kể cả qua nút Back/Forward hay mở lại từ Lịch sử),
        //     // đảm bảo mọi lần truy cập trang được bảo vệ đều phải qua kiểm tra Session mới trên server,
        //     // không hiển thị lại nội dung cũ từ bộ nhớ đệm sau khi đã Đăng xuất.
        //     Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //     Response.Cache.SetNoStore();
        //     Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
        //     Response.AppendHeader("Pragma", "no-cache");

        //     if (Session["TaiKhoanID"] == null)
        //     {
        //         string currentUrl = Server.UrlEncode(Request.RawUrl);
        //         Response.Redirect($"~/login.aspx?returnUrl={currentUrl}", true);
        //     }
        // }

        // public static bool IsAuthenticated()
        // {
        //     var context = HttpContext.Current;
        //     return context != null && context.Session != null && context.Session["TaiKhoanID"] != null;
        // }
        /*
        kiểm tra sau mồi phút nhẹ hơn, cho hệ thống lớn
        public static bool IsAuthenticated()
{
    var context = HttpContext.Current;
    if (context == null || context.Session == null || context.Session["TaiKhoanID"] == null)
        return false;

    var deviceId = context.Session["DeviceID"] as string;
    if (string.IsNullOrEmpty(deviceId))
        return true;

    // Chỉ kiểm tra lại DB mỗi 2 phút/lần, không phải mọi request -> giảm tải đáng kể
    var lastCheckObj = context.Session["LastDeviceCheck"];
    DateTime lastCheck = lastCheckObj != null ? (DateTime)lastCheckObj : DateTime.MinValue;

    if ((DateTime.UtcNow - lastCheck).TotalMinutes < 2)
    {
        return true; // Vừa kiểm tra gần đây, bỏ qua lần này cho nhẹ
    }

    long taiKhoanId = Convert.ToInt64(context.Session["TaiKhoanID"]);
    ConnectServer db = new ConnectServer();
    var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId }, { "@DeviceID", deviceId } };
    DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_KiemTraHieuLuc", pars);

    context.Session["LastDeviceCheck"] = DateTime.UtcNow; // Cập nhật mốc kiểm tra gần nhất

    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
    {
        bool conHieuLuc = Convert.ToBoolean(ds.Tables[0].Rows[0]["ConHieuLuc"]);
        if (!conHieuLuc)
        {
            context.Session.Clear();
            context.Session.Abandon();
            return false;
        }
    }

    return true;
}
        */
        public static bool IsAuthenticated()
        {
            var context = HttpContext.Current;
            if (context == null || context.Session == null || context.Session["TaiKhoanID"] == null)
                return false;

            // Kiểm tra thiết bị hiện tại có còn được phép hoạt động không (có thể đã bị người dùng tự thu hồi từ xa)
            var deviceId = context.Session["DeviceID"] as string;
            if (string.IsNullOrEmpty(deviceId))
                return true; // Không có DeviceID (trường hợp cũ/đặc biệt) -> bỏ qua kiểm tra, giữ hành vi an toàn cũ

            long taiKhoanId = Convert.ToInt64(context.Session["TaiKhoanID"]);
            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId }, { "@DeviceID", deviceId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_KiemTraHieuLuc", pars);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                bool conHieuLuc = Convert.ToBoolean(ds.Tables[0].Rows[0]["ConHieuLuc"]);
                if (!conHieuLuc)
                {
                    // Thiết bị đã bị thu hồi -> hủy Session ngay lập tức
                    context.Session.Clear();
                    context.Session.Abandon();
                    return false;
                }
            }

            return true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.AppendHeader("Pragma", "no-cache");

            if (!IsAuthenticated())
            {
                string currentUrl = Server.UrlEncode(Request.RawUrl);
                Response.Redirect($"~/login.aspx?returnUrl={currentUrl}", true);
            }
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
        protected static bool CheckPermission(string maTrang, string maChucNang, long targetPhongBanId = 0, long targetChiNhanhId = 0, long? nguoiTaoId = null)
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
                case "TU_TAO":
                    return nguoiTaoId.HasValue && nguoiTaoId.Value == GetCurrentUserId();

                default:
                    return false;
            }
        }
    }
}