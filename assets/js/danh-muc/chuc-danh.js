document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("chuc-danh.aspx/" + methodName, {
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
        .catch(err => {
            console.error("AJAX Error:", err);
            alert("Lỗi kết nối máy chủ hoặc hệ thống không phản hồi!");
        });
}

function loadChaOptions(excludeId, selectedId) {
    return callWebMethod("GetChaOptions", { excludeId: excludeId || 0 }, function (res) {
        var ddl = document.getElementById("ddlChucDanhCha");
        ddl.innerHTML = '<option value="">-- Không có (Cấp cao nhất) --</option>';
        (res.data || []).forEach(function (cd) {
            var selected = (selectedId && cd.ChucDanhID == selectedId) ? "selected" : "";
            ddl.innerHTML += `<option value="${cd.ChucDanhID}" ${selected}>${escapeHtml(cd.TenChucDanh)}</option>`;
        });
    });
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyChucDanh");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="8" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaChucDanh)}</strong></td>
                <td>${escapeHtml(item.TenChucDanh)}</td>
                <td>${escapeHtml(item.TenChucDanhCha || '(Cấp cao nhất)')}</td>
                <td style="text-align:center;">${item.CapBac}</td>
                <td style="text-align:center;">${item.ThuTu}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.ChucDanhID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.ChucDanhID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddChucDanhID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Chức danh";
        document.getElementById("txtMaChucDanh").value = "";
        document.getElementById("txtMaChucDanh").readOnly = false;
        document.getElementById("txtTenChucDanh").value = "";
        document.getElementById("txtCapBac").value = "10";
        document.getElementById("txtThuTu").value = "0";
        document.getElementById("ddlTrangThai").value = "1";
        loadChaOptions(0, null).then(function () {
            document.getElementById("modalChucDanh").style.display = "flex";
        });
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Chức danh";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaChucDanh").value = d.MaChucDanh;
            document.getElementById("txtMaChucDanh").readOnly = true;
            document.getElementById("txtTenChucDanh").value = d.TenChucDanh;
            document.getElementById("txtCapBac").value = d.CapBac;
            document.getElementById("txtThuTu").value = d.ThuTu;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            loadChaOptions(id, d.ChucDanhChaID).then(function () {
                document.getElementById("modalChucDanh").style.display = "flex";
            });
        });
    }
}

function closeModal() {
    document.getElementById("modalChucDanh").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddChucDanhID").value);
    var maChucDanh = document.getElementById("txtMaChucDanh").value.trim();
    var tenChucDanh = document.getElementById("txtTenChucDanh").value.trim();
    var chucDanhChaId = document.getElementById("ddlChucDanhCha").value;

    if (!maChucDanh) { alert("Vui lòng nhập Mã chức danh!"); document.getElementById("txtMaChucDanh").focus(); return; }
    if (!tenChucDanh) { alert("Vui lòng nhập Tên chức danh!"); document.getElementById("txtTenChucDanh").focus(); return; }

    var payload = {
        chucDanhId: id,
        maChucDanh: maChucDanh,
        tenChucDanh: tenChucDanh,
        capBac: parseInt(document.getElementById("txtCapBac").value) || 10,
        chucDanhChaId: chucDanhChaId ? parseInt(chucDanhChaId) : null,
        thuTu: parseInt(document.getElementById("txtThuTu").value) || 0,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Chức danh này khỏi hệ thống?")) {
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