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

        /*[WebMethod(EnableSession = true)]
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
        }*/
        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, object chiNhanhId, object phongBanId, string trangThai, int pageNumber)
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

            int pageSize = 12;
            if (pageNumber <= 0) pageNumber = 1;

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@ChiNhanhID", effectiveChiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(effectiveChiNhanhId) },
                    { "@PhongBanID", effectivePhongBanId == null ? DBNull.Value : (object)Convert.ToInt32(effectivePhongBanId) },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) },
                    { "@PageNumber", pageNumber },
                    { "@PageSize", pageSize }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetList", pars);

                int tongSoDong = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? Convert.ToInt32(ds.Tables[0].Rows[0]["TongSoDong"]) : 0;
                DataTable dt = ds.Tables.Count > 1 ? ds.Tables[1] : ds.Tables[0];

                long myUserId = GetCurrentUserId();

                List<object> list = new List<object>();
                // var (myNhomQuyenIds, isAdmin) = GetMyNhomQuyenDuyet(); // tính 1 lần trước vòng lặp
                foreach (DataRow dr in dt.Rows)
                {
                    long rowPhongBanId = Convert.ToInt64(dr["PhongBanID"]);
                    long rowChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                    long? rowNguoiTaoId = dr["NguoiTaoID"] == DBNull.Value ? (long?)null : Convert.ToInt64(dr["NguoiTaoID"]);

                    bool canEditRow = EvaluateScope(scopeSua, rowPhongBanId, rowChiNhanhId, rowNguoiTaoId);
                    bool canDeleteRow = EvaluateScope(scopeXoa, rowPhongBanId, rowChiNhanhId, rowNguoiTaoId);

                    byte trangThaiPheDuyet = dr["TrangThaiPheDuyet"] == DBNull.Value ? (byte)0 : Convert.ToByte(dr["TrangThaiPheDuyet"]);
                    object nhomQuyenDuyetIdObj = dr["NhomQuyenDuyetID"];
                    bool laNguoiTao = rowNguoiTaoId.HasValue && rowNguoiTaoId.Value == myUserId;

                    long rowDuAnId = Convert.ToInt64(dr["DuAnID"]);
                    var (canDuyetRow, isAdminRow) = KiemTraQuyenDuyet(rowDuAnId, myUserId);
                    // byte trangThaiPheDuyet = ...;
                    bool canGuiDuyet = (trangThaiPheDuyet == 0 || trangThaiPheDuyet == 2) && (laNguoiTao || isAdminRow || canEditRow);
                    bool canDuyet = trangThaiPheDuyet == 1 && canDuyetRow;

                    // bool canGuiDuyet = (trangThaiPheDuyet == 0 || trangThaiPheDuyet == 2)
                                    // && (laNguoiTao || isAdmin || canEditRow);

                    // bool canDuyet = trangThaiPheDuyet == 1 && nhomQuyenDuyetIdObj != DBNull.Value
                                    // && (isAdmin || myNhomQuyenIds.Contains(Convert.ToInt64(nhomQuyenDuyetIdObj)));

                    list.Add(new
                    {
                        DuAnID = dr["DuAnID"],
                        MaDuAn = dr["MaDuAn"].ToString(),
                        TenDuAn = dr["TenDuAn"].ToString(),
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        TienDo = dr["TienDo"],
                        TrangThaiDuAn = Convert.ToByte(dr["TrangThaiDuAn"]),
                        LaNguoiTao = laNguoiTao,
                        CanEditRow = canEditRow,
                        CanDeleteRow = canDeleteRow,
                        TrangThaiPheDuyet = trangThaiPheDuyet,
                        TenBuocHienTai = dr["TenBuocHienTai"] == DBNull.Value ? "" : dr["TenBuocHienTai"].ToString(),
                        CanGuiDuyet = canGuiDuyet,
                        CanDuyet = canDuyet,
                        CanTuChoi = canDuyet
                    });
                }

                return new { success = true, data = list, tongSoDong = tongSoDong, pageSize = pageSize };
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
        // Trả về (danh sách NhomQuyenID mà tài khoản đang sở hữu, có phải Admin không)
        /*private static (HashSet<long> NhomQuyenIds, bool IsAdmin) GetMyNhomQuyenDuyet()
        {
            var result = new HashSet<long>();
            bool isAdmin = false;
            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@TaiKhoanID", GetCurrentUserId() } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetNhomQuyenDuyet", pars);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    long id = Convert.ToInt64(dr["NhomQuyenID"]);
                    result.Add(id);
                    string ma = dr["MaNhomQuyen"] == DBNull.Value ? "" : dr["MaNhomQuyen"].ToString();
                    if (ma == "NQ_ADMIN" || ma == "NQ_CN_ADMIN") isAdmin = true;
                }
            }
            return (result, isAdmin);
        }*/
        private static (bool CoQuyenDuyet, bool LaAdmin) KiemTraQuyenDuyet(long duAnId, long taiKhoanId)
        {
            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@DuAnID", duAnId }, { "@TaiKhoanID", taiKhoanId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_KiemTraQuyenDuyet", pars);
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return (false, false);
            DataRow dr = ds.Tables[0].Rows[0];
            return (Convert.ToBoolean(dr["CoQuyenDuyet"]), Convert.ToBoolean(dr["LaAdmin"]));
        }

        [WebMethod(EnableSession = true)]
        public static object GetBuocHienTai(long duAnId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetBuocHienTai",
                    new Dictionary<string, object> { { "@DuAnID", duAnId } });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return new { success = false, message = "Không tìm thấy Dự án." };

                DataRow dr = ds.Tables[0].Rows[0];
                byte trangThaiPheDuyet = dr["TrangThaiPheDuyet"] == DBNull.Value ? (byte)0 : Convert.ToByte(dr["TrangThaiPheDuyet"]);
                object nhomQuyenDuyetIdObj = dr["NhomQuyenDuyetID"];

                // Xác định Người tạo + Phòng ban/Chi nhánh để tính quyền Gửi duyệt
                long phongBanId = 0, chiNhanhId = 0;
                long? nguoiTaoId = null;
                DataSet dsLookup = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetPhongBanChiNhanhID",
                    new Dictionary<string, object> { { "@DuAnID", duAnId } });
                if (dsLookup.Tables.Count > 0 && dsLookup.Tables[0].Rows.Count > 0)
                {
                    DataRow drL = dsLookup.Tables[0].Rows[0];
                    phongBanId = drL["PhongBanID"] == DBNull.Value ? 0 : Convert.ToInt64(drL["PhongBanID"]);
                    chiNhanhId = drL["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(drL["ChiNhanhID"]);
                    nguoiTaoId = drL["NguoiTaoID"] == DBNull.Value ? (long?)null : Convert.ToInt64(drL["NguoiTaoID"]);
                }
                bool laNguoiTao = nguoiTaoId.HasValue && nguoiTaoId.Value == GetCurrentUserId();
                /*var (nhomQuyenIds, isAdmin) = GetMyNhomQuyenDuyet();

                bool canGuiDuyet = (trangThaiPheDuyet == 0 || trangThaiPheDuyet == 2)
                                && (laNguoiTao || isAdmin || CheckPermission(Trang.DA, ChucNang.U, phongBanId, chiNhanhId, nguoiTaoId));

                bool canDuyet = false;
                if (trangThaiPheDuyet == 1 && nhomQuyenDuyetIdObj != DBNull.Value)
                    canDuyet = isAdmin || nhomQuyenIds.Contains(Convert.ToInt64(nhomQuyenDuyetIdObj));
                */
                var (canDuyet, isAdmin) = KiemTraQuyenDuyet(duAnId, GetCurrentUserId());
                bool canGuiDuyet = (trangThaiPheDuyet == 0 || trangThaiPheDuyet == 2)
                                    && (laNguoiTao || isAdmin || CheckPermission(Trang.DA, ChucNang.U, phongBanId, chiNhanhId, nguoiTaoId));
                if (trangThaiPheDuyet != 1) canDuyet = false; // chỉ hiện Duyệt/Từ chối khi đang chờ duyệt
                return new
                {
                    success = true,
                    data = new
                    {
                        BuocHienTaiID = dr["BuocHienTaiID"] == DBNull.Value ? (long?)null : Convert.ToInt64(dr["BuocHienTaiID"]),
                        TrangThaiPheDuyet = trangThaiPheDuyet,
                        TenBuoc = dr["TenBuoc"] == DBNull.Value ? "" : dr["TenBuoc"].ToString(),
                        TenNhomQuyen = dr["TenNhomQuyen"] == DBNull.Value ? "" : dr["TenNhomQuyen"].ToString(),
                        CanGuiDuyet = canGuiDuyet,
                        CanDuyet = canDuyet,
                        CanTuChoi = canDuyet,
                        IsAdmin = isAdmin
                    }
                };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetBuocHienTai: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GuiDuyet(long duAnId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                long phongBanId = 0, chiNhanhId = 0;
                long? nguoiTaoId = null;
                DataSet dsLookup = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetPhongBanChiNhanhID",
                    new Dictionary<string, object> { { "@DuAnID", duAnId } });
                if (dsLookup.Tables.Count > 0 && dsLookup.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsLookup.Tables[0].Rows[0];
                    phongBanId = dr["PhongBanID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PhongBanID"]);
                    chiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                    nguoiTaoId = dr["NguoiTaoID"] == DBNull.Value ? (long?)null : Convert.ToInt64(dr["NguoiTaoID"]);
                }

                var (_, isAdmin) = KiemTraQuyenDuyet(duAnId, GetCurrentUserId());
                bool laNguoiTao = nguoiTaoId.HasValue && nguoiTaoId.Value == GetCurrentUserId();

                if (!laNguoiTao && !isAdmin && !CheckPermission(Trang.DA, ChucNang.U, phongBanId, chiNhanhId, nguoiTaoId))
                    return new { success = false, message = "Bạn không có quyền Gửi duyệt Dự án này!" };

                db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GuiDuyet", new Dictionary<string, object> { { "@DuAnID", duAnId } });
                return new { success = true, message = "Đã gửi duyệt!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GuiDuyet: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object Duyet(long duAnId, string lyDo)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet dsCur = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetBuocHienTai",
                    new Dictionary<string, object> { { "@DuAnID", duAnId } });
                if (dsCur.Tables.Count == 0 || dsCur.Tables[0].Rows.Count == 0)
                    return new { success = false, message = "Không tìm thấy Dự án." };

                DataRow drCur = dsCur.Tables[0].Rows[0];
                byte trangThai = drCur["TrangThaiPheDuyet"] == DBNull.Value ? (byte)0 : Convert.ToByte(drCur["TrangThaiPheDuyet"]);
                if (trangThai != 1)
                    return new { success = false, message = "Dự án không ở trạng thái Đang chờ duyệt!" };

                /*object nhomQuyenDuyetIdObj = drCur["NhomQuyenDuyetID"];
                var (nhomQuyenIds, isAdmin) = GetMyNhomQuyenDuyet();

                bool coQuyen = isAdmin || (nhomQuyenDuyetIdObj != DBNull.Value && nhomQuyenIds.Contains(Convert.ToInt64(nhomQuyenDuyetIdObj))
                );
                if (!coQuyen)
                    return new { success = false, message = "Bạn không thuộc Nhóm quyền được phân công Duyệt Bước này!" };
                    

                if (!isAdmin && string.IsNullOrWhiteSpace(lyDo))
                    return new { success = false, message = "Vui lòng nhập Lý do/Ý kiến khi Duyệt!", requireReason = true };
                */
                var (coQuyen, isAdmin) = KiemTraQuyenDuyet(duAnId, GetCurrentUserId());
                if (!coQuyen)
                    return new { success = false, message = "Bạn không thuộc Nhóm quyền/Phòng ban-Chi nhánh được phân công Duyệt Bước này!" };

                if (!isAdmin && string.IsNullOrWhiteSpace(lyDo))
                    return new { success = false, message = "Vui lòng nhập Lý do/Ý kiến khi Duyệt!", requireReason = true };

                db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_Duyet", new Dictionary<string, object>
                {
                    { "@DuAnID", duAnId },
                    { "@NguoiXuLyID", GetCurrentUserId() },
                    { "@LyDo", string.IsNullOrEmpty(lyDo) ? DBNull.Value : (object)lyDo.Trim() }
                });

                return new { success = true, message = "Đã Duyệt thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi Duyet: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object TuChoi(long duAnId, string lyDo)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (string.IsNullOrWhiteSpace(lyDo))
                return new { success = false, message = "Vui lòng nhập Lý do Từ chối!", requireReason = true };

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet dsCur = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetBuocHienTai",
                    new Dictionary<string, object> { { "@DuAnID", duAnId } });
                if (dsCur.Tables.Count == 0 || dsCur.Tables[0].Rows.Count == 0)
                    return new { success = false, message = "Không tìm thấy Dự án." };

                DataRow drCur = dsCur.Tables[0].Rows[0];
                byte trangThai = drCur["TrangThaiPheDuyet"] == DBNull.Value ? (byte)0 : Convert.ToByte(drCur["TrangThaiPheDuyet"]);
                if (trangThai != 1)
                    return new { success = false, message = "Dự án không ở trạng thái Đang chờ duyệt!" };

                /*object nhomQuyenDuyetIdObj = drCur["NhomQuyenDuyetID"];
                var (nhomQuyenIds, isAdmin) = GetMyNhomQuyenDuyet();

                bool coQuyen = isAdmin || (nhomQuyenDuyetIdObj != DBNull.Value && nhomQuyenIds.Contains(Convert.ToInt64(nhomQuyenDuyetIdObj)));
                if (!coQuyen)
                    return new { success = false, message = "Bạn không thuộc Nhóm quyền được phân công Duyệt/Từ chối Bước này!" };
                */
                var (coQuyen, isAdmin) = KiemTraQuyenDuyet(duAnId, GetCurrentUserId());
                if (!coQuyen)
                    return new { success = false, message = "Bạn không thuộc Nhóm quyền/Phòng ban-Chi nhánh được phân công Duyệt/Từ chối Bước này!" };
                db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_TuChoi", new Dictionary<string, object>
                {
                    { "@DuAnID", duAnId },
                    { "@NguoiXuLyID", GetCurrentUserId() },
                    { "@LyDo", lyDo.Trim() }
                });

                return new { success = true, message = "Đã Từ chối!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi TuChoi: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}