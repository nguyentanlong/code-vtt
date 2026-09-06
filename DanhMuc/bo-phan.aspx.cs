using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class bo_phan : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(bo_phan));

        protected void Page_Load(object sender, EventArgs e) { }

        [WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            bool canThem = CheckPermission(Trang.BP, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            string scope = GetPermissionScope(Trang.BP, ChucNang.R);
            return new { success = true, data = new { canThem = canThem, scope = scope, myChiNhanhId = GetCurrentChiNhanhId() } };
        }

        [WebMethod(EnableSession = true)]
        public static object GetChiNhanhOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string themScope = GetPermissionScope(Trang.BP, ChucNang.I);
            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetChiNhanhOptions", new Dictionary<string, object>());

            List<object> list = new List<object>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                long id = Convert.ToInt64(dr["ChiNhanhID"]);
                if (themScope == "CONGTY" || id == GetCurrentChiNhanhId())
                    list.Add(new { ChiNhanhID = id, TenChiNhanh = dr["TenChiNhanh"].ToString() });
            }
            return new { success = true, data = list };
        }

        [WebMethod(EnableSession = true)]
        public static object GetPhongBanChaOptions(object chiNhanhId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string themScope = GetPermissionScope(Trang.BP, ChucNang.I);

            try
            {
                ConnectServer db = new ConnectServer();

                if (themScope == "PHONGBAN")
                {
                    // Trưởng phòng: chỉ được tạo Bộ phận thuộc đúng phòng mình
                    long myPb = GetCurrentPhongBanId();
                    DataSet dsSelf = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetById", new Dictionary<string, object> { { "@PhongBanID", myPb } });
                    var list0 = new List<object>();
                    if (dsSelf.Tables.Count > 0 && dsSelf.Tables[0].Rows.Count > 0)
                        list0.Add(new { PhongBanID = myPb, TenPhongBan = dsSelf.Tables[0].Rows[0]["TenPhongBan"].ToString() });
                    return new { success = true, data = list0 };
                }

                var pars = new Dictionary<string, object>();
                if (themScope == "CHINHANH")
                {
                    pars["@ChiNhanhID"] = GetCurrentChiNhanhId();
                    pars["@ChiNhanhFilterMode"] = true;
                }
                else
                {
                    pars["@ChiNhanhID"] = chiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(chiNhanhId);
                    pars["@ChiNhanhFilterMode"] = chiNhanhId != null;
                }

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_BoPhan_GetPhongBanChaOptions", pars);
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new { PhongBanID = dr["PhongBanID"], TenPhongBan = dr["TenPhongBan"].ToString() });
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetPhongBanChaOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, object chiNhanhId, string trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string myScope = GetPermissionScope(Trang.BP, ChucNang.R);
            if (myScope == null)
                return new { success = false, message = "Bạn không có quyền xem Danh mục Bộ phận!" };

            object effChiNhanhId = chiNhanhId;
            object effPhongBanId = null;

            if (myScope == "PHONGBAN")
            {
                effPhongBanId = GetCurrentPhongBanId();
                effChiNhanhId = null;
            }
            else if (myScope == "CHINHANH")
            {
                effChiNhanhId = GetCurrentChiNhanhId();
            }

            string scopeSua = GetPermissionScope(Trang.BP, ChucNang.U);
            string scopeXoa = GetPermissionScope(Trang.BP, ChucNang.D);

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@ChiNhanhID", effChiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(effChiNhanhId) },
                    { "@PhongBanID", effPhongBanId == null ? DBNull.Value : (object)Convert.ToInt32(effPhongBanId) },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_BoPhan_GetList", pars);
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    long rowPb = Convert.ToInt64(dr["PhongBanID"]);
                    long rowCn = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);

                    list.Add(new
                    {
                        PhongBanID = rowPb,
                        MaPhongBan = dr["MaPhongBan"].ToString(),
                        TenPhongBan = dr["TenPhongBan"].ToString(),
                        TenPhongBanCha = dr["TenPhongBanCha"] == DBNull.Value ? "" : dr["TenPhongBanCha"].ToString(),
                        TenChiNhanh = dr["TenChiNhanh"] == DBNull.Value ? "Trực thuộc Tổng công ty" : dr["TenChiNhanh"].ToString(),
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
                        CanEditRow = EvaluateScope(scopeSua, rowPb, rowCn),
                        CanDeleteRow = EvaluateScope(scopeXoa, rowPb, rowCn)
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
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_BoPhan_GetById", pars);

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
        public static object SaveData(long phongBanId, object chiNhanhId, long phongBanChaId, string maPhongBan, string tenPhongBan, int thuTu, byte trangThai, string lyDo)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyId();
            if (phongBanChaId <= 0)
                return new { success = false, message = "Vui lòng chọn Phòng ban cha!" };

            string maChucNang = phongBanId == 0 ? ChucNang.I : ChucNang.U;
            string dataScope = GetPermissionScope(Trang.BP, maChucNang);

            if (!EvaluateScope(dataScope, phongBanChaId, chiNhanhId == null ? 0 : Convert.ToInt64(chiNhanhId)))
            {
                string tenCN = phongBanId == 0 ? "thêm mới" : "sửa";
                return new { success = false, message = $"Bạn không có quyền {tenCN} Bộ phận này!" };
            }

            // Admin (CONGTY) không cần Lý do; CN_ADMIN/TRUONGPHONG (CHINHANH/PHONGBAN) bắt buộc Lý do
            if (dataScope != "CONGTY" && string.IsNullOrWhiteSpace(lyDo))
            {
                return new { success = false, message = "Vui lòng nhập Lý do thay đổi Bộ phận!", requireReason = true };
            }

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@PhongBanID", phongBanId },
                    { "@CongTyID", congTyId },
                    { "@ChiNhanhID", chiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(chiNhanhId) },
                    { "@PhongBanChaID", phongBanChaId },
                    { "@MaPhongBan", maPhongBan.Trim() },
                    { "@TenPhongBan", tenPhongBan.Trim() },
                    { "@ThuTu", thuTu },
                    { "@TrangThai", trangThai }
                };
                db.ExecuteDatasetStoredProcedure("sp_v2_BoPhan_Save", pars);

                if (dataScope != "CONGTY")
                {
                    var logPars = new Dictionary<string, object>
                    {
                        { "@TenBang", "PhongBan" },
                        { "@KhoaChinh", phongBanId == 0 ? 0 : phongBanId },
                        { "@HanhDong", maChucNang == ChucNang.I ? "THEM" : "SUA" },
                        { "@NguoiThucHienID", GetCurrentUserId() },
                        { "@LyDo", lyDo }
                    };
                    db.ExecuteDatasetStoredProcedure("sp_v2_GhiLyDoThayDoi", logPars);
                }

                return new { success = true, message = phongBanId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteData(long id, string lyDo)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var lookupPars = new Dictionary<string, object> { { "@PhongBanID", id } };
                DataSet dsLookup = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetChiNhanhID", lookupPars);

                long targetCn = 0;
                if (dsLookup.Tables.Count > 0 && dsLookup.Tables[0].Rows.Count > 0 && dsLookup.Tables[0].Rows[0]["ChiNhanhID"] != DBNull.Value)
                    targetCn = Convert.ToInt64(dsLookup.Tables[0].Rows[0]["ChiNhanhID"]);

                string dataScope = GetPermissionScope(Trang.BP, ChucNang.D);
                if (!EvaluateScope(dataScope, id, targetCn))
                    return new { success = false, message = "Bạn không có quyền xóa Bộ phận này!" };

                if (dataScope != "CONGTY" && string.IsNullOrWhiteSpace(lyDo))
                    return new { success = false, message = "Vui lòng nhập Lý do xóa Bộ phận!", requireReason = true };

                var pars = new Dictionary<string, object> { { "@PhongBanID", id } };
                db.ExecuteDatasetStoredProcedure("sp_v2_BoPhan_Delete", pars);

                if (dataScope != "CONGTY")
                {
                    var logPars = new Dictionary<string, object>
                    {
                        { "@TenBang", "PhongBan" }, { "@KhoaChinh", id }, { "@HanhDong", "XOA" },
                        { "@NguoiThucHienID", GetCurrentUserId() }, { "@LyDo", lyDo }
                    };
                    db.ExecuteDatasetStoredProcedure("sp_v2_GhiLyDoThayDoi", logPars);
                }

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