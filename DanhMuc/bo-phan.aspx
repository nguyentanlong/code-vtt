<%@ Page Title="Quản lý Bộ phận" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="bo-phan.aspx.cs" Inherits="VTT.DanhMuc.bo_phan" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC BỘ PHẬN</h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Bộ phận
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên Bộ phận..." onkeyup="if(event.keyCode===13) loadData();" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchChiNhanh" class="form-control" onchange="loadData()">
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
                <button type="button" class="btn btn-info" onclick="loadData()">
                    <i class="fa fa-search"></i> Tìm kiếm
                </button>
            </div>

            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 130px;">Mã Bộ phận</th>
                            <th>Tên Bộ phận</th>
                            <th style="width: 160px;">Thuộc Phòng ban</th>
                            <th style="width: 130px;">Chi nhánh</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 160px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyBoPhan"></tbody>
                </table>
            </div>
        </div>

        <div id="modalBoPhan" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Bộ phận</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddPhongBanID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã Bộ phận <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaPhongBan" class="form-control" placeholder="VD: BP_THICONG" />
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
                        <label>Tên Bộ phận <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenPhongBan" class="form-control" placeholder="Nhập tên Bộ phận..." />
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Chi nhánh</label>
                            <select id="ddlFormChiNhanh" class="form-control" onchange="onFormChiNhanhChange()">
                                <option value="">-- Trực thuộc Tổng công ty --</option>
                            </select>
                        </div>
                        <div class="form-group col-6">
                            <label>Thuộc Phòng ban <span class="text-danger">*</span></label>
                            <select id="ddlFormPhongBanCha" class="form-control">
                                <option value="">-- Chọn phòng ban --</option>
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
        <script src="../assets/js/danh-muc/bo-phan.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>