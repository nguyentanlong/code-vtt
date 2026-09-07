var canThemPermission = false;
var currentPageBoPhan = 1;

document.addEventListener("DOMContentLoaded", function () {
    Promise.all([loadPermission(), loadChiNhanhOptions()]).then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("bo-phan.aspx/" + methodName, {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(dataObj || {})
    })
        .then(response => response.json())
        .then(res => {
            var result = res.d;
            if (result && result.success) {
                successCallback(result);
            } else if (result && result.requireReason) {
                successCallback(result); // để hàm gọi tự xử lý hiện popup Lý do
            } else {
                showToast(result ? result.message : "Thao tác thất bại!", "error");
            }
        })
        .catch(function () {
            showToast("Lỗi kết nối máy chủ hoặc hệ thống không phản hồi!", "error");
        });
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

function loadPhongBanChaOptions(chiNhanhId, selectedId) {
    return callWebMethod("GetPhongBanChaOptions", { chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null }, function (res) {
        var ddl = document.getElementById("ddlFormPhongBanCha");
        ddl.innerHTML = '<option value="">-- Chọn phòng ban --</option>';
        (res.data || []).forEach(function (pb) {
            var selected = (selectedId && pb.PhongBanID == selectedId) ? "selected" : "";
            ddl.innerHTML += `<option value="${pb.PhongBanID}" ${selected}>${escapeHtml(pb.TenPhongBan)}</option>`;
        });
    });
}

function onFormChiNhanhChange() {
    var chiNhanhId = document.getElementById("ddlFormChiNhanh").value;
    loadPhongBanChaOptions(chiNhanhId, null);
}

function loadData(pageNumber) {
    if (pageNumber) currentPageBoPhan = pageNumber;
    var keyword = document.getElementById("txtSearchKeyword").value;
    var chiNhanhId = document.getElementById("ddlSearchChiNhanh").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", {
        keyword: keyword,
        chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null,
        trangThai: trangThai,
        pageNumber: currentPageBoPhan
    }, function (res) {
        var tbody = document.getElementById("tbodyBoPhan");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            renderPagination("paginationBoPhan", currentPageBoPhan, res.tongSoDong || 0, res.pageSize || 12, function (p) { loadData(p); });
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
                <td>${escapeHtml(item.TenPhongBan)}</td>
                <td>${escapeHtml(item.TenPhongBanCha || '')}</td>
                <td>${escapeHtml(item.TenChiNhanh || '')}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center; white-space:nowrap;">${actionsHtml}</td>
            `;
            tbody.appendChild(tr);
        });
        renderPagination("paginationBoPhan", currentPageBoPhan, res.tongSoDong || 0, res.pageSize || 12, function (p) { loadData(p); });
    });
}

function openModal(id) {
    document.getElementById("hddPhongBanID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Bộ phận";
        document.getElementById("txtMaPhongBan").value = "";
        document.getElementById("txtMaPhongBan").readOnly = false;
        document.getElementById("txtTenPhongBan").value = "";
        document.getElementById("ddlFormChiNhanh").value = "";
        document.getElementById("txtThuTu").value = "0";
        document.getElementById("ddlTrangThai").value = "1";
        loadPhongBanChaOptions(null, null).then(function () {
            document.getElementById("modalBoPhan").style.display = "flex";
        });
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Bộ phận";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaPhongBan").value = d.MaPhongBan;
            document.getElementById("txtMaPhongBan").readOnly = true;
            document.getElementById("txtTenPhongBan").value = d.TenPhongBan;
            document.getElementById("ddlFormChiNhanh").value = d.ChiNhanhID || "";
            document.getElementById("txtThuTu").value = d.ThuTu;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            loadPhongBanChaOptions(d.ChiNhanhID, d.PhongBanChaID).then(function () {
                document.getElementById("modalBoPhan").style.display = "flex";
            });
        });
    }
}

function closeModal() {
    document.getElementById("modalBoPhan").style.display = "none";
}

function saveData(lyDo) {
    var id = parseInt(document.getElementById("hddPhongBanID").value);
    var maPhongBan = document.getElementById("txtMaPhongBan").value.trim();
    var tenPhongBan = document.getElementById("txtTenPhongBan").value.trim();
    var chiNhanhId = document.getElementById("ddlFormChiNhanh").value;
    var phongBanChaId = document.getElementById("ddlFormPhongBanCha").value;

    if (!maPhongBan) { showToast("Vui lòng nhập Mã Bộ phận!", "error"); return; }
    if (!tenPhongBan) { showToast("Vui lòng nhập Tên Bộ phận!", "error"); return; }
    if (!phongBanChaId) { showToast("Vui lòng chọn Phòng ban!", "error"); return; }

    var payload = {
        phongBanId: id,
        chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null,
        phongBanChaId: parseInt(phongBanChaId),
        maPhongBan: maPhongBan,
        tenPhongBan: tenPhongBan,
        thuTu: parseInt(document.getElementById("txtThuTu").value) || 0,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value),
        lyDo: lyDo || ""
    };

    callWebMethod("SaveData", payload, function (res) {
        if (res.success) {
            showToast(res.message, "success");
            closeModal();
            loadData();
        } else if (res.requireReason) {
            askReasonAndRetry(res.message, function (nhapLyDo) {
                saveData(nhapLyDo);
            });
        } else {
            showToast(res.message, "error");
        }
    });
}

function deleteData(id, lyDo) {
    showConfirmDialog("Bạn có chắc chắn muốn xóa Bộ phận này khỏi hệ thống?").then(function (ok) {
        if (!ok) return;

        callWebMethod("DeleteData", { id: id, lyDo: lyDo || "" }, function (res) {
            if (res.success) {
                showToast(res.message, "success");
                loadData();
            } else if (res.requireReason) {
                askReasonAndRetry(res.message, function (nhapLyDo) {
                    deleteData(id, nhapLyDo);
                });
            } else {
                showToast(res.message, "error");
            }
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