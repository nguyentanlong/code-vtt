using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using VTT.libs;

namespace VTT
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["TaiKhoanID"] != null)
            {
                Response.Redirect("~/index.aspx");
            }
        }

        private static string GenerateOTP()
        {
            Random generator = new Random();
            return generator.Next(0, 1000000).ToString("D6");
        }

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
                System.Diagnostics.Trace.WriteLine("Lỗi gửi mail OTP: " + ex.ToString());
                return false;
            }
        }

        private static string GetClientIP()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_CF_CONNECTING_IP"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.UserHostAddress;
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
                    { "@TenDangNhap", username },
                    { "@IPAddress", clientIP }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_DangNhap", parameters);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    bool isSuccess = Convert.ToInt32(row["Success"]) == 1;
                    string message = row["Message"].ToString();

                    if (!isSuccess)
                    {
                        res.Success = false;
                        res.Message = message;
                        return res;
                    }

                    bool isLocked = row["IsLocked"] != DBNull.Value && Convert.ToBoolean(row["IsLocked"]);
                    byte trangThai = row["TrangThaiTK"] != DBNull.Value ? Convert.ToByte(row["TrangThaiTK"]) : (byte)0;

                    if (isLocked || trangThai == 0)
                    {
                        res.Success = false;
                        res.Message = "Tài khoản của bạn đang bị khóa hoặc ngưng hoạt động!";
                        return res;
                    }

                    string dbPasswordHash = row["MatKhauHash"].ToString();
                    bool isPasswordValid = libs.libs.VerifyPassword(password, dbPasswordHash);

                    object taiKhoanId = row["TaiKhoanID"];
                    object nhanVienId = row["NhanVienID"];

                    if (!isPasswordValid)
                    {
                        // Ghi log đăng nhập thất bại để phục vụ đếm số lần sai (2 tầng khóa)
                        var logPars = new Dictionary<string, object>
                        {
                            { "@TenDangNhap", username },
                            { "@IPAddress", clientIP },
                            { "@GhiChu", "Sai thông tin đăng nhập" }
                        };
                        db.ExecuteDatasetStoredProcedure("sp_v2_GhiLogDangNhapThatBai", logPars);
                        res.Success = false;
                        res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                        return res;
                    }else{
                        var logPars = new Dictionary<string, object>
                        {
                            { "@TenDangNhap", username },
                            { "@IPAddress", clientIP },
                            { "@GhiChu", "Tài khoản không tồn tại" }
                        };
                        db.ExecuteDatasetStoredProcedure("sp_v2_GhiLogDangNhapThatBai", logPars);

                        res.Success = false;
                        res.Message = "Tai khoản chưa kich hoạt!!";
                    }

                    string email = row["Email"] != DBNull.Value ? row["Email"].ToString() : "";
                    string hoTen = row["HoTen"] != DBNull.Value ? row["HoTen"].ToString() : username;

                    if (string.IsNullOrEmpty(email))
                    {
                        res.Success = false;
                        res.Message = "Tài khoản chưa được cấu hình Email nhận mã OTP. Vui lòng liên hệ Admin!";
                        return res;
                    }

                    string otpCode = GenerateOTP();

                    Dictionary<string, object> otpParams = new Dictionary<string, object>
                    {
                        { "@Action", "GENERATE" },
                        { "@TaiKhoanID", taiKhoanId },
                        { "@OTPCode", otpCode },
                        { "@IPAddress", clientIP }
                    };
                    db.ExecuteDatasetStoredProcedure("sp_chinh_TaiKhoan_OTP", otpParams);

                    bool sendMailOk = SendOTPEmail(email, hoTen, otpCode);
                    if (!sendMailOk)
                    {
                        res.Success = false;
                        res.Message = "Lỗi gửi email xác thực OTP. Vui lòng thử lại sau!";
                        return res;
                    }

                    // Lưu Session TẠM THỜI (theo đúng tên cột của schema v2)
                    HttpContext.Current.Session["Pending_TaiKhoanID"] = taiKhoanId;
                    HttpContext.Current.Session["Pending_TenDangNhap"] = row["TenDangNhap"];
                    HttpContext.Current.Session["Pending_NhanVienID"] = nhanVienId;
                    HttpContext.Current.Session["Pending_HoTen"] = hoTen;
                    HttpContext.Current.Session["Pending_Email"] = email;
                    HttpContext.Current.Session["Pending_CongTyID"] = row["CongTyID"];
                    HttpContext.Current.Session["Pending_PhongBanID"] = row["PhongBanChinhThucID"];
                    HttpContext.Current.Session["Pending_ChiNhanhID"] = row["ChiNhanhID"];
                    HttpContext.Current.Session["Pending_ChucVuID"] = row["ChucVuChinhThucID"];

                    res.Success = true;
                    res.Message = "Mã OTP đã được gửi về Email của bạn!";
                    res.RedirectUrl = "verify_login.aspx";
                }
                else
                {
                    res.Success = false;
                    res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Lỗi trong XuLyDangNhap WebMethod: " + ex.ToString());
                res.Success = false;
                res.Message = "Đã xảy ra lỗi hệ thống: " + ex.Message;
            }

            return res;
        }
    }
}