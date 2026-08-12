<%@ Page Title="Quản lý Danh mục Giai đoạn Dự án" Language="C#" MasterPageFile="~/DanhMuc/child.Master"
    AutoEventWireup="true" CodeFile="giai-doan.aspx.cs" Inherits="VTT.DanhMuc.giai_doan" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC GIAI ĐOẠN DỰ ÁN</h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Giai đoạn
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên giai đoạn..." onkeyup="if(event.keyCode===13) loadData();" />
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
                            <th style="width: 140px;">Mã Giai đoạn</th>
                            <th>Tên Giai đoạn</th>
                            <th>Mô tả</th>
                            <th style="width: 100px;">Thứ tự</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyTimeLine"></tbody>
                </table>
            </div>
        </div>

        <div id="modalTimeLine" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Giai đoạn</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddTimeLineID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã giai đoạn <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaGiaiDoan" class="form-control" placeholder="VD: KHOI_CONG" />
                        </div>
                        <div class="form-group col-6">
                            <label>Thứ tự hiển thị</label>
                            <input type="number" id="txtThuTu" class="form-control" value="0" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Tên giai đoạn <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenGiaiDoan" class="form-control"
                            placeholder="Nhập tên giai đoạn..." />
                    </div>

                    <div class="form-group">
                        <label>Mô tả</label>
                        <textarea id="txtMoTa" class="form-control" rows="3"
                            placeholder="Mô tả chi tiết giai đoạn..."></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/giai-doan.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>