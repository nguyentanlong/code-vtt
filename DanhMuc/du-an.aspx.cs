using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class du_an : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(du_an));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private static long GetChiNhanhIdOfPhongBan(long phongBanId)
        {
            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@PhongBanID", phongBanId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetChiNhanhID", pars);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["ChiNhanhID"] != DBNull.Value)
                return Convert.ToInt64(ds.Tables[0].Rows[0]["ChiNhanhID"]);
            return 0;
        }

        [WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            bool canThem = CheckPermission(Trang.DA, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            string myScope = GetPermissionScope(Trang.DA, ChucNang.R);

            return new { success = true, data = new { canThem = canThem, scope = myScope, myChiNhanhId = GetCurrentChiNhanhId() } };
        }

        [WebMethod(EnableSession = true)]
        public static object GetChiNhanhOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetChiNhanhOptions", new Dictionary<string, object>());

            List<object> list = new List<object>();
            foreach (DataRow dr in ds.Tables[0].Rows)
                list.Add(new { ChiNhanhID = dr["ChiNhanhID"], TenChiNhanh = dr["TenChiNhanh"].ToString() });

            return new { success = true, data = list };
        }

        [WebMethod(EnableSession = true)]
        public static object GetPhongBanOptions(object chiNhanhId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@ChiNhanhID", chiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(chiNhanhId) } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetPhongBanOptions", pars);

            List<object> list = new List<object>();
            foreach (DataRow dr in ds.Tables[0].Rows)
                list.Add(new { PhongBanID = dr["PhongBanID"], TenPhongBan = dr["TenPhongBan"].ToString() });

            return new { success = true, data = list };
        }

        [WebMethod(EnableSession = true)]
        public static object GetLoaiCongTrinhOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetLoaiCongTrinhOptions", new Dictionary<string, object>());

            List<object> list = new List<object>();
            foreach (DataRow dr in ds.Tables[0].Rows)
                list.Add(new { LoaiCongTrinhID = dr["LoaiCongTrinhID"], TenLoai = dr["TenLoai"].ToString() });

            return new { success = true, data = list };
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, object chiNhanhId, object phongBanId, string trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string myScope = GetPermissionScope(Trang.DA, ChucNang.R);
            if (myScope == null)
                return new { success = false, message = "Bạn không có quyền xem Danh sách Dự án!" };

            object effectiveChiNhanhId = chiNhanhId;
            object effectivePhongBanId = phongBanId;

            if (myScope == "PHONGBAN")
            {
                effectivePhongBanId = GetCurrentPhongBanId();
                effectiveChiNhanhId = null;
            }
            else if (myScope == "CHINHANH")
            {
                effectiveChiNhanhId = GetCurrentChiNhanhId();
            }

            string scopeSua = GetPermissionScope(Trang.DA, ChucNang.U);
            string scopeXoa = GetPermissionScope(Trang.DA, ChucNang.D);

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

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetList", pars);
                DataTable dt = ds.Tables[0];

                long myUserId = GetCurrentUserId();

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    long rowPhongBanId = Convert.ToInt64(dr["PhongBanID"]);
                    long rowChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                    long? rowNguoiTaoId = dr["NguoiTaoID"] == DBNull.Value ? (long?)null : Convert.ToInt64(dr["NguoiTaoID"]);

                    bool canEditRow = EvaluateScope(scopeSua, rowPhongBanId, rowChiNhanhId, rowNguoiTaoId);
                    bool canDeleteRow = EvaluateScope(scopeXoa, rowPhongBanId, rowChiNhanhId, rowNguoiTaoId);

                    list.Add(new
                    {
                        DuAnID = dr["DuAnID"],
                        MaDuAn = dr["MaDuAn"].ToString(),
                        TenDuAn = dr["TenDuAn"].ToString(),
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        TienDo = dr["TienDo"],
                        TrangThaiDuAn = Convert.ToByte(dr["TrangThaiDuAn"]),
                        LaNguoiTao = rowNguoiTaoId.HasValue && rowNguoiTaoId.Value == myUserId,
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
                var pars = new Dictionary<string, object> { { "@DuAnID", id } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetById", pars);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        DuAnID = dr["DuAnID"],
                        PhongBanID = dr["PhongBanID"],
                        ChiNhanhID = dr["ChiNhanhID"] == DBNull.Value ? (object)null : dr["ChiNhanhID"],
                        LoaiCongTrinhID = dr["LoaiCongTrinhID"] == DBNull.Value ? (object)null : dr["LoaiCongTrinhID"],
                        MaDuAn = dr["MaDuAn"].ToString(),
                        TenDuAn = dr["TenDuAn"].ToString(),
                        ChuDauTu = dr["ChuDauTu"] == DBNull.Value ? "" : dr["ChuDauTu"].ToString(),
                        DiaDiem = dr["DiaDiem"] == DBNull.Value ? "" : dr["DiaDiem"].ToString(),
                        MoTa = dr["MoTa"] == DBNull.Value ? "" : dr["MoTa"].ToString(),
                        NgayBatDau = dr["NgayBatDau"] == DBNull.Value ? null : (DateTime?)dr["NgayBatDau"],
                        NgayKetThucDuKien = dr["NgayKetThucDuKien"] == DBNull.Value ? null : (DateTime?)dr["NgayKetThucDuKien"],
                        TrangThaiDuAn = dr["TrangThaiDuAn"]
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

        private static void GhiLyDoNeuCan(string dataScope, long? nguoiTaoId, string hanhDong, long khoaChinh, string lyDo)
        {
            // Chỉ bắt buộc lý do khi phạm vi là PHONGBAN (Trưởng phòng/Phó phòng) VÀ không phải người tạo
            // Admin (CONGTY)/CN_Admin (CHINHANH)/chính chủ (TU_TAO hoặc đúng NguoiTaoID) không cần lý do
            bool khongPhaiNguoiTao = !nguoiTaoId.HasValue || nguoiTaoId.Value != GetCurrentUserId();

            if (dataScope == "PHONGBAN" && khongPhaiNguoiTao && !string.IsNullOrEmpty(lyDo))
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@TenBang", Trang.DA },
                    { "@KhoaChinh", khoaChinh },
                    { "@HanhDong", hanhDong },
                    { "@NguoiThucHienID", GetCurrentUserId() },
                    { "@LyDo", lyDo }
                };
                db.ExecuteDatasetStoredProcedure("sp_v2_GhiLyDoThayDoi", pars);
            }
        }

        [WebMethod(EnableSession = true)]
        public static object SaveData(long duAnId, long phongBanId, object loaiCongTrinhId, string maDuAn, string tenDuAn,
                                    string chuDauTu, string diaDiem, string moTa, string ngayBatDau, string ngayKetThuc, byte trangThaiDuAn, string lyDo)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyId();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            if (phongBanId <= 0)
                return new { success = false, message = "Vui lòng chọn Phòng ban!" };

            long targetChiNhanhId = GetChiNhanhIdOfPhongBan(phongBanId);
            long? nguoiTaoId = null;
            string maChucNang;
            string dataScope = null;

            if (duAnId == 0)
            {
                maChucNang = ChucNang.I;
            }
            else
            {
                maChucNang = ChucNang.U;
                ConnectServer dbLookup = new ConnectServer();
                var lookupPars = new Dictionary<string, object> { { "@DuAnID", duAnId } };
                DataSet dsLookup = dbLookup.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetPhongBanChiNhanhID", lookupPars);
                if (dsLookup.Tables.Count > 0 && dsLookup.Tables[0].Rows.Count > 0 && dsLookup.Tables[0].Rows[0]["NguoiTaoID"] != DBNull.Value)
                {
                    nguoiTaoId = Convert.ToInt64(dsLookup.Tables[0].Rows[0]["NguoiTaoID"]);
                }
            }

            dataScope = GetPermissionScope(Trang.DA, maChucNang);

            if (!CheckPermission(Trang.DA, maChucNang, phongBanId, targetChiNhanhId, nguoiTaoId))
            {
                string tenChucNang = duAnId == 0 ? "thêm mới" : "sửa";
                return new { success = false, message = $"Bạn không có quyền {tenChucNang} Dự án này!" };
            }

            // Kiểm tra bắt buộc Lý do TRƯỚC KHI lưu (chỉ áp dụng khi Sửa bản ghi không phải mình tạo, phạm vi PHONGBAN)
            bool khongPhaiNguoiTao = duAnId != 0 && (!nguoiTaoId.HasValue || nguoiTaoId.Value != GetCurrentUserId());
            if (duAnId != 0 && dataScope == "PHONGBAN" && khongPhaiNguoiTao && string.IsNullOrWhiteSpace(lyDo))
            {
                return new { success = false, message = "Vui lòng nhập Lý do vì bạn đang sửa Dự án không phải do mình tạo!", requireReason = true };
            }

            log.Info($"SaveData called with duAnId: {duAnId}, phongBanId: {phongBanId}, maDuAn: {maDuAn}, tenDuAn: {tenDuAn}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@DuAnID", duAnId },
                    { "@CongTyID", congTyId },
                    { "@PhongBanID", phongBanId },
                    { "@LoaiCongTrinhID", loaiCongTrinhId == null ? DBNull.Value : (object)Convert.ToInt32(loaiCongTrinhId) },
                    { "@MaDuAn", maDuAn.Trim() },
                    { "@TenDuAn", tenDuAn.Trim() },
                    { "@ChuDauTu", string.IsNullOrEmpty(chuDauTu) ? DBNull.Value : (object)chuDauTu.Trim() },
                    { "@DiaDiem", string.IsNullOrEmpty(diaDiem) ? DBNull.Value : (object)diaDiem.Trim() },
                    { "@MoTa", string.IsNullOrEmpty(moTa) ? DBNull.Value : (object)moTa.Trim() },
                    { "@NgayBatDau", string.IsNullOrEmpty(ngayBatDau) ? DBNull.Value : (object)DateTime.Parse(ngayBatDau) },
                    { "@NgayKetThucDuKien", string.IsNullOrEmpty(ngayKetThuc) ? DBNull.Value : (object)DateTime.Parse(ngayKetThuc) },
                    { "@TrangThaiDuAn", trangThaiDuAn },
                    { "@NguoiTaoID", duAnId == 0 ? (object)GetCurrentUserId() : DBNull.Value }
                };

                db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_Save", pars);

                // Ghi Lý do nếu có (chỉ khi Sửa bản ghi không phải mình tạo)
                if (duAnId != 0)
                {
                    GhiLyDoNeuCan(dataScope, nguoiTaoId, ChucNang.U, duAnId, lyDo);
                }

                return new { success = true, message = duAnId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
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
                var lookupPars = new Dictionary<string, object> { { "@DuAnID", id } };
                DataSet dsLookup = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetPhongBanChiNhanhID", lookupPars);

                long targetPhongBanId = 0, targetChiNhanhId = 0;
                long? nguoiTaoId = null;
                if (dsLookup.Tables.Count > 0 && dsLookup.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsLookup.Tables[0].Rows[0];
                    targetPhongBanId = dr["PhongBanID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PhongBanID"]);
                    targetChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                    nguoiTaoId = dr["NguoiTaoID"] == DBNull.Value ? (long?)null : Convert.ToInt64(dr["NguoiTaoID"]);
                }

                if (!CheckPermission(Trang.DA, ChucNang.D, targetPhongBanId, targetChiNhanhId, nguoiTaoId))
                    return new { success = false, message = "Bạn không có quyền xóa Dự án này!" };

                string dataScope = GetPermissionScope(Trang.DA, ChucNang.D);
                bool khongPhaiNguoiTao = !nguoiTaoId.HasValue || nguoiTaoId.Value != GetCurrentUserId();

                if (dataScope == "PHONGBAN" && khongPhaiNguoiTao && string.IsNullOrWhiteSpace(lyDo))
                {
                    return new { success = false, message = "Vui lòng nhập Lý do vì bạn đang xóa Dự án không phải do mình tạo!", requireReason = true };
                }

                var pars = new Dictionary<string, object> { { "@DuAnID", id } };
                db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_Delete", pars);

                GhiLyDoNeuCan(dataScope, nguoiTaoId, ChucNang.D, id, lyDo);

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