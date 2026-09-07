/**
 * Thư viện thay thế alert()/confirm() gốc của trình duyệt bằng dialog phong cách macOS/Ubuntu.
 * Sử dụng:
 *   showToast("Lưu thành công!", "success");
 *   showConfirmDialog("Bạn có chắc chắn muốn xóa?").then(function(ok) { if (ok) { ... } });
 */

function showToast(message, type) {
    type = type || "info";
    var container = document.getElementById("uiToastContainer");
    if (!container) {
        container = document.createElement("div");
        container.id = "uiToastContainer";
        container.className = "ui-toast-container";
        document.body.appendChild(container);
    }

    var toast = document.createElement("div");
    toast.className = "ui-toast " + type;
    toast.textContent = message;
    container.appendChild(toast);

    requestAnimationFrame(function () {
        toast.classList.add("show");
    });

    setTimeout(function () {
        toast.classList.remove("show");
        setTimeout(function () { toast.remove(); }, 200);
    }, 2600);
}

function showConfirmDialog(message, options) {
    options = options || {};
    var okText = options.okText || "Đồng ý";
    var cancelText = options.cancelText || "Hủy bỏ";
    var danger = options.danger !== false; // mặc định nút chính là màu đỏ (dùng cho Xóa)
    var iconType = options.icon || "warning";
    var iconChar = iconType === "warning" ? "!" : (iconType === "error" ? "×" : "?");

    return new Promise(function (resolve) {
        var backdrop = document.createElement("div");
        backdrop.className = "ui-dialog-backdrop";
        backdrop.innerHTML = `
            <div class="ui-dialog-box">
                <div class="ui-dialog-icon ${iconType}">${iconChar}</div>
                <div class="ui-dialog-message"></div>
                <div class="ui-dialog-actions">
                    <button type="button" class="ui-dialog-btn ui-dialog-btn-secondary" data-action="cancel">${cancelText}</button>
                    <button type="button" class="ui-dialog-btn ${danger ? 'ui-dialog-btn-danger' : 'ui-dialog-btn-primary'}" data-action="ok">${okText}</button>
                </div>
            </div>
        `;
        backdrop.querySelector(".ui-dialog-message").textContent = message;
        document.body.appendChild(backdrop);

        requestAnimationFrame(function () {
            backdrop.classList.add("show");
        });

        function close(result) {
            backdrop.classList.remove("show");
            setTimeout(function () { backdrop.remove(); }, 180);
            resolve(result);
        }

        backdrop.querySelector('[data-action="ok"]').addEventListener("click", function () { close(true); });
        backdrop.querySelector('[data-action="cancel"]').addEventListener("click", function () { close(false); });
        backdrop.addEventListener("click", function (e) {
            if (e.target === backdrop) close(false); // Bấm ra ngoài = Hủy
        });
        document.addEventListener("keydown", function escHandler(e) {
            if (e.key === "Escape") {
                close(false);
                document.removeEventListener("keydown", escHandler);
            }
        });
    });
}

function showAlertDialog(message, iconType) {
    iconType = iconType || "info";
    var iconChar = iconType === "success" ? "✓" : (iconType === "error" ? "×" : "i");

    return new Promise(function (resolve) {
        var backdrop = document.createElement("div");
        backdrop.className = "ui-dialog-backdrop";
        backdrop.innerHTML = `
            <div class="ui-dialog-box">
                <div class="ui-dialog-icon ${iconType}">${iconChar}</div>
                <div class="ui-dialog-message"></div>
                <div class="ui-dialog-actions">
                    <button type="button" class="ui-dialog-btn ui-dialog-btn-primary" data-action="ok">OK</button>
                </div>
            </div>
        `;
        backdrop.querySelector(".ui-dialog-message").textContent = message;
        document.body.appendChild(backdrop);

        requestAnimationFrame(function () { backdrop.classList.add("show"); });

        function close() {
            backdrop.classList.remove("show");
            setTimeout(function () { backdrop.remove(); }, 180);
            resolve();
        }

        backdrop.querySelector('[data-action="ok"]').addEventListener("click", close);
        backdrop.addEventListener("click", function (e) { if (e.target === backdrop) close(); });
    });
}
//lầy thông tin thiết bị
function getOrCreateDeviceId() {
    var key = "vtt_device_id";
    var id = localStorage.getItem(key);
    if (!id) {
        id = "dev_" + Date.now() + "_" + Math.random().toString(36).substr(2, 12);
        localStorage.setItem(key, id);
    }
    return id;
}

