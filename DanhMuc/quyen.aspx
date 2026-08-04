<%@ Page Title="Quản lý Danh mục Quyền" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="quyen.aspx.cs" Inherits="VTT.DanhMuc.quyen" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC QUYỀN</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Quyền
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên quyền..." onkeyup="if(event.keyCode===13) loadData();" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchNhomQuyen" class="form-control" onchange="loadData()">
                        <option value="">-- Tất cả nhóm quyền --</option>
                    </select>
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
                            <th style="width: 150px;">Mã Quyền</th>
                            <th>Tên Quyền</th>
                            <th style="width: 160px;">Nhóm quyền</th>
                            <th style="width: 120px;">Hành động</th>
                            <th style="width: 130px;">Đối tượng</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyQuyen"></tbody>
                </table>
            </div>
        </div>

        <div id="modalQuyen" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Quyền</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddQuyenID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã quyền <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaQuyen" class="form-control" placeholder="VD: TAILIEU_XEM" />
                        </div>
                        <div class="form-group col-6">
                            <label>Nhóm quyền</label>
                            <select id="ddlNhomQuyen" class="form-control">
                                <option value="">-- Không thuộc nhóm nào --</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Tên quyền <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenQuyen" class="form-control" placeholder="Nhập tên quyền..." />
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Hành động <span class="text-danger">*</span></label>
                            <input type="text" id="txtHanhDong" class="form-control"
                                placeholder="VD: Xem, Sửa, Xóa, Duyệt..." />
                        </div>
                        <div class="form-group col-6">
                            <label>Đối tượng <span class="text-danger">*</span></label>
                            <input type="text" id="txtDoiTuong" class="form-control"
                                placeholder="VD: TaiLieu, DuAn..." />
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Thứ tự</label>
                            <input type="number" id="txtThuTu" class="form-control" value="0" />
                        </div>
                        <div class="form-group col-6">
                            <label>Trạng thái</label>
                            <select id="ddlTrangThai" class="form-control">
                                <option value="1">Đang hoạt động</option>
                                <option value="0">Ngừng hoạt động</option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script src="../assets/js/danh-muc/quyen.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>