// 1. Khai báo thời gian
var otpTimeLeft = 300;     // 5 phút hiệu lực mã OTP
var resendTimeLeft = 60;   // 60 giây chờ để bấm Gửi lại mã

var otpInterval;
var resendInterval;

$(document).ready(function () {
    startOtpTimer();
    startResendTimer();

    $('#btnVerify').click(function () { doVerify(); });
    $('#txtOTP').keypress(function (e) { if (e.which === 13) { e.preventDefault(); doVerify(); } });

    // Xử lý sự kiện bấm Nút Gửi lại mã
    $('#btnResend').click(function () {
        if ($(this).is(':disabled')) return;

        var $btn = $(this);
        $btn.prop('disabled', true).text('Đang gửi...');

        $.ajax({
            type: "POST",
            url: "verify_login.aspx/GuiLaiOTP",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var res = response.d;
                if (res.Success) {
                    $('#alertMsg').removeClass('alert-danger').addClass('alert-success').text(res.Message).removeClass('d-none');

                    // Reset lại ô nhập
                    $('#txtOTP').prop('disabled', false).val('');
                    $('#btnVerify').prop('disabled', false);

                    // Reset bộ đếm 5 phút & 60s
                    clearInterval(otpInterval);
                    clearInterval(resendInterval);

                    otpTimeLeft = 300;
                    resendTimeLeft = 60;

                    startOtpTimer();
                    startResendTimer();
                } else {
                    $('#alertMsg').removeClass('alert-success').addClass('alert-danger').text(res.Message).removeClass('d-none');
                    $btn.prop('disabled', false).text('Gửi lại mã ngay');
                }
            },
            error: function () {
                $('#alertMsg').removeClass('alert-success').addClass('alert-danger').text('Lỗi kết nối máy chủ!').removeClass('d-none');
                $btn.prop('disabled', false).text('Gửi lại mã ngay');
            }
        });
    });
});

// Đếm ngược 5 phút cho thời hạn Mã OTP
function startOtpTimer() {
    otpInterval = setInterval(function () {
        if (otpTimeLeft <= 0) {
            clearInterval(otpInterval);
            $('#otpTimer').text("Đã hết hạn!");
            $('#txtOTP').prop('disabled', true);
            $('#btnVerify').prop('disabled', true);
            $('#alertMsg').removeClass('alert-success').addClass('alert-danger')
                .text('Mã OTP đã hết hạn. Vui lòng bấm "Gửi lại mã"!').removeClass('d-none');
        } else {
            var minutes = Math.floor(otpTimeLeft / 60);
            var seconds = otpTimeLeft % 60;
            minutes = minutes < 10 ? '0' + minutes : minutes;
            seconds = seconds < 10 ? '0' + seconds : seconds;
            $('#otpTimer').text(minutes + ':' + seconds);
            otpTimeLeft--;
        }
    }, 1000);
}

// Đếm ngược 60s chờ để được phép bấm nút Resend
function startResendTimer() {
    var $btn = $('#btnResend');
    $btn.prop('disabled', true);

    resendInterval = setInterval(function () {
        if (resendTimeLeft <= 0) {
            clearInterval(resendInterval);
            $btn.prop('disabled', false).text('Gửi lại mã ngay');
        } else {
            $btn.html('Gửi lại mã (<span id="resendTimer">' + resendTimeLeft + '</span>s)');
            resendTimeLeft--;
        }
    }, 1000);
}

function doVerify() {
    var otp = $('#txtOTP').val().trim();
    if (otp.length !== 6) {
        $('#alertMsg').removeClass('alert-success').addClass('alert-danger').text('Vui lòng nhập đủ 6 chữ số OTP!').removeClass('d-none');
        return;
    }

    $.ajax({
        type: "POST",
        url: "verify_login.aspx/XacNhanOTP",
        data: JSON.stringify({ otp: otp }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.Success) {
                window.location.href = res.RedirectUrl;
            } else {
                $('#alertMsg').removeClass('alert-success').addClass('alert-danger').text(res.Message).removeClass('d-none');
            }
        }
    });
}