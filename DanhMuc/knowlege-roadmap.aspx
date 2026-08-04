<%@ Page Title="Knowledge Road Map" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="knowlege-roadmap.aspx.cs" Inherits="VTT.DanhMuc.knowlege_roadmap" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">KNOWLEDGE ROAD MAP</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Chủ đề
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control" placeholder="Tìm theo Chủ đề..."
                        onkeyup="if(event.keyCode===13) loadData();" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchTrangThai" class="form-control" onchange="loadData()">
                        <option value="">-- Tất cả trạng thái --</option>
                        <option value="1">Đã hoàn thành</option>
                        <option value="0">Chưa hoàn thành</option>
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
                            <th>Chủ đề</th>
                            <th>Mô tả</th>
                            <th style="width: 130px;">Mức độ ưu tiên</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyRoadmap"></tbody>
                </table>
            </div>
        </div>

        <div id="modalRoadmap" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Chủ đề</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddRoadmapID" value="0" />

                    <div class="form-group">
                        <label>Chủ đề <span class="text-danger">*</span></label>
                        <input type="text" id="txtChuDe" class="form-control"
                            placeholder="VD: Quy trình xử lý sự cố thi công" />
                    </div>

                    <div class="form-group">
                        <label>Mô tả</label>
                        <textarea id="txtMoTa" class="form-control" rows="3"
                            placeholder="Mô tả chi tiết chủ đề tri thức cần bổ sung..."></textarea>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mức độ ưu tiên</label>
                            <select id="ddlMucDoUuTien" class="form-control">
                                <option value="1">Thấp</option>
                                <option value="2" selected>Trung bình</option>
                                <option value="3">Cao</option>
                            </select>
                        </div>
                        <div class="form-group col-6">
                            <label>Trạng thái</label>
                            <select id="ddlTrangThai" class="form-control">
                                <option value="0">Chưa hoàn thành</option>
                                <option value="1">Đã hoàn thành</option>
                            </select>
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
            src="../assets/js/danh-muc/knowlege-roadmap.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>