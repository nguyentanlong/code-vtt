var phongBanOptions = [];
var modelOptions = [];

document.addEventListener("DOMContentLoaded", function () {
    Promise.all([loadPhongBanOptions(), loadModelOptions()]).then(function () {
        loadData();
    });
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("tac-nhan-ai.aspx/" + methodName, {
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
        var ddl = document.getElementById("ddlPhongBan");
        ddl.innerHTML = '<option value="">-- Chọn phòng ban --</option>';
        phongBanOptions.forEach(function (pb) {
            ddl.innerHTML += `<option value="${pb.PhongBanID}">${escapeHtml(pb.TenPhongBan)}</option>`;
        });
    });
}

function loadModelOptions() {
    return callWebMethod("GetModelOptions", {}, function (res) {
        modelOptions = res.data || [];
        var ddl = document.getElementById("ddlModel");
        ddl.innerHTML = '<option value="">-- Không chọn --</option>';
        modelOptions.forEach(function (m) {
            ddl.innerHTML += `<option value="${m.ModelID}">${escapeHtml(m.TenModel)} (${escapeHtml(m.Provider)})</option>`;
        });
    });
}

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyAIAgent");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đang hoạt động" : "Ngừng hoạt động";

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaAgent)}</strong></td>
                <td>${escapeHtml(item.TenAgent)}</td>
                <td>${escapeHtml(item.TenPhongBan || '')}</td>
                <td>${escapeHtml(item.TenModel || '(Chưa chọn)')}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.AIAgentID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.AIAgentID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddAIAgentID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Tác nhân AI";
        document.getElementById("txtMaAgent").value = "";
        document.getElementById("txtMaAgent").readOnly = false;
        document.getElementById("txtTenAgent").value = "";
        document.getElementById("ddlPhongBan").value = "";
        document.getElementById("ddlModel").value = "";
        document.getElementById("ddlTrangThai").value = "1";
        document.getElementById("modalAIAgent").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Tác nhân AI";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaAgent").value = d.MaAgent;
            document.getElementById("txtMaAgent").readOnly = true;
            document.getElementById("txtTenAgent").value = d.TenAgent;
            document.getElementById("ddlPhongBan").value = d.PhongBanID;
            document.getElementById("ddlModel").value = d.ModelMacDinhID || "";
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("modalAIAgent").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalAIAgent").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddAIAgentID").value);
    var maAgent = document.getElementById("txtMaAgent").value.trim();
    var tenAgent = document.getElementById("txtTenAgent").value.trim();
    var phongBanId = document.getElementById("ddlPhongBan").value;
    var modelId = document.getElementById("ddlModel").value;

    if (!maAgent) {
        alert("Vui lòng nhập Mã tác nhân!");
        document.getElementById("txtMaAgent").focus();
        return;
    }
    if (!tenAgent) {
        alert("Vui lòng nhập Tên tác nhân!");
        document.getElementById("txtTenAgent").focus();
        return;
    }
    if (!phongBanId) {
        alert("Vui lòng chọn Phòng ban!");
        return;
    }

    var payload = {
        aiAgentId: id,
        phongBanId: parseInt(phongBanId),
        maAgent: maAgent,
        tenAgent: tenAgent,
        modelMacDinhId: modelId ? parseInt(modelId) : null,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Tác nhân AI này khỏi hệ thống?")) {
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