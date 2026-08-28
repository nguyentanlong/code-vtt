$(document).ready(function () {
    // Ẩn/hiện mật khẩu
    $('#togglePassword').click(function () {
        var passInput = $('#txtPassword');
        var eyeIcon = $('#eyeIcon');

        if (passInput.attr('type') === 'password') {
            passInput.attr('type', 'text');
            eyeIcon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passInput.attr('type', 'password');
            eyeIcon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    // Lắng nghe sự kiện click nút Đăng nhập
    $('#btnLogin').click(function () {
        doLogin();
    });

    // Nhấn Enter để Đăng nhập
    $('#txtUsername, #txtPassword').keypress(function (e) {
        if (e.which === 13) {
            e.preventDefault();
            doLogin();
        }
    });
});

function doLogin() {
    var username = $('#txtUsername').val().trim();
    var password = $('#txtPassword').val().trim();

    if (username === '' || password === '') {
        showAlert('Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!');
        return;
    }

    setLoading(true);
    hideAlert();

    // Gọi AJAX tới C# WebMethod
    $.ajax({
        type: "POST",
        url: "login.aspx/XuLyDangNhap",
        data: JSON.stringify({ username: username, password: password }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            setLoading(false);
            var result = response.d;

            if (result.Success) {
                // Đăng nhập thành công -> Chuyển hướng
                window.location.href = result.RedirectUrl;
            } else {
                // Hiển thị thông báo lỗi (Bao gồm cảnh báo sai mật khẩu hoặc bị Khóa IP)
                showAlert(result.Message);
            }
        },
        error: function (xhr, status, error) {
            setLoading(false);
            showAlert('Lỗi kết nối máy chủ! Vui lòng thử lại sau.');
            console.error(xhr.responseText);
        }
    });
}

function showAlert(msg) {
    $('#alertText').text(msg);
    $('#alertMessage').removeClass('d-none').addClass('d-flex');
}

function hideAlert() {
    $('#alertMessage').addClass('d-none').removeClass('d-flex');
}

function setLoading(isLoading) {
    if (isLoading) {
        $('#btnLogin').prop('disabled', true);
        $('#btnSpinner').removeClass('d-none');
        $('#btnText').text('Đang xác thực...');
    } else {
        $('#btnLogin').prop('disabled', false);
        $('#btnSpinner').addClass('d-none');
        $('#btnText').text('Đăng nhập');
    }
}