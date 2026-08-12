var phongBanOptions = [];
var loaiCongTrinhOptions = [];
var fpNgayBatDau, fpNgayKetThuc;

document.addEventListener("DOMContentLoaded", function () {
    var fpOptions = { dateFormat: "d/m/Y", allowInput: true };
    fpNgayBatDau = flatpickr("#txtNgayBatDau", fpOptions);
    fpNgayKetThuc = flatpickr("#txtNgayKetThuc", fpOptions);

    Promise.all([loadPhongBanOptions(), loadLoaiCongTrinhOptions()]).then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("du-an.aspx/" + methodName, {
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

function loadPhongBanOptions() {
    return callWebMethod("GetPhongBanOptions", {}, function (res) {
        phongBanOptions = res.data || [];

        var ddlFilter = document.getElementById("ddlSearchPhongBan");
        ddlFilter.innerHTML = '<option value="">-- Tất cả phòng ban --</option>';
        var ddlForm = document.getElementById("ddlPhongBan");
        ddlForm.innerHTML = '<option value="">-- Chọn phòng ban --</option>';

        phongBanOptions.forEach(function (pb) {
            var opt = `<option value="${pb.PhongBanID}">${escapeHtml(pb.TenPhongBan)}</option>`;
            ddlFilter.innerHTML += opt;
            ddlForm.innerHTML += opt;
        });
    });
}

function loadLoaiCongTrinhOptions() {
    return callWebMethod("GetLoaiCongTrinhOptions", {}, function (res) {
        loaiCongTrinhOptions = res.data || [];
        var ddl = document.getElementById("ddlLoaiCongTrinh");
        ddl.innerHTML = '<option value="">-- Không chọn --</option>';
        loaiCongTrinhOptions.forEach(function (lct) {
            ddl.innerHTML += `<option value="${lct.LoaiCongTrinhID}">${escapeHtml(lct.TenLoai)}</option>`;
        });
    });
}

var trangThaiConfig = {
    0: { text: "Chuẩn bị", cssClass: "badge-status-chua" },
    1: { text: "Đang thực hiện", cssClass: "badge-status-dang" },
    2: { text: "Hoàn thành", cssClass: "badge-status-ht" },
    3: { text: "Tạm dừng", cssClass: "badge-danger" }
};

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var phongBanId = document.getElementById("ddlSearchPhongBan").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, phongBanId: phongBanId, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyDuAn");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="8" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var st = trangThaiConfig[item.TrangThaiDuAn] || trangThaiConfig[0];

            // Chỉ hiện nút Sửa/Xóa nếu tài khoản có quyền trên Phòng ban của dự án này
            var actionsHtml = `
                <a href="chi-tiet-gd.aspx?id=${item.DuAnID}" class="btn-icon" style="color:#0284c7;" title="Xem Timeline">
                    <i class="fa fa-timeline"></i> Timeline
                </a>
            `;
            if (item.CanEditRow) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.DuAnID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.DuAnID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                `;
            }

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaDuAn)}</strong></td>
                <td>${escapeHtml(item.TenDuAn)}</td>
                <td>${escapeHtml(item.TenPhongBan || '')}</td>
                <td>${escapeHtml(item.TenLoai || '')}</td>
                <td style="text-align:center;">${item.TienDo}%</td>
                <td><span class="badge-status ${st.cssClass}">${st.text}</span></td>
                <td style="text-align:center; white-space:nowrap;">${actionsHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddDuAnID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Dự án";
        document.getElementById("txtMaDuAn").value = "";
        document.getElementById("txtMaDuAn").readOnly = false;
        document.getElementById("txtTenDuAn").value = "";
        document.getElementById("ddlPhongBan").value = "";
        document.getElementById("ddlLoaiCongTrinh").value = "";
        document.getElementById("txtChuDauTu").value = "";
        document.getElementById("txtDiaDiem").value = "";
        document.getElementById("txtMoTa").value = "";
        fpNgayBatDau.clear();
        fpNgayKetThuc.clear();
        document.getElementById("ddlTrangThai").value = "0";
        document.getElementById("modalDuAn").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Dự án";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaDuAn").value = d.MaDuAn;
            document.getElementById("txtMaDuAn").readOnly = true;
            document.getElementById("txtTenDuAn").value = d.TenDuAn;
            document.getElementById("ddlPhongBan").value = d.PhongBanID;
            document.getElementById("ddlLoaiCongTrinh").value = d.LoaiCongTrinhID || "";
            document.getElementById("txtChuDauTu").value = d.ChuDauTu || "";
            document.getElementById("txtDiaDiem").value = d.DiaDiem || "";
            document.getElementById("txtMoTa").value = d.MoTa || "";
            fpNgayBatDau.setDate(d.NgayBatDau ? new Date(d.NgayBatDau) : null, false);
            fpNgayKetThuc.setDate(d.NgayKetThucDuKien ? new Date(d.NgayKetThucDuKien) : null, false);
            document.getElementById("ddlTrangThai").value = d.TrangThaiDuAn;
            document.getElementById("modalDuAn").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalDuAn").style.display = "none";
}

function toIsoDate(flatpickrInstance) {
    var selected = flatpickrInstance.selectedDates[0];
    if (!selected) return "";
    return selected.getFullYear() + "-" + String(selected.getMonth() + 1).padStart(2, "0") + "-" + String(selected.getDate()).padStart(2, "0");
}

function saveData() {
    var id = parseInt(document.getElementById("hddDuAnID").value);
    var maDuAn = document.getElementById("txtMaDuAn").value.trim();
    var tenDuAn = document.getElementById("txtTenDuAn").value.trim();
    var phongBanId = document.getElementById("ddlPhongBan").value;
    var loaiCongTrinhId = document.getElementById("ddlLoaiCongTrinh").value;

    if (!maDuAn) { alert("Vui lòng nhập Mã dự án!"); document.getElementById("txtMaDuAn").focus(); return; }
    if (!tenDuAn) { alert("Vui lòng nhập Tên dự án!"); document.getElementById("txtTenDuAn").focus(); return; }
    if (!phongBanId) { alert("Vui lòng chọn Phòng ban!"); return; }

    var payload = {
        duAnId: id,
        phongBanId: parseInt(phongBanId),
        loaiCongTrinhId: loaiCongTrinhId ? parseInt(loaiCongTrinhId) : null,
        maDuAn: maDuAn,
        tenDuAn: tenDuAn,
        chuDauTu: document.getElementById("txtChuDauTu").value.trim(),
        diaDiem: document.getElementById("txtDiaDiem").value.trim(),
        moTa: document.getElementById("txtMoTa").value.trim(),
        ngayBatDau: toIsoDate(fpNgayBatDau),
        ngayKetThuc: toIsoDate(fpNgayKetThuc),
        trangThaiDuAn: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Dự án này khỏi hệ thống?")) {
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