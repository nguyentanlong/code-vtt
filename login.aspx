<%@ Page Language="C#" AutoEventWireup="true" %>
    <%@ Import Namespace="System.Data" %>
        <%@ Import Namespace="System.Collections.Generic" %>
            <%@ Import Namespace="System.Net.Mail" %>
                <%@ Import Namespace="System.Web.Services" %>
                    <%@ Import Namespace="VTT.libs" %>

                        <!DOCTYPE html>
                        <script runat="server">
        private static string GenerateOTP()
                            {
            Random generator = new Random();
                                return generator.Next(0, 1000000).ToString("D6");
                            }

        private static bool SendOTPEmail(string toEmail, string hoTen, string otpCode)
                            {
                                try {
                string subject = $"[{otpCode}] Mã xác thực đăng nhập hệ thống VTT";
                string body = $@"<div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f4f4;'><div style='max-width: 500px; margin: 0 auto; background: #ffffff; padding: 20px; border-radius: 8px;'><h2 style='color: #333;'>Mã Xác Thực Đăng Nhập</h2><p>Xin chào <b>{hoTen}</b>,</p><p>Mã OTP xác thực đăng nhập của bạn là:</p><div style='text-align: center; margin: 20px 0;'><span style='font-size: 28px; font-weight: bold; letter-spacing: 5px; color: #2563eb; background: #eff6ff; padding: 10px 20px; border-radius: 6px; display: inline-block;'>{otpCode}</span></div><p style='color: #666; font-size: 13px;'>Mã này có hiệu lực trong <b>5 phút</b>. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p></div></div>";

                                    using(SmtpClient smtp = new SmtpClient())
                                    using(MailMessage mail = new MailMessage())
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
                                var ctx = System.Web.HttpContext.Current;
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
                                try {
                string clientIP = GetClientIP();

                                    Dictionary < string, object > parameters = new Dictionary < string, object >
                                        {
                    { "@Username", username },
                                    { "@IPAddress", clientIP }
                                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("dbo.sp_chinh_TaiKhoan_DangNhap", parameters);

                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {
                                    var row = ds.Tables[0].Rows[0];
                    bool isSuccess = Convert.ToInt32(row["Success"]) == 1;
                    string message = row["Message"].ToString();

                                    if (!isSuccess) {
                                        res.Success = false; res.Message = message; return res;
                                    }

                    bool isLocked = row.Table.Columns.Contains("IsLocked") && row["IsLocked"] != DBNull.Value && Convert.ToBoolean(row["IsLocked"]);
                    byte trangThai = row.Table.Columns.Contains("TrangThaiTK") && row["TrangThaiTK"] != DBNull.Value ? Convert.ToByte(row["TrangThaiTK"]) : (byte)0;

                                    if (isLocked || trangThai == 0) {
                                        res.Success = false; res.Message = "Tài khoản của bạn đang bị khóa hoặc ngưng hoạt động!"; return res;
                                    }

                    string dbPasswordHash = row.Table.Columns.Contains("PasswordHash") ? row["PasswordHash"].ToString() : string.Empty;
                    bool isPasswordValid = libs.VerifyPassword(password, dbPasswordHash);

                    object taiKhoanId = row.Table.Columns.Contains("TaiKhoanID") ? row["TaiKhoanID"] : null;
                    object nhanVienId = row.Table.Columns.Contains("NhanVienID") && row["NhanVienID"] != DBNull.Value ? row["NhanVienID"] : null;

                                    if (!isPasswordValid) {
                                        Process.GhiNhatKyDangNhap(db, "LOGIN_FAILED", username, taiKhoanId, nhanVienId, clientIP, "Sai mật khẩu");
                                        res.Success = false; res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác!"; return res;
                                    }

                    string email = row.Table.Columns.Contains("EmailCongTy") && row["EmailCongTy"] != DBNull.Value ? row["EmailCongTy"].ToString() : "";
                    string hoTen = row.Table.Columns.Contains("HoTen") && row["HoTen"] != DBNull.Value ? row["HoTen"].ToString() : username;

                                    if (string.IsNullOrEmpty(email)) { res.Success = false; res.Message = "Tài khoản chưa được cấu hình Email nhận mã OTP. Vui lòng liên hệ Admin!"; return res; }

                    string otpCode = GenerateOTP();

                                    Dictionary < string, object > otpParams = new Dictionary < string, object >
                                        {
                        { "@Action", "GENERATE" },
                                    { "@TaiKhoanID", taiKhoanId },
                                    { "@OTPCode", otpCode },
                                    { "@IPAddress", clientIP }
                                };
                                db.ExecuteDatasetStoredProcedure("dbo.sp_chinh_TaiKhoan_OTP", otpParams);

                    bool sendMailOk = SendOTPEmail(email, hoTen, otpCode);
                                if (!sendMailOk) { res.Success = false; res.Message = "Lỗi gửi email xác thực OTP. Vui lòng thử lại sau!"; return res; }

                                System.Web.HttpContext.Current.Session["Pending_TaiKhoanID"] = taiKhoanId;
                                System.Web.HttpContext.Current.Session["Pending_Username"] = row.Table.Columns.Contains("Username") ? row["Username"] : username;
                                System.Web.HttpContext.Current.Session["Pending_NhanVienID"] = nhanVienId;
                                System.Web.HttpContext.Current.Session["Pending_HoTen"] = hoTen;
                                System.Web.HttpContext.Current.Session["Pending_Email"] = email;
                                System.Web.HttpContext.Current.Session["Pending_CongTyID"] = row.Table.Columns.Contains("CongTyID") ? row["CongTyID"] : null;
                                System.Web.HttpContext.Current.Session["Pending_PhongBanID"] = row.Table.Columns.Contains("PhongBanID") ? row["PhongBanID"] : null;
                                System.Web.HttpContext.Current.Session["Pending_ChucDanhID"] = row.Table.Columns.Contains("ChucDanhID") ? row["ChucDanhID"] : null;

                                res.Success = true; res.Message = "Mã OTP đã được gửi về Email của bạn!"; res.RedirectUrl = "verify_login.aspx";
                            }
                else
                            {
                                Process.GhiNhatKyDangNhap(db, "LOGIN_FAILED", username, null, null, clientIP, "Tài khoản không tồn tại");
                                res.Success = false; res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                            }
            }
            catch (Exception ex)
                            {
                                System.Diagnostics.Trace.WriteLine("Lỗi trong XuLyDangNhap WebMethod: " + ex.ToString());
                                res.Success = false; res.Message = "Đã xảy ra lỗi hệ thống: " + ex.Message;
                            }

                            return res;
        }
                        </script>
                        <html lang="vi">

                        <head runat="server">
                            <meta charset="utf-8" />
                            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                            <title>Đăng nhập - Hệ thống VTT-AI</title>

                            <!-- Bootstrap 5 CSS -->
                            <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"
                                rel="stylesheet" />
                            <!-- FontAwesome Icon -->
                            <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"
                                rel="stylesheet" />
                            <link href="assets/css/login.css" rel="stylesheet" />
                        </head>

                        <body>
                            <form id="form1" runat="server">
                                <div class="login-card">
                                    <!-- Header Logo/Title -->
                                    <div class="text-center mb-4">
                                        <div class="mb-2">
                                            <i class="fa-solid fa-shield-halved fa-3x text-primary"></i>
                                        </div>
                                        <h3 class="brand-title">VTT-AI SYSTEM</h3>
                                        <p class="text-muted small">Đăng nhập cổng quản trị thông tin</p>
                                    </div>

                                    <!-- Khung thông báo lỗi / Khóa IP -->
                                    <div id="alertMessage" class="alert alert-danger d-none align-items-center"
                                        role="alert">
                                        <i class="fa-solid fa-circle-exclamation me-2"></i>
                                        <span id="alertText"></span>
                                    </div>

                                    <!-- Form Inputs -->
                                    <div class="mb-3">
                                        <label for="txtUsername" class="form-label fw-semibold">Tên đăng nhập</label>
                                        <div class="input-group">
                                            <input type="text" class="form-control" id="txtUsername"
                                                placeholder="Nhập Username..." autocomplete="username" required />
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <label for="txtPassword" class="form-label fw-semibold">Mật khẩu</label>
                                        <div class="input-group">
                                            <input type="password" class="form-control password-field" id="txtPassword"
                                                placeholder="Nhập Mật khẩu..." autocomplete="current-password"
                                                required />
                                            <span class="input-group-text" id="togglePassword">
                                                <i class="fa-regular fa-eye" id="eyeIcon"></i>
                                            </span>
                                        </div>
                                    </div>

                                    <!-- Submit Button -->
                                    <div class="d-grid gap-2 mt-4">
                                        <button type="button" id="btnLogin" class="btn btn-primary">
                                            <span id="btnSpinner" class="spinner-border spinner-border-sm me-2 d-none"
                                                role="status" aria-hidden="true"></span>
                                            <span id="btnText">Đăng nhập</span>
                                        </button>
                                    </div>

                                    <div class="text-center mt-4">
                                        <small class="text-muted">&copy; 2026 VTT System. All rights reserved.</small>
                                    </div>
                                </div>
                            </form>

                            <!-- jQuery & Bootstrap JS -->
                            <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
                            <script
                                src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
                            <script src="assets/js/login.js"></script>
                        </body>

                        </html>