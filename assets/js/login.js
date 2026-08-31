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

    $('#btnLogin').click(function (e) {
        e.preventDefault();
        doLogin();
    });

    $('#txtUsername, #txtPassword').keypress(function (e) {
        if (e.which === 13) {
            e.preventDefault();
            e.stopPropagation();
            doLogin();
        }
    });
});

var isSubmittingLogin = false;

function doLogin() {
    if (isSubmittingLogin) return;

    var username = $('#txtUsername').val().trim();
    var password = $('#txtPassword').val().trim();

    if (username === '' || password === '') {
        showAlert('Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!');
        return;
    }

    isSubmittingLogin = true;
    setLoading(true);
    hideAlert();

    var deviceId = getOrCreateDeviceId();
    var deviceName = getDeviceName();

    $.ajax({
        type: "POST",
        url: "login.aspx/XuLyDangNhap",
        data: JSON.stringify({ username: username, password: password, deviceId: deviceId, deviceName: deviceName }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            setLoading(false);
            isSubmittingLogin = false;
            var result = response.d;

            if (result.Success) {
                window.location.href = result.RedirectUrl;
            } else if (result.ThietBiList && result.ThietBiList.length > 0) {
                // Bị chặn do đủ 3 thiết bị -> hiện danh sách để chọn đăng xuất bớt
                showDeviceLimitDialog(result.Message, result.ThietBiList, username, password);
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

/*function showDeviceLimitDialog(message, thietBiList, username, password) {
    var listHtml = thietBiList.map(function (tb) {
        return `
            <div style="display:flex; justify-content:space-between; align-items:center; padding:10px 0; border-bottom:1px solid #eee;">
                <div>
                    <div style="font-weight:600; font-size:13.5px;">${tb.TenThietBi || 'Thiết bị không rõ'}</div>
                    <div style="font-size:12px; color:#888;">Hoạt động cuối: ${tb.LanHoatDongCuoiText}</div>
                </div>
                <button type="button" class="ui-dialog-btn ui-dialog-btn-danger" style="padding:5px 12px; font-size:12.5px;"
                    onclick="dangXuatThietBiVaThuLai(${tb.ThietBiID}, '${username.replace(/'/g, "\\'")}', '${password.replace(/'/g, "\\'")}', this)">
                    Đăng xuất
                </button>
            </div>
        `;
    }).join('');

    var backdrop = document.createElement("div");
    backdrop.className = "ui-dialog-backdrop";
    backdrop.id = "deviceLimitBackdrop";
    backdrop.innerHTML = `
        <div class="ui-dialog-box" style="width:420px;">
            <div class="ui-dialog-icon warning">!</div>
            <div class="ui-dialog-message">${message}</div>
            <div id="deviceLimitList">${listHtml}</div>
            <div class="ui-dialog-actions" style="margin-top:14px;">
                <button type="button" class="ui-dialog-btn ui-dialog-btn-secondary" onclick="closeDeviceLimitDialog()">Đóng</button>
            </div>
        </div>
    `;
    document.body.appendChild(backdrop);
    requestAnimationFrame(function () { backdrop.classList.add("show"); });
}*/
function showDeviceLimitDialog(message, thietBiList, username, password) {
    var listHtml = thietBiList.map(function (tb) {
        return `
            <div class="device-item">
                <div class="device-item-icon"><i class="fa-solid fa-display"></i></div>
                <div class="device-item-info">
                    <div class="device-item-name">${tb.TenThietBi || 'Thiết bị không rõ'}</div>
                    <div class="device-item-time">Hoạt động cuối: ${tb.LanHoatDongCuoiText}</div>
                </div>
                <button type="button" class="device-item-btn"
                    onclick="dangXuatThietBiVaThuLai(${tb.ThietBiID}, '${username.replace(/'/g, "\\'")}', '${password.replace(/'/g, "\\'")}', this)">
                    Đăng xuất
                </button>
            </div>
        `;
    }).join('');

    var backdrop = document.createElement("div");
    backdrop.className = "ui-dialog-backdrop";
    backdrop.id = "deviceLimitBackdrop";
    backdrop.innerHTML = `
        <div class="ui-dialog-box" style="width:420px;">
            <div class="ui-dialog-icon warning">!</div>
            <div class="ui-dialog-message">${message}</div>
            <div class="device-limit-list">${listHtml}</div>
            <div class="ui-dialog-actions" style="margin-top:14px;">
                <button type="button" class="ui-dialog-btn ui-dialog-btn-secondary" onclick="closeDeviceLimitDialog()">Đóng</button>
            </div>
        </div>
    `;
    document.body.appendChild(backdrop);
    requestAnimationFrame(function () { backdrop.classList.add("show"); });
}

function closeDeviceLimitDialog() {
    var backdrop = document.getElementById("deviceLimitBackdrop");
    if (backdrop) {
        backdrop.classList.remove("show");
        setTimeout(function () { backdrop.remove(); }, 180);
    }
}

function dangXuatThietBiVaThuLai(thietBiId, username, password, btnEl) {
    btnEl.disabled = true;
    btnEl.textContent = "Đang xử lý...";

    $.ajax({
        type: "POST",
        url: "login.aspx/ThuHoiThietBiTruocDangNhap",
        data: JSON.stringify({ thietBiId: thietBiId, username: username }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var result = response.d;
            if (result.Success) {
                closeDeviceLimitDialog();
                showToast("Đã đăng xuất thiết bị cũ, đang đăng nhập lại...", "success");
                setTimeout(function () { doLogin(); }, 500);
            } else {
                showToast(result.Message, "error");
                btnEl.disabled = false;
                btnEl.textContent = "Đăng xuất";
            }
        },
        error: function () {
            showToast("Lỗi kết nối máy chủ!", "error");
            btnEl.disabled = false;
            btnEl.textContent = "Đăng xuất";
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