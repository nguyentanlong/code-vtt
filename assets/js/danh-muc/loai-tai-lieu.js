document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    fetch("loai-tai-lieu.aspx/" + methodName, {
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
        var tbody = document.getElementById("tbodyLoaiTaiLieu");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaLoai)}</strong></td>
                <td>${escapeHtml(item.TenLoai)}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.LoaiTaiLieuID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.LoaiTaiLieuID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddLoaiTaiLieuID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Loại tài liệu";
        document.getElementById("txtMaLoai").value = "";
        document.getElementById("txtMaLoai").readOnly = false;
        document.getElementById("txtTenLoai").value = "";
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("modalLoaiTaiLieu").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Loại tài liệu";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaLoai").value = d.MaLoai;
            document.getElementById("txtMaLoai").readOnly = true;
            document.getElementById("txtTenLoai").value = d.TenLoai;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("modalLoaiTaiLieu").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalLoaiTaiLieu").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddLoaiTaiLieuID").value);
    var maLoai = document.getElementById("txtMaLoai").value.trim();
    var tenLoai = document.getElementById("txtTenLoai").value.trim();

    if (!maLoai) {
        alert("Vui lòng nhập Mã loại!");
        document.getElementById("txtMaLoai").focus();
        return;
    }
    if (!tenLoai) {
        alert("Vui lòng nhập Tên loại tài liệu!");
        document.getElementById("txtTenLoai").focus();
        return;
    }

    var payload = {
        loaiTaiLieuId: id,
        maLoai: maLoai,
        tenLoai: tenLoai,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Loại tài liệu này khỏi hệ thống?")) {
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