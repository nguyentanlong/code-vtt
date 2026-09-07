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

            bool canThem = CheckPermission(Trang.PB, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            string myScope = GetPermissionScope(Trang.PB, ChucNang.R);

            return new { success = true, data = new { canThem = canThem, scope = myScope, myChiNhanhId = GetCurrentChiNhanhId() } };
        }

        [WebMethod(EnableSession = true)]
        public static object GetChiNhanhOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string themScope = GetPermissionScope(Trang.PB, ChucNang.I);
            try
            {
                List<object> list = new List<object>();
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetChiNhanhOptions", new Dictionary<string, object>());

                if (themScope == "CONGTY")
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        list.Add(new { ChiNhanhID = dr["ChiNhanhID"], TenChiNhanh = dr["TenChiNhanh"].ToString() });
                }
                else
                {
                    long myChiNhanhId = GetCurrentChiNhanhId();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (Convert.ToInt64(dr["ChiNhanhID"]) == myChiNhanhId)
                            list.Add(new { ChiNhanhID = dr["ChiNhanhID"], TenChiNhanh = dr["TenChiNhanh"].ToString() });
                    }
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

            string themScope = GetPermissionScope(Trang.PB, ChucNang.I);

            try
            {
                ConnectServer db = new ConnectServer();

                if (themScope == "PHONGBAN")
                {
                    // Trưởng phòng: chỉ được chọn đúng Phòng ban của mình làm Đơn vị cha
                    long myPb = GetCurrentPhongBanId();
                    var list0 = new List<object>();
                    if (myPb != excludeId && myPb != 0)
                    {
                        DataSet dsSelf = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetById", new Dictionary<string, object> { { "@PhongBanID", myPb } });
                        if (dsSelf.Tables.Count > 0 && dsSelf.Tables[0].Rows.Count > 0)
                        {
                            list0.Add(new { PhongBanID = myPb, TenPhongBan = dsSelf.Tables[0].Rows[0]["TenPhongBan"].ToString() });
                        }
                    }
                    return new { success = true, data = list0 };
                }

                var pars = new Dictionary<string, object> { { "@ExcludeID", excludeId } };

                if (themScope == "CHINHANH")
                {
                    pars["@ChiNhanhID"] = GetCurrentChiNhanhId();
                    pars["@ChiNhanhFilterMode"] = true;
                }
                else
                {
                    pars["@ChiNhanhID"] = DBNull.Value;
                    pars["@ChiNhanhFilterMode"] = false;
                }

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

            string myScope = GetPermissionScope(Trang.PB, ChucNang.R);
            if (myScope == null)
                return new { success = false, message = "Bạn không có quyền xem Cây tổ chức!" };

            object effectiveChiNhanhId = chiNhanhId;
            object effectivePhongBanId = null;

            if (myScope == Trang.PB)
            {
                effectivePhongBanId = GetCurrentPhongBanId();
                effectiveChiNhanhId = null;
            }
            else if (myScope == "CHINHANH")
            {
                effectiveChiNhanhId = GetCurrentChiNhanhId();
            }

            string scopeSua = GetPermissionScope(Trang.PB, ChucNang.U);
            string scopeXoa = GetPermissionScope(Trang.PB, ChucNang.D);

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

                    // Nếu là Bộ phận (CapDo > 1), phạm vi thật sự để so khớp là Phòng ban CHA, không phải chính nó
                    long effectiveTargetPhongBanId = rowPhongBanId;
                    if (capDo > 1 && dr["PhongBanChaID"] != DBNull.Value)
                    {
                        effectiveTargetPhongBanId = Convert.ToInt64(dr["PhongBanChaID"]);
                    }

                    bool canEditRow = EvaluateScope(scopeSua, effectiveTargetPhongBanId, rowChiNhanhId);
                    bool canDeleteRow = EvaluateScope(scopeXoa, effectiveTargetPhongBanId, rowChiNhanhId);

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
        public static object SaveData(long phongBanId, object chiNhanhId, object phongBanChaId, string maPhongBan, string tenPhongBan, int thuTu, byte trangThai, string lyDo)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                long congTyId = GetCurrentCongTyId();
                if (congTyId == 0)
                    return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

                long targetChiNhanhId = chiNhanhId == null ? 0 : Convert.ToInt64(chiNhanhId);
                string maChucNang = phongBanId == 0 ? ChucNang.I : ChucNang.U;

                // Xác định đúng mục tiêu để so khớp phạm vi quyền:
                // - Thêm mới (phongBanId=0): mục tiêu là PhongBanChaID vừa chọn trong form
                // - Sửa (phongBanId!=0): phải TRA LẠI xem bản ghi đang sửa có PhongBanChaID thật sự là gì
                //   (nếu là Bộ phận/Tổ, mục tiêu là Phòng ban CHA của nó, không phải chính nó)
                long targetPhongBanId;
                if (phongBanId == 0)
                {
                    targetPhongBanId = phongBanChaId == null ? 0 : Convert.ToInt64(phongBanChaId);
                }
                else
                {
                    var scopeInfoPars = new Dictionary<string, object> { { "@PhongBanID", phongBanId } };
                    ConnectServer dbLookup = new ConnectServer();
                    DataSet dsScope = dbLookup.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetScopeInfo", scopeInfoPars);

                    targetPhongBanId = phongBanId; // mặc định: chính nó (đúng cho Phòng ban cấp cao nhất, không có cha)
                    if (dsScope.Tables.Count > 0 && dsScope.Tables[0].Rows.Count > 0)
                    {
                        DataRow drScope = dsScope.Tables[0].Rows[0];
                        if (drScope["PhongBanChaID"] != DBNull.Value)
                        {
                            targetPhongBanId = Convert.ToInt64(drScope["PhongBanChaID"]);
                        }
                        if (drScope["ChiNhanhID"] != DBNull.Value)
                        {
                            targetChiNhanhId = Convert.ToInt64(drScope["ChiNhanhID"]);
                        }
                    }
                }

                string dataScope = GetPermissionScope(Trang.PB, maChucNang);
                if (!EvaluateScope(dataScope, targetPhongBanId, targetChiNhanhId))
                {
                    string tenChucNang = phongBanId == 0 ? "thêm mới" : "sửa";
                    return new { success = false, message = $"Bạn không có quyền {tenChucNang} đơn vị tổ chức này!" };
                }

                if (dataScope != "CONGTY" && string.IsNullOrWhiteSpace(lyDo))
                {
                    return new { success = false, message = "Vui lòng nhập Lý do thay đổi Phòng ban!", requireReason = true };
                }

                log.Info($"SaveData called with phongBanId: {phongBanId}, maPhongBan: {maPhongBan}, tenPhongBan: {tenPhongBan}");

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
                DataSet dsLookup = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetScopeInfo", lookupPars);

                long targetChiNhanhId = 0;
                long effectiveTargetPhongBanId = id;
                if (dsLookup.Tables.Count > 0 && dsLookup.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsLookup.Tables[0].Rows[0];
                    targetChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                    int capDo = Convert.ToInt32(dr["CapDo"]);
                    if (capDo > 1 && dr["PhongBanChaID"] != DBNull.Value)
                        effectiveTargetPhongBanId = Convert.ToInt64(dr["PhongBanChaID"]);
                }

                string dataScope = GetPermissionScope(Trang.PB, ChucNang.D);
                if (!EvaluateScope(dataScope, effectiveTargetPhongBanId, targetChiNhanhId))
                    return new { success = false, message = "Bạn không có quyền xóa đơn vị tổ chức này!" };

                if (dataScope != "CONGTY" && string.IsNullOrWhiteSpace(lyDo))
                    return new { success = false, message = "Vui lòng nhập Lý do xóa Phòng ban!", requireReason = true };

                var pars = new Dictionary<string, object> { { "@PhongBanID", id } };
                db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_Delete", pars);

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