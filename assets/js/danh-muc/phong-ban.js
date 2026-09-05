var canThemPermission = false;

document.addEventListener("DOMContentLoaded", function () {
    Promise.all([loadPermission(), loadChiNhanhOptions()]).then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("phong-ban.aspx/" + methodName, {
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

/*function loadPermission() {
    return callWebMethod("GetPermission", {}, function (res) {
        canThemPermission = res.data.canThem;
        document.getElementById("btnAddNew").style.display = canThemPermission ? "inline-flex" : "none";

        var scope = res.data.scope;
        var chiNhanhGroup = document.getElementById("ddlSearchChiNhanh").closest(".filter-group");
        var phongBanGroup = document.getElementById("ddlSearchPhongBan")
            ? document.getElementById("ddlSearchPhongBan").closest(".filter-group")
            : null; // phong-ban.aspx không có ddlSearchPhongBan riêng, chỉ nhan-vien/du-an mới có

        if (scope === "CONGTY") {
            // Admin: hiện cả 2, hoạt động cascade như cũ
            chiNhanhGroup.style.display = "";
            if (phongBanGroup) phongBanGroup.style.display = "";
        } else if (scope === "CHINHANH") {
            chiNhanhGroup.style.display = "none";
            if (phongBanGroup) {
                phongBanGroup.style.display = "";
                loadPhongBanOptions(res.data.myChiNhanhId, "ddlSearchPhongBan", null);
            }
        } else {
            // PHONGBAN (Trưởng phòng/Phó phòng/Nhân viên): ẩn cả 2 filter, luôn chỉ thao tác đúng phòng mình
            chiNhanhGroup.style.display = "none";
            if (phongBanGroup) phongBanGroup.style.display = "none";
        }
    });
}*/
function loadPermission() {
    return callWebMethod("GetPermission", {}, function (res) {
        canThemPermission = res.data.canThem;
        document.getElementById("btnAddNew").style.display = canThemPermission ? "inline-flex" : "none";

        applyScopeFilterVisibility(res.data.scope, "ddlSearchChiNhanh", null, res.data.myChiNhanhId, null);
    });
}

function loadChiNhanhOptions() {
    return callWebMethod("GetChiNhanhOptions", {}, function (res) {
        var ddlFilter = document.getElementById("ddlSearchChiNhanh");
        ddlFilter.innerHTML = '<option value="">-- Tất cả chi nhánh --</option>';
        var ddlForm = document.getElementById("ddlFormChiNhanh");
        ddlForm.innerHTML = '<option value="">-- Trực thuộc Tổng công ty --</option>';

        (res.data || []).forEach(function (cn) {
            var opt = `<option value="${cn.ChiNhanhID}">${escapeHtml(cn.TenChiNhanh)}</option>`;
            ddlFilter.innerHTML += opt;
            ddlForm.innerHTML += opt;
        });
    });
}

function loadChaOptions(excludeId, selectedId) {
    return callWebMethod("GetChaOptions", { excludeId: excludeId || 0 }, function (res) {
        var ddl = document.getElementById("ddlFormPhongBanCha");
        ddl.innerHTML = '<option value="">-- Không có (Cấp cao nhất) --</option>';
        (res.data || []).forEach(function (pb) {
            var selected = (selectedId && pb.PhongBanID == selectedId) ? "selected" : "";
            ddl.innerHTML += `<option value="${pb.PhongBanID}" ${selected}>${pb.TenPhongBan}</option>`;
        });
    });
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var chiNhanhId = document.getElementById("ddlSearchChiNhanh").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", {
        keyword: keyword,
        chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null,
        trangThai: trangThai
    }, function (res) {
        var tbody = document.getElementById("tbodyPhongBan");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";

            var actionsHtml = "";
            if (item.CanEditRow) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.PhongBanID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                `;
            }
            if (item.CanDeleteRow) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.PhongBanID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                `;
            }
            if (!actionsHtml) actionsHtml = '<span style="color:#94a3b8; font-size:12px;">Chỉ xem</span>';

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td><strong>${escapeHtml(item.MaPhongBan)}</strong></td>
                <td style="white-space:pre;">${item.TenPhongBan}</td>
                <td>${escapeHtml(item.TenChiNhanh || '')}</td>
                <td style="text-align:center;">${item.CapDo}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center; white-space:nowrap;">${actionsHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddPhongBanID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Đơn vị";
        document.getElementById("txtMaPhongBan").value = "";
        document.getElementById("txtMaPhongBan").readOnly = false;
        document.getElementById("txtTenPhongBan").value = "";
        document.getElementById("ddlFormChiNhanh").value = "";
        document.getElementById("txtThuTu").value = "0";
        document.getElementById("ddlTrangThai").value = "1";
        loadChaOptions(0, null).then(function () {
            document.getElementById("modalPhongBan").style.display = "flex";
        });
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Đơn vị";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaPhongBan").value = d.MaPhongBan;
            document.getElementById("txtMaPhongBan").readOnly = true;
            document.getElementById("txtTenPhongBan").value = d.TenPhongBan;
            document.getElementById("ddlFormChiNhanh").value = d.ChiNhanhID || "";
            document.getElementById("txtThuTu").value = d.ThuTu;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            loadChaOptions(id, d.PhongBanChaID).then(function () {
                document.getElementById("modalPhongBan").style.display = "flex";
            });
        });
    }
}

function closeModal() {
    document.getElementById("modalPhongBan").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddPhongBanID").value);
    var maPhongBan = document.getElementById("txtMaPhongBan").value.trim();
    var tenPhongBan = document.getElementById("txtTenPhongBan").value.trim();
    var chiNhanhId = document.getElementById("ddlFormChiNhanh").value;
    var phongBanChaId = document.getElementById("ddlFormPhongBanCha").value;

    if (!maPhongBan) { showToast("Vui lòng nhập Mã đơn vị!", "error"); return; }
    if (!tenPhongBan) { showToast("Vui lòng nhập Tên đơn vị!", "error"); return; }

    var payload = {
        phongBanId: id,
        chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null,
        phongBanChaId: phongBanChaId ? parseInt(phongBanChaId) : null,
        maPhongBan: maPhongBan,
        tenPhongBan: tenPhongBan,
        thuTu: parseInt(document.getElementById("txtThuTu").value) || 0,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        showToast(res.message, "success");
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    showConfirmDialog("Bạn có chắc chắn muốn xóa Đơn vị này khỏi hệ thống?").then(function (ok) {
        if (!ok) return;
        callWebMethod("DeleteData", { id: id }, function (res) {
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