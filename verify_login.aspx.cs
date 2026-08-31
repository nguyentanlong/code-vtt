using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using VTT.libs;

namespace VTT
{
    public partial class verify_login : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(verify_login));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Pending_TaiKhoanID"] == null)
                {
                    Response.Redirect("~/login.aspx");
                    return;
                }

                string email = Session["Pending_Email"] != null ? Session["Pending_Email"].ToString() : "";
                if (!string.IsNullOrEmpty(email) && email.Contains("@"))
                {
                    string[] parts = email.Split('@');
                    lblEmail.InnerText = parts[0].Substring(0, Math.Min(2, parts[0].Length)) + "***@" + parts[1];
                }
            }
        }

        [WebMethod(EnableSession = true)]
        public static LoginResponse XacNhanOTP(string otp)
        {
            LoginResponse res = new LoginResponse();
            ConnectServer db = new ConnectServer();

            if (HttpContext.Current.Session["Pending_TaiKhoanID"] == null)
            {
                res.Success = false;
                res.Message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!";
                return res;
            }

            long taiKhoanId = Convert.ToInt64(HttpContext.Current.Session["Pending_TaiKhoanID"]);

            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_TaiKhoan_OTP", new Dictionary<string, object> {
                { "@Action", "VERIFY" },
                { "@TaiKhoanID", taiKhoanId },
                { "@OTPCode", otp }
            });

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                bool isSuccess = Convert.ToInt32(row["Success"]) == 1;

                if (isSuccess)
                {
                    // 1. Chuyển từ Session Temp sang Session chính thức (theo đúng tên cột schema v2)
                    HttpContext.Current.Session["TaiKhoanID"] = taiKhoanId;
                    HttpContext.Current.Session["NhanVienID"] = HttpContext.Current.Session["Pending_NhanVienID"];
                    HttpContext.Current.Session["TenDangNhap"] = HttpContext.Current.Session["Pending_TenDangNhap"];
                    HttpContext.Current.Session["HoTen"] = HttpContext.Current.Session["Pending_HoTen"];
                    HttpContext.Current.Session["Email"] = HttpContext.Current.Session["Pending_Email"];
                    HttpContext.Current.Session["CongTyID"] = HttpContext.Current.Session["Pending_CongTyID"];
                    HttpContext.Current.Session["PhongBanID"] = HttpContext.Current.Session["Pending_PhongBanID"];
                    HttpContext.Current.Session["ChiNhanhID"] = HttpContext.Current.Session["Pending_ChiNhanhID"];
                    HttpContext.Current.Session["ChucVuID"] = HttpContext.Current.Session["Pending_ChucVuID"];
                    HttpContext.Current.Session["DeviceID"] = HttpContext.Current.Session["Pending_DeviceID"];

                    // 2. Dọn dẹp toàn bộ Temp Session
                    HttpContext.Current.Session.Remove("Pending_TaiKhoanID");
                    HttpContext.Current.Session.Remove("Pending_TenDangNhap");
                    HttpContext.Current.Session.Remove("Pending_NhanVienID");
                    HttpContext.Current.Session.Remove("Pending_HoTen");
                    HttpContext.Current.Session.Remove("Pending_Email");
                    HttpContext.Current.Session.Remove("Pending_CongTyID");
                    HttpContext.Current.Session.Remove("Pending_PhongBanID");
                    HttpContext.Current.Session.Remove("Pending_ChiNhanhID");
                    HttpContext.Current.Session.Remove("Pending_ChucVuID");
                    HttpContext.Current.Session.Remove("Pending_DeviceID");

                    res.Success = true;
                    res.Message = "Xác thực thành công!";
                    res.RedirectUrl = "child.aspx";
                }
                else
                {
                    res.Success = false;
                    res.Message = row["Message"].ToString();
                }
            }

            return res;
        }

        [WebMethod(EnableSession = true)]
        public static LoginResponse GuiLaiOTP()
        {
            LoginResponse res = new LoginResponse();
            ConnectServer db = new ConnectServer();

            if (HttpContext.Current.Session["Pending_TaiKhoanID"] == null)
            {
                res.Success = false;
                res.Message = "Phiên làm việc đã hết hạn. Vui lòng quay lại đăng nhập!";
                return res;
            }

            try
            {
                long taiKhoanId = Convert.ToInt64(HttpContext.Current.Session["Pending_TaiKhoanID"]);
                string email = HttpContext.Current.Session["Pending_Email"].ToString();
                string hoTen = HttpContext.Current.Session["Pending_HoTen"].ToString();
                string clientIP = HttpContext.Current.Request.UserHostAddress;

                Random generator = new Random();
                string otpCode = generator.Next(0, 1000000).ToString("D6");

                Dictionary<string, object> otpParams = new Dictionary<string, object>
                {
                    { "@Action", "GENERATE" },
                    { "@TaiKhoanID", taiKhoanId },
                    { "@OTPCode", otpCode },
                    { "@IPAddress", clientIP }
                };
                db.ExecuteDatasetStoredProcedure("sp_chinh_TaiKhoan_OTP", otpParams);

                string subject = $"[{otpCode}] Mã xác thực đăng nhập hệ thống VTT (Mã mới)";
                string body = $"Xin chào <b>{hoTen}</b>,<br/>Mã OTP mới của bạn là: <b style='font-size:20px; color:#2563eb;'>{otpCode}</b> (Hiệu lực 5 phút).";

                using (SmtpClient smtp = new SmtpClient())
                using (MailMessage mail = new MailMessage())
                {
                    mail.To.Add(email);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    smtp.Send(mail);
                }

                res.Success = true;
                res.Message = "Mã OTP mới đã được gửi thành công!";
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = "Lỗi gửi lại mã OTP: " + ex.Message;
            }

            return res;
        }
    }
}