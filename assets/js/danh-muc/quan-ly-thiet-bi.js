document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("quan-ly-thiet-bi.aspx/" + methodName, {
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
                var errorMsg = result ? result.message : "Thao tác thất bại!";
                showToast(errorMsg, "error");
                if (errorMsg && (errorMsg.includes("đăng nhập") || errorMsg.includes("hết hạn"))) {
                    var currentUrl = encodeURIComponent(window.location.href);
                    window.location.href = "../login.aspx?returnUrl=" + currentUrl;
                }
            }
        })
        .catch(err => {
            console.error("AJAX Error:", err);
            showToast("Lỗi kết nối máy chủ hoặc hệ thống không phản hồi!", "error");
        });
}

/*function loadData() {
    var myDeviceId = getOrCreateDeviceId();

    callWebMethod("GetList", { myDeviceId: myDeviceId }, function (res) {
        var tbody = document.getElementById("tbodyThietBi");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:#888;">Không có thiết bị nào đang hoạt động</td></tr>';
            return;
        }

        res.data.forEach(function (item) {
            var tenHienThi = escapeHtml(item.TenThietBi);
            if (item.LaThietBiHienTai) {
                tenHienThi += ' <span class="badge badge-success" style="margin-left:6px;">Thiết bị này</span>';
            }

            var actionHtml = item.LaThietBiHienTai
                ? '<span style="color:#94a3b8; font-size:12px;">Đang dùng</span>'
                : `<button type="button" class="btn-icon text-delete" onclick="deleteData(${item.ThietBiID})" title="Đăng xuất thiết bị này">
                       <i class="fa fa-right-from-bracket"></i> Đăng xuất
                   </button>`;

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${tenHienThi}</td>
                <td>${escapeHtml(item.IPDangNhap || '')}</td>
                <td>${item.LanDangNhapDauText}</td>
                <td>${item.LanHoatDongCuoiText}</td>
                <td style="text-align:center;">${actionHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}*/
function loadData() {
    var myDeviceId = getOrCreateDeviceId();

    callWebMethod("GetList", { myDeviceId: myDeviceId }, function (res) {
        var tbody = document.getElementById("tbodyThietBi");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:#888;">Không có thiết bị nào</td></tr>';
            return;
        }

        res.data.forEach(function (item) {
            var tenHienThi = escapeHtml(item.TenThietBi);
            if (item.LaThietBiHienTai) {
                tenHienThi += ' <span class="badge badge-success" style="margin-left:6px;">Thiết bị này</span>';
            }
            if (!item.DangHoatDong) {
                tenHienThi += ' <span class="badge" style="margin-left:6px; background:#f1f5f9; color:#64748b;">Đã đăng xuất</span>';
            }

            var actionHtml;
            if (item.LaThietBiHienTai) {
                actionHtml = '<span style="color:#94a3b8; font-size:12px;">Đang dùng</span>';
            } else if (item.DangHoatDong) {
                actionHtml = `<button type="button" class="btn-icon text-delete" onclick="deleteData(${item.ThietBiID})" title="Đăng xuất thiết bị này">
                                  <i class="fa fa-right-from-bracket"></i> Đăng xuất
                              </button>`;
            } else {
                // Thiết bị cũ, đã đăng xuất -> cho phép xóa hẳn (phục vụ trường hợp đổi máy)
                actionHtml = `<button type="button" class="btn-icon text-delete" onclick="xoaVinhVien(${item.ThietBiID})" title="Xóa vĩnh viễn khỏi danh sách">
                                  <i class="fa fa-trash"></i> Xóa
                              </button>`;
            }

            var tr = document.createElement("tr");
            tr.style.opacity = item.DangHoatDong ? "1" : "0.6";
            tr.innerHTML = `
                <td>${tenHienThi}</td>
                <td>${escapeHtml(item.IPDangNhap || '')}</td>
                <td>${item.LanDangNhapDauText}</td>
                <td>${item.LanHoatDongCuoiText}</td>
                <td style="text-align:center;">${actionHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function xoaVinhVien(id) {
    showConfirmDialog("Xóa vĩnh viễn thiết bị này khỏi danh sách? Hành động này không thể hoàn tác.").then(function (ok) {
        if (!ok) return;
        callWebMethod("XoaVinhVien", { thietBiId: id }, function (res) {
            showToast(res.message, "success");
            loadData();
        });
    });
}

function deleteData(id) {
    showConfirmDialog("Bạn có chắc chắn muốn đăng xuất thiết bị này? Thiết bị đó sẽ cần đăng nhập lại từ đầu.").then(function (ok) {
        if (!ok) return;
        callWebMethod("DeleteData", { thietBiId: id }, function (res) {
            showToast(res.message, "success");
            loadData();
        });
    });
}

function escapeHtml(text) {
    if (!text) return "";
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}