var currentDuAnId = 0;
var fpKeHoachBatDau, fpKeHoachKetThuc, fpThucTeBatDau, fpThucTeKetThuc;

document.addEventListener("DOMContentLoaded", function () {
    var params = new URLSearchParams(window.location.search);
    currentDuAnId = parseInt(params.get("id")) || 0;
    document.getElementById("lblDuAnID").textContent = currentDuAnId;

    var fpOptions = { dateFormat: "d/m/Y", allowInput: true };
    fpKeHoachBatDau = flatpickr("#txtKeHoachBatDau", fpOptions);
    fpKeHoachKetThuc = flatpickr("#txtKeHoachKetThuc", fpOptions);
    fpThucTeBatDau = flatpickr("#txtThucTeBatDau", fpOptions);
    fpThucTeKetThuc = flatpickr("#txtThucTeKetThuc", fpOptions);

    loadData();
});

function callWebMethod(methodName, dataObj, successCallback) {
    return fetch("chi-tiet-gd.aspx/" + methodName, {
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

var trangThaiConfig = {
    0: { text: "Chưa", cssClass: "badge-status-chua" },
    1: { text: "Đang", cssClass: "badge-status-dang" },
    2: { text: "HT", cssClass: "badge-status-ht" }
};

function formatDateVN(isoString) {
    // if (!isoString) return "--";
    // var d = new Date(isoString);
    // var dd = String(d.getDate()).padStart(2, "0");
    // var mm = String(d.getMonth() + 1).padStart(2, "0");
    // return `${dd}/${mm}/${d.getFullYear()}`;

    var d = parseAspNetDate(isoString);
    if (!d || isNaN(d.getTime())) return "--";
    var dd = String(d.getDate()).padStart(2, "0");
    var mm = String(d.getMonth() + 1).padStart(2, "0");
    return `${dd}/${mm}/${d.getFullYear()}`;

    /*if (!isoString) return "--";

    var d;
    if (typeof isoString === "string" && isoString.indexOf("/Date(") === 0) {
        // Định dạng ASP.NET AJAX: "/Date(1785600000000)/"
        var timestamp = parseInt(isoString.substr(6));
        d = new Date(timestamp);
    } else {
        d = new Date(isoString);
    }

    if (isNaN(d.getTime())) return "--";

    var dd = String(d.getDate()).padStart(2, "0");
    var mm = String(d.getMonth() + 1).padStart(2, "0");
    return `${dd}/${mm}/${d.getFullYear()}`;*/
}

function loadData() {
    callWebMethod("GetList", { duAnId: currentDuAnId }, function (res) {
        var tbody = document.getElementById("tbodyTimeline");
        tbody.innerHTML = "";

        if (!res.data || res.data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="8" style="text-align:center; color:#888;">Chưa có giai đoạn nào</td></tr>';
            return;
        }

        res.data.forEach(function (item, index) {
            var st = trangThaiConfig[item.TrangThai] || trangThaiConfig[0];

            var keHoachText = `${formatDateVN(item.KeHoachBatDau)}<br/><span style="color:#94a3b8;">&rarr; ${formatDateVN(item.KeHoachKetThuc)}</span>`;
            var thucTeText = `${formatDateVN(item.ThucTeBatDau)}<br/><span style="color:#94a3b8;">&rarr; ${formatDateVN(item.ThucTeKetThuc)}</span>`;

            var tr = document.createElement("tr");
            tr.innerHTML = `
                <td style="text-align:center;">${index + 1}</td>
                <td>
                    <strong>${escapeHtml(item.TenGiaiDoan)}</strong><br/>
                    <small style="color:#94a3b8;">${escapeHtml(item.MaGiaiDoan)}</small>
                </td>
                <td>${keHoachText}</td>
                <td>${thucTeText}</td>
                <td>
                    <div class="progress-bar-wrap">
                        <div class="progress-bar-fill" style="width:${item.TienDo}%;"></div>
                    </div>
                    <small>${item.TienDo}%</small>
                </td>
                <td style="text-align:center;">
                    <span class="badge-status ${st.cssClass}">${st.text}</span>
                </td>
                <td>${escapeHtml(item.GhiChu || '')}</td>
                <td style="text-align:center;">
                    <button type="button" class="btn-icon text-edit" onclick="openModal(${item.DuAnGiaiDoanID})" title="Sửa">
                        <i class="fa fa-edit"></i> Sửa
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function parseAspNetDate(value) {
    if (!value) return null;
    if (typeof value === "string" && value.indexOf("/Date(") === 0) {
        var timestamp = parseInt(value.substr(6));
        return new Date(timestamp);
    }
    return new Date(value);
}

function openModal(id) {
    document.getElementById("hddDuAnGiaiDoanID").value = id;

    callWebMethod("GetById", { id: id }, function (res) {
        var d = res.data;
        fpKeHoachBatDau.setDate(parseAspNetDate(d.KeHoachBatDau), false);
        fpKeHoachKetThuc.setDate(parseAspNetDate(d.KeHoachKetThuc), false);
        fpThucTeBatDau.setDate(parseAspNetDate(d.ThucTeBatDau), false);
        fpThucTeKetThuc.setDate(parseAspNetDate(d.ThucTeKetThuc), false);
        document.getElementById("txtTienDo").value = d.TienDo;
        document.getElementById("ddlTrangThai").value = d.TrangThai;
        document.getElementById("txtGhiChu").value = d.GhiChu || "";
        document.getElementById("modalGiaiDoan").style.display = "flex";
    });
}

function closeModal() {
    document.getElementById("modalGiaiDoan").style.display = "none";
}

function toIsoDate(flatpickrInstance) {
    var selected = flatpickrInstance.selectedDates[0];
    if (!selected) return "";
    return selected.getFullYear() + "-" + String(selected.getMonth() + 1).padStart(2, "0") + "-" + String(selected.getDate()).padStart(2, "0");
}

function saveData() {
    var id = parseInt(document.getElementById("hddDuAnGiaiDoanID").value);
    var tienDo = parseFloat(document.getElementById("txtTienDo").value) || 0;

    if (tienDo < 0 || tienDo > 100) {
        alert("Tiến độ phải trong khoảng 0 - 100!");
        return;
    }

    var payload = {
        duAnGiaiDoanId: id,
        duAnId: currentDuAnId,
        keHoachBatDau: toIsoDate(fpKeHoachBatDau),
        keHoachKetThuc: toIsoDate(fpKeHoachKetThuc),
        thucTeBatDau: toIsoDate(fpThucTeBatDau),
        thucTeKetThuc: toIsoDate(fpThucTeKetThuc),
        tienDo: tienDo,
        trangThai: parseInt(document.getElementById("ddlTrangThai").value),
        ghiChu: document.getElementById("txtGhiChu").value.trim()
    };

    callWebMethod("SaveData", payload, function (res) {
        alert(res.message);
        closeModal();
        loadData();
    });
}

function syncTimeline() {
    callWebMethod("Sync", { duAnId: currentDuAnId }, function (res) {
        alert(res.message);
        loadData();
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