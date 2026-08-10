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
        public static object GetNhanVienOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", congTyId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_GetNhanVienOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { NhanVienID = dr["NhanVienID"], HoTen = dr["HoTen"].ToString(), MaNhanVien = dr["MaNhanVien"].ToString() });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetNhanVienOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetVaiTroOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", congTyId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_GetVaiTroOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { VaiTroID = dr["VaiTroID"], TenVaiTro = dr["TenVaiTro"].ToString() });
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
        public static object GetList(string keyword, string trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

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

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        TaiKhoanID = dr["TaiKhoanID"],
                        Username = dr["Username"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        MaNhanVien = dr["MaNhanVien"].ToString(),
                        // VaiTroNames = dr["VaiTroNames"] == DBNull.Value ? "" : dr["VaiTroNames"].ToString(),
                        TenVaiTro = dr["TenVaiTro"] == DBNull.Value ? "" : dr["TenVaiTro"].ToString(),
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        LastLoginText = dr["LastLoginText"] == DBNull.Value ? "" : dr["LastLoginText"].ToString(),
                        IsLocked = Convert.ToBoolean(dr["IsLocked"]),
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
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            log.Info($"GetById called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_GetById", pars);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    List<object> vaiTroIds = new List<object>();
                    if (ds.Tables.Count > 1)
                    {
                        foreach (DataRow r in ds.Tables[1].Rows)
                        {
                            vaiTroIds.Add(r["VaiTroID"]);
                        }
                    }

                    var data = new
                    {
                        TaiKhoanID = dr["TaiKhoanID"],
                        NhanVienID = dr["NhanVienID"],
                        HoTen = dr["HoTen"].ToString(),
                        Username = dr["Username"].ToString(),
                        IsLocked = Convert.ToBoolean(dr["IsLocked"]),
                        TrangThai = dr["TrangThai"],
                        VaiTroIds = vaiTroIds
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

        /* [WebMethod(EnableSession = true)]
        public static object SaveData(long taiKhoanId, long nhanVienId, string username, string password, bool isLocked, byte trangThai, string vaiTroIds)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (nhanVienId <= 0)
                return new { success = false, message = "Vui lòng chọn Nhân viên!" };

            log.Info($"SaveData called with taiKhoanId: {taiKhoanId}, nhanVienId: {nhanVienId}, username: {username}");
            try
            {
                // Chỉ hash mật khẩu khi người dùng có nhập (tạo mới bắt buộc, sửa thì optional)
                string passwordHash = null;
                if (!string.IsNullOrEmpty(password))
                {
                    // LƯU Ý: giả định VTT.libs.libs có hàm HashPassword tương ứng với VerifyPassword đã dùng ở login.
                    // Nếu tên hàm thực tế khác, đổi lại đúng tên tại đây.
                    passwordHash = libs.libs.HashPassword(password);
                }

                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@TaiKhoanID", taiKhoanId },
                    { "@NhanVienID", nhanVienId },
                    { "@Username", username.Trim() },
                    { "@PasswordHash", passwordHash == null ? DBNull.Value : (object)passwordHash },
                    { "@IsLocked", isLocked },
                    { "@TrangThai", trangThai }
                };

                DataSet dsSave = db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_Save", pars);
                long newId = taiKhoanId;
                if (dsSave.Tables.Count > 0 && dsSave.Tables[0].Rows.Count > 0)
                {
                    newId = Convert.ToInt64(dsSave.Tables[0].Rows[0]["NewTaiKhoanID"]);
                }

                // Gán lại danh sách Vai trò (Phân quyền)
                var rolePars = new Dictionary<string, object>
                {
                    { "@TaiKhoanID", newId },
                    { "@VaiTroIDs", string.IsNullOrEmpty(vaiTroIds) ? DBNull.Value : (object)vaiTroIds }
                };
                db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_SaveVaiTro", rolePars);

                return new { success = true, message = taiKhoanId == 0 ? "Thêm mới tài khoản thành công!" : "Cập nhật tài khoản thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }*/

        [WebMethod(EnableSession = true)]
        public static object SaveData(long taiKhoanId, long nhanVienId, string username, string password, bool isLocked, byte trangThai, long vaiTroId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (nhanVienId <= 0)
                return new { success = false, message = "Vui lòng chọn Nhân viên!" };

            if (vaiTroId <= 0)
                return new { success = false, message = "Vui lòng chọn Vai trò!" };

            try
            {
                string passwordHash = null;
                if (!string.IsNullOrEmpty(password))
                {
                    passwordHash = libs.libs.HashPassword(password);
                }

                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@TaiKhoanID", taiKhoanId },
                    { "@NhanVienID", nhanVienId },
                    { "@Username", username.Trim() },
                    { "@PasswordHash", passwordHash == null ? DBNull.Value : (object)passwordHash },
                    { "@IsLocked", isLocked },
                    { "@TrangThai", trangThai }
                };

                DataSet dsSave = db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_Save", pars);
                long newId = taiKhoanId;
                if (dsSave.Tables.Count > 0 && dsSave.Tables[0].Rows.Count > 0)
                {
                    newId = Convert.ToInt64(dsSave.Tables[0].Rows[0]["NewTaiKhoanID"]);
                }

                var rolePars = new Dictionary<string, object>
                {
                    { "@TaiKhoanID", newId },
                    { "@VaiTroID", vaiTroId }
                };
                db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_SaveVaiTro", rolePars);

                return new { success = true, message = taiKhoanId == 0 ? "Thêm mới tài khoản thành công!" : "Cập nhật tài khoản thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object ToggleLock(long id, bool isLocked)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", id }, { "@IsLocked", isLocked } };
                db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_ToggleLock", pars);
                return new { success = true, message = isLocked ? "Đã khóa tài khoản!" : "Đã mở khóa tài khoản!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi ToggleLock: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteData(long id)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            log.Info($"DeleteData called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", id } };
                db.ExecuteDatasetStoredProcedure("sp_long_DMTaiKhoan_Delete", pars);
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