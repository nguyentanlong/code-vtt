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
    $('#btnLogin').click(function (e) {
        e.preventDefault(); // Chặn hành vi submit mặc định nếu nút nằm trong <form>
        doLogin();
    });

    // Nhấn Enter để Đăng nhập
    $('#txtUsername, #txtPassword').keypress(function (e) {
        if (e.which === 13) {
            e.preventDefault();
            e.stopPropagation();
            doLogin();
        }
    });
});

var isSubmittingLogin = false; // Cờ chặn gọi trùng lặp (double-submit)

function doLogin() {
    if (isSubmittingLogin) return; // Đang xử lý request trước đó -> bỏ qua request mới

    var username = $('#txtUsername').val().trim();
    var password = $('#txtPassword').val().trim();

    if (username === '' || password === '') {
        showAlert('Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!');
        return;
    }

    isSubmittingLogin = true;
    setLoading(true);
    hideAlert();

    $.ajax({
        type: "POST",
        url: "login.aspx/XuLyDangNhap",
        data: JSON.stringify({ username: username, password: password }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            setLoading(false);
            isSubmittingLogin = false;
            var result = response.d;

            if (result.Success) {
                window.location.href = result.RedirectUrl;
            } else {
                showAlert(result.Message);
            }
        },
        error: function (xhr, status, error) {
            setLoading(false);
            isSubmittingLogin = false;
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