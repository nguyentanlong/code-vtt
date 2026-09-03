using System;
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

        public static bool IsAuthenticated()
        {
            var context = HttpContext.Current;
            if (context == null || context.Session == null || context.Session["TaiKhoanID"] == null)
                return false;

            var deviceId = context.Session["DeviceID"] as string;
            if (string.IsNullOrEmpty(deviceId))
                return true;

            long taiKhoanId = Convert.ToInt64(context.Session["TaiKhoanID"]);
            ConnectServer db = new ConnectServer();
            var pars = new System.Collections.Generic.Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId }, { "@DeviceID", deviceId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_KiemTraHieuLuc", pars);

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

        public static long GetCurrentUserId() => PermissionHelper.GetCurrentUserId();

        public static long GetCurrentCongTyId()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["CongTyID"] != null && context.Session["CongTyID"] != DBNull.Value)
                return Convert.ToInt64(context.Session["CongTyID"]);
            return 0;
        }

        public static long GetCurrentPhongBanId() => PermissionHelper.GetCurrentPhongBanId();
        public static long GetCurrentChiNhanhId() => PermissionHelper.GetCurrentChiNhanhId();

        protected static string GetPermissionScope(string maTrang, string maChucNang)
            => PermissionHelper.GetPermissionScope(maTrang, maChucNang);

        protected static bool CheckPermission(string maTrang, string maChucNang, long targetPhongBanId = 0, long targetChiNhanhId = 0, long? nguoiTaoId = null)
            => PermissionHelper.CheckPermission(maTrang, maChucNang, targetPhongBanId, targetChiNhanhId, nguoiTaoId);
                [Obsolete("Tạm thời, cần refactor trang dùng hàm này sang CheckPermission()")]
        protected static AccessScope GetCurrentAccessScope()
        {
            return new AccessScope
            {
                IsFullAccess = false,
                IsTongCtyAccess = false,
                IsChiNhanhAccess = false,
                CongTyID = GetCurrentCongTyId(),
                PhongBanID = GetCurrentPhongBanId()
            };
        }

        [Obsolete("Tạm thời, cần refactor trang dùng hàm này sang CheckPermission()")]
        protected static bool CanEdit(long targetPhongBanId)
        {
            return targetPhongBanId != 0 && targetPhongBanId == GetCurrentPhongBanId();
        }
    }
}