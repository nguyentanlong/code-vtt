var fpNgayBatDau, fpNgayKetThuc;
var canThemPermission = false;

document.addEventListener("DOMContentLoaded", function () {
    var fpOptions = { dateFormat: "d/m/Y", allowInput: true };
    fpNgayBatDau = flatpickr("#txtNgayBatDau", fpOptions);
    fpNgayKetThuc = flatpickr("#txtNgayKetThuc", fpOptions);

    Promise.all([loadPermission(), loadChiNhanhOptions(), loadLoaiCongTrinhOptions()]).then(function () {
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

function loadPermission() {
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

function loadPhongBanOptions(chiNhanhId, targetSelectId, selectedId) {
    return callWebMethod("GetPhongBanOptions", { chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null }, function (res) {
        var ddl = document.getElementById(targetSelectId);
        ddl.innerHTML = '<option value="">-- Chọn phòng ban --</option>';
        (res.data || []).forEach(function (pb) {
            var selected = (selectedId && pb.PhongBanID == selectedId) ? "selected" : "";
            ddl.innerHTML += `<option value="${pb.PhongBanID}" ${selected}>${escapeHtml(pb.TenPhongBan)}</option>`;
        });
    });
}

function loadLoaiCongTrinhOptions() {
    return callWebMethod("GetLoaiCongTrinhOptions", {}, function (res) {
        var ddl = document.getElementById("ddlLoaiCongTrinh");
        ddl.innerHTML = '<option value="">-- Không chọn --</option>';
        (res.data || []).forEach(function (lct) {
            ddl.innerHTML += `<option value="${lct.LoaiCongTrinhID}">${escapeHtml(lct.TenLoai)}</option>`;
        });
    });
}

function onFilterChiNhanhChange() {
    var chiNhanhId = document.getElementById("ddlSearchChiNhanh").value;
    loadPhongBanOptions(chiNhanhId, "ddlSearchPhongBan", null).then(function () { loadData(); });
}

function onFormChiNhanhChange() {
    var chiNhanhId = document.getElementById("ddlFormChiNhanh").value;
    loadPhongBanOptions(chiNhanhId, "ddlFormPhongBan", null);
}

var trangThaiConfig = {
    0: { text: "Chuẩn bị", cssClass: "badge-status-chua" },
    1: { text: "Đang thực hiện", cssClass: "badge-status-dang" },
    2: { text: "Hoàn thành", cssClass: "badge-status-ht" },
    3: { text: "Tạm dừng", cssClass: "badge-danger" }
};

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var chiNhanhId = document.getElementById("ddlSearchChiNhanh").value;
    var phongBanId = document.getElementById("ddlSearchPhongBan").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", {
        keyword: keyword,
        chiNhanhId: chiNhanhId ? parseInt(chiNhanhId) : null,
        phongBanId: phongBanId ? parseInt(phongBanId) : null,
        trangThai: trangThai
    }, function (res) {
        var tbody = document.getElementById("tbodyDuAn");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var st = trangThaiConfig[item.TrangThaiDuAn] || trangThaiConfig[0];

            var tenHienThi = escapeHtml(item.TenDuAn);
            if (item.LaNguoiTao) {
                tenHienThi += ' <span class="badge badge-success" style="margin-left:6px;">Của bạn</span>';
            }

            var actionsHtml = `
                <a href="chi-tiet-gd.aspx?id=${item.DuAnID}" class="btn-icon" style="color:#0284c7;" title="Xem Timeline">
                    <i class="fa fa-timeline"></i>
                </a>
            `;
            if (item.CanEditRow) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.DuAnID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                `;
            }
            if (item.CanDeleteRow) {
                actionsHtml += `
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.DuAnID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                `;
            }

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaDuAn)}</strong></td>
                <td>${tenHienThi}</td>
                <td>${escapeHtml(item.TenPhongBan || '')}</td>
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
        document.getElementById("ddlFormChiNhanh").value = "";
        document.getElementById("ddlLoaiCongTrinh").value = "";
        document.getElementById("txtChuDauTu").value = "";
        document.getElementById("txtDiaDiem").value = "";
        document.getElementById("txtMoTa").value = "";
        fpNgayBatDau.clear();
        fpNgayKetThuc.clear();
        document.getElementById("ddlTrangThai").value = "0";
        loadPhongBanOptions(null, "ddlFormPhongBan", null).then(function () {
            document.getElementById("modalDuAn").style.display = "flex";
        });
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Dự án";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaDuAn").value = d.MaDuAn;
            document.getElementById("txtMaDuAn").readOnly = true;
            document.getElementById("txtTenDuAn").value = d.TenDuAn;
            document.getElementById("ddlFormChiNhanh").value = d.ChiNhanhID || "";
            document.getElementById("ddlLoaiCongTrinh").value = d.LoaiCongTrinhID || "";
            document.getElementById("txtChuDauTu").value = d.ChuDauTu || "";
            document.getElementById("txtDiaDiem").value = d.DiaDiem || "";
            document.getElementById("txtMoTa").value = d.MoTa || "";
            fpNgayBatDau.setDate(parseAspNetDate(d.NgayBatDau), false);
            fpNgayKetThuc.setDate(parseAspNetDate(d.NgayKetThucDuKien), false);
            document.getElementById("ddlTrangThai").value = d.TrangThaiDuAn;
            loadPhongBanOptions(d.ChiNhanhID, "ddlFormPhongBan", d.PhongBanID).then(function () {
                document.getElementById("modalDuAn").style.display = "flex";
            });
        });
    }
}

function closeModal() {
    document.getElementById("modalDuAn").style.display = "none";
}

function toIsoDate(fp) {
    var selected = fp.selectedDates[0];
    if (!selected) return "";
    return selected.getFullYear() + "-" + String(selected.getMonth() + 1).padStart(2, "0") + "-" + String(selected.getDate()).padStart(2, "0");
}

function saveData(lyDo) {
    var id = parseInt(document.getElementById("hddDuAnID").value);
    var maDuAn = document.getElementById("txtMaDuAn").value.trim();
    var tenDuAn = document.getElementById("txtTenDuAn").value.trim();
    var phongBanId = document.getElementById("ddlFormPhongBan").value;
    var loaiCongTrinhId = document.getElementById("ddlLoaiCongTrinh").value;

    if (!maDuAn) { showToast("Vui lòng nhập Mã dự án!", "error"); return; }
    if (!tenDuAn) { showToast("Vui lòng nhập Tên dự án!", "error"); return; }
    if (!phongBanId) { showToast("Vui lòng chọn Phòng ban!", "error"); return; }

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
        trangThaiDuAn: parseInt(document.getElementById("ddlTrangThai").value),
        lyDo: lyDo || ""
    };

    fetch("du-an.aspx/SaveData", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(payload)
    })
        .then(response => response.json())
        .then(res => {
            var result = res.d;
            if (result.success) {
                showToast(result.message, "success");
                closeModal();
                loadData();
            } else if (result.requireReason) {
                askReasonAndRetry(result.message, function (nhapLyDo) {
                    saveData(nhapLyDo);
                });
            } else {
                showToast(result.message, "error");
            }
        })
        .catch(function () {
            showToast("Lỗi kết nối máy chủ!", "error");
        });
}

function deleteData(id, lyDo) {
    showConfirmDialog("Bạn có chắc chắn muốn xóa Dự án này khỏi hệ thống?").then(function (ok) {
        if (!ok) return;

        fetch("du-an.aspx/DeleteData", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ id: id, lyDo: lyDo || "" })
        })
            .then(response => response.json())
            .then(res => {
                var result = res.d;
                if (result.success) {
                    showToast(result.message, "success");
                    loadData();
                } else if (result.requireReason) {
                    askReasonAndRetry(result.message, function (nhapLyDo) {
                        deleteData(id, nhapLyDo);
                    });
                } else {
                    showToast(result.message, "error");
                }
            })
            .catch(function () {
                showToast("Lỗi kết nối máy chủ!", "error");
            });
    });
}

function askReasonAndRetry(message, onConfirm) {
    var backdrop = document.createElement("div");
    backdrop.className = "ui-dialog-backdrop";
    backdrop.innerHTML = `
        <div class="ui-dialog-box">
            <div class="ui-dialog-icon warning">!</div>
            <div class="ui-dialog-message">${message}</div>
            <textarea id="txtLyDoInput" class="form-control" rows="3" placeholder="Nhập lý do..." style="margin-top:8px;"></textarea>
            <div class="ui-dialog-actions" style="margin-top:14px;">
                <button type="button" class="ui-dialog-btn ui-dialog-btn-secondary" id="btnHuyLyDo">Hủy bỏ</button>
                <button type="button" class="ui-dialog-btn ui-dialog-btn-primary" id="btnXacNhanLyDo">Xác nhận</button>
            </div>
        </div>
    `;
    document.body.appendChild(backdrop);
    requestAnimationFrame(function () { backdrop.classList.add("show"); });

    function close() {
        backdrop.classList.remove("show");
        setTimeout(function () { backdrop.remove(); }, 180);
    }

    backdrop.querySelector("#btnHuyLyDo").addEventListener("click", close);
    backdrop.querySelector("#btnXacNhanLyDo").addEventListener("click", function () {
        var lyDo = backdrop.querySelector("#txtLyDoInput").value.trim();
        if (!lyDo) {
            showToast("Vui lòng nhập Lý do!", "error");
            return;
        }
        close();
        onConfirm(lyDo);
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