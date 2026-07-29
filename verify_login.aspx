<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="verify_login.aspx.cs" Inherits="VTT.verify_login" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>Xác minh OTP - VTT System</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/css/verify_login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="otp-card text-center">
            <h4 class="fw-bold mb-2">Xác minh OTP</h4>
            <p class="text-muted small mb-3">Mã xác thực đã được gửi tới email:<br/><b id="lblEmail" runat="server">***@gmail.com</b></p>

            <div id="alertMsg" class="alert alert-danger d-none" role="alert"></div>

            <!-- Đếm ngược hiệu lực mã OTP (5 phút) -->
            <div class="mb-3">
                <span class="timer-text">Mã OTP hết hạn sau: <span id="otpTimer">05:00</span></span>
            </div>

            <div class="mb-3">
                <input type="text" id="txtOTP" class="form-control otp-input" maxlength="6" placeholder="******" autocomplete="off" />
            </div>

            <button type="button" id="btnVerify" class="btn btn-primary w-100 py-2 fw-semibold mb-3">Xác thực & Đăng nhập</button>
            
            <!-- Khu vực Gửi lại mã kèm đếm ngược 60s -->
            <div class="resend-box mb-2">
                <span>Chưa nhận được mã? </span>
                <button type="button" id="btnResend" class="btn btn-link p-0 text-decoration-none fw-semibold" disabled>
                    Gửi lại mã (<span id="resendTimer">60</span>s)
                </button>
            </div>
            
            <div class="mt-3">
                <a href="login.aspx" class="text-decoration-none small text-muted">← Quay lại Đăng nhập</a>
            </div>
        </div>
    </form>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="assets/js/verify_login.js"></script>
</body>
</html>