function getDeviceName() {
    var ua = navigator.userAgent;
    var os = "Unknown OS";
    if (ua.indexOf("Windows") !== -1) os = "Windows";
    else if (ua.indexOf("Mac OS") !== -1) os = "macOS";
    else if (ua.indexOf("Android") !== -1) os = "Android";
    else if (ua.indexOf("iPhone") !== -1 || ua.indexOf("iPad") !== -1) os = "iOS";
    else if (ua.indexOf("Linux") !== -1) os = "Linux";

    var browser = "Trình duyệt";
    // if (ua.indexOf("CocCoc") !== -1) browser = "Cốc Cốc";
    if (ua.indexOf("Edg") !== -1) browser = "Edge";
    else if (ua.indexOf("Chrome") !== -1) browser = "Chrome";
    else if (ua.indexOf("Firefox") !== -1) browser = "Firefox";
    else if (ua.indexOf("Safari") !== -1) browser = "Safari";

    return browser + " trên " + os;
}
function parseAspNetDate(value) {
    if (!value) return null;
    if (typeof value === "string" && value.indexOf("/Date(") === 0) {
        var timestamp = parseInt(value.substr(6));
        return new Date(timestamp);
    }
    return new Date(value);
}
/**
 * Áp dụng logic ẩn/hiện filter Chi nhánh + Phòng ban dựa theo scope trả về từ GetPermission.
 * Dùng chung cho mọi trang có 2 dropdown filter kiểu này (Phòng ban, Nhân viên, Dự án...).
 *
 * @param {string} scope - "CONGTY" / "CHINHANH" / "PHONGBAN" (lấy từ res.data.scope)
 * @param {string} chiNhanhSelectId - id của <select> filter Chi nhánh
 * @param {string|null} phongBanSelectId - id của <select> filter Phòng ban (null nếu trang không có)
 * @param {number} myChiNhanhId - ChiNhanhID của tài khoản hiện tại
 * @param {function|null} loadPhongBanCallback - hàm (chiNhanhId) => tự load lại options Phòng ban khi scope=CHINHANH
 */
function applyScopeFilterVisibility(scope, chiNhanhSelectId, phongBanSelectId, myChiNhanhId, loadPhongBanCallback) {
    var chiNhanhEl = document.getElementById(chiNhanhSelectId);
    var chiNhanhGroup = chiNhanhEl ? chiNhanhEl.closest(".filter-group") : null;

    var phongBanEl = phongBanSelectId ? document.getElementById(phongBanSelectId) : null;
    var phongBanGroup = phongBanEl ? phongBanEl.closest(".filter-group") : null;

    if (scope === "CONGTY") {
        if (chiNhanhGroup) chiNhanhGroup.style.display = "";
        if (phongBanGroup) phongBanGroup.style.display = "";
    } else if (scope === "CHINHANH") {
        if (chiNhanhGroup) chiNhanhGroup.style.display = "none";
        if (phongBanGroup) {
            phongBanGroup.style.display = "";
            if (loadPhongBanCallback) loadPhongBanCallback(myChiNhanhId);
        }
    } else {
        if (chiNhanhGroup) chiNhanhGroup.style.display = "none";
        if (phongBanGroup) phongBanGroup.style.display = "none";
    }
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
/**
 * Render thanh phân trang dùng chung.
 * @param {string} containerId - id của <div> chứa thanh phân trang (tạo mới nếu chưa có)
 * @param {number} currentPage
 * @param {number} totalRows
 * @param {number} pageSize
 * @param {function} onPageChange - callback(newPage) khi người dùng đổi trang
 */
function renderPagination(containerId, currentPage, totalRows, pageSize, onPageChange) {
    var container = document.getElementById(containerId);
    if (!container) return;

    var totalPages = Math.max(1, Math.ceil(totalRows / pageSize));
    if (totalPages <= 1) {
        container.innerHTML = "";
        return;
    }

    var html = '<div style="display:flex; align-items:center; justify-content:center; gap:6px; margin-top:16px; flex-wrap:wrap;">';

    html += `<button type="button" class="btn-icon" ${currentPage <= 1 ? "disabled" : ""} onclick="__paginationGoTo('${containerId}', ${currentPage - 1})" style="padding:5px 10px;">‹ Trước</button>`;

    var startPage = Math.max(1, currentPage - 2);
    var endPage = Math.min(totalPages, startPage + 4);
    startPage = Math.max(1, endPage - 4);

    for (var p = startPage; p <= endPage; p++) {
        var isActive = p === currentPage;
        html += `<button type="button" class="btn-icon" onclick="__paginationGoTo('${containerId}', ${p})"
            style="padding:5px 10px; ${isActive ? 'background:#2563eb; color:#fff; border-radius:6px;' : ''}">${p}</button>`;
    }

    html += `<button type="button" class="btn-icon" ${currentPage >= totalPages ? "disabled" : ""} onclick="__paginationGoTo('${containerId}', ${currentPage + 1})" style="padding:5px 10px;">Sau ›</button>`;
    html += `<span style="margin-left:10px; color:#64748b; font-size:12.5px;">Tổng ${totalRows} bản ghi</span>`;
    html += '</div>';

    container.innerHTML = html;
    window["__paginationCallback_" + containerId] = onPageChange;
}

function __paginationGoTo(containerId, page) {
    var cb = window["__paginationCallback_" + containerId];
    if (cb) cb(page);
}