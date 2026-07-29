let currentCongTyId = 1; // ID công ty hiện tại (có thể lấy từ Session/HiddenField)

$(document).ready(function () {
    loadData();
    loadDropdownPhongBanCha();
});

// 1. LẤY DANH SÁCH PHÒNG BAN (Đồng bộ với GetList Backend)
function loadData() {
    const keyword = $("#txtKeyword").val() ? $("#txtKeyword").val().trim() : "";
    const trangThai = $("#ddlFilterTrangThai").val() || "";

    $.ajax({
        type: "POST",
        url: "phong-ban.aspx/GetList",
        data: JSON.stringify({
            congTyId: currentCongTyId,
            keyword: keyword,
            trangThai: trangThai
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            const response = res.d;
            if (response.success) {
                renderTable(response.data);
            } else {
                alert("Thông báo: " + response.message);
            }
        },
        error: function (err) {
            console.error("Lỗi AJAX GetList:", err);
            alert("Không thể kết nối đến máy chủ!");
        }
    });
}

// 2. RENDER DỮ LIỆU LÊN BẢNG
function renderTable(list) {
    let html = "";
    if (!list || list.length === 0) {
        html = '<tr><td colspan="9" class="text-center text-muted py-4">Không có dữ liệu phòng ban nào</td></tr>';
        $("#tblDataPhongBan").html(html);
        $("#lblPaginationInfo").text("Hiển thị 0 bản ghi"); // Cập nhật khi rỗng
        return;
    }

    list.forEach((item, index) => {
        const stt = index + 1;
        const statusBadge = item.TrangThai === 1
            ? '<span class="badge bg-success">Đang hoạt động</span>'
            : '<span class="badge bg-danger">Ngừng hoạt động</span>';

        let indent = "";
        for (let i = 1; i < item.CapDo; i++) {
            indent += '<span class="tree-indent">&nbsp;&nbsp;&nbsp;&nbsp;</span>|-- ';
        }

        html += `
            <tr>
                <td class="text-center">${stt}</td>
                <td><strong>${item.MaPhongBan}</strong></td>
                <td>${indent}${item.TenPhongBan}</td>
                <td>${item.TenPhongBanCha || "-"}</td>
                <td>${item.TenTruongPhong || "-"}</td>
                <td class="text-center">${item.CapDo}</td>
                <td class="text-center">${item.ThuTu}</td>
                <td class="text-center">${statusBadge}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-primary me-1" title="Sửa" onclick="openModalEdit(${item.PhongBanID})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger" title="Xóa" onclick="deleteData(${item.PhongBanID})">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    });

    $("#tblDataPhongBan").html(html);

    // THÊM DÒNG NÀY ĐỂ THAY THẾ CHỮ "ĐANG TẢI..."
    $("#lblPaginationInfo").html(`Hiển thị tổng số <strong>${list.length}</strong> phòng ban`);
}

// 3. LOAD DROPDOWN PHÒNG BAN CHA
function loadDropdownPhongBanCha() {
    $.ajax({
        type: "POST",
        url: "phong-ban.aspx/GetList",
        data: JSON.stringify({
            congTyId: currentCongTyId,
            keyword: "",
            trangThai: "1" // Chỉ lấy phòng ban đang hoạt động làm cấp cha
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            const response = res.d;
            if (response.success) {
                let options = '<option value="0">-- Là phòng ban cấp cao nhất --</option>';
                response.data.forEach(item => {
                    options += `<option value="${item.PhongBanID}">${item.TenPhongBan} (${item.MaPhongBan})</option>`;
                });
                $("#ddlPhongBanCha").html(options);
            }
        }
    });
}

// 4. LẤY CHI TIẾT ĐỂ SỬA (Đồng bộ với GetById Backend)
function openModalEdit(id) {
    $.ajax({
        type: "POST",
        url: "phong-ban.aspx/GetById",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            const response = res.d;
            if (response.success) {
                const item = response.data;
                $("#hdfPhongBanID").val(item.PhongBanID);
                $("#txtMaPhongBan").val(item.MaPhongBan);
                $("#txtTenPhongBan").val(item.TenPhongBan);
                $("#ddlPhongBanCha").val(item.PhongBanChaID || 0);
                $("#txtTruongPhongID").val(item.TruongPhongID || "");
                $("#txtThuTu").val(item.ThuTu);
                $("#ddlTrangThai").val(item.TrangThai);

                $("#modalTitle").text("Cập nhật Phòng Ban");
                $("#modalPhongBan").modal("show");
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            console.error("Lỗi AJAX GetById:", err);
        }
    });
}

// 5. MỞ MODAL THÊM MỚI
function openModalAdd() {
    $("#hdfPhongBanID").val(0);
    $("#txtMaPhongBan").val("");
    $("#txtTenPhongBan").val("");
    $("#ddlPhongBanCha").val(0);
    $("#txtTruongPhongID").val("");
    $("#txtThuTu").val(0);
    $("#ddlTrangThai").val(1);

    $("#modalTitle").text("Thêm mới Phòng Ban");
    $("#modalPhongBan").modal("show");
}

// 6. LƯU DỮ LIỆU (Đồng bộ với SaveData Backend)
function saveData() {
    const phongBanId = parseInt($("#hdfPhongBanID").val()) || 0;
    const maPhongBan = $("#txtMaPhongBan").val().trim();
    const tenPhongBan = $("#txtTenPhongBan").val().trim();
    const phongBanChaId = parseInt($("#ddlPhongBanCha").val()) || null;
    const truongPhongId = parseInt($("#txtTruongPhongID").val()) || null;
    const thuTu = parseInt($("#txtThuTu").val()) || 0;
    const trangThai = parseInt($("#ddlTrangThai").val());

    // Validate client
    if (!maPhongBan) {
        alert("Vui lòng nhập Mã phòng ban!");
        $("#txtMaPhongBan").focus();
        return;
    }
    if (!tenPhongBan) {
        alert("Vui lòng nhập Tên phòng ban!");
        $("#txtTenPhongBan").focus();
        return;
    }

    $.ajax({
        type: "POST",
        url: "phong-ban.aspx/SaveData",
        data: JSON.stringify({
            phongBanId: phongBanId,
            congTyId: currentCongTyId,
            maPhongBan: maPhongBan,
            tenPhongBan: tenPhongBan,
            phongBanChaId: phongBanChaId,
            truongPhongId: truongPhongId,
            thuTu: thuTu,
            trangThai: trangThai
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            const response = res.d;
            alert(response.message);
            if (response.success) {
                $("#modalPhongBan").modal("hide");
                loadData();
                loadDropdownPhongBanCha();
            }
        },
        error: function (err) {
            console.error("Lỗi AJAX SaveData:", err);
            alert("Lưu dữ liệu thất bại!");
        }
    });
}

// 7. XÓA PHÒNG BAN (Đồng bộ với DeleteData Backend)
function deleteData(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa phòng ban này?")) return;

    $.ajax({
        type: "POST",
        url: "phong-ban.aspx/DeleteData",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            const response = res.d;
            alert(response.message);
            if (response.success) {
                loadData();
                loadDropdownPhongBanCha();
            }
        },
        error: function (err) {
            console.error("Lỗi AJAX DeleteData:", err);
            alert("Xóa thất bại!");
        }
    });
}