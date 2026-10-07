document.addEventListener("DOMContentLoaded", function () {
    loadThongTin();
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("thong-tin-tai-khoan.aspx/" + methodName, {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(dataObj || {})
    })
        .then(response => response.json())
        .then(res => {
            var result = res.d;
            if (result && result.success) {
                successCallback(result);
            } else {
                showToast(result ? result.message : "Thao tác thất bại!", "error");
            }
        })
        .catch(function () { showToast("Lỗi kết nối máy chủ!", "error"); });
}

function loadThongTin() {
    callWebMethod("GetThongTin", {}, function (res) {
        var d = res.data;
        document.getElementById("txtTenDangNhap").value = d.TenDangNhap;
        document.getElementById("txtHoTen").value = d.HoTen;
        document.getElementById("txtMaNhanVien").value = d.MaNhanVien;
        document.getElementById("txtPhongBan").value = d.TenPhongBan;
        document.getElementById("txtChiNhanh").value = d.TenChiNhanh;
        document.getElementById("txtVaiTro").value = d.TenVaiTro;
        document.getElementById("txtSoDienThoai").value = d.SoDienThoai;
        document.getElementById("txtEmail").value = d.Email;
    });
}

function luuLienHe() {
    var soDienThoai = document.getElementById("txtSoDienThoai").value.trim();
    var email = document.getElementById("txtEmail").value.trim();

    callWebMethod("CapNhatLienHe", { soDienThoai: soDienThoai, email: email }, function (res) {
        showToast(res.message, "success");
    });
}

function doiMatKhau() {
    var matKhauCu = document.getElementById("txtMatKhauCu").value;
    var matKhauMoi = document.getElementById("txtMatKhauMoi").value;
    var xacNhan = document.getElementById("txtXacNhanMatKhau").value;

    if (!matKhauCu) { showToast("Vui lòng nhập mật khẩu hiện tại!", "error"); return; }
    if (!matKhauMoi || matKhauMoi.length < 6) { showToast("Mật khẩu mới phải có ít nhất 6 ký tự!", "error"); return; }
    if (matKhauMoi !== xacNhan) { showToast("Xác nhận mật khẩu mới không khớp!", "error"); return; }

    callWebMethod("DoiMatKhau", { matKhauCu: matKhauCu, matKhauMoi: matKhauMoi }, function (res) {
        showToast(res.message, "success");
        document.getElementById("txtMatKhauCu").value = "";
        document.getElementById("txtMatKhauMoi").value = "";
        document.getElementById("txtXacNhanMatKhau").value = "";
    });
}