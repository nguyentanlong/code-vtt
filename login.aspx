<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="VTT.login" %>
    <!DOCTYPE html>
    <html lang="vi">

    <head runat="server">
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Đăng nhập - Hệ thống VTT-AI</title>

        <!-- Bootstrap 5 CSS -->
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
        <!-- FontAwesome Icon -->
        <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
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
                <div id="alertMessage" class="alert alert-danger d-none align-items-center" role="alert">
                    <i class="fa-solid fa-circle-exclamation me-2"></i>
                    <span id="alertText"></span>
                </div>

                <!-- Form Inputs -->
                <div class="mb-3">
                    <label for="txtUsername" class="form-label fw-semibold">Tên đăng nhập</label>
                    <div class="input-group">
                        <input type="text" class="form-control" id="txtUsername" placeholder="Nhập Username..."
                            autocomplete="username" required />
                    </div>
                </div>

                <div class="mb-3">
                    <label for="txtPassword" class="form-label fw-semibold">Mật khẩu</label>
                    <div class="input-group">
                        <input type="password" class="form-control password-field" id="txtPassword"
                            placeholder="Nhập Mật khẩu..." autocomplete="current-password" required />
                        <span class="input-group-text" id="togglePassword">
                            <i class="fa-regular fa-eye" id="eyeIcon"></i>
                        </span>
                    </div>
                </div>

                <!-- Submit Button -->
                <div class="d-grid gap-2 mt-4">
                    <button type="button" id="btnLogin" class="btn btn-primary">
                        <span id="btnSpinner" class="spinner-border spinner-border-sm me-2 d-none" role="status"
                            aria-hidden="true"></span>
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
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
        <script src="assets/js/login.js"></script>
    </body>

    </html>