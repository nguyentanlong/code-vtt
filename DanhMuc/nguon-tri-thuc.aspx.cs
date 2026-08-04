using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class nguon_tri_thuc : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(nguon_tri_thuc));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private static long GetCurrentCongTyID()
        {
            var val = System.Web.HttpContext.Current.Session["CongTyID"];
            if (val == null) return 0;
            return Convert.ToInt64(val);
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword)
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

            log.Info($"GetList called with congTyId: {congTyId}, keyword: {keyword}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMNguonTriThuc_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        NguonTriThucID = dr["NguonTriThucID"],
                        MaNguon = dr["MaNguon"].ToString(),
                        TenNguon = dr["TenNguon"].ToString(),
                        DoUuTien = dr["DoUuTien"]
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
                var pars = new Dictionary<string, object> { { "@NguonTriThucID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMNguonTriThuc_GetById", pars);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        NguonTriThucID = dr["NguonTriThucID"],
                        MaNguon = dr["MaNguon"].ToString(),
                        TenNguon = dr["TenNguon"].ToString(),
                        DoUuTien = dr["DoUuTien"]
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
        public static object SaveData(long nguonTriThucId, string maNguon, string tenNguon, int doUuTien)
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

            log.Info($"SaveData called with nguonTriThucId: {nguonTriThucId}, congTyId: {congTyId}, maNguon: {maNguon}, tenNguon: {tenNguon}, doUuTien: {doUuTien}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@NguonTriThucID", nguonTriThucId },
                    { "@CongTyID", congTyId },
                    { "@MaNguon", maNguon.Trim() },
                    { "@TenNguon", tenNguon.Trim() },
                    { "@DoUuTien", doUuTien }
                };

                db.ExecuteDatasetStoredProcedure("sp_long_DMNguonTriThuc_Save", pars);
                return new { success = true, message = nguonTriThucId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
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
                var pars = new Dictionary<string, object> { { "@NguonTriThucID", id } };

                db.ExecuteDatasetStoredProcedure("sp_long_DMNguonTriThuc_Delete", pars);
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