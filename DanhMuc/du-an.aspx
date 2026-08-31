<%@ Page Title="Danh sách Dự án" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="du-an.aspx.cs" Inherits="VTT.DanhMuc.du_an" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH SÁCH DỰ ÁN</h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Dự án
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên dự án..." onkeyup="if(event.keyCode===13) loadData();" />
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
                        <option value="0">Chuẩn bị</option>
                        <option value="1">Đang thực hiện</option>
                        <option value="2">Hoàn thành</option>
                        <option value="3">Tạm dừng</option>
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
                            <th style="width: 130px;">Mã Dự án</th>
                            <th>Tên Dự án</th>
                            <th style="width: 150px;">Phòng ban</th>
                            <th style="width: 90px;">Tiến độ</th>
                            <th style="width: 120px;">Trạng thái</th>
                            <th style="width: 170px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyDuAn"></tbody>
                </table>
            </div>
        </div>

        <div id="modalDuAn" class="modal-backdrop" style="display: none;">
            <div class="modal-box" style="width: 700px;">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Dự án</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddDuAnID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã dự án <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaDuAn" class="form-control" placeholder="VD: DA_2026_001" />
                        </div>
                        <div class="form-group col-6">
                            <label>Trạng thái</label>
                            <select id="ddlTrangThai" class="form-control">
                                <option value="0">Chuẩn bị</option>
                                <option value="1">Đang thực hiện</option>
                                <option value="2">Hoàn thành</option>
                                <option value="3">Tạm dừng</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Tên dự án <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenDuAn" class="form-control" placeholder="Nhập tên dự án..." />
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
                        <label>Loại công trình</label>
                        <select id="ddlLoaiCongTrinh" class="form-control">
                            <option value="">-- Không chọn --</option>
                        </select>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Chủ đầu tư</label>
                            <input type="text" id="txtChuDauTu" class="form-control" />
                        </div>
                        <div class="form-group col-6">
                            <label>Địa điểm</label>
                            <input type="text" id="txtDiaDiem" class="form-control" />
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Ngày bắt đầu</label>
                            <input type="text" id="txtNgayBatDau" class="form-control" placeholder="dd/MM/yyyy" />
                        </div>
                        <div class="form-group col-6">
                            <label>Ngày kết thúc dự kiến</label>
                            <input type="text" id="txtNgayKetThuc" class="form-control" placeholder="dd/MM/yyyy" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Mô tả</label>
                        <textarea id="txtMoTa" class="form-control" rows="3"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
        <script src="../assets/js/ui-alert.js"></script>
        <script src="../assets/js/danh-muc/du-an.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>