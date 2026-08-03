var congTyOptions = [];
var vaiTroList = [];
var editingId = null; // null = không có dòng nào đang sửa

document.addEventListener("DOMContentLoaded", function () {
    loadCongTyOptions(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    fetch("vai-tro-du-an.aspx/" + methodName, {
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

// Tải danh sách công ty cho dropdown lọc + dropdown trong dòng edit
function loadCongTyOptions(callback) {
    callWebMethod("GetCongTyOptions", {}, function (res) {
        congTyOptions = res.data || [];

        var ddlFilter = document.getElementById("ddlSearchCongTy");
        ddlFilter.innerHTML = '<option value="">-- Tất cả công ty --</option>';
        congTyOptions.forEach(function (ct) {
            ddlFilter.innerHTML += `<option value="${ct.CongTyID}">${escapeHtml(ct.TenCongTy)}</option>`;
        });

        if (callback) callback();
    });
}

function buildCongTySelectHtml(selectedId) {
    var html = '<select class="form-control edit-congty">';
    congTyOptions.forEach(function (ct) {
        var selected = (selectedId && ct.CongTyID == selectedId) ? "selected" : "";
        html += `<option value="${ct.CongTyID}" ${selected}>${escapeHtml(ct.TenCongTy)}</option>`;
    });
    html += "</select>";
    return html;
}

// 1. Tải danh sách vai trò dự án
function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var congTyId = document.getElementById("ddlSearchCongTy").value;

    callWebMethod("GetList", { keyword: keyword, congTyId: congTyId }, function (res) {
        vaiTroList = res.data || [];
        editingId = null;
        renderTable();
    });
}

function renderTable() {
    var tbody = document.getElementById("tbodyVaiTro");
    tbody.innerHTML = "";

    if (!vaiTroList || vaiTroList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
        return;
    }

    vaiTroList.forEach(function (item, index) {
        var tr = document.createElement("tr");
        tr.setAttribute("data-id", item.VaiTroDuAnID);

        if (editingId !== null && editingId == item.VaiTroDuAnID) {
            tr.innerHTML = buildEditRowHtml(index + 1, item);
        } else {
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaVaiTro)}</strong></td>
                <td>${escapeHtml(item.TenVaiTro)}</td>
                <td>${escapeHtml(item.TenCongTy)}</td>
                <td style="text-align:center;">${item.ThuTu}</td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="editRow(${item.VaiTroDuAnID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.VaiTroDuAnID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
        }
        tbody.appendChild(tr);
    });
}

function buildEditRowHtml(stt, item) {
    var id = item ? item.VaiTroDuAnID : 0;
    var maVaiTro = item ? item.MaVaiTro : "";
    var tenVaiTro = item ? item.TenVaiTro : "";
    var thuTu = item ? item.ThuTu : (vaiTroList.length + 1);
    var congTyId = item ? item.CongTyID : "";

    return `
        <td style="text-align:center;">${stt}</td>
        <td><input type="text" class="form-control edit-mavaitro" value="${escapeAttr(maVaiTro)}" placeholder="VD: PM" /></td>
        <td><input type="text" class="form-control edit-tenvaitro" value="${escapeAttr(tenVaiTro)}" placeholder="Tên vai trò..." /></td>
        <td>${buildCongTySelectHtml(congTyId)}</td>
        <td><input type="number" class="form-control edit-thutu" value="${thuTu}" style="text-align:center;" /></td>
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

// 2. Bấm "Sửa" -> chuyển dòng đó sang chế độ input
function editRow(id) {
    editingId = id;
    renderTable();
}

// 3. Bấm "Thêm mới" -> chèn 1 dòng trống ở đầu bảng, ở chế độ edit
function addNewRow() {
    if (congTyOptions.length === 0) {
        alert("Chưa có Công ty nào để chọn. Vui lòng thêm Công ty trước!");
        return;
    }
    editingId = 0;

    var tbody = document.getElementById("tbodyVaiTro");
    var tr = document.createElement("tr");
    tr.setAttribute("data-id", "0");
    tr.innerHTML = buildEditRowHtml(vaiTroList.length + 1, null);
    tbody.insertBefore(tr, tbody.firstChild);
}

// 4. Hủy sửa -> render lại bảng theo dữ liệu cũ (bỏ dòng mới thêm nếu có)
function cancelEdit() {
    editingId = null;
    renderTable();
}

// 5. Lưu dòng đang sửa (id = 0 là thêm mới)
function saveRow(id) {
    var row = document.querySelector('tr[data-id="' + id + '"]');
    var maVaiTro = row.querySelector(".edit-mavaitro").value.trim();
    var tenVaiTro = row.querySelector(".edit-tenvaitro").value.trim();
    var congTyId = row.querySelector(".edit-congty").value;
    var thuTu = parseInt(row.querySelector(".edit-thutu").value) || 0;

    if (!maVaiTro) {
        alert("Vui lòng nhập Mã vai trò!");
        return;
    }
    if (!tenVaiTro) {
        alert("Vui lòng nhập Tên vai trò!");
        return;
    }

    callWebMethod("SaveData", {
        vaiTroDuAnId: id,
        congTyId: parseInt(congTyId),
        maVaiTro: maVaiTro,
        tenVaiTro: tenVaiTro,
        thuTu: thuTu
    }, function (res) {
        editingId = null;
        loadData();
    });
}

// 6. Xóa
function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Vai trò dự án này khỏi hệ thống?")) {
        callWebMethod("DeleteData", { id: id }, function (res) {
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

function escapeAttr(text) {
    return escapeHtml(text).replace(/`/g, "&#96;");
}