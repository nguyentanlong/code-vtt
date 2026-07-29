/**
 * Quản lý Danh mục Nhân Viên (nhan-vien.js)
 * Liên kết liên thông với DMPhongBan & DMBoPhan
 */
var CURRENT_CONG_TY_ID = 1;

$(document).ready(function () {
    // 1. Load danh sách Phòng ban vào dropdown & Load bảng Nhân viên
    loadDropdownPhongBan();
    loadData();

    // 2. Lọc danh sách khi bấm Enter ở ô tìm kiếm
    $('#txtSearch').on('keyup', function (e) {
        if (e.keyCode === 13) {
            loadData();
        }
    });

    // 3. Sự kiện click nút Search
    $('#btnSearch').on('click', function (e) {
        e.preventDefault();
        loadData();
    });

    // 4. Khi đổi Phòng Ban trên Modal -> Tự động load Bộ Phận thuộc Phòng Ban đó
    $('#ddlModalPhongBan').on('change', function () {
        var phongBanId = parseInt($(this).val()) || 0;
        loadDropdownBoPhanByPhongBan(phongBanId, null);
    });
});

// ==========================================
// LOAD DỮ LIỆU BẢNG VÀ DROPDOWN
// ==========================================
function loadData() {
    var keyword = $('#txtSearch').val() || '';

    $.ajax({
        type: "POST",
        url: window.location.pathname + "/GetList",
        data: JSON.stringify({
            congTyId: CURRENT_CONG_TY_ID,
            keyword: keyword,
            trangThai: 1 // 1: Đang làm việc
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success) {
                renderTableNhanVien(res.data);
            } else {
                alert(res.message || "Lỗi tải dữ liệu nhân viên!");
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi GetList NhanVien:", xhr.responseText);
        }
    });
}

// Tải danh sách Phòng ban
function loadDropdownPhongBan() {
    $.ajax({
        type: "POST",
        url: "bo-phan.aspx/GetListPhongBan", // Tái sử dụng WebMethod Phòng ban
        data: JSON.stringify({ congTyId: CURRENT_CONG_TY_ID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success) {
                var html = '<option value="">-- Chọn Phòng Ban --</option>';
                if (res.data && res.data.length > 0) {
                    $.each(res.data, function (i, item) {
                        html += '<option value="' + item.PhongBanID + '">' + item.TenPhongBan + '</option>';
                    });
                }
                $('#ddlModalPhongBan').html(html);
            }
        }
    });
}

// Tải danh sách Bộ Phận theo Phòng Ban đã chọn (Cascading)
function loadDropdownBoPhanByPhongBan(phongBanId, selectedBoPhanId) {
    var $ddlBoPhan = $('#ddlModalBoPhan');

    if (!phongBanId || phongBanId <= 0) {
        $ddlBoPhan.html('<option value="">-- Chọn Bộ Phận --</option>');
        return;
    }

    $.ajax({
        type: "POST",
        url: window.location.pathname + "/GetBoPhanByPhongBan",
        data: JSON.stringify({ phongBanId: phongBanId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            var html = '<option value="">-- Chọn Bộ Phận --</option>';
            if (res.success && res.data && res.data.length > 0) {
                $.each(res.data, function (i, item) {
                    html += '<option value="' + item.BoPhanID + '">' + item.TenBoPhan + '</option>';
                });
            }
            $ddlBoPhan.html(html);

            if (selectedBoPhanId) {
                $ddlBoPhan.val(selectedBoPhanId);
            }
        }
    });
}

// Render dữ liệu bảng Nhân viên
function renderTableNhanVien(list) {
    var html = '';
    if (list && list.length > 0) {
        $.each(list, function (index, item) {
            var trangThaiBadge = item.TrangThai === 1
                ? '<span class="badge bg-success">Đang làm việc</span>'
                : '<span class="badge bg-secondary">Đã nghỉ việc</span>';

            html += '<tr>';
            html += '<td class="text-center">' + (index + 1) + '</td>';
            html += '<td><b>' + (item.MaNhanVien || '') + '</b></td>';
            html += '<td>' + (item.HoTen || '') + '</td>';
            html += '<td>' + (item.TenPhongBan || '') + '</td>';
            html += '<td>' + (item.TenBoPhan || '<i class="text-muted">Chưa phân bộ phận</i>') + '</td>';
            html += '<td class="text-center">' + trangThaiBadge + '</td>';
            html += '<td class="text-center">';

            // NÚT SỬA (Thêm type="button" và return false để KHÔNG BỊ REFRESH/F5 TRANG)
            html += '  <button type="button" class="btn btn-sm btn-outline-primary me-1 btn-edit" '
                + ' data-id="' + item.NhanVienID + '"'
                + ' data-ma="' + (item.MaNhanVien || '') + '"'
                + ' data-ten="' + (item.HoTen || '') + '"'
                + ' data-phongban="' + (item.PhongBanID || '') + '"'
                + ' data-bophan="' + (item.BoPhanID || '') + '"'
                + ' data-trangthai="' + item.TrangThai + '" onclick="onClickEdit(this); return false;">';
            html += '     <i class="fa fa-edit"></i> Sửa';
            html += '  </button>';

            // NÚT XÓA (Thêm type="button" và return false)
            html += '  <button type="button" class="btn btn-sm btn-outline-danger" onclick="deleteNhanVien(' + item.NhanVienID + ', \'' + (item.MaNhanVien || '') + '\'); return false;">';
            html += '     <i class="fa fa-trash"></i> Xóa';
            html += '  </button>';

            html += '</td>';
            html += '</tr>';
        });
    } else {
        html = '<tr><td colspan="7" class="text-center text-muted">Không có dữ liệu nhân viên</td></tr>';
    }
    $('#tbodyNhanVien').html(html);
}

// Hàm sự kiện khi click nút Sửa (Bắt trực tiếp từ thuộc tính data-*)
function onClickEdit(btn) {
    var $btn = $(btn);
    var id = $btn.data('id');
    var ma = $btn.data('ma');
    var ten = $btn.data('ten');
    var phongBanId = parseInt($btn.data('phongban')) || null;
    var boPhanId = parseInt($btn.data('bophan')) || null;
    var trangThai = parseInt($btn.data('trangthai')) || 1;

    openModalEdit(id, ma, ten, phongBanId, boPhanId, trangThai);
}

// Hàm Xóa Nhân Viên
function deleteNhanVien(nhanVienId, maNhanVien) {
    if (!confirm('Bạn có chắc chắn muốn xóa nhân viên [' + maNhanVien + '] không?')) {
        return;
    }

    $.ajax({
        type: "POST",
        url: window.location.pathname + "/DeleteData",
        data: JSON.stringify({ nhanVienId: nhanVienId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success) {
                alert(res.message || "Xóa nhân viên thành công!");
                loadData();
            } else {
                alert("Lỗi: " + res.message);
            }
        },
        error: function (xhr) {
            console.error("Lỗi DeleteData:", xhr.responseText);
            alert("Lỗi hệ thống khi thực hiện xóa dữ liệu!");
        }
    });
}

// ==========================================
// MODAL & XỬ LÝ LƯU DỮ LIỆU
// ==========================================
function openModalAdd() {
    clearForm();
    $('#modalTitle').text('Thêm Mới Nhân Viên');
    $('#btnSave').attr('onclick', 'saveNhanVien(0); return false;');
    $('#modalNhanVien').modal('show');
}

function openModalEdit(nhanVienId, maNhanVien, hoTen, phongBanId, boPhanId, trangThai) {
    clearForm();
    $('#modalTitle').text('Cập Nhật Nhân Viên');

    $('#txtMaNhanVien').val(maNhanVien);
    $('#txtHoTen').val(hoTen);
    $('#ddlModalPhongBan').val(phongBanId ? phongBanId : "");
    $('#chkTrangThai').prop('checked', trangThai === 1);

    if (phongBanId) {
        loadDropdownBoPhanByPhongBan(phongBanId, boPhanId);
    }

    $('#btnSave').attr('onclick', 'saveNhanVien(' + nhanVienId + '); return false;');
    $('#modalNhanVien').modal('show');
}

function clearForm() {
    $('#txtMaNhanVien').val('');
    $('#txtHoTen').val('');
    $('#ddlModalPhongBan').val('');
    $('#ddlModalBoPhan').html('<option value="">-- Chọn Bộ Phận --</option>');
    $('#chkTrangThai').prop('checked', true);
}

function saveNhanVien(nhanVienId) {
    var maNhanVien = $('#txtMaNhanVien').val().trim();
    var hoTen = $('#txtHoTen').val().trim();
    var phongBanId = parseInt($('#ddlModalPhongBan').val()) || null;
    var trangThai = $('#chkTrangThai').is(':checked') ? 1 : 0;

    if (!maNhanVien) {
        alert('Vui lòng nhập Mã Nhân Viên!');
        $('#txtMaNhanVien').focus();
        return;
    }
    if (!hoTen) {
        alert('Vui lòng nhập Họ Tên Nhân Viên!');
        $('#txtHoTen').focus();
        return;
    }

    var payload = {
        nhanVienId: nhanVienId,
        congTyId: CURRENT_CONG_TY_ID,
        maNhanVien: maNhanVien,
        hoTen: hoTen,
        phongBanId: phongBanId,
        trangThai: trangThai
    };

    $.ajax({
        type: "POST",
        url: window.location.pathname + "/SaveData",
        data: JSON.stringify(payload),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success) {
                alert(res.message);
                $('#modalNhanVien').modal('hide');
                loadData();
            } else {
                alert("Lỗi: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi SaveData Nhân viên:", xhr.responseText);
            alert("Có lỗi xảy ra trong quá trình lưu dữ liệu!");
        }
    });
}