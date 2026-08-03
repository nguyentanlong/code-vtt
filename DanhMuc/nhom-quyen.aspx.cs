using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class nhom_quyen : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(nhom_quyen));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Lấy CongTyID của người dùng hiện tại từ Session (không cho phép client tự truyền lên)
        /// </summary>
        private static long GetCurrentCongTyID()
        {
            var val = System.Web.HttpContext.Current.Session["CongTyID"];
            if (val == null) return 0;
            return Convert.ToInt64(val);
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, string trangThai)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
            {
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };
            }

            log.Info($"GetList called with congTyId: {congTyId}, keyword: {keyword}, trangThai: {trangThai}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMNhomQuyen_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        NhomQuyenID = dr["NhomQuyenID"],
                        CongTyID = dr["CongTyID"],
                        MaNhomQuyen = dr["MaNhomQuyen"].ToString(),
                        TenNhomQuyen = dr["TenNhomQuyen"].ToString(),
                        ThuTu = dr["ThuTu"],
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
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

        [WebMethod(EnableSession = true)]
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
                var pars = new Dictionary<string, object> { { "@NhomQuyenID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMNhomQuyen_GetById", pars);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        NhomQuyenID = dr["NhomQuyenID"],
                        CongTyID = dr["CongTyID"],
                        MaNhomQuyen = dr["MaNhomQuyen"].ToString(),
                        TenNhomQuyen = dr["TenNhomQuyen"].ToString(),
                        ThuTu = dr["ThuTu"],
                        TrangThai = dr["TrangThai"]
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

        [WebMethod(EnableSession = true)]
        public static object SaveData(long nhomQuyenId, string maNhomQuyen, string tenNhomQuyen, int thuTu, byte trangThai)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
            {
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };
            }

            log.Info($"SaveData called with nhomQuyenId: {nhomQuyenId}, congTyId: {congTyId}, maNhomQuyen: {maNhomQuyen}, tenNhomQuyen: {tenNhomQuyen}, thuTu: {thuTu}, trangThai: {trangThai}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@NhomQuyenID", nhomQuyenId },
                    { "@CongTyID", congTyId },
                    { "@MaNhomQuyen", maNhomQuyen.Trim() },
                    { "@TenNhomQuyen", tenNhomQuyen.Trim() },
                    { "@ThuTu", thuTu },
                    { "@TrangThai", trangThai }
                };

                db.ExecuteDatasetStoredProcedure("sp_long_DMNhomQuyen_Save", pars);
                return new { success = true, message = nhomQuyenId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
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
                var pars = new Dictionary<string, object> { { "@NhomQuyenID", id } };

                db.ExecuteDatasetStoredProcedure("sp_long_DMNhomQuyen_Delete", pars);
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