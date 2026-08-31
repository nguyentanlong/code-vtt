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
        public static LoginResponse XuLyDangNhap(string username, string password, string deviceId, string deviceName)
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
                    }

                    // --- KIỂM TRA GIỚI HẠN THIẾT BỊ (tối đa 3 thiết bị đang hoạt động) ---
                    var thietBiPars = new Dictionary<string, object>
                    {
                        { "@TaiKhoanID", taiKhoanId },
                        { "@DeviceID", string.IsNullOrEmpty(deviceId) ? "unknown_" + Guid.NewGuid().ToString("N") : deviceId },
                        { "@TenThietBi", string.IsNullOrEmpty(deviceName) ? "Thiết bị không rõ" : deviceName },
                        { "@IPAddress", clientIP }
                    };
                    DataSet dsThietBi = db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_KiemTraDangNhap", thietBiPars);

                    bool choPhepThietBi = dsThietBi.Tables.Count > 0 && dsThietBi.Tables[0].Rows.Count > 0
                                        && Convert.ToInt32(dsThietBi.Tables[0].Rows[0]["ChoPhep"]) == 1;

                    if (!choPhepThietBi)
                    {
                        string thietBiMessage = dsThietBi.Tables[0].Rows[0]["Message"].ToString();

                        // Lấy danh sách 3 thiết bị đang hoạt động để trả về cho client chọn đăng xuất bớt
                        var listPars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId } };
                        DataSet dsList = db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_GetList", listPars);

                        var thietBiList = new List<object>();
                        foreach (DataRow tbRow in dsList.Tables[0].Rows)
                        {
                            thietBiList.Add(new
                            {
                                ThietBiID = tbRow["ThietBiID"],
                                TenThietBi = tbRow["TenThietBi"].ToString(),
                                LanHoatDongCuoiText = Convert.ToDateTime(tbRow["LanHoatDongCuoi"]).ToString("dd/MM/yyyy HH:mm")
                            });
                        }

                        res.Success = false;
                        res.Message = thietBiMessage;
                        res.ThietBiList = thietBiList;
                        return res;
                    }
                    // --- HẾT KIỂM TRA THIẾT BỊ ---

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

                    HttpContext.Current.Session["Pending_TaiKhoanID"] = taiKhoanId;
                    HttpContext.Current.Session["Pending_TenDangNhap"] = row["TenDangNhap"];
                    HttpContext.Current.Session["Pending_NhanVienID"] = nhanVienId;
                    HttpContext.Current.Session["Pending_HoTen"] = hoTen;
                    HttpContext.Current.Session["Pending_Email"] = email;
                    HttpContext.Current.Session["Pending_CongTyID"] = row["CongTyID"];
                    HttpContext.Current.Session["Pending_PhongBanID"] = row["PhongBanChinhThucID"];
                    HttpContext.Current.Session["Pending_ChiNhanhID"] = row["ChiNhanhID"];
                    HttpContext.Current.Session["Pending_ChucVuID"] = row["ChucVuChinhThucID"];
                    HttpContext.Current.Session["Pending_DeviceID"] = deviceId;

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

        /// <summary>
        /// Cho phép người dùng đăng xuất 1 thiết bị cũ ngay tại màn hình bị chặn (chưa cần đăng nhập lại từ đầu),
        /// rồi client tự động gọi lại XuLyDangNhap sau khi thu hồi thành công.
        /// </summary>
        [WebMethod(EnableSession = true)]
        public static LoginResponse ThuHoiThietBiTruocDangNhap(long thietBiId, string username)
        {
            LoginResponse res = new LoginResponse();
            ConnectServer db = new ConnectServer();

            try
            {
                // Tra TaiKhoanID từ Username để đảm bảo chỉ thu hồi đúng thiết bị của chính tài khoản đang thử đăng nhập
                var lookupPars = new Dictionary<string, object> { { "@TenDangNhap", username }, { "@IPAddress", GetClientIP() } };
                DataSet dsLookup = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_DangNhap", lookupPars);

                if (dsLookup.Tables.Count == 0 || dsLookup.Tables[0].Rows.Count == 0)
                {
                    res.Success = false;
                    res.Message = "Không xác định được tài khoản!";
                    return res;
                }

                long taiKhoanId = Convert.ToInt64(dsLookup.Tables[0].Rows[0]["TaiKhoanID"]);

                var pars = new Dictionary<string, object>
                {
                    { "@ThietBiID", thietBiId },
                    { "@TaiKhoanID", taiKhoanId }
                };
                db.ExecuteDatasetStoredProcedure("sp_v2_ThietBi_ThuHoi", pars);

                res.Success = true;
                res.Message = "Đã đăng xuất thiết bị cũ.";
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = "Lỗi: " + ex.Message;
            }

            return res;
        }
    }
}