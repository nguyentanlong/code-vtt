var canThemPermission = false;
var chiNhanhOptions = [];
var chucVuOptions = [];

document.addEventListener("DOMContentLoaded", function () {
    Promise.all([loadPermission(), loadChiNhanhOptions(), loadChucVuOptions()]).then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("nhan-vien.aspx/" + methodName, {
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
                alert("Lỗi: " + errorMsg);
                if (errorMsg && (errorMsg.includes("đăng nhập") || errorMsg.includes("hết hạn"))) {
                    var currentUrl = encodeURIComponent(window.location.href);
                    window.location.href = "../login.aspx?returnUrl=" + currentUrl;
                }
            }
        })
        .catch(err => {
            console.error("AJAX Error:", err);
            alert("Lỗi kết nối máy chủ hoặc hệ thống không phản hồi!");
        });
}

function loadPermission() {
    return callWebMethod("GetPermission", {}, function (res) {
        canThemPermission = res.data.canThem;
        document.getElementById("btnAddNew").style.display = canThemPermission ? "inline-flex" : "none";
    });
}

function loadChiNhanhOptions() {
    return callWebMethod("GetChiNhanhOptions", {}, function (res) {
        chiNhanhOptions = res.data || [];

        var ddlFilter = document.getElementById("ddlSearchChiNhanh");
        ddlFilter.innerHTML = '<option value="">-- Tất cả chi nhánh --</option>';
        var ddlForm = document.getElementById("ddlFormChiNhanh");
        ddlForm.innerHTML = '<option value="">-- Trực thuộc Tổng công ty --</option>';

        chiNhanhOptions.forEach(function (cn) {
            var opt = `<option value="${cn.ChiNhanhID}">${escapeHtml(cn.TenChiNhanh)}</option>`;
            ddlFilter.innerHTML += opt;
            ddlForm.innerHTML += opt;
        });
    });
}

function loadChucVuOptions() {
    return callWebMethod("GetChucVuOptions", {}, function (res) {
        chucVuOptions = res.data || [];
        var ddl = document.getElementById("ddlFormChucVu");
        ddl.innerHTML = '<option value="">-- Không chọn --</option>';
        chucVuOptions.forEach(function (cv) {
            ddl.innerHTML += `<option value="${cv.ChucVuID}">${escapeHtml(cv.TenChucVu)}</option>`;
        });
    });
}

function loadPhongBanOptions(chiNhanhId, targetSelectId, selectedId) {
    return callWebMethod("GetPhongBanOptions", { chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null }, function (res) {
        var ddl = document.getElementById(targetSelectId);
        ddl.innerHTML = '<option value="">-- Chọn phòng ban --</option>';
        (res.data || []).forEach(function (pb) {
            var selected = (selectedId && pb.PhongBanID == selectedId) ? "selected" : "";
            ddl.innerHTML += `<option value="${pb.PhongBanID}" ${selected}>${escapeHtml(pb.TenPhongBan)}</option>`;
        });
    });
}

function onFilterChiNhanhChange() {
    var chiNhanhId = document.getElementById("ddlSearchChiNhanh").value;
    loadPhongBanOptions(chiNhanhId, "ddlSearchPhongBan", null).then(function () {
        loadData();
    });
}

function onFormChiNhanhChange() {
    var chiNhanhId = document.getElementById("ddlFormChiNhanh").value;
    loadPhongBanOptions(chiNhanhId, "ddlFormPhongBan", null);
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var chiNhanhId = document.getElementById("ddlSearchChiNhanh").value;
    var phongBanId = document.getElementById("ddlSearchPhongBan").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", {
        keyword: keyword,
        chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null,
        phongBanId: phongBanId ? parseInt(phongBanId) : null,
        trangThai: trangThai
    }, function (res) {
        var tbody = document.getElementById("tbodyNhanVien");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="8" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";

            var actionsHtml = "";
            if (item.CanEditRow) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.NhanVienID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                `;
            }
            if (item.CanDeleteRow) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.NhanVienID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                `;
            }
            if (!actionsHtml) {
                actionsHtml = '<span style="color:#94a3b8; font-size:12px;">Chỉ xem</span>';
            }

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaNhanVien)}</strong></td>
                <td>${escapeHtml(item.HoTen)}</td>
                <td>${escapeHtml(item.TenPhongBan || '')}</td>
                <td>${escapeHtml(item.TenChiNhanh || '')}</td>
                <td>${escapeHtml(item.TenChucVu || '')}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center; white-space:nowrap;">${actionsHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddNhanVienID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Nhân viên";
        document.getElementById("txtMaNhanVien").value = "";
        document.getElementById("txtMaNhanVien").readOnly = false;
        document.getElementById("txtHoTen").value = "";
        document.getElementById("txtEmail").value = "";
        document.getElementById("txtSoDienThoai").value = "";
        document.getElementById("ddlFormChiNhanh").value = "";
        document.getElementById("ddlFormChucVu").value = "";
        document.getElementById("ddlTrangThai").value = "1";
        loadPhongBanOptions(null, "ddlFormPhongBan", null).then(function () {
            document.getElementById("modalNhanVien").style.display = "flex";
        });
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Nhân viên";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaNhanVien").value = d.MaNhanVien;
            document.getElementById("txtMaNhanVien").readOnly = true;
            document.getElementById("txtHoTen").value = d.HoTen;
            document.getElementById("txtEmail").value = d.Email || "";
            document.getElementById("txtSoDienThoai").value = d.SoDienThoai || "";
            document.getElementById("ddlFormChiNhanh").value = d.ChiNhanhID || "";
            document.getElementById("ddlFormChucVu").value = d.ChucVuChinhThucID || "";
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            loadPhongBanOptions(d.ChiNhanhID, "ddlFormPhongBan", d.PhongBanChinhThucID).then(function () {
                document.getElementById("modalNhanVien").style.display = "flex";
            });
        });
    }
}

function closeModal() {
    document.getElementById("modalNhanVien").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddNhanVienID").value);
    var maNhanVien = document.getElementById("txtMaNhanVien").value.trim();
    var hoTen = document.getElementById("txtHoTen").value.trim();
    var phongBanId = document.getElementById("ddlFormPhongBan").value;
    var chucVuId = document.getElementById("ddlFormChucVu").value;

    if (!maNhanVien) { alert("Vui lòng nhập Mã nhân viên!"); document.getElementById("txtMaNhanVien").focus(); return; }
    if (!hoTen) { alert("Vui lòng nhập Họ tên!"); document.getElementById("txtHoTen").focus(); return; }
    if (!phongBanId) { alert("Vui lòng chọn Phòng ban!"); return; }

    var payload = {
        nhanVienId: id,
        hoTen: hoTen,
        maNhanVien: maNhanVien,
        email: document.getElementById("txtEmail").value.trim(),
        soDienThoai: document.getElementById("txtSoDienThoai").value.trim(),
        phongBanChinhThucId: parseInt(phongBanId),
        chucVuChinhThucId: chucVuId ? parseInt(chucVuId) : null,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Nhân viên này khỏi hệ thống?")) {
        callWebMethod("DeleteData", { id: id }, function (res) {
            alert(res.message);
            loadData();
        });
    }
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