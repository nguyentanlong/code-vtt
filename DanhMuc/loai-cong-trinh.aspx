<%@ Page Title="Quản lý Danh mục Loại công trình" Language="C#" MasterPageFile="~/DanhMuc/child.Master"
    AutoEventWireup="true" CodeFile="loai-cong-trinh.aspx.cs" Inherits="VTT.DanhMuc.loai_cong_trinh" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC LOẠI CÔNG TRÌNH</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Loại công trình
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên loại công trình..."
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
                            <th style="width: 130px;">Mã Loại</th>
                            <th>Tên Loại công trình</th>
                            <th style="width: 180px;">Loại cha</th>
                            <th style="width: 90px;">Thứ tự</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyLoaiCongTrinh"></tbody>
                </table>
            </div>
        </div>

        <div id="modalLoaiCongTrinh" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Loại công trình</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddLoaiCongTrinhID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã loại <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaLoai" class="form-control" placeholder="VD: NHA_XUONG" />
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
                        <label>Tên loại công trình <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenLoai" class="form-control"
                            placeholder="Nhập tên loại công trình..." />
                    </div>

                    <div class="form-group">
                        <label>Loại cha</label>
                        <select id="ddlLoaiCha" class="form-control">
                            <option value="">-- Không có (Cấp cao nhất) --</option>
                        </select>
                    </div>

                    <div class="form-group">
                        <label>Thứ tự</label>
                        <input type="number" id="txtThuTu" class="form-control" value="0" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/loai-cong-trinh.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>