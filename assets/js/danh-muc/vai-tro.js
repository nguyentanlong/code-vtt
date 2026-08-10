document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    fetch("vai-tro.aspx/" + methodName, {
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

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyVaiTro");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";
            var quanTriBadge = item.LaQuanTri
                ? '<span class="badge badge-success">Quản trị</span>'
                : '<span class="badge" style="background:#f1f5f9; color:#64748b;">Thông thường</span>';

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaVaiTro)}</strong></td>
                <td>${escapeHtml(item.TenVaiTro)}</td>
                <td style="text-align:center;">${quanTriBadge}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.VaiTroID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.VaiTroID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddVaiTroID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Vai trò";
        document.getElementById("txtMaVaiTro").value = "";
        document.getElementById("txtMaVaiTro").readOnly = false;
        document.getElementById("txtTenVaiTro").value = "";
        document.getElementById("chkLaQuanTri").checked = false;
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("modalVaiTro").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Vai trò";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaVaiTro").value = d.MaVaiTro;
            document.getElementById("txtMaVaiTro").readOnly = true;
            document.getElementById("txtTenVaiTro").value = d.TenVaiTro;
            document.getElementById("chkLaQuanTri").checked = d.LaQuanTri;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("modalVaiTro").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalVaiTro").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddVaiTroID").value);
    var maVaiTro = document.getElementById("txtMaVaiTro").value.trim();
    var tenVaiTro = document.getElementById("txtTenVaiTro").value.trim();

    if (!maVaiTro) { alert("Vui lòng nhập Mã vai trò!"); document.getElementById("txtMaVaiTro").focus(); return; }
    if (!tenVaiTro) { alert("Vui lòng nhập Tên vai trò!"); document.getElementById("txtTenVaiTro").focus(); return; }

    var payload = {
        vaiTroId: id,
        maVaiTro: maVaiTro,
        tenVaiTro: tenVaiTro,
        laQuanTri: document.getElementById("chkLaQuanTri").checked,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Vai trò này khỏi hệ thống?")) {
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