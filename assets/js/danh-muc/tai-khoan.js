var canThemPermission = false;
var currentPageTaiKhoan = 1;

document.addEventListener("DOMContentLoaded", function () {
    Promise.all([loadPermission(), loadChiNhanhOptions()]).then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("tai-khoan.aspx/" + methodName, {
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

function loadPermission() {
    return callWebMethod("GetPermission", {}, function (res) {
        canThemPermission = res.data.canThem;
        document.getElementById("btnAddNew").style.display = canThemPermission ? "inline-flex" : "none";
        applyScopeFilterVisibility(res.data.scope, "ddlSearchChiNhanh", null, res.data.myChiNhanhId, null);
    });
}

function loadChiNhanhOptions() {
    return callWebMethod("GetChiNhanhOptions", {}, function (res) {
        var ddl = document.getElementById("ddlSearchChiNhanh");
        ddl.innerHTML = '<option value="">-- Tất cả chi nhánh --</option>';
        (res.data || []).forEach(function (cn) {
            ddl.innerHTML += `<option value="${cn.ChiNhanhID}">${escapeHtml(cn.TenChiNhanh)}</option>`;
        });
    });
}

function loadData(pageNumber) {
    if (pageNumber) currentPageTaiKhoan = pageNumber;
    var keyword = document.getElementById("txtSearchKeyword").value;
    var chiNhanhId = document.getElementById("ddlSearchChiNhanh").value;

    callWebMethod("GetList", { keyword: keyword, chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null, pageNumber: currentPageTaiKhoan }, function (res) {
        var tbody = document.getElementById("tbodyTaiKhoan");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
        } else {
            res.data.forEach(function (item) {
                var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
                var statusText = item.IsLocked ? "Đã khóa" : (item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động");

                var actionHtml = "";
                if (item.LaChinhMinh) {
                    actionHtml = '<span style="color:#94a3b8; font-size:12px;">Tài khoản của bạn</span>';
                } else if (item.CanToggle) {
                    actionHtml = item.IsLocked
                        ? `<button type="button" class="btn-icon text-edit" onclick="toggleLock(${item.TaiKhoanID}, false)">Mở khóa</button>`
                        : `<button type="button" class="btn-icon text-delete" onclick="toggleLock(${item.TaiKhoanID}, true)">Khóa</button>`;
                } else {
                    actionHtml = '<span style="color:#94a3b8; font-size:12px;">Chỉ xem</span>';
                }

                var tr = document.createElement("tr");
                tr.innerHTML = `
                <td><strong>${escapeHtml(item.TenDangNhap)}</strong></td>
                <td>${escapeHtml(item.HoTen)}</td>
                <td>${escapeHtml(item.TenPhongBan || '')}</td>
                <td>${escapeHtml(item.TenVaiTro)}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">${actionHtml}</td>
            `;
                tbody.appendChild(tr);
            });
        }

        renderPagination("paginationTaiKhoan", currentPageTaiKhoan, res.tongSoDong || 0, res.pageSize || 12, function (p) { loadData(p); });
    });
}

function openModal() {
    document.getElementById("txtTenDangNhap").value = "";
    document.getElementById("txtMatKhau").value = "";
    document.getElementById("groupPhongBanVaiTro").style.display = "none";

    Promise.all([loadNhanVienOptions(), loadVaiTroOptions()]).then(function () {
        document.getElementById("modalTaiKhoan").style.display = "flex";
    });
}

function loadNhanVienOptions() {
    return callWebMethod("GetNhanVienChuaCoTK", {}, function (res) {
        var ddl = document.getElementById("ddlNhanVien");
        ddl.innerHTML = '<option value="">-- Chọn nhân viên --</option>';
        (res.data || []).forEach(function (nv) {
            ddl.innerHTML += `<option value="${nv.NhanVienID}">${escapeHtml(nv.HoTen)} (${escapeHtml(nv.MaNhanVien)}) - ${escapeHtml(nv.TenPhongBan)}</option>`;
        });
    });
}

var vaiTroOptionsCache = [];

/*function loadVaiTroOptions() {
    return callWebMethod("GetVaiTroOptions", {}, function (res) {
        vaiTroOptionsCache = res.data || [];
        var ddl = document.getElementById("ddlVaiTro");
        ddl.innerHTML = '<option value="">-- Chọn vai trò --</option>';
        vaiTroOptionsCache.forEach(function (vt) {
            ddl.innerHTML += `<option value="${vt.NhomQuyenID}" data-can-gan-pb="${vt.CanGanPhongBan}">${escapeHtml(vt.TenNhomQuyen)}</option>`;
        });
    });
}*/
function loadVaiTroOptions() {
    return callWebMethod("GetVaiTroOptions", {}, function (res) {
        vaiTroOptionsCache = res.data || [];
        var ddl = document.getElementById("ddlVaiTro");
        ddl.innerHTML = '<option value="">-- Chọn vai trò --</option>';

        // Luôn thêm "Nhân viên" làm lựa chọn mặc định đầu tiên (không cần gán gì thêm ở server)
        ddl.innerHTML += `<option value="NHANVIEN_MACDINH" data-can-gan-pb="false">Nhân viên (mặc định)</option>`;

        vaiTroOptionsCache.forEach(function (vt) {
            ddl.innerHTML += `<option value="${vt.NhomQuyenID}" data-can-gan-pb="${vt.CanGanPhongBan}">${escapeHtml(vt.TenNhomQuyen)}</option>`;
        });
    });
}

function onVaiTroChange() {
    var ddl = document.getElementById("ddlVaiTro");
    var selectedOption = ddl.options[ddl.selectedIndex];
    var canGanPb = selectedOption.getAttribute("data-can-gan-pb") === "true";

    document.getElementById("groupPhongBanVaiTro").style.display = canGanPb ? "block" : "none";
    if (canGanPb) {
        callWebMethod("GetPhongBanOptions", {}, function (res) {
            var ddlPb = document.getElementById("ddlPhongBanVaiTro");
            ddlPb.innerHTML = '<option value="">-- Chọn phòng ban --</option>';
            (res.data || []).forEach(function (pb) {
                ddlPb.innerHTML += `<option value="${pb.PhongBanID}">${escapeHtml(pb.TenPhongBan)}</option>`;
            });
        });
    }
}

function closeModal() {
    document.getElementById("modalTaiKhoan").style.display = "none";
}

/*function saveData() {
    var nhanVienId = document.getElementById("ddlNhanVien").value;
    var tenDangNhap = document.getElementById("txtTenDangNhap").value.trim();
    var matKhau = document.getElementById("txtMatKhau").value;
    var nhomQuyenId = document.getElementById("ddlVaiTro").value;
    var phongBanId = document.getElementById("ddlPhongBanVaiTro").value;

    if (!nhanVienId) { showToast("Vui lòng chọn Nhân viên!", "error"); return; }
    if (!tenDangNhap) { showToast("Vui lòng nhập Tên đăng nhập!", "error"); return; }
    if (!matKhau || matKhau.length < 6) { showToast("Mật khẩu phải có ít nhất 6 ký tự!", "error"); return; }
    if (!nhomQuyenId) { showToast("Vui lòng chọn Vai trò!", "error"); return; }

    var selectedOption = document.getElementById("ddlVaiTro").options[document.getElementById("ddlVaiTro").selectedIndex];
    var canGanPb = selectedOption.getAttribute("data-can-gan-pb") === "true";
    if (canGanPb && !phongBanId) { showToast("Vui lòng chọn Phòng ban áp dụng Vai trò!", "error"); return; }

    callWebMethod("SaveData", {
        nhanVienId: parseInt(nhanVienId),
        tenDangNhap: tenDangNhap,
        matKhau: matKhau,
        nhomQuyenId: parseInt(nhomQuyenId),
        phongBanId: phongBanId ? parseInt(phongBanId) : null
    }, function (res) {
        showToast(res.message, "success");
        closeModal();
        loadData();
    });
}*/
function saveData() {
    var nhanVienId = document.getElementById("ddlNhanVien").value;
    var tenDangNhap = document.getElementById("txtTenDangNhap").value.trim();
    var matKhau = document.getElementById("txtMatKhau").value;
    var nhomQuyenValue = document.getElementById("ddlVaiTro").value;
    var phongBanId = document.getElementById("ddlPhongBanVaiTro").value;

    if (!nhanVienId) { showToast("Vui lòng chọn Nhân viên!", "error"); return; }
    if (!tenDangNhap) { showToast("Vui lòng nhập Tên đăng nhập!", "error"); return; }
    if (!matKhau || matKhau.length < 6) { showToast("Mật khẩu phải có ít nhất 6 ký tự!", "error"); return; }
    if (!nhomQuyenValue) { showToast("Vui lòng chọn Vai trò!", "error"); return; }

    var selectedOption = document.getElementById("ddlVaiTro").options[document.getElementById("ddlVaiTro").selectedIndex];
    var canGanPb = selectedOption.getAttribute("data-can-gan-pb") === "true";
    if (canGanPb && !phongBanId) { showToast("Vui lòng chọn Phòng ban áp dụng Vai trò!", "error"); return; }

    // "Nhân viên (mặc định)" -> không gọi GanVaiTro ở server, chỉ tạo Tài khoản thuần túy
    var isDefaultNhanVien = nhomQuyenValue === "NHANVIEN_MACDINH";

    callWebMethod("SaveData", {
        nhanVienId: parseInt(nhanVienId),
        tenDangNhap: tenDangNhap,
        matKhau: matKhau,
        nhomQuyenId: isDefaultNhanVien ? 0 : parseInt(nhomQuyenValue),
        phongBanId: phongBanId ? parseInt(phongBanId) : null
    }, function (res) {
        showToast(res.message, "success");
        closeModal();
        loadData();
    });
}

function escapeHtml(text) {
    if (!text) return "";
    return text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
}
function toggleLock(taiKhoanId, isLocked, lyDo) {
    var actionText = isLocked ? "khóa" : "mở khóa";
    showConfirmDialog("Bạn có chắc chắn muốn " + actionText + " tài khoản này?").then(function (ok) {
        if (!ok) return;

        fetch("tai-khoan.aspx/ToggleLock", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ taiKhoanId: taiKhoanId, isLocked: isLocked, lyDo: lyDo || "" })
        })
            .then(response => response.json())
            .then(res => {
                var result = res.d;
                if (result.success) {
                    showToast(result.message, "success");
                    loadData();
                } else if (result.requireReason) {
                    askReasonAndRetry(result.message, function (nhapLyDo) {
                        toggleLock(taiKhoanId, isLocked, nhapLyDo);
                    });
                } else {
                    showToast(result.message, "error");
                }
            })
            .catch(function () { showToast("Lỗi kết nối máy chủ!", "error"); });
    });
}