using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    // Kế thừa BasePage để tự động kiểm tra Login khi người dùng load trang
    public partial class cong_ty : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cong_ty));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static object GetList(string keyword, string trangThai)
        {
            // Kiểm tra Authentication trước khi thực thi
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"GetList called with keyword: {keyword}, trangThai: {trangThai}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMCongTy_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        CongTyID = dr["CongTyID"],
                        MaCongTy = dr["MaCongTy"].ToString(),
                        TenCongTy = dr["TenCongTy"].ToString(),
                        TenVietTat = dr["TenVietTat"].ToString(),
                        MaSoThue = dr["MaSoThue"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
                        MoTa = dr["MoTa"].ToString(),
                        NgayTaoText = dr["NgayTaoText"].ToString()
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
        public static object GetById(long id)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"GetById called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMCongTy_GetById", pars);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        CongTyID = dr["CongTyID"],
                        MaCongTy = dr["MaCongTy"].ToString(),
                        TenCongTy = dr["TenCongTy"].ToString(),
                        TenVietTat = dr["TenVietTat"].ToString(),
                        MaSoThue = dr["MaSoThue"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        TrangThai = dr["TrangThai"],
                        MoTa = dr["MoTa"].ToString()
                    };
                    return new { success = true, data = data };
                }
                return new { success = false, message = "Không tìm thấy bản ghi." };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetById: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object SaveData(long congTyId, string maCongTy, string tenCongTy, string tenVietTat, string maSoThue, string diaChi, byte trangThai, string moTa)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"SaveData called with congTyId: {congTyId}, maCongTy: {maCongTy}, tenCongTy: {tenCongTy}, tenVietTat: {tenVietTat}, maSoThue: {maSoThue}, diaChi: {diaChi}, trangThai: {trangThai}, moTa: {moTa}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@MaCongTy", maCongTy.Trim() },
                    { "@TenCongTy", tenCongTy.Trim() },
                    { "@TenVietTat", string.IsNullOrEmpty(tenVietTat) ? DBNull.Value : (object)tenVietTat.Trim() },
                    { "@MaSoThue", string.IsNullOrEmpty(maSoThue) ? DBNull.Value : (object)maSoThue.Trim() },
                    { "@DiaChi", string.IsNullOrEmpty(diaChi) ? DBNull.Value : (object)diaChi.Trim() },
                    { "@TrangThai", trangThai },
                    { "@MoTa", string.IsNullOrEmpty(moTa) ? DBNull.Value : (object)moTa.Trim() }
                };

                db.ExecuteDatasetStoredProcedure("sp_chinh_DMCongTy_Save", pars);
                return new { success = true, message = congTyId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
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
                var pars = new Dictionary<string, object> { { "@CongTyID", id } };

                db.ExecuteDatasetStoredProcedure("sp_chinh_DMCongTy_Delete", pars);
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