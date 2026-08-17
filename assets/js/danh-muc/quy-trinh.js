var canEditPermission = false;

document.addEventListener("DOMContentLoaded", function () {
    loadPermission().then(function () { loadData(); });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("quy-trinh.aspx/" + methodName, {
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
                    window.location.href = "../Login.aspx?returnUrl=" + currentUrl;
                }
            }
        })
        .catch(err => { console.error("AJAX Error:", err); alert("Lỗi kết nối máy chủ!"); });
}

function loadPermission() {
    return callWebMethod("GetPermission", {}, function (res) {
        canEditPermission = res.data.canEdit;
        document.getElementById("btnAddNew").style.display = canEditPermission ? "inline-flex" : "none";
    });
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyQuyTrinh");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";

            var actionsHtml = `
                <a href="chi-tiet-quy-trinh.aspx?id=${item.QuyTrinhID}" class="btn-icon" style="color:#0284c7;" title="Quản lý các Bước">
                    <i class="fa fa-list-ol"></i> Bước
                </a>
            `;
            if (canEditPermission) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.QuyTrinhID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.QuyTrinhID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                `;
            }

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaQuyTrinh)}</strong></td>
                <td>${escapeHtml(item.TenQuyTrinh)}</td>
                <td>${escapeHtml(item.LoaiDoiTuong)}</td>
                <td style="text-align:center;">${item.SoBuoc}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center; white-space:nowrap;">${actionsHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    if (!canEditPermission) { alert("Bạn không có quyền thực hiện thao tác này!"); return; }
    document.getElementById("hddQuyTrinhID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Quy trình";
        document.getElementById("txtMaQuyTrinh").value = "";
        document.getElementById("txtMaQuyTrinh").readOnly = false;
        document.getElementById("txtTenQuyTrinh").value = "";
        document.getElementById("ddlLoaiDoiTuong").value = "DuAn";
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("modalQuyTrinh").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Quy trình";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaQuyTrinh").value = d.MaQuyTrinh;
            document.getElementById("txtMaQuyTrinh").readOnly = true;
            document.getElementById("txtTenQuyTrinh").value = d.TenQuyTrinh;
            document.getElementById("ddlLoaiDoiTuong").value = d.LoaiDoiTuong;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("modalQuyTrinh").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalQuyTrinh").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddQuyTrinhID").value);
    var maQuyTrinh = document.getElementById("txtMaQuyTrinh").value.trim();
    var tenQuyTrinh = document.getElementById("txtTenQuyTrinh").value.trim();

    if (!maQuyTrinh) { alert("Vui lòng nhập Mã quy trình!"); return; }
    if (!tenQuyTrinh) { alert("Vui lòng nhập Tên quy trình!"); return; }

    var payload = {
        quyTrinhId: id,
        maQuyTrinh: maQuyTrinh,
        tenQuyTrinh: tenQuyTrinh,
        loaiDoiTuong: document.getElementById("ddlLoaiDoiTuong").value,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Quy trình này khỏi hệ thống?")) {
        callWebMethod("DeleteData", { id: id }, function (res) {
            alert(res.message);
            loadData();
        });
    }
}

function escapeHtml(text) {
    if (!text) return "";
    return text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
}