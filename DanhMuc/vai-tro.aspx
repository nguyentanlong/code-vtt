<%@ Page Title="Quản lý Danh mục Vai trò" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="vai-tro.aspx.cs" Inherits="VTT.DanhMuc.vai_tro" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC VAI TRÒ</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Vai trò
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên vai trò..." onkeyup="if(event.keyCode===13) loadData();" />
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
                            <th style="width: 140px;">Mã Vai trò</th>
                            <th>Tên Vai trò</th>
                            <th style="width: 130px;">Quản trị?</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyVaiTro"></tbody>
                </table>
            </div>
        </div>

        <div id="modalVaiTro" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Vai trò</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddVaiTroID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã vai trò <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaVaiTro" class="form-control"
                                placeholder="VD: ADMIN, IT, NV_KHOTHONG" />
                        </div>
                        <div class="form-group col-6">
                            <label>Trạng thái</label>
                            <select id="ddlTrangThai" class="form-control">
                                <option value="1">Đang hoạt động</option>
                                <option value="0">Ngừng hoạt động</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Tên vai trò <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenVaiTro" class="form-control" placeholder="Nhập tên vai trò..." />
                    </div>

                    <div class="form-group">
                        <label style="display:flex; align-items:center; gap:8px; font-weight:400;">
                            <input type="checkbox" id="chkLaQuanTri" style="width:auto;" />
                            Là vai trò Quản trị (Admin/IT) — được toàn quyền CRUD mọi phòng ban
                        </label>
                        <small class="text-muted">Lưu ý: hệ thống kiểm tra quyền dựa trên <b>Mã vai trò</b> = "ADMIN"
                            hoặc "IT". Đánh dấu ô này chỉ mang tính hiển thị/quản lý, hãy đảm bảo đặt đúng Mã vai trò
                            tương ứng.</small>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script src="../assets/js/danh-muc/vai-tro.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>