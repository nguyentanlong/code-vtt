var canEditPermission = false;

document.addEventListener("DOMContentLoaded", function () {
    loadPermission().then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("giai-doan.aspx/" + methodName, {
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
        .catch(function () {
            showToast("Lỗi kết nối máy chủ hoặc hệ thống không phản hồi!", "error");
        });
}

function loadPermission() {
    return callWebMethod("GetPermission", {}, function (res) {
        canEditPermission = res.data.canEdit;
        document.getElementById("btnAddNew").style.display = canEditPermission ? "inline-flex" : "none";
    });
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;

    callWebMethod("GetList", { keyword: keyword }, function (res) {
        var tbody = document.getElementById("tbodyTimeLine");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var actionsHtml = canEditPermission
                ? `
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.TimeLineID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.TimeLineID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                `
                : `<span style="color:#94a3b8; font-size:12px;">Chỉ xem</span>`;

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaGiaiDoan)}</strong></td>
                <td>${escapeHtml(item.TenGiaiDoan)}</td>
                <td>${escapeHtml(item.MoTa || '')}</td>
                <td style="text-align:center;">${item.ThuTuHienThi}</td>
                <td style="text-align:center;">${actionsHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    if (!canEditPermission) {
        showToast("Bạn không có quyền thực hiện thao tác này!", "error");
        return;
    }

    document.getElementById("hddTimeLineID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Giai đoạn";
        document.getElementById("txtMaGiaiDoan").value = "";
        document.getElementById("txtMaGiaiDoan").readOnly = false;
        document.getElementById("txtTenGiaiDoan").value = "";
        document.getElementById("txtThuTu").value = "0";
        document.getElementById("txtMoTa").value = "";
        document.getElementById("modalTimeLine").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Giai đoạn";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaGiaiDoan").value = d.MaGiaiDoan;
            document.getElementById("txtMaGiaiDoan").readOnly = true;
            document.getElementById("txtTenGiaiDoan").value = d.TenGiaiDoan;
            document.getElementById("txtThuTu").value = d.ThuTuHienThi;
            document.getElementById("txtMoTa").value = d.MoTa || "";
            document.getElementById("modalTimeLine").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalTimeLine").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddTimeLineID").value);
    var maGiaiDoan = document.getElementById("txtMaGiaiDoan").value.trim();
    var tenGiaiDoan = document.getElementById("txtTenGiaiDoan").value.trim();

    if (!maGiaiDoan) { showToast("Vui lòng nhập Mã giai đoạn!", "error"); return; }
    if (!tenGiaiDoan) { showToast("Vui lòng nhập Tên giai đoạn!", "error"); return; }

    var payload = {
        timeLineId: id,
        maGiaiDoan: maGiaiDoan,
        tenGiaiDoan: tenGiaiDoan,
        thuTuHienThi: parseInt(document.getElementById("txtThuTu").value) || 0,
        moTa: document.getElementById("txtMoTa").value.trim()
    };

    callWebMethod("SaveData", payload, function (res) {
        showToast(res.message, "success");
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    showConfirmDialog("Bạn có chắc chắn muốn xóa Giai đoạn này khỏi hệ thống?").then(function (ok) {
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