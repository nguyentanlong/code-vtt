document.addEventListener("DOMContentLoaded", function () {
    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    fetch("nguon-tri-thuc.aspx/" + methodName, {
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

    callWebMethod("GetList", { keyword: keyword }, function (res) {
        var tbody = document.getElementById("tbodyNguonTriThuc");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:#888;">Không tìm thấy dữ liệu nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td><strong>${escapeHtml(item.MaNguon)}</strong></td>
                <td>${escapeHtml(item.TenNguon)}</td>
                <td style="text-align:center;">${item.DoUuTien}</td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.NguonTriThucID})" title="Chỉnh sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                    <button type="button" class="btn-icon text-delete" onclick="deleteData(${item.NguonTriThucID})" title="Xóa">
                        <i class="fa fa-trash"></i> Xóa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function openModal(id) {
    document.getElementById("hddNguonTriThucID").value = id;

    if (id === 0) {
        document.getElementById("modalTitle").innerText = "Thêm mới Nguồn tri thức";
        document.getElementById("txtMaNguon").value = "";
        document.getElementById("txtMaNguon").readOnly = false;
        document.getElementById("txtTenNguon").value = "";
        document.getElementById("txtDoUuTien").value = "0";
        document.getElementById("modalNguonTriThuc").style.display = "flex";
    } else {
        document.getElementById("modalTitle").innerText = "Chỉnh sửa Nguồn tri thức";
        callWebMethod("GetById", { id: id }, function (res) {
            var d = res.data;
            document.getElementById("txtMaNguon").value = d.MaNguon;
            document.getElementById("txtMaNguon").readOnly = true;
            document.getElementById("txtTenNguon").value = d.TenNguon;
            document.getElementById("txtDoUuTien").value = d.DoUuTien;
            document.getElementById("modalNguonTriThuc").style.display = "flex";
        });
    }
}

function closeModal() {
    document.getElementById("modalNguonTriThuc").style.display = "none";
}

function saveData() {
    var id = parseInt(document.getElementById("hddNguonTriThucID").value);
    var maNguon = document.getElementById("txtMaNguon").value.trim();
    var tenNguon = document.getElementById("txtTenNguon").value.trim();

    if (!maNguon) {
        alert("Vui lòng nhập Mã nguồn!");
        document.getElementById("txtMaNguon").focus();
        return;
    }
    if (!tenNguon) {
        alert("Vui lòng nhập Tên nguồn tri thức!");
        document.getElementById("txtTenNguon").focus();
        return;
    }

    var payload = {
        nguonTriThucId: id,
        maNguon: maNguon,
        tenNguon: tenNguon,
        doUuTien: parseInt(document.getElementById("txtDoUuTien").value) || 0
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function deleteData(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Nguồn tri thức này khỏi hệ thống?")) {
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