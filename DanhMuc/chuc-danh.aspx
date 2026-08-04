<%@ Page Title="Quản lý Danh mục Chức danh" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="chuc-danh.aspx.cs" Inherits="VTT.DanhMuc.chuc_danh" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC CHỨC DANH</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Chức danh
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên chức danh..." onkeyup="if(event.keyCode===13) loadData();" />
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
                            <th style="width: 130px;">Mã Chức danh</th>
                            <th>Tên Chức danh</th>
                            <th style="width: 180px;">Chức danh cha</th>
                            <th style="width: 90px;">Cấp bậc</th>
                            <th style="width: 90px;">Thứ tự</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyChucDanh"></tbody>
                </table>
            </div>
        </div>

        <div id="modalChucDanh" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Chức danh</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddChucDanhID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã chức danh <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaChucDanh" class="form-control" placeholder="VD: GD" />
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
                        <label>Tên chức danh <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenChucDanh" class="form-control"
                            placeholder="Nhập tên chức danh..." />
                    </div>

                    <div class="form-group">
                        <label>Chức danh cha</label>
                        <select id="ddlChucDanhCha" class="form-control">
                            <option value="">-- Không có (Cấp cao nhất) --</option>
                        </select>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Cấp bậc</label>
                            <input type="number" id="txtCapBac" class="form-control" value="10" />
                        </div>
                        <div class="form-group col-6">
                            <label>Thứ tự</label>
                            <input type="number" id="txtThuTu" class="form-control" value="0" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/chuc-danh.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>