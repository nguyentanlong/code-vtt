/**
 * Quản lý Danh mục Bộ Phận (bo-phan.js)
 * Khớp chuẩn 100% với HTML ID: #ddlModalPhongBan, #ddlFilterPhongBan và Custom Select #chonTruongBoPhan
 */

var CURRENT_CONG_TY_ID = 1;
var CURRENT_EDIT_ID = 0; // Biến lưu trữ ID đang thao tác (0: Thêm mới, >0: Cập nhật)
var CACHE_NHAN_VIEN_DATA = []; // Cache danh sách nhân viên để phục vụ re-render Modal

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
function loadDropdownNhanVien(selectedId) {
    var defaultVal = (selectedId !== undefined && selectedId !== null) ? selectedId : -1;

    $.ajax({
        type: "POST",
        url: window.location.pathname + "/GetListNhanVien",
        data: JSON.stringify({ congTyId: CURRENT_CONG_TY_ID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var res = response.d;
            if (res.success && res.data) {
                CACHE_NHAN_VIEN_DATA = res.data; // Lưu cache danh sách
                renderCustomSelectTruongBoPhan(defaultVal);
            } else {
                console.warn("Lỗi GetListNhanVien: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi GetListNhanVien:", xhr.responseText);
        }
    });
}

// Dựng giao diện Custom Select Trưởng bộ phận (#chonTruongBoPhan)
function renderCustomSelectTruongBoPhan(defaultVal) {
    var containerId = 'chonTruongBoPhan';
    var $wrap = $('#' + containerId);
    if (!$wrap.length) return;

    var valCol = $wrap.data('value') || "NhanVienID";
    var memCol = $wrap.data('member') || "TenHienThi";

    // 1. Render lại danh sách HTML và gán giá trị mặc định
    if (typeof window.renderSelect === 'function') {
        window.renderSelect(CACHE_NHAN_VIEN_DATA, containerId, valCol, memCol, defaultVal);
    }

    // 2. Kích hoạt lại toàn bộ sự kiện Mouse/Click cho các thẻ <li>
    if (typeof window.resetClickSelect === 'function') {
        window.resetClickSelect(containerId);
    }

    // 3. Bind lại sự kiện Mở Dropdown cho ô Input vừa được sinh ra
    bindEventForInput(containerId);
}

// Hàm bổ trợ bind lại sự kiện Click/Focus cho ô Input của Custom Select
function bindEventForInput(_id) {
    var $wrap = $('#' + _id);
    var $input = $wrap.find('input[type="text"]');
    var $ul = $('#sl' + _id);
    var $list = $ul.find('li');

    // Chống trùng lặp event bằng .off()
    $input.off('focus click').on('focus click', function (e) {
        e.stopPropagation();
        var curVal = $(this).val() || "";

        // Ẩn các dropdown khác đang mở trên màn hình
        $('.clSelect').not('#' + _id).find('ul').removeClass('show').addClass('hide');

        // Hiển thị danh sách của dropdown hiện tại
        $list.removeClass('hide');
        $ul.removeClass('hide').addClass('show');

        if (curVal === "-- Chọn --") {
            $(this).val('');
        }
    });

    // Bắt sự kiện click ngoài màn hình để đóng dropdown
    $(document).off('click.' + _id).on('click.' + _id, function (e) {
        if (!$(e.target).closest('#' + _id).length) {
            $ul.removeClass('show').addClass('hide');
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
            html += '<td class="text-center">' + (item.NgayTaoText || '') + '</td>';
            html += '<td class="text-center">';
            html += '  <button type="button" class="btn btn-sm btn-outline-primary me-1" onclick="openModalEdit('
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
    CURRENT_EDIT_ID = 0; // Đặt ID về 0 khi thêm mới
    $('#modalTitle').text('Thêm Mới Bộ Phận');

    // Tự động chọn phòng ban đang lọc ngoài màn hình (nếu có)
    var currentFilterPb = $('#ddlFilterPhongBan').val();
    if (currentFilterPb && currentFilterPb !== "0") {
        $('#ddlModalPhongBan').val(currentFilterPb);
    }

    if ($.fn.select2) {
        $('#ddlModalPhongBan').trigger('change.select2');
    }

    // Reset Custom Select Trưởng bộ phận về mặc định
    renderCustomSelectTruongBoPhan(-1);

    $('#modalBoPhan').modal('show');
}

function openModalEdit(boPhanId, phongBanId, maBoPhan, tenBoPhan, truongBoPhanId, thuTu, trangThai) {
    clearForm();
    CURRENT_EDIT_ID = boPhanId; // Gán ID bộ phận cần sửa
    $('#modalTitle').text('Cập Nhật Bộ Phận');

    $('#ddlModalPhongBan').val(phongBanId);
    $('#txtMaBoPhan').val(maBoPhan);
    $('#txtTenBoPhan').val(tenBoPhan);
    $('#txtThuTu').val(thuTu);
    $('#chkTrangThai').prop('checked', trangThai === 1);

    if ($.fn.select2) {
        $('#ddlModalPhongBan').trigger('change.select2');
    }

    // Chọn đúng Nhân viên Trưởng bộ phận cho Custom Select
    var selId = truongBoPhanId ? truongBoPhanId : -1;
    renderCustomSelectTruongBoPhan(selId);

    $('#modalBoPhan').modal('show');
}

function clearForm() {
    $('#txtMaBoPhan').val('');
    $('#txtTenBoPhan').val('');
    $('#ddlModalPhongBan').val('');
    $('#txtThuTu').val(0);
    $('#chkTrangThai').prop('checked', true);

    if (typeof window.clearSelect === 'function') {
        window.clearSelect('chonTruongBoPhan');
    }

    if ($.fn.select2) {
        $('#ddlModalPhongBan').trigger('change.select2');
    }
}

// ==========================================
// LƯU DỮ LIỆU
// ==========================================

// Hàm saveData() khớp chính xác với onclick="saveData()" trên HTML
function saveData() {
    saveBoPhan(CURRENT_EDIT_ID);
}

function saveBoPhan(boPhanId) {
    var phongBanId = parseInt($('#ddlModalPhongBan').val()) || 0;
    var maBoPhan = $('#txtMaBoPhan').val().trim();
    var tenBoPhan = $('#txtTenBoPhan').val().trim();

    // Lấy đúng ID Nhân viên từ thuộc tính data-selected của Custom Select #chonTruongBoPhan
    var $inputSelected = $('#chonTruongBoPhan input[type="text"]');
    var rawSelected = $inputSelected.attr('data-selected');
    var truongBoPhanId = (rawSelected && rawSelected !== "-1" && rawSelected !== "") ? parseInt(rawSelected) : null;

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