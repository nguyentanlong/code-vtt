<%@ Page Title="Quản lý Phòng ban" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="phong-ban.aspx.cs" Inherits="VTT.DanhMuc.phong_ban" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">CÂY TỔ CHỨC (Phòng ban / Bộ phận / Tổ)</h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Đơn vị
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control" placeholder="Tìm theo Mã hoặc Tên..."
                        onkeyup="if(event.keyCode===13) loadData(1);" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchChiNhanh" class="form-control" onchange="loadData(1)">
                        <option value="">-- Tất cả chi nhánh --</option>
                    </select>
                </div>
                <div class="filter-group">
                    <select id="ddlSearchTrangThai" class="form-control" onchange="loadData()">
                        <option value="">-- Tất cả trạng thái --</option>
                        <option value="1">Đang hoạt động</option>
                        <option value="0">Ngừng hoạt động</option>
                    </select>
                </div>
                <button type="button" class="btn btn-info" onclick="currentPagePhongBan = 1;loadData()">
                    <i class="fa fa-search"></i> Tìm kiếm
                </button>
            </div>

            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 130px;">Mã</th>
                            <th>Tên đơn vị</th>
                            <th style="width: 130px;">Chi nhánh</th>
                            <th style="width: 90px;">Cấp</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 160px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyPhongBan"></tbody>
                </table>
            </div>
        </div>
        <div id="paginationPhongBan"></div>

        <div id="modalPhongBan" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Đơn vị</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddPhongBanID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã đơn vị <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaPhongBan" class="form-control" placeholder="VD: PB_KYTHUAT" />
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
                        <label>Tên đơn vị <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenPhongBan" class="form-control"
                            placeholder="Nhập tên Phòng ban/Bộ phận/Tổ..." />
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Chi nhánh</label>
                            <select id="ddlFormChiNhanh" class="form-control">
                                <option value="">-- Trực thuộc Tổng công ty --</option>
                            </select>
                        </div>
                        <div class="form-group col-6">
                            <label>Đơn vị cha</label>
                            <select id="ddlFormPhongBanCha" class="form-control">
                                <option value="">-- Không có (Cấp cao nhất) --</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Thứ tự hiển thị</label>
                        <input type="number" id="txtThuTu" class="form-control" value="0" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script src="../assets/js/ui-alert.js"></script>
        <script
            src="../assets/js/danh-muc/phong-ban.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>