document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    fetch("danh-muc-rule.aspx/" + methodName, {
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
        var tbody = document.getElementById("tbodyRule");
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
                <td><strong>${escapeHtml(item.MaRule)}</strong></td>
                <td>${escapeHtml(item.TenRule)}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.RuleID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.RuleID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddRuleID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Rule";
        document.getElementById("txtMaRule").value = "";
        document.getElementById("txtMaRule").readOnly = false;
        document.getElementById("txtTenRule").value = "";
        document.getElementById("txtDieuKien").value = "";
        document.getElementById("txtHanhDong").value = "";
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("modalRule").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Rule";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaRule").value = d.MaRule;
            document.getElementById("txtMaRule").readOnly = true;
            document.getElementById("txtTenRule").value = d.TenRule;
            document.getElementById("txtDieuKien").value = d.DieuKien || "";
            document.getElementById("txtHanhDong").value = d.HanhDong || "";
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("modalRule").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalRule").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddRuleID").value);
    var maRule = document.getElementById("txtMaRule").value.trim();
    var tenRule = document.getElementById("txtTenRule").value.trim();

    if (!maRule) { alert("Vui lòng nhập Mã rule!"); document.getElementById("txtMaRule").focus(); return; }
    if (!tenRule) { alert("Vui lòng nhập Tên rule!"); document.getElementById("txtTenRule").focus(); return; }

    var payload = {
        ruleId: id,
        maRule: maRule,
        tenRule: tenRule,
        dieuKien: document.getElementById("txtDieuKien").value.trim(),
        hanhDong: document.getElementById("txtHanhDong").value.trim(),
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Rule này khỏi hệ thống?")) {
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