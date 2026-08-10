<%@ Page Title="Quản lý Tài khoản" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="tai-khoan.aspx.cs" Inherits="VTT.DanhMuc.tai_khoan" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC TÀI KHOẢN</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Tài khoản
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Username hoặc Tên nhân viên..."
                        onkeyup="if(event.keyCode===13) loadData();" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchTrangThai" class="form-control" onchange="loadData()">
                        <option value="">-- Tất cả trạng thái --</option>
                        <option value="1">Đang hoạt động</option>
                        <option value="0">Ngừng hoạt động</option>
                    </select>
                </div>
                <button type="button" class="btn btn-info" onclick="loadData()">
                    <i class="fa fa-search"></i> Tìm kiếm
                </button>
            </div>

            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 50px;">STT</th>
                            <th style="width: 150px;">Username</th>
                            <th>Nhân viên</th>
                            <th>Vai trò được gán</th>
                            <th style="width: 140px;">Đăng nhập cuối</th>
                            <th style="width: 100px;">Khóa</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 140px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyTaiKhoan"></tbody>
                </table>
            </div>
        </div>

        <div id="modalTaiKhoan" class="modal-backdrop" style="display: none;">
            <div class="modal-box" style="width: 650px;">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Tài khoản</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddTaiKhoanID" value="0" />

                    <div class="form-group">
                        <label>Nhân viên <span class="text-danger">*</span></label>
                        <select id="ddlNhanVien" class="form-control">
                            <option value="">-- Chọn nhân viên --</option>
                        </select>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Username <span class="text-danger">*</span></label>
                            <input type="text" id="txtUsername" class="form-control" placeholder="VD: nguyenvana" />
                        </div>
                        <div class="form-group col-6">
                            <label id="lblPasswordLabel">Mật khẩu <span class="text-danger">*</span></label>
                            <input type="password" id="txtPassword" class="form-control" placeholder="Nhập mật khẩu..."
                                autocomplete="new-password" />
                        </div>
                    </div>
                    <div class="form-group" id="passwordHint" style="display:none;">
                        <small class="text-muted">Để trống nếu không muốn đổi mật khẩu.</small>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Trạng thái</label>
                            <select id="ddlTrangThai" class="form-control">
                                <option value="1">Đang hoạt động</option>
                                <option value="0">Ngừng hoạt động</option>
                            </select>
                        </div>
                        <div class="form-group col-6">
                            <label>Khóa tài khoản</label>
                            <select id="ddlIsLocked" class="form-control">
                                <option value="0">Không khóa</option>
                                <option value="1">Khóa</option>
                            </select>
                        </div>
                    </div>

                    <!-- <div class="form-group">
                        <label>Vai trò được gán (Phân quyền)</label>
                        <div id="vaiTroCheckboxList"
                            style="border:1px solid #cbd5e1; border-radius:4px; padding:10px; max-height:180px; overflow-y:auto;">
                             checkbox list render bằng JS 
                        </div>
                    </div> -->
                    <div class="form-group">
                        <label>Vai trò (Phân quyền) <span class="text-danger">*</span></label>
                        <select id="ddlVaiTro" class="form-control">
                            <option value="">-- Chọn vai trò --</option>
                        </select>
                        <small class="text-muted">Mỗi tài khoản chỉ có 1 Vai trò. Vai trò Admin/IT được toàn quyền mọi
                            phòng ban; các vai trò khác chỉ CRUD trong phòng ban của nhân viên, xem-only ở phòng ban
                            khác.</small>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/tai-khoan.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>