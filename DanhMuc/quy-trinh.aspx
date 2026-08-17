<%@ Page Title="Quản lý Quy trình" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="quy-trinh.aspx.cs" Inherits="VTT.DanhMuc.quy_trinh" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC QUY TRÌNH</h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Quy trình
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên quy trình..." onkeyup="if(event.keyCode===13) loadData();" />
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
                            <th style="width: 140px;">Mã Quy trình</th>
                            <th>Tên Quy trình</th>
                            <th style="width: 150px;">Áp dụng cho</th>
                            <th style="width: 90px;">Số bước</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 160px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyQuyTrinh"></tbody>
                </table>
            </div>
        </div>

        <div id="modalQuyTrinh" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Quy trình</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddQuyTrinhID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã quy trình <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaQuyTrinh" class="form-control"
                                placeholder="VD: QT_DUYET_DUAN" />
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
                        <label>Tên quy trình <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenQuyTrinh" class="form-control"
                            placeholder="Nhập tên quy trình..." />
                    </div>

                    <div class="form-group">
                        <label>Áp dụng cho đối tượng <span class="text-danger">*</span></label>
                        <select id="ddlLoaiDoiTuong" class="form-control">
                            <option value="DuAn">Dự án</option>
                            <option value="CongViec">Công việc</option>
                            <option value="TaiLieu">Tài liệu</option>
                            <option value="TriThuc">Tri thức</option>
                            <option value="Khac">Khác</option>
                        </select>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/quy-trinh.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>