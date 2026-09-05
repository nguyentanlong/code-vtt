var currentQuyTrinhId = 0;
var vaiTroOptions = [];
var buocList = [];
var canEditPermission = false;
var editingId = null;

document.addEventListener("DOMContentLoaded", function () {
    var params = new URLSearchParams(window.location.search);
    currentQuyTrinhId = parseInt(params.get("id")) || 0;
    document.getElementById("lblQuyTrinhID").textContent = currentQuyTrinhId;

    if (currentQuyTrinhId <= 0) {
        document.getElementById("tbodyBuoc").innerHTML = '<tr><td colspan="5" style="text-align:center; color:#dc2626;">Thiếu tham số QuyTrinhID trên URL.</td></tr>';
        return;
    }

    Promise.all([loadPermission(), loadVaiTroOptions()]).then(function () {
        loadData();
        loadLichSu();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("chi-tiet-quy-trinh.aspx/" + methodName, {
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
                showToast(result ? result.message : "Thao tác thất bại!", "error");
            }
        })
        .catch(function () { showToast("Lỗi kết nối máy chủ!", "error"); });
}

function loadPermission() {
    return callWebMethod("GetPermission", {}, function (res) {
        canEditPermission = res.data.canEdit;
        document.getElementById("btnAddNew").style.display = canEditPermission ? "inline-flex" : "none";
    });
}

function loadVaiTroOptions() {
    return callWebMethod("GetVaiTroOptions", {}, function (res) { vaiTroOptions = res.data || []; });
}

function buildVaiTroSelectHtml(selectedId) {
    var html = '<select class="form-control edit-vaitro"><option value="">-- Không yêu cầu duyệt --</option>';
    vaiTroOptions.forEach(function (vt) {
        var selected = (selectedId && vt.VaiTroID == selectedId) ? "selected" : "";
        html += `<option value="${vt.VaiTroID}" ${selected}>${escapeHtml(vt.TenVaiTro)}</option>`;
    });
    html += "</select>";
    return html;
}

function loadData() {
    callWebMethod("GetList", { quyTrinhId: currentQuyTrinhId }, function (res) {
        buocList = res.data || [];
        editingId = null;
        renderTable();
    });
}

function renderTable() {
    var tbody = document.getElementById("tbodyBuoc");
    tbody.innerHTML = "";

    if (!buocList || buocList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:#888;">Chưa có bước nào</td></tr>';
        return;
    }

    buocList.forEach(function (item) {
        var tr = document.createElement("tr");
        tr.setAttribute("data-id", item.BuocID);

        if (editingId !== null && editingId == item.BuocID) {
            tr.innerHTML = buildEditRowHtml(item);
        } else {
            tr.innerHTML = `
                <td style="text-align:center;">${item.ThuTu}</td>
                <td>${escapeHtml(item.TenBuoc)}</td>
                <td>${escapeHtml(item.TenVaiTro || '(Không yêu cầu)')}</td>
                <td>${escapeHtml(item.HanhDong || '')}</td>
                <td style="text-align:center;">
                    ${canEditPermission ? `
                        <button type="button" class="btn-icon text-edit" onclick="editRow(${item.BuocID})" title="Sửa">
                            <i class="fa fa-edit"></i> Sửa
                        </button>
                        <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.BuocID})" title="Xóa">
                            <i class="fa fa-trash"></i> Xóa
                        </button>
                    ` : '<span style="color:#94a3b8; font-size:12px;">Chỉ xem</span>'}
                </td>
            `;
        }
        tbody.appendChild(tr);
    });
}

function buildEditRowHtml(item) {
    var id = item ? item.BuocID : 0;
    var tenBuoc = item ? item.TenBuoc : "";
    var thuTu = item ? item.ThuTu : (buocList.length + 1);
    var vaiTroId = item ? item.VaiTroDuyetID : null;
    var hanhDong = item ? item.HanhDong : "";

    return `
        <td><input type="number" class="form-control edit-thutu" value="${thuTu}" style="text-align:center;" /></td>
        <td><input type="text" class="form-control edit-tenbuoc" value="${escapeAttr(tenBuoc)}" placeholder="Tên bước..." /></td>
        <td>${buildVaiTroSelectHtml(vaiTroId)}</td>
        <td><input type="text" class="form-control edit-hanhdong" value="${escapeAttr(hanhDong)}" placeholder="VD: Duyệt/Từ chối" /></td>
        <td style="text-align:center;">
            <button type="button" class="btn-icon text-save" onclick="saveRow(${id})" title="Lưu">
                <i class="fa fa-save"></i> Lưu
            </button>
            <button type="button" class="btn-icon text-cancel" onclick="cancelEdit()" title="Hủy">
                <i class="fa fa-times"></i> Hủy
            </button>
        </td>
    `;
}

function editRow(id) { if (!canEditPermission) return; editingId = id; renderTable(); }

function addNewRow() {
    if (!canEditPermission) return;
    editingId = 0;
    var tbody = document.getElementById("tbodyBuoc");
    var tr = document.createElement("tr");
    tr.setAttribute("data-id", "0");
    tr.innerHTML = buildEditRowHtml(null);
    tbody.appendChild(tr);
}

function cancelEdit() { editingId = null; renderTable(); }

function saveRow(id) {
    var row = document.querySelector('tr[data-id="' + id + '"]');
    var tenBuoc = row.querySelector(".edit-tenbuoc").value.trim();
    var thuTu = parseInt(row.querySelector(".edit-thutu").value) || 0;
    var vaiTroId = row.querySelector(".edit-vaitro").value;
    var hanhDong = row.querySelector(".edit-hanhdong").value.trim();

    if (!tenBuoc) { showToast("Vui lòng nhập Tên bước!", "error"); return; }

    callWebMethod("SaveData", {
        buocId: id, quyTrinhId: currentQuyTrinhId, tenBuoc: tenBuoc, thuTu: thuTu,
        vaiTroDuyetId: vaiTroId ? parseInt(vaiTroId) : null, hanhDong: hanhDong
    }, function () {
        editingId = null;
        loadData();
    });
}

function deleteData(id) {
    showConfirmDialog("Bạn có chắc chắn muốn xóa Bước này khỏi Quy trình?").then(function (ok) {
        if (!ok) return;
        callWebMethod("DeleteData", { id: id }, function () { loadData(); });
    });
}

var ketQuaConfig = { 0: { text: "Từ chối", cssClass: "badge-danger" }, 1: { text: "Đồng ý", cssClass: "badge-success" } };

function loadLichSu() {
    callWebMethod("GetLichSu", { quyTrinhId: currentQuyTrinhId }, function (res) {
        var tbody = document.getElementById("tbodyLichSu");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align:center; color:#888;">Chưa có lịch sử duyệt nào</td></tr>';
            return;
        }

        res.data.forEach(function (item) {
            var kq = item.KetQua !== null ? (ketQuaConfig[item.KetQua] || { text: "--", cssClass: "" }) : { text: "Đang chờ", cssClass: "" };
            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${escapeHtml(item.NgayXuLyText)}</td>
                <td>${escapeHtml(item.TenBuoc)}</td>
                <td>${escapeHtml(item.ObjectType)} #${item.ObjectID}</td>
                <td>${escapeHtml(item.TenNguoiXuLy || '')}</td>
                <td><span class="badge ${kq.cssClass}">${kq.text}</span></td>
                <td>${escapeHtml(item.LyDo || '')}</td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function escapeHtml(text) {
    if (!text) return "";
    return text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
}
function escapeAttr(text) { return escapeHtml(text).replace(/`/g, "&#96;"); }