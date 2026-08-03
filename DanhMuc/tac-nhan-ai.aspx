<%@ Page Title="Quản lý Tác nhân AI" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="tac-nhan-ai.aspx.cs" Inherits="VTT.DanhMuc.tac_nhan_ai" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC TÁC NHÂN AI</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Tác nhân AI
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên tác nhân..." onkeyup="if(event.keyCode===13) loadData();" />
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
                            <th style="width: 130px;">Mã Tác nhân</th>
                            <th>Tên Tác nhân</th>
                            <th style="width: 180px;">Phòng ban</th>
                            <th style="width: 160px;">Model mặc định</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyAIAgent"></tbody>
                </table>
            </div>
        </div>

        <div id="modalAIAgent" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Tác nhân AI</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddAIAgentID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã tác nhân <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaAgent" class="form-control" placeholder="VD: AGENT_QA" />
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
                        <label>Tên tác nhân <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenAgent" class="form-control"
                            placeholder="Nhập tên tác nhân AI..." />
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Phòng ban <span class="text-danger">*</span></label>
                            <select id="ddlPhongBan" class="form-control">
                                <option value="">-- Chọn phòng ban --</option>
                            </select>
                        </div>
                        <div class="form-group col-6">
                            <label>Model mặc định</label>
                            <select id="ddlModel" class="form-control">
                                <option value="">-- Không chọn --</option>
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
            src="../assets/js/danh-muc/tac-nhan-ai.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>