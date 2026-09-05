using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class giai_doan : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(giai_doan));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            bool canEdit = CheckPermission(Trang.GD, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            return new { success = true, data = new { canEdit = canEdit } };
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string myScope = GetPermissionScope(Trang.GD, ChucNang.R);
            if (myScope == null)
                return new { success = false, message = "Bạn không có quyền xem Danh mục Giai đoạn!" };

            log.Info($"GetList called with keyword: {keyword}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_GiaiDoan_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        TimeLineID = dr["TimeLineID"],
                        MaGiaiDoan = dr["MaGiaiDoan"].ToString(),
                        TenGiaiDoan = dr["TenGiaiDoan"].ToString(),
                        ThuTuHienThi = dr["ThuTuHienThi"],
                        MoTa = dr["MoTa"] == DBNull.Value ? "" : dr["MoTa"].ToString()
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
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            log.Info($"GetById called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TimeLineID", id } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_GiaiDoan_GetById", pars);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        TimeLineID = dr["TimeLineID"],
                        MaGiaiDoan = dr["MaGiaiDoan"].ToString(),
                        TenGiaiDoan = dr["TenGiaiDoan"].ToString(),
                        ThuTuHienThi = dr["ThuTuHienThi"],
                        MoTa = dr["MoTa"] == DBNull.Value ? "" : dr["MoTa"].ToString()
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
        public static object SaveData(long timeLineId, string maGiaiDoan, string tenGiaiDoan, int thuTuHienThi, string moTa)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string maChucNang = timeLineId == 0 ? ChucNang.I : ChucNang.U;
            if (!CheckPermission(Trang.GD, maChucNang, GetCurrentPhongBanId(), GetCurrentChiNhanhId()))
            {
                return new { success = false, message = "Bạn không có quyền thêm/sửa Danh mục Giai đoạn! Chỉ Admin được phép." };
            }

            log.Info($"SaveData called with timeLineId: {timeLineId}, maGiaiDoan: {maGiaiDoan}, tenGiaiDoan: {tenGiaiDoan}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@TimeLineID", timeLineId },
                    { "@MaGiaiDoan", maGiaiDoan.Trim() },
                    { "@TenGiaiDoan", tenGiaiDoan.Trim() },
                    { "@ThuTuHienThi", thuTuHienThi },
                    { "@MoTa", string.IsNullOrEmpty(moTa) ? DBNull.Value : (object)moTa.Trim() }
                };

                db.ExecuteDatasetStoredProcedure("sp_v2_GiaiDoan_Save", pars);
                return new { success = true, message = timeLineId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
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
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (!CheckPermission(Trang.GD, ChucNang.D, GetCurrentPhongBanId(), GetCurrentChiNhanhId()))
            {
                return new { success = false, message = "Bạn không có quyền xóa Danh mục Giai đoạn! Chỉ Admin được phép." };
            }

            log.Info($"DeleteData called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TimeLineID", id } };
                db.ExecuteDatasetStoredProcedure("sp_v2_GiaiDoan_Delete", pars);
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