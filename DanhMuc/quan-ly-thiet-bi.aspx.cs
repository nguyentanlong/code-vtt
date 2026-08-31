using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class quan_ly_thiet_bi : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(quan_ly_thiet_bi));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /*[WebMethod(EnableSession = true)]
        public static object GetList(string myDeviceId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", GetCurrentUserId() } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_GetList", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string deviceId = dr["DeviceID"].ToString();
                    list.Add(new
                    {
                        ThietBiID = dr["ThietBiID"],
                        TenThietBi = dr["TenThietBi"] == DBNull.Value ? "Thiết bị không rõ" : dr["TenThietBi"].ToString(),
                        IPDangNhap = dr["IPDangNhap"] == DBNull.Value ? "" : dr["IPDangNhap"].ToString(),
                        LanDangNhapDauText = Convert.ToDateTime(dr["LanDangNhapDau"]).ToString("dd/MM/yyyy HH:mm"),
                        LanHoatDongCuoiText = Convert.ToDateTime(dr["LanHoatDongCuoi"]).ToString("dd/MM/yyyy HH:mm"),
                        LaThietBiHienTai = !string.IsNullOrEmpty(myDeviceId) && deviceId == myDeviceId
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
        public static object DeleteData(long thietBiId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@ThietBiID", thietBiId },
                    { "@TaiKhoanID", GetCurrentUserId() } // ràng buộc chỉ được thu hồi thiết bị của chính mình
                };
                db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_ThuHoi", pars);

                return new { success = true, message = "Đã đăng xuất thiết bị thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }*/
        [WebMethod(EnableSession = true)]
        public static object GetList(string myDeviceId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@TaiKhoanID", GetCurrentUserId() },
                    { "@BaoGomDaThuHoi", true } // Hiện cả thiết bị cũ đã thu hồi để người dùng có thể xóa vĩnh viễn
                };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_GetList", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string deviceId = dr["DeviceID"].ToString();
                    list.Add(new
                    {
                        ThietBiID = dr["ThietBiID"],
                        TenThietBi = dr["TenThietBi"] == DBNull.Value ? "Thiết bị không rõ" : dr["TenThietBi"].ToString(),
                        IPDangNhap = dr["IPDangNhap"] == DBNull.Value ? "" : dr["IPDangNhap"].ToString(),
                        LanDangNhapDauText = Convert.ToDateTime(dr["LanDangNhapDau"]).ToString("dd/MM/yyyy HH:mm"),
                        LanHoatDongCuoiText = Convert.ToDateTime(dr["LanHoatDongCuoi"]).ToString("dd/MM/yyyy HH:mm"),
                        DangHoatDong = Convert.ToBoolean(dr["DangHoatDong"]),
                        LaThietBiHienTai = !string.IsNullOrEmpty(myDeviceId) && deviceId == myDeviceId
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
        public static object DeleteData(long thietBiId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@ThietBiID", thietBiId }, { "@TaiKhoanID", GetCurrentUserId() } };
                db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_ThuHoi", pars);

                return new { success = true, message = "Đã đăng xuất thiết bị thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object XoaVinhVien(long thietBiId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@ThietBiID", thietBiId }, { "@TaiKhoanID", GetCurrentUserId() } };
                db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_XoaVinhVien", pars);

                return new { success = true, message = "Đã xóa thiết bị khỏi danh sách!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi XoaVinhVien: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}