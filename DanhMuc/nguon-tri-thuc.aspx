<%@ Page Title="Quản lý Danh mục Nguồn tri thức" Language="C#" MasterPageFile="~/DanhMuc/child.Master"
    AutoEventWireup="true" CodeFile="nguon-tri-thuc.aspx.cs" Inherits="VTT.DanhMuc.nguon_tri_thuc" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC NGUỒN TRI THỨC</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Nguồn tri thức
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên nguồn..." onkeyup="if(event.keyCode===13) loadData();" />
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
                            <th style="width: 140px;">Mã Nguồn</th>
                            <th>Tên Nguồn tri thức</th>
                            <th style="width: 120px;">Độ ưu tiên</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyNguonTriThuc"></tbody>
                </table>
            </div>
        </div>

        <div id="modalNguonTriThuc" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Nguồn tri thức</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddNguonTriThucID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã nguồn <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaNguon" class="form-control" placeholder="VD: NGUON_NOIBO" />
                        </div>
                        <div class="form-group col-6">
                            <label>Độ ưu tiên</label>
                            <input type="number" id="txtDoUuTien" class="form-control" value="0" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Tên nguồn tri thức <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenNguon" class="form-control"
                            placeholder="Nhập tên nguồn tri thức..." />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/nguon-tri-thuc.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>