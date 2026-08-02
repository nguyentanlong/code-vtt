using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using log4net;
using VTT.libs;

namespace VTT
{
    public partial class login : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(login));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["TaiKhoanID"] != null)
            {
                // Nếu đã đăng nhập thành công thì vào thẳng index.aspx
                Response.Redirect("~/index.aspx");
            }
        }

        public class LoginResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string RedirectUrl { get; set; }
        }

        /// <summary>
        /// Hàm sinh ngẫu nhiên 6 chữ số OTP
        /// </summary>
        private static string GenerateOTP()
        {
            Random generator = new Random();
            return generator.Next(0, 1000000).ToString("D6");
        }

        /// <summary>
        /// Hàm gửi Email OTP tự động đọc cấu hình SMTP từ web.config
        /// </summary>
        private static bool SendOTPEmail(string toEmail, string hoTen, string otpCode)
        {
            try
            {
                string subject = $"[{otpCode}] Mã xác thực đăng nhập hệ thống VTT";
                string body = $@"<div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f4f4;'><div style='max-width: 500px; margin: 0 auto; background: #ffffff; padding: 20px; border-radius: 8px;'><h2 style='color: #333;'>Mã Xác Thực Đăng Nhập</h2><p>Xin chào <b>{hoTen}</b>,</p><p>Mã OTP xác thực đăng nhập của bạn là:</p><div style='text-align: center; margin: 20px 0;'><span style='font-size: 28px; font-weight: bold; letter-spacing: 5px; color: #2563eb; background: #eff6ff; padding: 10px 20px; border-radius: 6px; display: inline-block;'>{otpCode}</span></div><p style='color: #666; font-size: 13px;'>Mã này có hiệu lực trong <b>5 phút</b>. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p></div></div>";

                using (SmtpClient smtp = new SmtpClient())
                using (MailMessage mail = new MailMessage())
                {
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    smtp.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Lỗi gửi mail OTP: ", ex);
                return false;
            }
        }

        /// <summary>
        /// Lấy IP Client (Xử lý qua Cloudflare / Proxy)
        /// </summary>
        private static string GetClientIP()
        {
            var ctx = HttpContext.Current;
            string ip = ctx.Request.ServerVariables["HTTP_CF_CONNECTING_IP"];
            if (string.IsNullOrEmpty(ip)) ip = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip)) ip = ctx.Request.UserHostAddress;
            return ip ?? "127.0.0.1";
        }

        [WebMethod(EnableSession = true)]
        public static LoginResponse XuLyDangNhap(string username, string password)
        {
            LoginResponse res = new LoginResponse();
            ConnectServer db = new ConnectServer();

            try
            {
                string clientIP = GetClientIP();

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Username", username },
                    { "@IPAddress", clientIP }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("dbo.sp_chinh_TaiKhoan_DangNhap", parameters);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    bool isSuccess = Convert.ToInt32(row["Success"]) == 1;
                    string message = row["Message"].ToString();

                    // Nếu IP đang bị khóa 5 phút
                    if (!isSuccess)
                    {
                        res.Success = false;
                        res.Message = message;
                        return res;
                    }

                    // Kiểm tra trạng thái khóa tài khoản
                    bool isLocked = row.Table.Columns.Contains("IsLocked") && row["IsLocked"] != DBNull.Value && Convert.ToBoolean(row["IsLocked"]);
                    byte trangThai = row.Table.Columns.Contains("TrangThaiTK") && row["TrangThaiTK"] != DBNull.Value ? Convert.ToByte(row["TrangThaiTK"]) : (byte)0;

                    if (isLocked || trangThai == 0)
                    {
                        res.Success = false;
                        res.Message = "Tài khoản của bạn đang bị khóa hoặc ngưng hoạt động!";
                        return res;
                    }

                    // --- XÁC THỰC MẬT KHẨU PBKDF2 ---
                    string dbPasswordHash = row.Table.Columns.Contains("PasswordHash") ? row["PasswordHash"].ToString() : string.Empty;
                    // bool isPasswordValid = libs.VerifyPassword(password, dbPasswordHash);
                    bool isPasswordValid = libs.libs.VerifyPassword(password, dbPasswordHash);

                    object taiKhoanId = row.Table.Columns.Contains("TaiKhoanID") ? row["TaiKhoanID"] : null;
                    object nhanVienId = row.Table.Columns.Contains("NhanVienID") && row["NhanVienID"] != DBNull.Value ? row["NhanVienID"] : null;

                    if (!isPasswordValid)
                    {
                        // Gọi Proc ghi log thất bại (Sai mật khẩu)
                        Process.GhiNhatKyDangNhap(db, "LOGIN_FAILED", username, taiKhoanId, nhanVienId, clientIP, "Sai mật khẩu");

                        res.Success = false;
                        res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                        return res;
                    }

                    // --- MẬT KHẨU ĐÚNG -> CHUYỂN BƯỚC XÁC THỰC OTP ---
                    string email = row.Table.Columns.Contains("EmailCongTy") && row["EmailCongTy"] != DBNull.Value ? row["EmailCongTy"].ToString() : "";
                    string hoTen = row.Table.Columns.Contains("HoTen") && row["HoTen"] != DBNull.Value ? row["HoTen"].ToString() : username;

                    if (string.IsNullOrEmpty(email))
                    {
                        res.Success = false;
                        res.Message = "Tài khoản chưa được cấu hình Email nhận mã OTP. Vui lòng liên hệ Admin!";
                        return res;
                    }

                    // 1. Sinh mã OTP 6 số
                    string otpCode = GenerateOTP();

                    // 2. Lưu OTP vào Database
                    Dictionary<string, object> otpParams = new Dictionary<string, object>
                    {
                        { "@Action", "GENERATE" },
                        { "@TaiKhoanID", taiKhoanId },
                        { "@OTPCode", otpCode },
                        { "@IPAddress", clientIP }
                    };
                    db.ExecuteDatasetStoredProcedure("dbo.sp_chinh_TaiKhoan_OTP", otpParams);

                    // 3. Gửi Mail OTP
                    bool sendMailOk = SendOTPEmail(email, hoTen, otpCode);
                    if (!sendMailOk)
                    {
                        res.Success = false;
                        res.Message = "Lỗi gửi email xác thực OTP. Vui lòng thử lại sau!";
                        return res;
                    }

                    // 4. Lưu Session TẠM THỜI (Lưu thông tin User để cấp Session chính thức bên verify_login)
                    HttpContext.Current.Session["Pending_TaiKhoanID"] = taiKhoanId;
                    HttpContext.Current.Session["Pending_Username"] = row.Table.Columns.Contains("Username") ? row["Username"] : username;
                    HttpContext.Current.Session["Pending_NhanVienID"] = nhanVienId;
                    HttpContext.Current.Session["Pending_HoTen"] = hoTen;
                    HttpContext.Current.Session["Pending_Email"] = email;
                    HttpContext.Current.Session["Pending_CongTyID"] = row.Table.Columns.Contains("CongTyID") ? row["CongTyID"] : null;
                    HttpContext.Current.Session["Pending_PhongBanID"] = row.Table.Columns.Contains("PhongBanID") ? row["PhongBanID"] : null;
                    HttpContext.Current.Session["Pending_ChucDanhID"] = row.Table.Columns.Contains("ChucDanhID") ? row["ChucDanhID"] : null;

                    res.Success = true;
                    res.Message = "Mã OTP đã được gửi về Email của bạn!";
                    res.RedirectUrl = "verify_login.aspx"; // Chuyển sang trang nhập OTP
                }
                else
                {
                    // Gọi Proc ghi log thất bại (Không tồn tại Username)
                    Process.GhiNhatKyDangNhap(db, "LOGIN_FAILED", username, null, null, clientIP, "Tài khoản không tồn tại");

                    res.Success = false;
                    res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi trong XuLyDangNhap WebMethod: ", ex);
                res.Success = false;
                res.Message = "Đã xảy ra lỗi hệ thống: " + ex.Message;
            }

            return res;
        }
    }
}