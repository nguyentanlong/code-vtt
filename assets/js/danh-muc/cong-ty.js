document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

/**
 * Hàm gọi AJAX chung gửi request đến các WebMethod C#
 * @param {string} methodName Tên hàm [WebMethod] trong C#
 * @param {object} dataObj Tham số truyền vào dạng JS Object
 * @param {function} successCallback Callback xử lý khi thành công
 */
function callWebMethod(methodName, dataObj, successCallback) {
    fetch("cong-ty.aspx/" + methodName, {
        method: "POST",
        headers: {
            "Content-Type": "application/json; charset=utf-8"
        },
        body: JSON.stringify(dataObj || {})
    })
        .then(response => response.json())
        .then(res => {
            var result = res.d; // ASP.NET WebMethod trả về dữ liệu qua thuộc tính .d

            if (result && result.success) {
                successCallback(result);
            } else {
                var errorMsg = result ? result.message : "Thao tác thất bại!";
                alert("Lỗi: " + errorMsg);

                // Nếu phản hồi thông báo hết hạn phiên làm việc/chưa đăng nhập
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

// 1. Tải danh sách công ty
function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyCongTy");
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
                <td><strong>${escapeHtml(item.MaCongTy)}</strong></td>
                <td>${escapeHtml(item.TenCongTy)}</td>
                <td>${escapeHtml(item.TenVietTat || '')}</td>
                <td>${escapeHtml(item.MaSoThue || '')}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td>${item.NgayTaoText}</td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.CongTyID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.CongTyID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

// 2. Mở Popup Modal (Thêm mới id = 0, Cập nhật id > 0)
function openModal(id) {
    document.getElementById("hddCongTyID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Công ty";
        document.getElementById("txtMaCongTy").value = "";
        document.getElementById("txtMaCongTy").readOnly = false;
        document.getElementById("txtTenCongTy").value = "";
        document.getElementById("txtTenVietTat").value = "";
        document.getElementById("txtMaSoThue").value = "";
        document.getElementById("txtDiaChi").value = "";
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("txtMoTa").value = "";
        document.getElementById("modalCongTy").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Công ty";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaCongTy").value = d.MaCongTy;
            document.getElementById("txtMaCongTy").readOnly = true; // Khóa Mã công ty khi sửa
            document.getElementById("txtTenCongTy").value = d.TenCongTy;
            document.getElementById("txtTenVietTat").value = d.TenVietTat || "";
            document.getElementById("txtMaSoThue").value = d.MaSoThue || "";
            document.getElementById("txtDiaChi").value = d.DiaChi || "";
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("txtMoTa").value = d.MoTa || "";
            document.getElementById("modalCongTy").style.display = "flex";
        });
    }
}

// 3. Đóng Modal
function closeModal() {
    document.getElementById("modalCongTy").style.display = "none";
}

// 4. Lưu thông tin (Thêm mới / Cập nhật)
function saveData() {
    var id = parseInt(document.getElementById("hddCongTyID").value);
    var maCongTy = document.getElementById("txtMaCongTy").value.trim();
    var tenCongTy = document.getElementById("txtTenCongTy").value.trim();

    if (!maCongTy) {
        alert("Vui lòng nhập Mã công ty!");
        document.getElementById("txtMaCongTy").focus();
        return;
    }

    if (!tenCongTy) {
        alert("Vui lòng nhập Tên công ty!");
        document.getElementById("txtTenCongTy").focus();
        return;
    }

    var payload = {
        congTyId: id,
        maCongTy: maCongTy,
        tenCongTy: tenCongTy,
        tenVietTat: document.getElementById("txtTenVietTat").value.trim(),
        maSoThue: document.getElementById("txtMaSoThue").value.trim(),
        diaChi: document.getElementById("txtDiaChi").value.trim(),
        trangThai: parseInt(document.getElementById("ddlTrangThai").value),
        moTa: document.getElementById("txtMoTa").value.trim()
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

// 5. Xóa thông tin (Soft Delete)
function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Công ty này khỏi hệ thống?")) {
        callWebMethod("DeleteData", { id: id }, function (res) {
            alert(res.message);
            loadData();
        });
    }
}

// Hàm hỗ trợ mã hóa HTML chống XSS khi render chuỗi ra UI
function escapeHtml(text) {
    if (!text) return "";
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}