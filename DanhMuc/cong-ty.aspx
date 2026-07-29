<%@ Page Title="Quản lý Danh mục Công ty" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true" CodeBehind="cong-ty.aspx.cs" Inherits="VTT.DanhMuc.cong_ty" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../assets/css/danh-muc/cong-ty.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="dm-container">
        <!-- Header Page -->
        <div class="dm-header">
            <h2 class="dm-title">DANH MỤC CÔNG TY</h2>
            <button type="button" class="btn btn-primary" onclick="openModal(0)">
                <i class="fa fa-plus"></i> Thêm mới Công ty
            </button>
        </div>

        <!-- Filter Bar -->
        <div class="dm-filter">
            <div class="filter-group">
                <input type="text" id="txtSearchKeyword" class="form-control" placeholder="Tìm theo Mã hoặc Tên công ty..." onkeyup="if(event.keyCode===13) loadData();" />
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

        <!-- Data Table -->
        <div class="dm-table-wrapper">
            <table class="dm-table">
                <thead>
                    <tr>
                        <th style="width: 50px;">STT</th>
                        <th style="width: 120px;">Mã Công ty</th>
                        <th>Tên Công ty</th>
                        <th style="width: 120px;">Viết tắt</th>
                        <th style="width: 130px;">Mã số thuế</th>
                        <th style="width: 130px;">Trạng thái</th>
                        <th style="width: 110px;">Ngày tạo</th>
                        <th style="width: 100px; text-align: center;">Thao tác</th>
                    </tr>
                </thead>
                <tbody id="tbodyCongTy">
                    <!-- Dynamic rendering by JS -->
                </tbody>
            </table>
        </div>
    </div>

    <!-- Modal Form (Popup) -->
    <div id="modalCongTy" class="modal-backdrop" style="display: none;">
        <div class="modal-box">
            <div class="modal-header">
                <h3 id="modalTitle">Thêm mới Công ty</h3>
                <span class="modal-close" onclick="closeModal()">&times;</span>
            </div>
            <div class="modal-body">
                <input type="hidden" id="hddCongTyID" value="0" />
                
                <div class="form-row">
                    <div class="form-group col-6">
                        <label>Mã công ty <span class="text-danger">*</span></label>
                        <input type="text" id="txtMaCongTy" class="form-control" placeholder="VD: VTT_GROUP" />
                    </div>
                    <div class="form-group col-6">
                        <label>Mã số thuế</label>
                        <input type="text" id="txtMaSoThue" class="form-control" placeholder="Mã số thuế..." />
                    </div>
                </div>

                <div class="form-group">
                    <label>Tên công ty <span class="text-danger">*</span></label>
                    <input type="text" id="txtTenCongTy" class="form-control" placeholder="Nhập tên công ty đầy đủ..." />
                </div>

                <div class="form-row">
                    <div class="form-group col-6">
                        <label>Tên viết tắt</label>
                        <input type="text" id="txtTenVietTat" class="form-control" placeholder="VD: VTT Group" />
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
                    <label>Địa chỉ</label>
                    <input type="text" id="txtDiaChi" class="form-control" placeholder="Địa chỉ trụ sở..." />
                </div>

                <div class="form-group">
                    <label>Mô tả / Ghi chú</label>
                    <textarea id="txtMoTa" class="form-control" rows="3" placeholder="Ghi chú thêm..."></textarea>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
            </div>
        </div>
    </div>

    <!-- SCRIPT REFERENCE -->
    <script src="../assets/js/danh-muc/cong-ty.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
</asp:Content>