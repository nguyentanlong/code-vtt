/**
 * Quản lý Danh mục Bộ Phận (bo-phan.js)
 * Khớp chuẩn 100% với HTML ID: #ddlModalPhongBan và #ddlFilterPhongBan
 */

var CURRENT_CONG_TY_ID = 1;

$(document).ready(function () {
    // 1. Load các dropdown trước
    loadDropdownPhongBan();
    loadDropdownNhanVien();

    // 2. Load bảng danh sách
    loadData();

    // 3. Lọc khi nhấn Enter ở ô tìm kiếm
    $('#txtSearch').on('keyup', function (e) {
        if (e.keyCode === 13) {
            loadData();
        }
    });
});

// ==========================================
// TẢI DỮ LIỆU TỪ SERVER & RENDER DROPDOWN
// ==========================================

// Hàm load danh sách bộ phận (được gọi từ onchange="loadData()" ở ddlFilterPhongBan)
function loadData() {
    var keyword = $('#txtSearch').val() || '';
    var phongBanId = parseInt($('#ddlFilterPhongBan').val()) || 0;

    $.ajax({
        type: "POST",
        url: window.location.pathname + "/GetList",
        data: JSON.stringify({
            phongBanId: phongBanId,
            keyword: keyword
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success) {
                renderTableBoPhan(res.data);
            } else {
                alert(res.message || "Lỗi tải dữ liệu!");
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi GetList:", xhr.responseText);
        }
    });
}

// Tải danh sách Phòng Ban đổ vào ddlFilterPhongBan và ddlModalPhongBan
function loadDropdownPhongBan() {
    $.ajax({
        type: "POST",
        url: window.location.pathname + "/GetListPhongBan",
        data: JSON.stringify({ congTyId: CURRENT_CONG_TY_ID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success) {
                var optionsModal = '<option value="">-- Chọn Phòng Ban --</option>';
                var optionsFilter = '<option value="0">-- Tất cả phòng ban --</option>';

                if (res.data && res.data.length > 0) {
                    $.each(res.data, function (i, item) {
                        var opt = '<option value="' + item.PhongBanID + '">' + item.TenPhongBan + ' (' + item.MaPhongBan + ')</option>';
                        optionsModal += opt;
                        optionsFilter += opt;
                    });
                }

                // Đổ dữ liệu vào select Bộ lọc ngoài trang
                if ($('#ddlFilterPhongBan').length) {
                    $('#ddlFilterPhongBan').html(optionsFilter);
                }

                // Đổ dữ liệu vào select ddlModalPhongBan trong Modal
                if ($('#ddlModalPhongBan').length) {
                    $('#ddlModalPhongBan').html(optionsModal);
                }

                // Trigger cập nhật giao diện nếu dùng Select2 / Bootstrap-Select
                if ($.fn.select2) {
                    $('#ddlFilterPhongBan, #ddlModalPhongBan').select2().trigger('change.select2');
                }
                if ($.fn.selectpicker) {
                    $('#ddlFilterPhongBan, #ddlModalPhongBan').selectpicker('refresh');
                }
            } else {
                console.warn("Lỗi GetListPhongBan: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi GetListPhongBan:", xhr.responseText);
        }
    });
}

// Tải danh sách Nhân viên vào Dropdown Trưởng Bộ Phận
function loadDropdownNhanVien() {
    $.ajax({
        type: "POST",
        url: window.location.pathname + "/GetListNhanVien",
        data: JSON.stringify({ congTyId: CURRENT_CONG_TY_ID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success) {
                var html = '<option value="">-- Chọn Trưởng Bộ Phận --</option>';
                if (res.data && res.data.length > 0) {
                    $.each(res.data, function (i, item) {
                        html += '<option value="' + item.NhanVienID + '">' + item.TenHienThi + '</option>';
                    });
                }
                $('#ddlTruongBoPhan').html(html);

                if ($.fn.select2) {
                    $('#ddlTruongBoPhan').select2().trigger('change.select2');
                }
                if ($.fn.selectpicker) {
                    $('#ddlTruongBoPhan').selectpicker('refresh');
                }
            } else {
                console.warn("Lỗi GetListNhanVien: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi GetListNhanVien:", xhr.responseText);
        }
    });
}

// Render dữ liệu lên bảng
function renderTableBoPhan(list) {
    var html = '';

    if (list && list.length > 0) {
        $.each(list, function (index, item) {
            var trangThaiBadge = item.TrangThai === 1
                ? '<span class="badge bg-success">Hoạt động</span>'
                : '<span class="badge bg-secondary">Ngừng</span>';

            var safeMa = (item.MaBoPhan || '').replace(/'/g, "\\'");
            var safeTen = (item.TenBoPhan || '').replace(/'/g, "\\'");

            html += '<tr>';
            html += '<td class="text-center">' + (index + 1) + '</td>';
            html += '<td><b>' + item.MaBoPhan + '</b></td>';
            html += '<td>' + item.TenBoPhan + '</td>';
            html += '<td>' + (item.TenTruongBoPhan || '<i class="text-muted">Chưa có</i>') + '</td>';
            html += '<td class="text-center">' + item.ThuTu + '</td>';
            html += '<td class="text-center">' + trangThaiBadge + '</td>';
            html += '<td class="text-center">' + item.NgayTaoText + '</td>';
            html += '<td class="text-center">';
            html += '  <button class="btn btn-sm btn-outline-primary me-1" onclick="openModalEdit('
                + item.BoPhanID + ','
                + item.PhongBanID + ',\''
                + safeMa + '\',\''
                + safeTen + '\','
                + (item.TruongBoPhanID || 'null') + ','
                + item.ThuTu + ','
                + item.TrangThai + ')"><i class="fa fa-edit"></i> Sửa</button>';
            html += '</td>';
            html += '</tr>';
        });
    } else {
        html = '<tr><td colspan="8" class="text-center text-muted">Không tìm thấy dữ liệu bộ phận</td></tr>';
    }

    $('#tbodyBoPhan').html(html);
}

// ==========================================
// THAO TÁC FORM & MODAL
// ==========================================

function openModalAdd() {
    clearForm();
    $('#modalTitle').text('Thêm Mới Bộ Phận');

    // Tự động chọn phòng ban đang lọc ngoài màn hình (nếu có)
    var currentFilterPb = $('#ddlFilterPhongBan').val();
    if (currentFilterPb && currentFilterPb !== "0") {
        $('#ddlModalPhongBan').val(currentFilterPb);
    }

    if ($.fn.select2) {
        $('#ddlModalPhongBan').trigger('change.select2');
    }

    $('#btnSave').attr('onclick', 'saveBoPhan(0)');
    $('#modalBoPhan').modal('show');
}

function openModalEdit(boPhanId, phongBanId, maBoPhan, tenBoPhan, truongBoPhanId, thuTu, trangThai) {
    clearForm();
    $('#modalTitle').text('Cập Nhật Bộ Phận');

    // Đã đổi thành #ddlModalPhongBan
    $('#ddlModalPhongBan').val(phongBanId);
    $('#txtMaBoPhan').val(maBoPhan);
    $('#txtTenBoPhan').val(tenBoPhan);
    $('#ddlTruongBoPhan').val(truongBoPhanId ? truongBoPhanId : "");
    $('#txtThuTu').val(thuTu);
    $('#chkTrangThai').prop('checked', trangThai === 1);

    if ($.fn.select2) {
        $('#ddlModalPhongBan, #ddlTruongBoPhan').trigger('change.select2');
    }

    $('#btnSave').attr('onclick', 'saveBoPhan(' + boPhanId + ')');
    $('#modalBoPhan').modal('show');
}

function clearForm() {
    $('#txtMaBoPhan').val('');
    $('#txtTenBoPhan').val('');
    $('#ddlModalPhongBan').val('');
    $('#ddlTruongBoPhan').val('');
    $('#txtThuTu').val(0);
    $('#chkTrangThai').prop('checked', true);

    if ($.fn.select2) {
        $('#ddlModalPhongBan, #ddlTruongBoPhan').trigger('change.select2');
    }
}

// ==========================================
// LƯU DỮ LIỆU
// ==========================================

function saveBoPhan(boPhanId) {
    // Đã đổi thành #ddlModalPhongBan
    var phongBanId = parseInt($('#ddlModalPhongBan').val()) || 0;
    var maBoPhan = $('#txtMaBoPhan').val().trim();
    var tenBoPhan = $('#txtTenBoPhan').val().trim();
    var truongBoPhanVal = $('#ddlTruongBoPhan').val();
    var truongBoPhanId = truongBoPhanVal ? parseInt(truongBoPhanVal) : null;
    var thuTu = parseInt($('#txtThuTu').val()) || 0;
    var trangThai = $('#chkTrangThai').is(':checked') ? 1 : 0;

    // Validate
    if (phongBanId <= 0) {
        alert('Vui lòng chọn Phòng Ban!');
        $('#ddlModalPhongBan').focus();
        return;
    }
    if (!maBoPhan) {
        alert('Vui lòng nhập Mã Bộ Phận!');
        $('#txtMaBoPhan').focus();
        return;
    }
    if (!tenBoPhan) {
        alert('Vui lòng nhập Tên Bộ Phận!');
        $('#txtTenBoPhan').focus();
        return;
    }

    var payload = {
        boPhanId: boPhanId,
        phongBanId: phongBanId,
        maBoPhan: maBoPhan,
        tenBoPhan: tenBoPhan,
        truongBoPhanId: truongBoPhanId,
        thuTu: thuTu,
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
                $('#modalBoPhan').modal('hide');
                loadData();
            } else {
                alert("Lỗi: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi SaveData:", xhr.responseText);
            alert("Đã xảy ra lỗi hệ thống khi lưu dữ liệu!");
        }
    });
}