using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT
{
    public partial class thong_tin_tai_khoan : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(thong_tin_tai_khoan));

        protected void Page_Load(object sender, EventArgs e) { }

        [WebMethod(EnableSession = true)]
        public static object GetThongTin()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetThongTinCaNhan",
                    new Dictionary<string, object> { { "@TaiKhoanID", GetCurrentUserId() } });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return new { success = false, message = "Không tìm thấy thông tin tài khoản." };

                DataRow dr = ds.Tables[0].Rows[0];
                return new
                {
                    success = true,
                    data = new
                    {
                        TenDangNhap = dr["TenDangNhap"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        MaNhanVien = dr["MaNhanVien"].ToString(),
                        Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString(),
                        SoDienThoai = dr["SoDienThoai"] == DBNull.Value ? "" : dr["SoDienThoai"].ToString(),
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        TenChiNhanh = dr["TenChiNhanh"] == DBNull.Value ? "" : dr["TenChiNhanh"].ToString(),
                        TenVaiTro = dr["TenVaiTro"] == DBNull.Value ? "Nhân viên" : dr["TenVaiTro"].ToString()
                    }
                };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetThongTin: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object CapNhatLienHe(string soDienThoai, string email)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_CapNhatLienHe", new Dictionary<string, object>
                {
                    { "@TaiKhoanID", GetCurrentUserId() },
                    { "@SoDienThoai", string.IsNullOrWhiteSpace(soDienThoai) ? DBNull.Value : (object)soDienThoai.Trim() },
                    { "@Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : (object)email.Trim() }
                });
                return new { success = true, message = "Đã cập nhật thông tin liên hệ!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi CapNhatLienHe: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object DoiMatKhau(string matKhauCu, string matKhauMoi)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 6)
                return new { success = false, message = "Mật khẩu mới phải có ít nhất 6 ký tự!" };

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetThongTinCaNhan",
                    new Dictionary<string, object> { { "@TaiKhoanID", GetCurrentUserId() } });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return new { success = false, message = "Không tìm thấy tài khoản." };

                string hashHienTai = ds.Tables[0].Rows[0]["MatKhauHash"].ToString();

                // TODO_VERIFY_PASSWORD: thay đúng tên hàm verify hash hiện có trong libs.libs
                if (!libs.libs.VerifyPassword(matKhauCu, hashHienTai))
                    return new { success = false, message = "Mật khẩu hiện tại không đúng!" };

                string hashMoi = libs.libs.HashPassword(matKhauMoi);
                db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_DoiMatKhau", new Dictionary<string, object>
                {
                    { "@TaiKhoanID", GetCurrentUserId() },
                    { "@MatKhauHashMoi", hashMoi }
                });

                return new { success = true, message = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại." };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DoiMatKhau: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}