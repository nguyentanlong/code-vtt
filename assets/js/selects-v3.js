jQuery(document).ready(function ($) {
    "use strict";

    // =========================================================================
    // 1. HELPERS: ĐÓNG / MỞ DROPDOWN
    // =========================================================================

    // Ẩn tất cả dropdown đang mở trên màn hình
    function hideAllDropdowns() {
        $('.clSelect.multi').each(function () {
            var $ul = $(this).find('ul');
            if ($ul.hasClass('show')) {
                confirmMultiSelection($(this).attr('id'));
            }
        });
        $('.clSelect').find('ul').removeClass('show').addClass('hide');
    }

    // Ẩn tất cả dropdown ngoại trừ dropdown có ID được chỉ định
    function hideDropdownsExcept(exceptId) {
        if (!exceptId) {
            hideAllDropdowns();
            return;
        }
        $('.clSelect.multi').not('#' + exceptId).each(function () {
            var $ul = $(this).find('ul');
            if ($ul.hasClass('show')) {
                confirmMultiSelection($(this).attr('id'));
            }
        });
        $('.clSelect').not('#' + exceptId).find('ul').removeClass('show').addClass('hide');
    }

    // Hiển thị dropdown của một ID cụ thể, hỗ trợ bộ lọc tìm kiếm (filter)
    function showDropdownForId(id, filterText) {
        var $ul = $('#sl' + id);
        if (!$ul.length) return;

        $ul.find('li').removeClass('hide');

        if (typeof filterText === 'string' && filterText.trim() !== '') {
            var v = filterText.toLowerCase();
            $ul.find('li').filter(function () {
                var itemText = $(this).text();
                if (itemText === null || itemText === undefined) return true;
                return itemText.toLowerCase().indexOf(v) < 0;
            }).addClass('hide');
        }

        $ul.removeClass('hide').addClass('show');
    }

    // =========================================================================
    // 2. HÀM XỬ LÝ DỮ LIỆU MULTI SELECT (CHỐT VÀ HOÀN TÁC)
    // =========================================================================

    // Hàm chốt dữ liệu đã tích chọn và đẩy chuỗi văn bản lên ô input hiển thị
    function confirmMultiSelection(_id) {
        var $wrap = $('#' + _id);
        var $ul = $('#sl' + _id);
        var $input = $wrap.find('input[type="text"]');

        var selectedIds = [];
        var selectedTexts = [];

        // Thu thập toàn bộ các dòng đang có class .selected thực tế
        $ul.find('li.selected').each(function () {
            var dataId = $(this).attr('data-id');
            if (dataId !== undefined && dataId !== null) {
                selectedIds.push(dataId);
                selectedTexts.push($(this).text().trim());
            }
        });

        // Đẩy chuỗi kết quả ngăn cách bằng dấu phẩy lên ô hiển thị công khai
        if (selectedIds.length > 0) {
            $input.val(selectedTexts.join(', ')).attr('data-selected', selectedIds.join(','));
            $wrap.attr('data-default', selectedIds.join(','));
        } else {
            $input.val("-- Chọn --").attr('data-selected', -1);
            $wrap.attr('data-default', -1);
        }
    }

    // Hàm khôi phục lại text hiển thị dựa trên dữ liệu đã tích từ trước (khi hủy tìm kiếm hoặc bấm nút thoát)
    function restoreMultiText(_id) {
        var $wrap = $('#' + _id);
        var $ul = $('#sl' + _id);
        var $input = $wrap.find('input[type="text"]');

        var selectedTexts = [];
        $ul.find('li.selected').each(function () {
            selectedTexts.push($(this).text().trim());
        });

        if (selectedTexts.length > 0) {
            $input.val(selectedTexts.join(', '));
        } else {
            $input.val("-- Chọn --");
        }
    }

    // =========================================================================
    // 3. HÀM CLEAR/RESET TRẠNG THÁI CHỌN (RESET Ô CHỌN VỀ TRỐNG TRƠN)
    // =========================================================================
    window.clearSelect = function (_id) {
        var $wrap = $('#' + _id);
        if (!$wrap.length) return;

        var $input = $wrap.find('input[type="text"]');
        var $ul = $('#sl' + _id);

        $input.val("-- Chọn --").attr('data-selected', -1);
        $wrap.attr('data-default', -1);

        if ($ul.length) {
            $ul.find('li').removeClass('selected').removeClass('hover');
            $ul.find('.chk-item').prop('checked', false);
            $ul.find('li').removeClass('hide'); // Hiện lại toàn bộ danh sách ẩn
            $ul.find('li').first().addClass('hover');
        }
    };

    // =========================================================================
    // 4. HÀM BIND SỰ KIỆN CHUỘT (CLICK & MOUSEENTER) CHO ITEM TỪNG DÒNG
    // =========================================================================
    window.resetClickSelect = function (_id) {
        var $wrap = $('#' + _id);
        var $ul = $('#sl' + _id);
        var $input = $wrap.find('input[type="text"]');
        var isMulti = $wrap.hasClass('multi');

        $ul.find('li').off('click').on('click', function (e) {
            e.stopPropagation(); // Ngăn chặn sự kiện click lan ra ngoài gây đóng tab đột ngột
            var $li = $(this);

            if (isMulti) {
                // [CHẾ ĐỘ MULTI]: Bật/Tắt class selected và checkbox đi kèm
                $li.toggleClass('selected');
                var $chk = $li.find('.chk-item');
                $chk.prop('checked', $li.hasClass('selected'));

                // Đồng bộ vệt sáng hover về dòng vừa click chuột
                $ul.find('li').removeClass('hover');
                $li.addClass('hover');

                $input.focus(); // Giữ tiêu điểm tại ô text để người dùng gõ tiếp từ khóa khác
            } else {
                // [CHẾ ĐỘ SINGLE]: Gán giá trị đơn, cập nhật class selected và đóng bảng chọn
                var txt = $li.text().trim();
                var idVal = $li.attr('data-id');

                $input.val(txt).attr('data-selected', idVal);
                $wrap.attr('data-default', idVal);

                $ul.find('li').removeClass('selected');
                $li.addClass('selected');

                hideAllDropdowns();

                // Đồng bộ dữ liệu đơn vị tính liên kết (data-linked)
                var _dvtid = $li.attr("data-dvt");
                var dl = $wrap.attr('data-linked');
                if (dl && dl !== "undefined") {
                    $('#sl' + dl + ' li').each(function () {
                        var targetId = $(this).attr('data-id');
                        if (targetId !== undefined && targetId !== null && _dvtid == targetId) {
                            $('#' + dl + ' input').val($(this).text()).attr('data-selected', _dvtid);
                            $('#sl' + dl).removeClass('show').addClass('hide');
                        }
                    });
                }
            }
        });

        // Rà chuột đến dòng nào thì dòng đó sáng hover trực quan, xóa hover cũ
        $ul.find('li').off('mouseenter').on('mouseenter', function () {
            $ul.find('li').removeClass('hover');
            $(this).addClass('hover');
        });

        // Chặn lỗi click đúp hoặc lệch khi người dùng click trực tiếp vào ô vuông checkbox nhỏ
        $ul.find('.chk-item').off('click').on('click', function (e) {
            e.stopPropagation();
            $(this).closest('li').trigger('click');
        });
    };

    // =========================================================================
    // 5. HÀM KHỞI TẠO DOM VÀ XỬ LÝ BÀN PHÍM (RENDER SELECT)
    // =========================================================================
    window.renderSelect = function (strJson, _id, _value, _member, _df, _dl, _addCol) {
        if (strJson === null || strJson === undefined || strJson === "" || strJson === "null") return;

        var $wrap = $('#' + _id);
        if (!$wrap.length || !$wrap.hasClass('clSelect')) return;

        var isMulti = $wrap.hasClass('multi');

        if ($('#sl' + _id).length) {
            $wrap.empty();
        }

        var dataArray = [];
        if (typeof strJson === 'string') {
            try { dataArray = JSON.parse(strJson); } catch (e) { return; }
        } else if (Array.isArray(strJson)) {
            dataArray = strJson;
        }

        // Tạo cấu trúc mã HTML cho Dropdown
        var html = '<input type="text" class="form-control" autocomplete="off"> <ul class="hide" id="sl' + _id + '">';
        dataArray.forEach(function (data) {
            if (!data) return;
            var addCols = "";
            if (_addCol != undefined && Array.isArray(_addCol)) {
                addCols = "_";
                _addCol.forEach(function (col) {
                    var colVal = data[col] !== undefined && data[col] !== null ? data[col] : "";
                    addCols += colVal + "_";
                });
                addCols = addCols.substring(0, addCols.length - 1);
            }

            var valItem = data[_value] !== undefined && data[_value] !== null ? data[_value] : "";
            var dvtItem = data["DVTID"] !== undefined && data["DVTID"] !== null ? data["DVTID"] : "";
            var memberItem = data[_member] !== undefined && data[_member] !== null ? data[_member] : "";

            if (isMulti) {
                html += '<li data-id="' + valItem + addCols + '" data-dvt="' + dvtItem + '"><input type="checkbox" class="chk-item" style="margin-right:8px; pointer-events:none;">' + memberItem + '</li>';
            } else {
                html += '<li data-id="' + valItem + addCols + '" data-dvt="' + dvtItem + '">' + memberItem + '</li>';
            }
        });
        html += '</ul>';

        $wrap.append(html);

        if (_dl !== undefined) {
            $wrap.attr('data-linked', _dl);
        }

        setValueSelect(_id, _df);

        var $ul = $('#sl' + _id);
        var $list = $ul.find('li');
        var $input = $wrap.find('input[type="text"]');

        // ---------------------------------------------------------------------
        // XỬ LÝ SỰ KIỆN PHÍM TẮT BÀN PHÍM
        // ---------------------------------------------------------------------
        $input.off('keydown').on('keydown', function (e) {
            var $this = $(this);
            // 🔥 QUAN TRỌNG: Chỉ tính toán trên các dòng đang hiển thị vượt qua bộ lọc Search (:not(.hide))
            var $visible = $ul.find('li:not(.hide)');
            var hoverIndex = $visible.index($visible.filter('li.hover').first());

            switch (e.keyCode) {
                case 38: // MŨI TÊN LÊN (Up Arrow)
                    if ($visible.length === 0) break;
                    if (hoverIndex <= 0) hoverIndex = $visible.length - 1;
                    else hoverIndex = hoverIndex - 1;

                    $visible.removeClass('hover');
                    if ($visible.eq(hoverIndex).length) {
                        $visible.eq(hoverIndex).addClass('hover')[0].scrollIntoView({ block: 'nearest' });
                    }
                    e.preventDefault();
                    break;

                case 40: // MŨI TÊN XUỐNG (Down Arrow)
                    if ($visible.length === 0) break;
                    if (hoverIndex === -1 || hoverIndex >= $visible.length - 1) hoverIndex = 0;
                    else hoverIndex = hoverIndex + 1;

                    $visible.removeClass('hover');
                    if ($visible.eq(hoverIndex).length) {
                        $visible.eq(hoverIndex).addClass('hover')[0].scrollIntoView({ block: 'nearest' });
                    }
                    e.preventDefault();
                    break;

                case 32: // PHÍM SPACE (Khoảng trắng): Đóng/Mở tích chọn checkbox dòng đang hover
                    if (isMulti && hoverIndex !== -1) {
                        var $currentLi = $visible.eq(hoverIndex);
                        $currentLi.toggleClass('selected');
                        $currentLi.find('.chk-item').prop('checked', $currentLi.hasClass('selected'));
                        e.preventDefault(); // Ngăn trình duyệt cuộn dọc trang Web do tính năng gốc của phím Space
                    }
                    break;

                case 13: // PHÍM ENTER: Lưu kết quả chốt dữ liệu
                    if (isMulti) {
                        confirmMultiSelection(_id);
                        hideAllDropdowns();
                    } else {
                        var $sel = $visible.filter('.hover');
                        if ($sel.length === 0 && $visible.length > 0) $sel = $visible.eq(0);
                        if ($sel.length) {
                            $sel.trigger('click');
                        }
                    }
                    e.preventDefault();
                    break;

                case 9:  // Phím Tab
                case 27: // Phím Esc
                    if (isMulti) {
                        restoreMultiText(_id); // Trả lại chuỗi văn bản cũ nếu người dùng hủy không tìm nữa
                    }
                    hideAllDropdowns();
                    break;

                default:
                    // Cho phép gõ ký tự bình thường vào ô nhập để search bộ lọc
                    break;
            }
        });

        // ---------------------------------------------------------------------
        // SỰ KIỆN GÕ Ô INPUT: BỘ LỌC TÌM KIẾM CHO CẢ CHẾ ĐỘ SINGLE VÀ MULTI
        // ---------------------------------------------------------------------
        $input.off('input').on('input', function () {
            var v = $(this).val() || "";

            if (v.trim() === "") {
                $list.removeClass('hide');
                showDropdownForId(_id);
                // Đưa vệt sáng hover về dòng đầu tiên của bảng danh sách đầy đủ
                $ul.find('li').removeClass('hover');
                $ul.find('li:not(.hide)').first().addClass('hover');
                return;
            }

            // Tiến hành ẩn các dòng không chứa từ khóa tìm kiếm
            $list.removeClass('hide').filter(function () {
                var txt = $(this).text();
                return txt ? txt.toLowerCase().indexOf(v.toLowerCase()) < 0 : true;
            }).addClass('hide');

            showDropdownForId(_id, v);

            // 🔥 BẮT BUỘC: Đóng đinh vệt sáng xanh vào dòng đầu tiên hiển thị sau bộ lọc, ngăn lỗi phím mũi tên
            $ul.find('li').removeClass('hover');
            $ul.find('li:not(.hide)').first().addClass('hover');
        });

        // ---------------------------------------------------------------------
        // SỰ KIỆN FOCUS HOẶC CLICK VÀO Ô INPUT ĐỂ KHỞI ĐỘNG BẢNG
        // ---------------------------------------------------------------------
        $input.off('focus click').on('focus click', function (ev) {
            var curVal = $(this).val() || "";
            var curSelected = $(this).attr('data-selected') || "";

            hideDropdownsExcept(_id);

            if (isMulti) {
                // Đối với ô nhiều lựa chọn: Tạm thời xóa trống text để họ gõ từ khóa tìm kiếm mới dễ dàng
                $(this).val('');
                $list.removeClass('hide');
                showDropdownForId(_id);

                // Mặc định hover dòng đầu
                $ul.find('li').removeClass('hover');
                $ul.find('li:not(.hide)').first().addClass('hover');
            } else {
                // Đối với ô đơn lựa chọn
                if (curSelected == "-1" || curVal === "-- Chọn --" || _df == -1) {
                    $list.removeClass('hide');
                    showDropdownForId(_id);
                } else {
                    showDropdownForId(_id, curVal);
                }
                if (curVal === "-- Chọn --") $(this).val('');
            }
        });

        window.resetClickSelect(_id);
    };

    // =========================================================================
    // 6. SET GIÁ TRỊ MẶC ĐỊNH LÊN Ô CHỌN KHI LOAD TRANG BAN ĐẦU
    // =========================================================================
    function setValueSelect(_id, _value) {
        var $wrap = $('#' + _id);
        var $input = $wrap.find('input[type="text"]');
        var isMulti = $wrap.hasClass('multi');

        if (_value === undefined || _value === null || _value == -1 || _value === "null") {
            $input.val("-- Chọn --").attr('data-selected', -1);
            return;
        }

        var matched = false;
        var arrValues = _value.toString().split(',');
        var arrTexts = [];

        $wrap.find('li').each(function () {
            var rawId = $(this).data('id');
            if (rawId === undefined || rawId === null) return;

            var _v = rawId.toString();
            if (arrValues.indexOf(_v) > -1) {
                $(this).addClass('selected');
                if (isMulti) {
                    $(this).find('.chk-item').prop('checked', true);
                }
                arrTexts.push($(this).text().trim());
                matched = true;
            }
        });

        if (matched) {
            $input.val(arrTexts.join(', ')).attr('data-selected', arrValues.join(','));
        } else {
            $input.val("-- Chọn --").attr('data-selected', -1);
        }
    }

    // =========================================================================
    // 7. HÀM CẬP NHẬT SOURCE DATA MỚI CHO BẢNG QUA AJAX (UPDATE SELECT)
    // =========================================================================
    window.updateSelect = function (_id, strJson, _value, _member, _addCol) {
        var $ul = $('#sl' + _id);
        if (!$ul.length) return;

        if (strJson === null || strJson === undefined || strJson === "") {
            $ul.empty();
            return;
        }

        var isMulti = $('#' + _id).hasClass('multi');
        var dataArray = [];
        if (typeof strJson === 'string') {
            try { dataArray = JSON.parse(strJson); } catch (e) { return; }
        } else if (Array.isArray(strJson)) {
            dataArray = strJson;
        }

        var html = '';
        dataArray.forEach(function (data) {
            if (!data) return;
            var addCols = "";
            if (_addCol != undefined && Array.isArray(_addCol)) {
                addCols = "_";
                _addCol.forEach(function (col) {
                    var colVal = data[col] !== undefined && data[col] !== null ? data[col] : "";
                    addCols += colVal + "_";
                });
                addCols = addCols.substring(0, addCols.length - 1);
            }
            var valItem = data[_value] !== undefined && data[_value] !== null ? data[_value] : "";
            var dvtItem = data["DVTID"] !== undefined && data["DVTID"] !== null ? data["DVTID"] : "";
            var memberItem = data[_member] !== undefined && data[_member] !== null ? data[_member] : "";

            if (isMulti) {
                html += '<li data-id="' + valItem + addCols + '" data-dvt="' + dvtItem + '"><input type="checkbox" class="chk-item" style="margin-right:8px; pointer-events:none;">' + memberItem + '</li>';
            } else {
                html += '<li data-id="' + valItem + addCols + '" data-dvt="' + dvtItem + '">' + memberItem + '</li>';
            }
        });
        $ul.html(html);
        window.resetClickSelect(_id);
    };

    // =========================================================================
    // 8. VÒNG LẶP KHỞI TẠO ĐỐI TƯỢNG TỰ ĐỘNG BAN ĐẦU QUA CLASS '.clSelect'
    // =========================================================================
    $('.clSelect').not('.wait').each(function () {
        var _ar = $(this).data('array'),
            _value = $(this).data('value'),
            _member = $(this).data('member'),
            _id = $(this).attr('id'),
            _df = $(this).data('default'),
            _dl = $(this).data('link'),
            _addCol = $(this).data('mergecol');
        renderSelect(_ar, _id, _value, _member, _df, _dl, _addCol);
    });

    // =========================================================================
    // 9. SỰ KIỆN CLICK RA NGOÀI MÀN HÌNH ĐỂ TỰ ĐỘNG CHỐT DỮ LIỆU & ĐÓNG DROPDOWN
    // =========================================================================
    $(document).off('click.hideClSelects').on('click.hideClSelects', function (e) {
        if (!$(e.target).closest('.clSelect').length) {
            // Khôi phục lại text hiển thị cho toàn bộ các ô multi select đang mở dang dở trước khi đóng ẩn
            $('.clSelect.multi').each(function () {
                var $ul = $(this).find('ul');
                if ($ul.hasClass('show')) {
                    restoreMultiText($(this).attr('id'));
                }
            });
            hideAllDropdowns();
        }
    });

    // =========================================================================
    // 10. ENTER NHẢY DÒNG TRONG TABLE (ĐÃ FIX LIÊN KẾT ĐẾN NÚT #newXN)
    // =========================================================================
    $(document).on('keydown', '.c-Table__addNew input', function (e) {
        if (e.key === 'Enter') {
            if ($(this).closest('.clSelect').hasClass('multi')) {
                return; // Đang ở ô gõ tìm kiếm multi thì nhường Enter cho việc đóng chốt dropdown trước
            }
            e.preventDefault();
            var $lastInput = $(this).closest('.c-Table__addNew').find('input:last');
            if ($(this).is($lastInput)) {
                var $btnNew = $(this).closest('.c-Table__addNew').find('#newXN, .new');
                var _this = $(this).closest('.c-Table__addNew').find('.dropCheck, .inputCheck');

                if (_this.length > 0 && _this.find('input').val() !== "") {
                    $btnNew.trigger('click');
                    $(this).parents('.c-Table__addNew').find('.cfocus input').focus();
                }
            }
        }
    });
});