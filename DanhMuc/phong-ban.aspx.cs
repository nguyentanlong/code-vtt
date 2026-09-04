using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class phong_ban : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(phong_ban));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            bool canThem = CheckPermission("PHONGBAN", "THEM", GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            string myScope = GetPermissionScope("PHONGBAN", "XEM");

            return new { success = true, data = new { canThem = canThem, scope = myScope } };
        }

        [WebMethod(EnableSession = true)]
        public static object GetChiNhanhOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetChiNhanhOptions", new Dictionary<string, object>());

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { ChiNhanhID = dr["ChiNhanhID"], TenChiNhanh = dr["TenChiNhanh"].ToString() });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetChiNhanhOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetChaOptions(long excludeId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@ExcludeID", excludeId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetChaOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string tenHienThi = new string(' ', (Convert.ToInt32(dr["CapDo"]) - 1) * 3) + dr["TenPhongBan"].ToString();
                    list.Add(new { PhongBanID = dr["PhongBanID"], TenPhongBan = tenHienThi });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetChaOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, object chiNhanhId, string trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string myScope = GetPermissionScope("PHONGBAN", "XEM");
            if (myScope == null)
                return new { success = false, message = "Bạn không có quyền xem Cây tổ chức!" };

            object effectiveChiNhanhId = chiNhanhId;
            object effectivePhongBanId = null;

            if (myScope == "PHONGBAN")
            {
                effectivePhongBanId = GetCurrentPhongBanId();
                effectiveChiNhanhId = null;
            }
            else if (myScope == "CHINHANH")
            {
                effectiveChiNhanhId = GetCurrentChiNhanhId();
            }

            string scopeSua = GetPermissionScope("PHONGBAN", "SUA");
            string scopeXoa = GetPermissionScope("PHONGBAN", "XOA");

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@ChiNhanhID", effectiveChiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(effectiveChiNhanhId) },
                    { "@PhongBanID", effectivePhongBanId == null ? DBNull.Value : (object)Convert.ToInt32(effectivePhongBanId) },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    long rowPhongBanId = Convert.ToInt64(dr["PhongBanID"]);
                    long rowChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                    int capDo = Convert.ToInt32(dr["CapDo"]);

                    bool canEditRow = EvaluateScope(scopeSua, rowPhongBanId, rowChiNhanhId);
                    bool canDeleteRow = EvaluateScope(scopeXoa, rowPhongBanId, rowChiNhanhId);

                    list.Add(new
                    {
                        PhongBanID = rowPhongBanId,
                        MaPhongBan = dr["MaPhongBan"].ToString(),
                        TenPhongBan = new string(' ', (capDo - 1) * 3) + (capDo > 1 ? "↳ " : "") + dr["TenPhongBan"].ToString(),
                        TenChiNhanh = dr["TenChiNhanh"] == DBNull.Value ? "Trực thuộc Tổng công ty" : dr["TenChiNhanh"].ToString(),
                        CapDo = capDo,
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
                        CanEditRow = canEditRow,
                        CanDeleteRow = canDeleteRow
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

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@PhongBanID", id } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetById", pars);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        PhongBanID = dr["PhongBanID"],
                        ChiNhanhID = dr["ChiNhanhID"] == DBNull.Value ? (object)null : dr["ChiNhanhID"],
                        PhongBanChaID = dr["PhongBanChaID"] == DBNull.Value ? (object)null : dr["PhongBanChaID"],
                        MaPhongBan = dr["MaPhongBan"].ToString(),
                        TenPhongBan = dr["TenPhongBan"].ToString(),
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
        public static object SaveData(long phongBanId, object chiNhanhId, object phongBanChaId, string maPhongBan, string tenPhongBan, int thuTu, byte trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyId();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            long targetChiNhanhId = chiNhanhId == null ? 0 : Convert.ToInt64(chiNhanhId);
            string maChucNang = phongBanId == 0 ? "THEM" : "SUA";

            // Với Sửa, target phải theo đúng phòng ban đang sửa; với Thêm mới, target theo Chi nhánh vừa chọn
            long targetPhongBanId = phongBanId == 0 ? 0 : phongBanId;
            if (!CheckPermission("PHONGBAN", maChucNang, targetPhongBanId != 0 ? targetPhongBanId : GetCurrentPhongBanId(), targetChiNhanhId))
            {
                string tenChucNang = phongBanId == 0 ? "thêm mới" : "sửa";
                return new { success = false, message = $"Bạn không có quyền {tenChucNang} đơn vị tổ chức này!" };
            }

            log.Info($"SaveData called with phongBanId: {phongBanId}, maPhongBan: {maPhongBan}, tenPhongBan: {tenPhongBan}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@PhongBanID", phongBanId },
                    { "@CongTyID", congTyId },
                    { "@ChiNhanhID", chiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(chiNhanhId) },
                    { "@PhongBanChaID", phongBanChaId == null ? DBNull.Value : (object)Convert.ToInt32(phongBanChaId) },
                    { "@MaPhongBan", maPhongBan.Trim() },
                    { "@TenPhongBan", tenPhongBan.Trim() },
                    { "@ThuTu", thuTu },
                    { "@TrangThai", trangThai }
                };

                db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_Save", pars);
                return new { success = true, message = phongBanId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
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

            try
            {
                ConnectServer db = new ConnectServer();
                var lookupPars = new Dictionary<string, object> { { "@PhongBanID", id } };
                DataSet dsLookup = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetChiNhanhID", lookupPars);

                long targetChiNhanhId = 0;
                if (dsLookup.Tables.Count > 0 && dsLookup.Tables[0].Rows.Count > 0 && dsLookup.Tables[0].Rows[0]["ChiNhanhID"] != DBNull.Value)
                {
                    targetChiNhanhId = Convert.ToInt64(dsLookup.Tables[0].Rows[0]["ChiNhanhID"]);
                }

                if (!CheckPermission("PHONGBAN", "XOA", id, targetChiNhanhId))
                    return new { success = false, message = "Bạn không có quyền xóa đơn vị tổ chức này!" };

                var pars = new Dictionary<string, object> { { "@PhongBanID", id } };
                db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_Delete", pars);

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