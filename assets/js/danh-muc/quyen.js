var nhomQuyenOptions = [];

document.addEventListener("DOMContentLoaded", function () {
    loadNhomQuyenOptions().then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("quyen.aspx/" + methodName, {
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

function loadNhomQuyenOptions() {
    return callWebMethod("GetNhomQuyenOptions", {}, function (res) {
        nhomQuyenOptions = res.data || [];

        var ddlFilter = document.getElementById("ddlSearchNhomQuyen");
        ddlFilter.innerHTML = '<option value="">-- Tất cả nhóm quyền --</option>';

        var ddlForm = document.getElementById("ddlNhomQuyen");
        ddlForm.innerHTML = '<option value="">-- Không thuộc nhóm nào --</option>';

        nhomQuyenOptions.forEach(function (nq) {
            var opt = `<option value="${nq.NhomQuyenID}">${escapeHtml(nq.TenNhomQuyen)}</option>`;
            ddlFilter.innerHTML += opt;
            ddlForm.innerHTML += opt;
        });
    });
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var nhomQuyenId = document.getElementById("ddlSearchNhomQuyen").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, nhomQuyenId: nhomQuyenId, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyQuyen");
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
                <td><strong>${escapeHtml(item.MaQuyen)}</strong></td>
                <td>${escapeHtml(item.TenQuyen)}</td>
                <td>${escapeHtml(item.TenNhomQuyen || '(Không có)')}</td>
                <td>${escapeHtml(item.HanhDong)}</td>
                <td>${escapeHtml(item.DoiTuong)}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.QuyenID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.QuyenID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddQuyenID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Quyền";
        document.getElementById("txtMaQuyen").value = "";
        document.getElementById("txtMaQuyen").readOnly = false;
        document.getElementById("txtTenQuyen").value = "";
        document.getElementById("ddlNhomQuyen").value = "";
        document.getElementById("txtHanhDong").value = "";
        document.getElementById("txtDoiTuong").value = "";
        document.getElementById("txtThuTu").value = "0";
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("modalQuyen").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Quyền";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaQuyen").value = d.MaQuyen;
            document.getElementById("txtMaQuyen").readOnly = true;
            document.getElementById("txtTenQuyen").value = d.TenQuyen;
            document.getElementById("ddlNhomQuyen").value = d.NhomQuyenID || "";
            document.getElementById("txtHanhDong").value = d.HanhDong;
            document.getElementById("txtDoiTuong").value = d.DoiTuong;
            document.getElementById("txtThuTu").value = d.ThuTu;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("modalQuyen").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalQuyen").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddQuyenID").value);
    var maQuyen = document.getElementById("txtMaQuyen").value.trim();
    var tenQuyen = document.getElementById("txtTenQuyen").value.trim();
    var hanhDong = document.getElementById("txtHanhDong").value.trim();
    var doiTuong = document.getElementById("txtDoiTuong").value.trim();
    var nhomQuyenId = document.getElementById("ddlNhomQuyen").value;

    if (!maQuyen) { alert("Vui lòng nhập Mã quyền!"); document.getElementById("txtMaQuyen").focus(); return; }
    if (!tenQuyen) { alert("Vui lòng nhập Tên quyền!"); document.getElementById("txtTenQuyen").focus(); return; }
    if (!hanhDong) { alert("Vui lòng nhập Hành động!"); document.getElementById("txtHanhDong").focus(); return; }
    if (!doiTuong) { alert("Vui lòng nhập Đối tượng!"); document.getElementById("txtDoiTuong").focus(); return; }

    var payload = {
        quyenId: id,
        nhomQuyenId: nhomQuyenId ? parseInt(nhomQuyenId) : null,
        maQuyen: maQuyen,
        tenQuyen: tenQuyen,
        hanhDong: hanhDong,
        doiTuong: doiTuong,
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
    if (confirm("Bạn có chắc chắn muốn xóa Quyền này khỏi hệ thống?")) {
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