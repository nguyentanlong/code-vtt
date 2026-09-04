using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class nhan_vien : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(nhan_vien));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private static string TenMacDinhChiNhanh() => "Trực thuộc Tổng công ty";

        /// <summary>
        /// Tra ChiNhanhID của 1 PhongBanID cụ thể — dùng để CheckPermission biết đúng phạm vi Chi nhánh
        /// </summary>
        private static long GetChiNhanhIdOfPhongBan(long phongBanId)
        {
            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@PhongBanID", phongBanId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_PhongBan_GetChiNhanhID", pars);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["ChiNhanhID"] != DBNull.Value)
            {
                return Convert.ToInt64(ds.Tables[0].Rows[0]["ChiNhanhID"]);
            }
            return 0; // 0 nghĩa là Phòng ban thuộc thẳng Tổng công ty, không qua Chi nhánh
        }

        /*[WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            bool canXem = CheckPermission("NHANVIEN", "XEM", GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            bool canThem = CheckPermission("NHANVIEN", "THEM", GetCurrentPhongBanId(), GetCurrentChiNhanhId());

            return new { success = true, data = new { canXem = canXem, canThem = canThem } };
        }*/
        [WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            // goi từ App_Code
            // string module = AppConstants.Trang.Emp;
            // string actionXem = AppConstants.ChucNang.R;
            // string actionThem = AppConstants.ChucNang.I;
            bool canXem = CheckPermission(Trang.NV, ChucNang.R, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            bool canThem = CheckPermission(Trang.NV, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            string myScope = GetPermissionScope(Trang.NV, ChucNang.R); // "CONGTY" / "CHINHANH" / "PHONGBAN" / null

            return new { success = true, data = new { canXem = canXem, canThem = canThem, scope = myScope } };
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
        public static object GetPhongBanOptions(object chiNhanhId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@ChiNhanhID", chiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(chiNhanhId) }
                };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetPhongBanOptions", pars);

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
        public static object GetChucVuOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetChucVuOptions", new Dictionary<string, object>());

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { ChucVuID = dr["ChucVuID"], TenChucVu = dr["TenChucVu"].ToString() });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetChucVuOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, object chiNhanhId, object phongBanId, string trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string myScope = GetPermissionScope("NHANVIEN", "XEM");
            if (myScope == null)
                return new { success = false, message = "Bạn không có quyền xem Danh sách Nhân viên!" };

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

            // Lấy DataScope cho Sửa/Xóa ĐÚNG 1 LẦN trước vòng lặp (thay vì gọi CheckPermission cho từng dòng)
            string scopeSua = GetPermissionScope("NHANVIEN", "SUA");
            string scopeXoa = GetPermissionScope("NHANVIEN", "XOA");

            log.Info($"GetList called with keyword: {keyword}, myScope: {myScope}, effectiveChiNhanhId: {effectiveChiNhanhId}, effectivePhongBanId: {effectivePhongBanId}, trangThai: {trangThai}");
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

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    long rowPhongBanId = dr["PhongBanChinhThucID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PhongBanChinhThucID"]);
                    long rowChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);

                    // So khớp thuần C#, không chạm DB
                    bool canEditRow = EvaluateScope(scopeSua, rowPhongBanId, rowChiNhanhId);
                    bool canDeleteRow = EvaluateScope(scopeXoa, rowPhongBanId, rowChiNhanhId);

                    list.Add(new
                    {
                        NhanVienID = dr["NhanVienID"],
                        HoTen = dr["HoTen"].ToString(),
                        MaNhanVien = dr["MaNhanVien"].ToString(),
                        Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString(),
                        SoDienThoai = dr["SoDienThoai"] == DBNull.Value ? "" : dr["SoDienThoai"].ToString(),
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        TenChiNhanh = dr["TenChiNhanh"] == DBNull.Value ? "Trực thuộc Tổng công ty" : dr["TenChiNhanh"].ToString(),
                        TenChucVu = dr["TenChucVu"] == DBNull.Value ? "" : dr["TenChucVu"].ToString(),
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

            log.Info($"GetById called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@NhanVienID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetById", pars);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    long rowPhongBanId = dr["PhongBanChinhThucID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PhongBanChinhThucID"]);
                    long rowChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);

                    if (!CheckPermission("NHANVIEN", "SUA", rowPhongBanId, rowChiNhanhId))
                        return new { success = false, message = "Bạn không có quyền xem chi tiết Nhân viên này!" };

                    var data = new
                    {
                        NhanVienID = dr["NhanVienID"],
                        HoTen = dr["HoTen"].ToString(),
                        MaNhanVien = dr["MaNhanVien"].ToString(),
                        Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString(),
                        SoDienThoai = dr["SoDienThoai"] == DBNull.Value ? "" : dr["SoDienThoai"].ToString(),
                        PhongBanChinhThucID = dr["PhongBanChinhThucID"],
                        ChiNhanhID = dr["ChiNhanhID"] == DBNull.Value ? (object)null : dr["ChiNhanhID"],
                        ChucVuChinhThucID = dr["ChucVuChinhThucID"] == DBNull.Value ? (object)null : dr["ChucVuChinhThucID"],
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
        public static object SaveData(long nhanVienId, string hoTen, string maNhanVien, string email, string soDienThoai,
                                       long phongBanChinhThucId, object chucVuChinhThucId, byte trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyId();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            if (phongBanChinhThucId <= 0)
                return new { success = false, message = "Vui lòng chọn Phòng ban!" };

            // Tra đúng ChiNhanhID của Phòng ban vừa chọn trong form, để CheckPermission so khớp chính xác
            long targetChiNhanhId = GetChiNhanhIdOfPhongBan(phongBanChinhThucId);
            string maChucNang = nhanVienId == 0 ? "THEM" : "SUA";

            if (!CheckPermission("NHANVIEN", maChucNang, phongBanChinhThucId, targetChiNhanhId))
            {
                string tenChucNang = nhanVienId == 0 ? "thêm mới" : "sửa";
                return new { success = false, message = $"Bạn không có quyền {tenChucNang} Nhân viên thuộc Phòng ban này!" };
            }

            log.Info($"SaveData called with nhanVienId: {nhanVienId}, hoTen: {hoTen}, maNhanVien: {maNhanVien}, phongBanChinhThucId: {phongBanChinhThucId}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@NhanVienID", nhanVienId },
                    { "@CongTyID", congTyId },
                    { "@HoTen", hoTen.Trim() },
                    { "@MaNhanVien", maNhanVien.Trim() },
                    { "@Email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim() },
                    { "@SoDienThoai", string.IsNullOrEmpty(soDienThoai) ? DBNull.Value : (object)soDienThoai.Trim() },
                    { "@PhongBanChinhThucID", phongBanChinhThucId },
                    { "@ChucVuChinhThucID", chucVuChinhThucId == null ? DBNull.Value : (object)Convert.ToInt32(chucVuChinhThucId) },
                    { "@TrangThai", trangThai }
                };

                db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_Save", pars);
                return new { success = true, message = nhanVienId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteData(long nhanVienId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();

                var checkPars = new Dictionary<string, object> { { "@NhanVienID", nhanVienId } };
                DataSet dsCheck = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetPhongBanID", checkPars);

                long targetPhongBanId = 0, targetChiNhanhId = 0;
                if (dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsCheck.Tables[0].Rows[0];
                    targetPhongBanId = dr["PhongBanID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PhongBanID"]);
                    targetChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                }

                if (!CheckPermission("NHANVIEN", "XOA", targetPhongBanId, targetChiNhanhId))
                    return new { success = false, message = "Bạn không có quyền xóa Nhân viên này!" };

                var pars = new Dictionary<string, object> { { "@NhanVienID", nhanVienId } };
                db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_Delete", pars);

                return new { success = true, message = "Xóa nhân viên thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}