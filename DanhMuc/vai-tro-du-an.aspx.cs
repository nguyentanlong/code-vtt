using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class vai_tro_du_an : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(vai_tro_du_an));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Lấy danh sách công ty (đang hoạt động) để đổ vào dropdown lọc + form thêm/sửa
        /// </summary>
        [WebMethod]
        public static object GetCongTyOptions()
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", DBNull.Value },
                    { "@TrangThai", (object)(byte)1 }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMCongTy_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        CongTyID = dr["CongTyID"],
                        TenCongTy = dr["TenCongTy"].ToString()
                    });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetCongTyOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object GetList(string keyword, string congTyId)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"GetList called with keyword: {keyword}, congTyId: {congTyId}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@CongTyID", string.IsNullOrEmpty(congTyId) ? DBNull.Value : (object)Convert.ToInt64(congTyId) }
                };

                // SP cần JOIN sang DMCongTy để trả về TenCongTy, ORDER BY ThuTu ASC
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMVaiTroDuAn_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        VaiTroDuAnID = dr["VaiTroDuAnID"],
                        CongTyID = dr["CongTyID"],
                        TenCongTy = dr["TenCongTy"].ToString(),
                        MaVaiTro = dr["MaVaiTro"].ToString(),
                        TenVaiTro = dr["TenVaiTro"].ToString(),
                        ThuTu = dr["ThuTu"]
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

        [WebMethod]
        public static object SaveData(long vaiTroDuAnId, long congTyId, string maVaiTro, string tenVaiTro, int thuTu)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"SaveData called with vaiTroDuAnId: {vaiTroDuAnId}, congTyId: {congTyId}, maVaiTro: {maVaiTro}, tenVaiTro: {tenVaiTro}, thuTu: {thuTu}");
            try
            {
                if (congTyId <= 0)
                {
                    return new { success = false, message = "Vui lòng chọn Công ty!" };
                }
                if (string.IsNullOrWhiteSpace(maVaiTro))
                {
                    return new { success = false, message = "Vui lòng nhập Mã vai trò!" };
                }
                if (string.IsNullOrWhiteSpace(tenVaiTro))
                {
                    return new { success = false, message = "Vui lòng nhập Tên vai trò!" };
                }

                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@VaiTroDuAnID", vaiTroDuAnId },
                    { "@CongTyID", congTyId },
                    { "@MaVaiTro", maVaiTro.Trim() },
                    { "@TenVaiTro", tenVaiTro.Trim() },
                    { "@ThuTu", thuTu }
                };

                db.ExecuteDatasetStoredProcedure("sp_long_DMVaiTroDuAn_Save", pars);
                return new { success = true, message = vaiTroDuAnId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object DeleteData(long id)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"DeleteData called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@VaiTroDuAnID", id } };

                db.ExecuteDatasetStoredProcedure("sp_long_DMVaiTroDuAn_Delete", pars);
                return new { success = true, message = "Xóa thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}