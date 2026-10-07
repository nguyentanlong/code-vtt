using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT
{
    public partial class ThongBao : VTT.libs.BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ThongBao));

        protected void Page_Load(object sender, EventArgs e) { }

        [WebMethod(EnableSession = true)]
        public static object GetUnreadCount()
        {
            if (!IsAuthenticated()) return new { success = false, message = "Phiên làm việc đã hết hạn." };
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", GetCurrentUserId() } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_ThongBao_DemChuaDoc", pars);
                int soLuong = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? Convert.ToInt32(ds.Tables[0].Rows[0]["SoLuong"]) : 0;
                return new { success = true, data = soLuong };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetUnreadCount: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetList()
        {
            if (!IsAuthenticated()) return new { success = false, message = "Phiên làm việc đã hết hạn." };
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", GetCurrentUserId() }, { "@Top", 20 } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_ThongBao_GetList", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new
                    {
                        ThongBaoID = dr["ThongBaoID"],
                        TieuDe = dr["TieuDe"].ToString(),
                        NoiDung = dr["NoiDung"] == DBNull.Value ? "" : dr["NoiDung"].ToString(),
                        DuongDan = dr["DuongDan"] == DBNull.Value ? "" : dr["DuongDan"].ToString(),
                        DaDoc = Convert.ToBoolean(dr["DaDoc"]),
                        NgayTao = Convert.ToDateTime(dr["NgayTao"]).AddHours(7).ToString("dd/MM/yyyy HH:mm")
                        // NgayTao = Convert.ToDateTime(dr["NgayTao"]).ToString("dd/MM/yyyy HH:mm")
                    });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetList: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object MarkAsRead(long thongBaoId)
        {
            if (!IsAuthenticated()) return new { success = false, message = "Phiên làm việc đã hết hạn." };
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@ThongBaoID", thongBaoId }, { "@TaiKhoanID", GetCurrentUserId() } };
                db.ExecuteDatasetStoredProcedure("sp_v2_ThongBao_DanhDauDaDoc", pars);
                return new { success = true };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi MarkAsRead: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object MarkAllAsRead()
        {
            if (!IsAuthenticated()) return new { success = false, message = "Phiên làm việc đã hết hạn." };
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", GetCurrentUserId() } };
                db.ExecuteDatasetStoredProcedure("sp_v2_ThongBao_DanhDauTatCaDaDoc", pars);
                return new { success = true };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi MarkAllAsRead: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}