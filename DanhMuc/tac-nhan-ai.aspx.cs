using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class tac_nhan_ai : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(tac_nhan_ai));

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
        public static object GetPhongBanOptions()
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

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", congTyId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_AIAgent_GetPhongBanOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { PhongBanID = dr["PhongBanID"], TenPhongBan = dr["TenPhongBan"].ToString() });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetPhongBanOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetModelOptions()
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_AIAgent_GetModelOptions", new Dictionary<string, object>());

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new
                    {
                        ModelID = dr["ModelID"],
                        TenModel = dr["TenModel"].ToString(),
                        Provider = dr["Provider"].ToString()
                    });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetModelOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
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

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_AIAgent_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        AIAgentID = dr["AIAgentID"],
                        PhongBanID = dr["PhongBanID"],
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        MaAgent = dr["MaAgent"].ToString(),
                        TenAgent = dr["TenAgent"].ToString(),
                        ModelMacDinhID = dr["ModelMacDinhID"] == DBNull.Value ? (object)null : dr["ModelMacDinhID"],
                        TenModel = dr["TenModel"] == DBNull.Value ? "" : dr["TenModel"].ToString(),
                        TrangThai = Convert.ToByte(dr["TrangThai"])
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
                var pars = new Dictionary<string, object> { { "@AIAgentID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_AIAgent_GetById", pars);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        AIAgentID = dr["AIAgentID"],
                        PhongBanID = dr["PhongBanID"],
                        MaAgent = dr["MaAgent"].ToString(),
                        TenAgent = dr["TenAgent"].ToString(),
                        ModelMacDinhID = dr["ModelMacDinhID"] == DBNull.Value ? (object)null : dr["ModelMacDinhID"],
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
        public static object SaveData(long aiAgentId, long phongBanId, string maAgent, string tenAgent, object modelMacDinhId, byte trangThai)
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

            if (phongBanId <= 0)
            {
                return new { success = false, message = "Vui lòng chọn Phòng ban!" };
            }

            log.Info($"SaveData called with aiAgentId: {aiAgentId}, congTyId: {congTyId}, phongBanId: {phongBanId}, maAgent: {maAgent}, tenAgent: {tenAgent}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@AIAgentID", aiAgentId },
                    { "@CongTyID", congTyId },
                    { "@PhongBanID", phongBanId },
                    { "@MaAgent", maAgent.Trim() },
                    { "@TenAgent", tenAgent.Trim() },
                    { "@ModelMacDinhID", modelMacDinhId == null ? DBNull.Value : (object)Convert.ToInt64(modelMacDinhId) },
                    { "@TrangThai", trangThai }
                };

                db.ExecuteDatasetStoredProcedure("sp_long_AIAgent_Save", pars);
                return new { success = true, message = aiAgentId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
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
                var pars = new Dictionary<string, object> { { "@AIAgentID", id } };

                db.ExecuteDatasetStoredProcedure("sp_long_AIAgent_Delete", pars);
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