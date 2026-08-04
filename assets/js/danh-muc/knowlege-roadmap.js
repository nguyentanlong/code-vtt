document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    fetch("knowlege-roadmap.aspx/" + methodName, {
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

var mucDoText = { 1: "Thấp", 2: "Trung bình", 3: "Cao" };

function loadData() {
    var keyword = document.getElementById("txtSearchKeyword").value;
    var trangThai = document.getElementById("ddlSearchTrangThai").value;

    callWebMethod("GetList", { keyword: keyword, trangThai: trangThai }, function (res) {
        var tbody = document.getElementById("tbodyRoadmap");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var badgeClass = item.TrangThai === 1 ? "badge-success" : "badge-danger";
            var statusText = item.TrangThai === 1 ? "Đã hoàn thành" : "Chưa hoàn thành";

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.ChuDe)}</strong></td>
                <td>${escapeHtml(item.MoTa || '')}</td>
                <td style="text-align:center;">${mucDoText[item.MucDoUuTien] || item.MucDoUuTien}</td>
                <td><span class="badge ${badgeClass}">${statusText}</span></td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.RoadmapID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.RoadmapID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddRoadmapID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Chủ đề";
        document.getElementById("txtChuDe").value = "";
        document.getElementById("txtMoTa").value = "";
        document.getElementById("ddlMucDoUuTien").value = "2";
        document.getElementById("ddlTrangThai").value = "0";
        document.getElementById("modalRoadmap").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Chủ đề";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtChuDe").value = d.ChuDe;
            document.getElementById("txtMoTa").value = d.MoTa || "";
            document.getElementById("ddlMucDoUuTien").value = d.MucDoUuTien;
            document.getElementById("ddlTrangThai").value = d.TrangThai;
            document.getElementById("modalRoadmap").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalRoadmap").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddRoadmapID").value);
    var chuDe = document.getElementById("txtChuDe").value.trim();

    if (!chuDe) {
        alert("Vui lòng nhập Chủ đề!");
        document.getElementById("txtChuDe").focus();
        return;
    }

    var payload = {
        roadmapId: id,
        chuDe: chuDe,
        moTa: document.getElementById("txtMoTa").value.trim(),
        mucDoUuTien: parseInt(document.getElementById("ddlMucDoUuTien").value),
        trangThai: parseInt(document.getElementById("ddlTrangThai").value)
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Chủ đề này khỏi hệ thống?")) {
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