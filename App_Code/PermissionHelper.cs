using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using VTT.libs;

namespace VTT.libs
{
    public static class PermissionHelper
    {
        public static long GetCurrentUserId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["TaiKhoanID"] != null)
                return Convert.ToInt64(context.Session["TaiKhoanID"]);
            return 0;
        }

        public static long GetCurrentPhongBanId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["PhongBanID"] != null && context.Session["PhongBanID"] != DBNull.Value)
                return Convert.ToInt64(context.Session["PhongBanID"]);
            return 0;
        }

        public static long GetCurrentChiNhanhId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["ChiNhanhID"] != null && context.Session["ChiNhanhID"] != DBNull.Value)
                return Convert.ToInt64(context.Session["ChiNhanhID"]);
            return 0;
        }

        /// <summary>
        /// Truy vấn DB đúng 1 lần để lấy DataScope tài khoản hiện tại được cấp cho 1 Chức năng trên 1 Trang.
        /// Nên gọi hàm này 1 LẦN duy nhất (ví dụ trước vòng lặp GetList), không gọi lại cho từng bản ghi.
        /// </summary>
        public static string GetPermissionScope(string maTrang, string maChucNang)
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
        /// So khớp phạm vi (DataScope) đã lấy sẵn với 1 bản ghi cụ thể — KHÔNG chạm DB.
        /// Dùng trong vòng lặp GetList sau khi đã gọi GetPermissionScope 1 lần ở ngoài vòng lặp.
        /// </summary>
        public static bool EvaluateScope(string dataScope, long targetPhongBanId = 0, long targetChiNhanhId = 0, long? nguoiTaoId = null)
        {
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

        /// <summary>
        /// Kiểm tra quyền cho ĐÚNG 1 bản ghi (gọi DB 1 lần) — dùng cho SaveData/DeleteData (chỉ có 1 bản ghi liên quan).
        /// KHÔNG dùng hàm này trong vòng lặp GetList — dùng GetPermissionScope + EvaluateScope thay thế.
        /// </summary>
        public static bool CheckPermission(string maTrang, string maChucNang, long targetPhongBanId = 0, long targetChiNhanhId = 0, long? nguoiTaoId = null)
        {
            string dataScope = GetPermissionScope(maTrang, maChucNang);
            return EvaluateScope(dataScope, targetPhongBanId, targetChiNhanhId, nguoiTaoId);
        }

        public static List<string> GetVisibleTrangList()
        {
            var result = new List<string>();
            if (GetCurrentUserId() == 0) return result;

            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_Trang_GetAllMaTrang", new Dictionary<string, object>());

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string maTrang = dr["MaTrang"].ToString();
                    if (GetPermissionScope(maTrang, ChucNang.R) != null)
                    {
                        result.Add(maTrang);
                    }
                }
            }
            return result;
        }
        public static bool IsAdminOwner()
        {
            long taiKhoanId = GetCurrentUserId();
            if (taiKhoanId == 0) return false;

            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_KiemTraLaAdmin", pars);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return Convert.ToInt32(ds.Tables[0].Rows[0]["LaAdmin"]) == 1;
            return false;
        }

        public static int GetCurrentCapBac()
        {
            long taiKhoanId = GetCurrentUserId();
            if (taiKhoanId == 0) return 999; // Không xác định -> coi như cấp thấp nhất, an toàn

            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetCapBac", pars);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["CapBac"] != DBNull.Value)
                return Convert.ToInt32(ds.Tables[0].Rows[0]["CapBac"]);
            return 999;
        }
    }
}