<%@ Page Title="Quản lý Tài khoản" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="tai-khoan.aspx.cs" Inherits="VTT.DanhMuc.tai_khoan" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">QUẢN LÝ TÀI KHOẢN</h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="openModal()">
                    <i class="fa fa-plus"></i> Tạo Tài khoản mới
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Tên đăng nhập hoặc Họ tên..."
                        onkeyup="if(event.keyCode===13) loadData(1);" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchChiNhanh" class="form-control" onchange="loadData(1)">
                        <option value="">-- Tất cả chi nhánh --</option>
                    </select>
                </div>
                <button type="button" class="btn btn-info" onclick="loadData(1)">
                    <i class="fa fa-search"></i> Tìm kiếm
                </button>
            </div>

            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 150px;">Tên đăng nhập</th>
                            <th>Họ tên</th>
                            <th style="width: 160px;">Phòng ban</th>
                            <th style="width: 150px;">Vai trò</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 120px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyTaiKhoan"></tbody>
                </table>
            </div>
            <div id="paginationTaiKhoan"></div>
        </div>

        <div id="modalTaiKhoan" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3>Tạo Tài khoản mới</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label>Chọn Nhân viên (chưa có Tài khoản) <span class="text-danger">*</span></label>
                        <select id="ddlNhanVien" class="form-control">
                            <option value="">-- Chọn nhân viên --</option>
                        </select>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Tên đăng nhập <span class="text-danger">*</span></label>
                            <input type="text" id="txtTenDangNhap" class="form-control" placeholder="VD: nguyenvana" />
                        </div>
                        <div class="form-group col-6">
                            <label>Mật khẩu <span class="text-danger">*</span></label>
                            <input type="password" id="txtMatKhau" class="form-control" placeholder="Tối thiểu 6 ký tự"
                                autocomplete="new-password" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Vai trò <span class="text-danger">*</span></label>
                        <select id="ddlVaiTro" class="form-control" onchange="onVaiTroChange()">
                            <option value="">-- Chọn vai trò --</option>
                        </select>
                        <small class="text-muted">Chỉ hiển thị các Vai trò thấp hơn cấp của bạn.</small>
                    </div>

                    <div class="form-group" id="groupPhongBanVaiTro" style="display:none;">
                        <label>Phòng ban áp dụng Vai trò <span class="text-danger">*</span></label>
                        <select id="ddlPhongBanVaiTro" class="form-control">
                            <option value="">-- Chọn phòng ban --</option>
                        </select>
                        <small class="text-muted">Vai trò này gắn với 1 Phòng ban cụ thể (Trưởng phòng/Phó phòng/Tổ
                            trưởng).</small>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Tạo Tài khoản</button>
                </div>
            </div>
        </div>

        <script src="../assets/js/ui-alert.js"></script>
        <script
            src="../assets/js/danh-muc/tai-khoan.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>