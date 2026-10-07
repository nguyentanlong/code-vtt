(function () {
    var trangChucNangList = [];
    var currentPermissions = {}; // QuyenID -> { GiaTri, DataScope } (chỉ dùng khi chọn 1 tài khoản)
    // var originalKeys = {};       // Set các QuyenID đang ALLOW lúc load (để tính diff khi Lưu)
    // var scopeOptions = ["CONGTY", "CHINHANH", "PHONGBAN"];
    var scopeOptions = ["CONGTY", "PHONGBAN", "TU_TAO"];

    function callMethod(method, data, onSuccess) {
        $.ajax({
            type: "POST",
            url: "quyen.aspx/" + method,
            data: JSON.stringify(data || {}),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                var body = res.d || res;
                if (!body.success) {
                    // TODO: thay bằng hàm thông báo dùng chung (vd showAlert) nếu tên khác
                    // alert(body.message || "Có lỗi xảy ra!");
                    showAlertDialog(body.message || "Có lỗi xảy ra!", "error")
                    return;
                }
                onSuccess(body.data);
            },
            error: function (xhr) {
                // alert("Lỗi hệ thống: " + xhr.responseText);
                showAlertDialog("Lỗi hệ thống: " + xhr.responseText, "error")
            }
        });
    }

    function loadCongTy() {
        callMethod("GetCongTyOptions", {}, function (data) {
            var $ddl = $("#ddlCongTy").empty().append('<option value="">-- Chọn Công ty --</option>');
            data.forEach(function (item) {
                $ddl.append('<option value="' + item.CongTyID + '">' + item.TenCongTy + '</option>');
            });
        });
    }
    function loadVaiTro() {
        callMethod("GetVaiTroOptions", {}, function (data) {
            var $ddl = $("#ddlVaiTro").empty().append('<option value="">-- Không gán Vai trò --</option>');
            data.forEach(function (item) {
                $ddl.append('<option value="' + item.NhomQuyenID + '">' + item.TenNhomQuyen + '</option>');
            });
        });
    }

    $(document).on("change", "#ddlThoiHan", function () {
        $("#txtSoGioTuyChinh").toggle(this.value === "custom");
    });

    function loadPhongBan(congTyId) {
        var $ddl = $("#ddlPhongBan");
        var $ddlTK = $("#ddlTaiKhoan");
        $ddl.empty().prop("disabled", true);
        $ddlTK.empty().prop("disabled", true);
        renderGrid([]);

        if (!congTyId) {
            $ddl.append('<option value="">-- Chọn Công ty trước --</option>');
            return;
        }
        callMethod("GetPhongBanByCongTy", { congTyId: congTyId }, function (data) {
            $ddl.append('<option value="">-- Chọn Phòng ban --</option>');
            data.forEach(function (item) {
                $ddl.append('<option value="' + item.PhongBanID + '">' + item.TenPhongBan + '</option>');
            });
            $ddl.prop("disabled", false);
        });
    }

    function loadTaiKhoan(phongBanId) {
        var $ddlTK = $("#ddlTaiKhoan");
        $ddlTK.empty().prop("disabled", true);
        renderGrid([]);

        if (!phongBanId) return;
        callMethod("GetTaiKhoanByPhongBan", { phongBanId: phongBanId }, function (data) {
            data.forEach(function (item) {
                $ddlTK.append('<option value="' + item.TaiKhoanID + '">' + item.HoTen + ' (' + item.TenDangNhap + ')</option>');
            });
            $ddlTK.prop("disabled", false);
        });
    }

    function getSelectedTaiKhoanIds() {
        return $("#ddlTaiKhoan").val() ? $("#ddlTaiKhoan").val().map(Number) : [];
    }

    function onTaiKhoanChanged() {
        var ids = getSelectedTaiKhoanIds();
        currentPermissions = {};
        // originalKeys = {};

        if (ids.length === 0) {
            $("#grantModeNote").hide();
            renderGrid([]);
            return;
        }

        if (trangChucNangList.length === 0) {
            callMethod("GetTrangChucNangList", {}, function (data) {
                trangChucNangList = data;
                afterTrangLoaded(ids);
            });
        } else {
            afterTrangLoaded(ids);
        }
    }

    /*function afterTrangLoaded(ids) {
        if (ids.length === 1) {
            $("#grantModeNote").text("Đang sửa quyền hiện có của 1 tài khoản — bỏ tick = thu hồi quyền.").show();
            callMethod("GetCurrentByTaiKhoan", { taiKhoanId: ids[0] }, function (data) {
                data.forEach(function (p) {
                    currentPermissions[p.QuyenID] = { GiaTri: p.GiaTri, DataScope: p.DataScope };
                    if (p.GiaTri === "ALLOW") originalKeys[p.QuyenID] = true;
                });
                renderGrid(trangChucNangList);
            });
        } else {
            $("#grantModeNote").text("Đang chọn " + ids.length + " tài khoản — Lưu sẽ CẤP THÊM các quyền đã tick cho tất cả (không đụng quyền khác).").show();
            renderGrid(trangChucNangList);
        }
    }*/

    function afterTrangLoaded(ids) {
        if (ids.length === 1) {
            $("#grantModeNote").text("Đang sửa quyền hiện có — ô nền vàng là mặc định theo Vai trò; bỏ tick ô đó sẽ CHẶN HẲN (DENY), không chỉ xóa.").show();
            callMethod("GetEffectiveByTaiKhoan", { taiKhoanId: ids[0] }, function (data) {
                data.forEach(function (p) {
                    currentPermissions[p.QuyenID] = { GiaTri: p.GiaTri, DataScope: p.DataScope, TuVaiTro: p.TuVaiTro };
                });
                renderGrid(trangChucNangList);
            });
        } else {
            $("#grantModeNote").text("Đang chọn " + ids.length + " tài khoản — Lưu sẽ CẤP THÊM các quyền đã tick cho tất cả (không đụng quyền khác).").show();
            renderGrid(trangChucNangList);
        }
    }

    function scopeSelectHtml(quyenId, chucNangCode, selectedScope) {
        var html = '<select class="form-control scope-select" data-quyen="' + quyenId + '" disabled>';
        scopeOptions.forEach(function (s) {
            html += '<option value="' + s + '"' + (s === selectedScope ? " selected" : "") + '>' + s + '</option>';
        });
        html += '</select>';
        return html;
    }

    /*function renderGrid(list) {
        var $tbody = $("#tbodyQuyenGrid").empty();
        if (!list || list.length === 0) {
            $tbody.append('<tr><td colspan="6">Vui lòng chọn Công ty / Phòng ban / Tài khoản ở trên.</td></tr>');
            return;
        }

        var byTrang = {};
        list.forEach(function (row) {
            if (!byTrang[row.TrangID]) byTrang[row.TrangID] = { TenTrang: row.TenTrang, items: {} };
            byTrang[row.TrangID].items[row.MaChucNang] = row;
        });

        var thuTuChucNang = ["XEM", "THEM", "SUA", "XOA"];

        Object.keys(byTrang).forEach(function (trangId) {
            var trang = byTrang[trangId];
            var $tr = $('<tr></tr>');
            $tr.append('<td><input type="checkbox" class="chk-trang" data-trang="' + trangId + '" /></td>');
            $tr.append('<td>' + trang.TenTrang + '</td>');

            thuTuChucNang.forEach(function (ma) {
                var item = trang.items[ma];
                if (!item || !item.QuyenID) {
                    $tr.append('<td class="text-muted">Chưa cấu hình</td>');
                    return;
                }
                var existed = currentPermissions[item.QuyenID];
                var checked = !!existed;
                var scope = existed ? existed.DataScope : "CONGTY";

                var $td = $('<td></td>');
                $td.append(
                    '<input type="checkbox" class="chk-quyen" data-quyen="' + item.QuyenID + '" data-trang="' + trangId + '"' + (checked ? " checked" : "") + ' /> '
                );
                $td.append(scopeSelectHtml(item.QuyenID, ma, scope));
                $tr.append($td);
            });

            $tbody.append($tr);
        });

        // Bật/tắt dropdown scope theo checkbox tương ứng
        $tbody.find(".chk-quyen").on("change", function () {
            var $scope = $(this).closest("td").find(".scope-select");
            $scope.prop("disabled", !this.checked);
        }).trigger("change");

        // Checkbox tổng theo Trang: tick/bỏ tick toàn bộ CRUD của trang đó
        $tbody.find(".chk-trang").on("change", function () {
            var trangId = $(this).data("trang");
            var checked = this.checked;
            $tbody.find('.chk-quyen[data-trang="' + trangId + '"]').prop("checked", checked).trigger("change");
        });
    }*/
    function renderGrid(list) {
        var $tbody = $("#tbodyQuyenGrid").empty();
        if (!list || list.length === 0) {
            $tbody.append('<tr><td colspan="6">Vui lòng chọn Công ty / Phòng ban / Tài khoản ở trên.</td></tr>');
            return;
        }

        var byTrang = {};
        list.forEach(function (row) {
            if (!byTrang[row.TrangID]) byTrang[row.TrangID] = { TenTrang: row.TenTrang, items: {} };
            byTrang[row.TrangID].items[row.MaChucNang] = row;
        });

        var thuTuChucNang = ["XEM", "THEM", "SUA", "XOA"];

        Object.keys(byTrang).forEach(function (trangId) {
            var trang = byTrang[trangId];
            var $tr = $('<tr></tr>');
            $tr.append('<td><input type="checkbox" class="chk-trang" data-trang="' + trangId + '" /></td>');
            $tr.append('<td>' + trang.TenTrang + '</td>');

            thuTuChucNang.forEach(function (ma) {
                var item = trang.items[ma];
                if (!item || !item.QuyenID) {
                    $tr.append('<td class="text-muted">Chưa cấu hình</td>');
                    return;
                }
                var existed = currentPermissions[item.QuyenID];
                var checked = !!existed && existed.GiaTri === "ALLOW";
                var scope = existed ? existed.DataScope : "CONGTY";
                var tuVaiTro = existed && existed.TuVaiTro;

                var $td = $('<td' + (tuVaiTro ? ' style="background:#fff8e1;" title="Mặc định theo Vai trò"' : '') + '></td>');
                $td.append(
                    '<input type="checkbox" class="chk-quyen" data-quyen="' + item.QuyenID + '" data-trang="' + trangId + '"' + (checked ? " checked" : "") + ' /> '
                );
                $td.append(scopeSelectHtml(item.QuyenID, ma, scope));
                $tr.append($td);
            });

            $tbody.append($tr);
        });

        $tbody.find(".chk-quyen").on("change", function () {
            var $scope = $(this).closest("td").find(".scope-select");
            $scope.prop("disabled", !this.checked);
        }).trigger("change");

        $tbody.find(".chk-trang").on("change", function () {
            var trangId = $(this).data("trang");
            var checked = this.checked;
            $tbody.find('.chk-quyen[data-trang="' + trangId + '"]').prop("checked", checked).trigger("change");
        });
    }

    /*function collectChanges() {
        var grants = [];
        var revokes = [];

        $("#tbodyQuyenGrid .chk-quyen").each(function () {
            var quyenId = parseInt($(this).data("quyen"), 10);
            var checked = this.checked;
            var scope = $(this).closest("td").find(".scope-select").val();
            var wasAllowed = !!originalKeys[quyenId];

            if (checked) {
                grants.push({ QuyenID: quyenId, DataScope: scope });
            } else if (wasAllowed) {
                revokes.push(quyenId);
            }
        });

        return { grants: grants, revokes: revokes };
    }

    function saveData() {
        var ids = getSelectedTaiKhoanIds();
        if (ids.length === 0) {
            // alert("Vui lòng chọn ít nhất 1 Tài khoản!");
            showAlertDialog("Vui lòng chọn ít nhất 1 Tài Khoản!", "info")
            return;
        }

        /*var changes = collectChanges();
        if (changes.grants.length === 0 && changes.revokes.length === 0) {
            // alert("Không có thay đổi nào để lưu.");
            showAlertDialog("Không có thay đổi nào để lưu!!", "info")
            return;
        }*
        var changes = collectChanges();
        var vaiTroDaChon = $("#ddlVaiTro").val();

        if (changes.grants.length === 0 && changes.revokes.length === 0 && !vaiTroDaChon) {
            showAlertDialog("Không có thay đổi nào để lưu!!", "info")
            return;
        }

        $.ajax({
            type: "POST",
            url: "quyen.aspx/SaveData",
            /*data: JSON.stringify({
                taiKhoanIdsJson: JSON.stringify(ids),
                grantsJson: JSON.stringify(changes.grants),
                revokeQuyenIdsJson: JSON.stringify(changes.revokes)
            }),*
            data: JSON.stringify({
                taiKhoanIdsJson: JSON.stringify(ids),
                grantsJson: JSON.stringify(changes.grants),
                revokeQuyenIdsJson: JSON.stringify(changes.revokes),
                vaiTroNhomQuyenId: $("#ddlVaiTro").val() ? parseInt($("#ddlVaiTro").val()) : null,
                phongBanIdChoVaiTro: $("#ddlPhongBan").val() ? parseInt($("#ddlPhongBan").val()) : null,
                thoiHanLoai: $("#ddlThoiHan").val(),
                soGioTuyChinh: $("#txtSoGioTuyChinh").val() ? parseInt($("#txtSoGioTuyChinh").val()) : null
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                var body = res.d || res;
                // alert(body.message);
                showAlertDialog(body.message, "info")
                if (body.success) onTaiKhoanChanged();
            },
            error: function (xhr) {
                // alert("Lỗi hệ thống: " + xhr.responseText);
                showAlertDialog("Lỗi hệ thống: " + xhr.responseText, "error")
            }
        });
    }*/
    function collectChanges() {
        var grants = [];
        var denies = [];
        var revokes = [];

        $("#tbodyQuyenGrid .chk-quyen").each(function () {
            var quyenId = parseInt($(this).data("quyen"), 10);
            var checked = this.checked;
            var scope = $(this).closest("td").find(".scope-select").val();
            var original = currentPermissions[quyenId];
            var wasAllowed = original && original.GiaTri === "ALLOW";

            if (checked) {
                if (!wasAllowed || original.DataScope !== scope) {
                    grants.push({ QuyenID: quyenId, DataScope: scope });
                }
            } else if (wasAllowed) {
                if (original.TuVaiTro) {
                    denies.push({ QuyenID: quyenId, DataScope: original.DataScope });
                } else {
                    revokes.push(quyenId);
                }
            }
        });

        return { grants: grants, denies: denies, revokes: revokes };
    }

    function saveData() {
        var ids = getSelectedTaiKhoanIds();
        if (ids.length === 0) {
            alert("Vui lòng chọn ít nhất 1 Tài khoản!");
            return;
        }

        var changes = collectChanges();
        var vaiTroDaChon = $("#ddlVaiTro").val();

        if (changes.grants.length === 0 && changes.denies.length === 0 && changes.revokes.length === 0 && !vaiTroDaChon) {
            alert("Không có thay đổi nào để lưu.");
            return;
        }

        $.ajax({
            type: "POST",
            url: "quyen.aspx/SaveData",
            data: JSON.stringify({
                taiKhoanIdsJson: JSON.stringify(ids),
                grantsJson: JSON.stringify(changes.grants),
                denyGrantsJson: JSON.stringify(changes.denies),
                revokeQuyenIdsJson: JSON.stringify(changes.revokes),
                vaiTroNhomQuyenId: $("#ddlVaiTro").val() ? parseInt($("#ddlVaiTro").val()) : null,
                phongBanIdChoVaiTro: $("#ddlPhongBan").val() ? parseInt($("#ddlPhongBan").val()) : null,
                thoiHanLoai: $("#ddlThoiHan").val(),
                soGioTuyChinh: $("#txtSoGioTuyChinh").val() ? parseInt($("#txtSoGioTuyChinh").val()) : null
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                var body = res.d || res;
                alert(body.message);
                if (body.success) onTaiKhoanChanged();
            },
            error: function (xhr) {
                alert("Lỗi hệ thống: " + xhr.responseText);
            }
        });
    }

    $(document).ready(function () {
        loadCongTy();
        loadVaiTro();

        $("#ddlCongTy").on("change", function () { loadPhongBan($(this).val()); });
        $("#ddlPhongBan").on("change", function () { loadTaiKhoan($(this).val()); });
        $("#ddlTaiKhoan").on("change", onTaiKhoanChanged);
        $("#btnSaveQuyen").on("click", saveData);

    });
})();

function escapeHtml(text) {
    if (!text) return "";
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}