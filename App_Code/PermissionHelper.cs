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

        public static bool CheckPermission(string maTrang, string maChucNang, long targetPhongBanId = 0, long targetChiNhanhId = 0, long? nguoiTaoId = null)
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

        /// <summary>
        /// Trả về danh sách MaTrang mà tài khoản hiện tại có quyền Xem (dùng cho menu sidebar).
        /// </summary>
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
                    if (GetPermissionScope(maTrang, "XEM") != null)
                    {
                        result.Add(maTrang);
                    }
                }
            }
            return result;
        }
    }
}