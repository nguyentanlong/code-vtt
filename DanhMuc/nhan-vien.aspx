<%@ Page Title="Quản lý Nhân viên" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="nhan-vien.aspx.cs" Inherits="VTT.DanhMuc.nhan_vien" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH SÁCH NHÂN VIÊN</h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Nhân viên
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên nhân viên..." onkeyup="if(event.keyCode===13) loadData();" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchChiNhanh" class="form-control" onchange="onFilterChiNhanhChange()">
                        <option value="">-- Tất cả chi nhánh --</option>
                    </select>
                </div>
                <div class="filter-group">
                    <select id="ddlSearchPhongBan" class="form-control" onchange="loadData()">
                        <option value="">-- Tất cả phòng ban --</option>
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
                            <th style="width: 130px;">Mã NV</th>
                            <th>Họ tên</th>
                            <th style="width: 160px;">Phòng ban</th>
                            <th style="width: 140px;">Chi nhánh</th>
                            <th style="width: 130px;">Chức vụ</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyNhanVien"></tbody>
                </table>
            </div>
        </div>

        <div id="modalNhanVien" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Nhân viên</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddNhanVienID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã nhân viên <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaNhanVien" class="form-control" placeholder="VD: NV_0012" />
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
                        <label>Họ tên <span class="text-danger">*</span></label>
                        <input type="text" id="txtHoTen" class="form-control" placeholder="Nhập họ tên..." />
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Email</label>
                            <input type="email" id="txtEmail" class="form-control" placeholder="email@vtt.com.vn" />
                        </div>
                        <div class="form-group col-6">
                            <label>Số điện thoại</label>
                            <input type="text" id="txtSoDienThoai" class="form-control" placeholder="09xxxxxxxx" />
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Chi nhánh</label>
                            <select id="ddlFormChiNhanh" class="form-control" onchange="onFormChiNhanhChange()">
                                <option value="">-- Trực thuộc Tổng công ty --</option>
                            </select>
                        </div>
                        <div class="form-group col-6">
                            <label>Phòng ban <span class="text-danger">*</span></label>
                            <select id="ddlFormPhongBan" class="form-control">
                                <option value="">-- Chọn phòng ban --</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Chức vụ</label>
                        <select id="ddlFormChucVu" class="form-control">
                            <option value="">-- Không chọn --</option>
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
            src="../assets/js/danh-muc/nhan-vien.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>