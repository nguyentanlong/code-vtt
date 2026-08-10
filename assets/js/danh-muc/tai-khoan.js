var nhanVienOptions = [];
var vaiTroOptions = [];

document.addEventListener("DOMContentLoaded", function () {
    Promise.all([loadNhanVienOptions(), loadVaiTroOptions()]).then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("tai-khoan.aspx/" + methodName, {
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

function loadNhanVienOptions() {
    return callWebMethod("GetNhanVienOptions", {}, function (res) {
        nhanVienOptions = res.data || [];
    });
}

function loadVaiTroOptions() {
    return callWebMethod("GetVaiTroOptions", {}, function (res) {
        vaiTroOptions = res.data || [];
    });
}

function renderNhanVienDropdown(selectedId, isEdit) {
    var ddl = document.getElementById("ddlNhanVien");
    ddl.innerHTML = '<option value="">-- Chọn nhân viên --</option>';
    nhanVienOptions.forEach(function (nv) {
        var selected = (selectedId && nv.NhanVienID == selectedId) ? "selected" : "";
        ddl.innerHTML += `<option value="${nv.NhanVienID}" ${selected}>${escapeHtml(nv.HoTen)} (${escapeHtml(nv.MaNhanVien)})</option>`;
    });
    ddl.disabled = !!isEdit; // Không cho đổi Nhân viên khi sửa tài khoản đã tồn tại
}

/*function renderVaiTroCheckboxes(selectedIds) {
    var container = document.getElementById("vaiTroCheckboxList");
    container.innerHTML = "";
    selectedIds = selectedIds || [];

    if (vaiTroOptions.length === 0) {
        container.innerHTML = '<span style="color:#888;">Chưa có Vai trò nào trong hệ thống.</span>';
        return;
    }

    vaiTroOptions.forEach(function (vt) {
        var checked = selectedIds.indexOf(vt.VaiTroID) !== -1 ? "checked" : "";
        container.innerHTML += `
            <label style="display:block; margin-bottom:6px; font-weight:400;">
                <input type="checkbox" class="chk-vaitro" value="${vt.VaiTroID}" ${checked} style="margin-right:6px;" />
                ${escapeHtml(vt.TenVaiTro)}
            </label>
        `;
    });
}*/
function renderVaiTroDropdown(selectedId) {
    var ddl = document.getElementById("ddlVaiTro");
    ddl.innerHTML = '<option value="">-- Chọn vai trò --</option>';
    vaiTroOptions.forEach(function (vt) {
        var selected = (selectedId && vt.VaiTroID == selectedId) ? "selected" : "";
        ddl.innerHTML += `<option value="${vt.VaiTroID}" ${selected}>${escapeHtml(vt.TenVaiTro)}</option>`;
    });
}
function getSelectedVaiTroIds() {
    var checked = document.querySelectorAll(".chk-vaitro:checked");
    var ids = [];
    checked.forEach(function (c) { ids.push(c.value); });
    return ids.join(",");
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyTaiKhoan");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="8" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";
            var lockBadge = item.IsLocked
                ? '<span class="badge badge-danger">Đã khóa</span>'
                : '<span class="badge badge-success">Bình thường</span>';

            var tr = document.createElement("tr");
            // đổi // <td>${escapeHtml(item.VaiTroNames || '(Chưa gán)')}</td> thành TenVaiTro
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.Username)}</strong></td>
                <td>${escapeHtml(item.HoTen)} (${escapeHtml(item.MaNhanVien)})</td>
                <td>${escapeHtml(item.TenVaiTro || '(Chưa gán)')}</td>
                <td>${escapeHtml(item.LastLoginText || 'Chưa đăng nhập')}</td>
                <td style="text-align:center; cursor:pointer;" onclick="toggleLock(${item.TaiKhoanID}, ${!item.IsLocked})" title="Bấm để đổi trạng thái khóa">
                    ${lockBadge}
                </td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.TaiKhoanID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.TaiKhoanID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function toggleLock(id, newLockState) {
    var actionText = newLockState ? "khóa" : "mở khóa";
    if (confirm("Bạn có chắc chắn muốn " + actionText + " tài khoản này?")) {
        callWebMethod("ToggleLock", { id: id, isLocked: newLockState }, function (res) {
            loadData();
        });
    }
}

function openModal(id) {
    document.getElementById("hddTaiKhoanID").value = id;
    document.getElementById("txtPassword").value = "";

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Tài khoản";
        document.getElementById("txtUsername").value = "";
        document.getElementById("lblPasswordLabel").innerHTML = 'Mật khẩu <span class="text-danger">*</span>';
        document.getElementById("passwordHint").style.display = "none";
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("ddlIsLocked").value = "0";
        renderNhanVienDropdown(null, false);
        renderVaiTroDropdown([]);
        document.getElementById("modalTaiKhoan").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Tài khoản";
        document.getElementById("lblPasswordLabel").innerText = "Mật khẩu mới";
        document.getElementById("passwordHint").style.display = "block";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtUsername").value = d.Username;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("ddlIsLocked").value = d.IsLocked ? "1" : "0";
            renderNhanVienDropdown(d.NhanVienID, true);
            renderVaiTroDropdown(d.VaiTroIds || []);
            document.getElementById("modalTaiKhoan").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalTaiKhoan").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddTaiKhoanID").value);
    var nhanVienId = document.getElementById("ddlNhanVien").value;
    var username = document.getElementById("txtUsername").value.trim();
    var password = document.getElementById("txtPassword").value;

    if (!nhanVienId) { alert("Vui lòng chọn Nhân viên!"); return; }
    if (!username) { alert("Vui lòng nhập Username!"); document.getElementById("txtUsername").focus(); return; }
    if (id === 0 && !password) { alert("Vui lòng nhập Mật khẩu cho tài khoản mới!"); document.getElementById("txtPassword").focus(); return; }

    var payload = {
        taiKhoanId: id,
        nhanVienId: parseInt(nhanVienId),
        username: username,
        password: password, // để trống khi sửa = giữ mật khẩu cũ
        isLocked: document.getElementById("ddlIsLocked").value === "1",
        trangThai: parseInt(document.getElementById("ddlTrangThai").value),
        vaiTroIds: getSelectedVaiTroIds()
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadNhanVienOptions().then(function () { loadData(); }); // reload options vì NV vừa gán account sẽ biến mất khỏi list "chưa có account"
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Tài khoản này khỏi hệ thống?")) {
        callWebMethod("DeleteData", { id: id }, function (res) {
            alert(res.message);
            loadNhanVienOptions().then(function () { loadData(); });
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