using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class tai_khoan : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(tai_khoan));

        protected void Page_Load(object sender, EventArgs e) { }

        private static int? GetMyCapBacTuongUng()
        {
            long taiKhoanId = GetCurrentUserId();
            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetCapBacTuongUng", pars);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["CapBacTuongUng"] != DBNull.Value)
                return Convert.ToInt32(ds.Tables[0].Rows[0]["CapBacTuongUng"]);
            return 5; // Không có gán đặc biệt -> mặc định cấp Nhân viên (thấp nhất)
        }

        [WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            bool canThem = CheckPermission(Trang.TK, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            string scope = GetPermissionScope(Trang.TK, ChucNang.R);
            return new { success = true, data = new { canThem = canThem, scope = scope, myChiNhanhId = GetCurrentChiNhanhId() } };
        }

        [WebMethod(EnableSession = true)]
        public static object GetChiNhanhOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string scope = GetPermissionScope(Trang.TK, ChucNang.R);
            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_NhanVien_GetChiNhanhOptions", new Dictionary<string, object>());

            List<object> list = new List<object>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                long id = Convert.ToInt64(dr["ChiNhanhID"]);
                if (scope == "CONGTY" || id == GetCurrentChiNhanhId())
                    list.Add(new { ChiNhanhID = id, TenChiNhanh = dr["TenChiNhanh"].ToString() });
            }
            return new { success = true, data = list };
        }

        [WebMethod(EnableSession = true)]
        public static object GetNhanVienChuaCoTK()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string scope = GetPermissionScope(Trang.TK, ChucNang.I);
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>();

                if (scope == "PHONGBAN")
                {
                    pars["@ChiNhanhID"] = DBNull.Value;
                    pars["@PhongBanID"] = GetCurrentPhongBanId();
                }
                else if (scope == "CHINHANH")
                {
                    pars["@ChiNhanhID"] = GetCurrentChiNhanhId();
                    pars["@PhongBanID"] = DBNull.Value;
                }
                else
                {
                    pars["@ChiNhanhID"] = DBNull.Value;
                    pars["@PhongBanID"] = DBNull.Value;
                }

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetNhanVienChuaCoTK", pars);
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { NhanVienID = dr["NhanVienID"], HoTen = dr["HoTen"].ToString(), MaNhanVien = dr["MaNhanVien"].ToString(), TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString() });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetNhanVienChuaCoTK: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetVaiTroOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                int? myCapBac = GetMyCapBacTuongUng();
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@MinCapBacTuongUng", myCapBac.HasValue ? (object)myCapBac.Value : DBNull.Value } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetVaiTroOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string maNq = dr["MaNhomQuyen"].ToString();
                    bool canGanPhongBan = maNq == "NQ_TRUONGPHONG" || maNq == "NQ_PHOPHONG" || maNq == "NQ_TOTRUONG";
                    list.Add(new { NhomQuyenID = dr["NhomQuyenID"], MaNhomQuyen = maNq, TenNhomQuyen = dr["TenNhomQuyen"].ToString(), CanGanPhongBan = canGanPhongBan });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetVaiTroOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
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
        public static object ToggleLock(long taiKhoanId, bool isLocked, string lyDo)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (taiKhoanId == GetCurrentUserId())
                return new { success = false, message = "Bạn không thể tự khóa/mở khóa chính tài khoản đang đăng nhập!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var scopePars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId } };
                DataSet dsScope = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetScopeInfo", scopePars);

                long targetPhongBanId = 0, targetChiNhanhId = 0;
                if (dsScope.Tables.Count > 0 && dsScope.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsScope.Tables[0].Rows[0];
                    targetPhongBanId = dr["PhongBanID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PhongBanID"]);
                    targetChiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
                }

                string dataScope = GetPermissionScope(Trang.TK, ChucNang.U);
                if (!EvaluateScope(dataScope, targetPhongBanId, targetChiNhanhId))
                    return new { success = false, message = "Bạn không có quyền khóa/mở khóa tài khoản này!" };

                if (dataScope != "CONGTY" && string.IsNullOrWhiteSpace(lyDo))
                    return new { success = false, message = "Vui lòng nhập Lý do!", requireReason = true };

                var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId }, { "@IsLocked", isLocked } };
                db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_ToggleLock", pars);

                if (dataScope != "CONGTY")
                {
                    var logPars = new Dictionary<string, object>
                    {
                        { "@TenBang", "TaiKhoan" }, { "@KhoaChinh", taiKhoanId },
                        { "@HanhDong", isLocked ? "KHOA" : "MOKHOA" },
                        { "@NguoiThucHienID", GetCurrentUserId() }, { "@LyDo", lyDo }
                    };
                    db.ExecuteDatasetStoredProcedure("sp_v2_GhiLyDoThayDoi", logPars);
                }

                return new { success = true, message = isLocked ? "Đã khóa tài khoản!" : "Đã mở khóa tài khoản!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi ToggleLock: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, object chiNhanhId, int pageNumber)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string myScope = GetPermissionScope(Trang.TK, ChucNang.R);
            if (myScope == null)
                return new { success = false, message = "Bạn không có quyền xem Danh sách Tài khoản!" };

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

            int pageSize = 12;
            if (pageNumber <= 0) pageNumber = 1;

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@ChiNhanhID", effChiNhanhId == null ? DBNull.Value : (object)Convert.ToInt32(effChiNhanhId) },
                    { "@PhongBanID", effPhongBanId == null ? DBNull.Value : (object)Convert.ToInt32(effPhongBanId) },
                    { "@PageNumber", pageNumber },
                    { "@PageSize", pageSize }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetList", pars);
                int tongSoDong = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? Convert.ToInt32(ds.Tables[0].Rows[0]["TongSoDong"]) : 0;
                DataTable dt = ds.Tables.Count > 1 ? ds.Tables[1] : ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        TaiKhoanID = dr["TaiKhoanID"],
                        TenDangNhap = dr["TenDangNhap"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        TenVaiTro = dr["TenVaiTro"] == DBNull.Value ? "Nhân viên" : dr["TenVaiTro"].ToString(),
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
                        IsLocked = Convert.ToBoolean(dr["IsLocked"])
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
        public static object SaveData(long nhanVienId, string tenDangNhap, string matKhau, long nhomQuyenId, object phongBanId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (!CheckPermission(Trang.TK, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId()))
                return new { success = false, message = "Bạn không có quyền tạo Tài khoản!" };

            if (nhanVienId <= 0) return new { success = false, message = "Vui lòng chọn Nhân viên!" };
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return new { success = false, message = "Vui lòng nhập Tên đăng nhập!" };
            if (string.IsNullOrWhiteSpace(matKhau) || matKhau.Length < 6) return new { success = false, message = "Mật khẩu phải có ít nhất 6 ký tự!" };
            // if (nhomQuyenId <= 0) return new { success = false, message = "Vui lòng chọn Vai trò!" };


            // Kiểm tra: Vai trò được chọn phải THẤP HƠN cấp của người tạo (chặn cả khi client bị can thiệp)
            /*int? myCapBac = GetMyCapBacTuongUng();
            ConnectServer dbCheck = new ConnectServer();
            DataSet dsNq = dbCheck.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetVaiTroOptions",
                new Dictionary<string, object> { { "@MinCapBacTuongUng", myCapBac.HasValue ? (object)myCapBac.Value : DBNull.Value } });

            bool hopLe = false;
            foreach (DataRow dr in dsNq.Tables[0].Rows)
            {
                if (Convert.ToInt64(dr["NhomQuyenID"]) == nhomQuyenId) { hopLe = true; break; }
            }
            if (!hopLe)
                return new { success = false, message = "Bạn không được phép gán Vai trò này (chỉ được gán Vai trò thấp hơn cấp của bạn)!" };
*/
            // nhomQuyenId = 0 ("Nhân viên mặc định") luôn hợp lệ, bỏ qua kiểm tra cấp bậc
            if (nhomQuyenId > 0)
            {
                int? myCapBac = GetMyCapBacTuongUng();
                ConnectServer dbCheck = new ConnectServer();
                DataSet dsNq = dbCheck.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetVaiTroOptions",
                    new Dictionary<string, object> { { "@MinCapBacTuongUng", myCapBac.HasValue ? (object)myCapBac.Value : DBNull.Value } });

                bool hopLe = false;
                foreach (DataRow dr in dsNq.Tables[0].Rows)
                {
                    if (Convert.ToInt64(dr["NhomQuyenID"]) == nhomQuyenId) { hopLe = true; break; }
                }
                if (!hopLe)
                    return new { success = false, message = "Bạn không được phép gán Vai trò này (chỉ được gán Vai trò thấp hơn cấp của bạn)!" };
            }
            try
            {
                ConnectServer db = new ConnectServer();
                string hash = libs.libs.HashPassword(matKhau); // dùng đúng hàm hash sẵn có trong hệ thống

                var createPars = new Dictionary<string, object>
                {
                    { "@NhanVienID", nhanVienId },
                    { "@TenDangNhap", tenDangNhap.Trim() },
                    { "@MatKhauHash", hash }
                };
                DataSet dsCreate = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_Create", createPars);
                long newTaiKhoanId = Convert.ToInt64(dsCreate.Tables[0].Rows[0]["NewTaiKhoanID"]);

/*                var ganPars = new Dictionary<string, object>
                {
                    { "@TaiKhoanID", newTaiKhoanId },
                    { "@NhanVienID", nhanVienId },
                    { "@NhomQuyenID", nhomQuyenId },
                    { "@PhongBanID", phongBanId == null ? DBNull.Value : (object)Convert.ToInt32(phongBanId) }
                };
                db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GanVaiTro", ganPars);*/
                // Chỉ gọi gán Vai trò đặc biệt nếu KHÔNG phải "Nhân viên mặc định" (nhomQuyenId=0)
                if (nhomQuyenId > 0)
                {
                    var ganPars = new Dictionary<string, object>
                    {
                        { "@TaiKhoanID", newTaiKhoanId },
                        { "@NhanVienID", nhanVienId },
                        { "@NhomQuyenID", nhomQuyenId },
                        { "@PhongBanID", phongBanId == null ? DBNull.Value : (object)Convert.ToInt32(phongBanId) }
                    };
                    db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GanVaiTro", ganPars);
                }

                return new { success = true, message = "Tạo Tài khoản thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}