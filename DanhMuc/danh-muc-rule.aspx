<%@ Page Title="Quản lý Danh mục Rules" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="danh-muc-rule.aspx.cs" Inherits="VTT.DanhMuc.danh_muc_rule" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC RULES</h2>
                <button type="button" class="btn btn-primary" onclick="openModal(0)">
                    <i class="fa fa-plus"></i> Thêm mới Rule
                </button>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên rule..." onkeyup="if(event.keyCode===13) loadData();" />
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
                            <th style="width: 140px;">Mã Rule</th>
                            <th>Tên Rule</th>
                            <th style="width: 130px;">Trạng thái</th>
                            <th style="width: 100px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyRule"></tbody>
                </table>
            </div>
        </div>

        <div id="modalRule" class="modal-backdrop" style="display: none;">
            <div class="modal-box" style="width: 700px;">
                <div class="modal-header">
                    <h3 id="modalTitle">Thêm mới Rule</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddRuleID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Mã rule <span class="text-danger">*</span></label>
                            <input type="text" id="txtMaRule" class="form-control"
                                placeholder="VD: RULE_CANHBAO_TIENDO" />
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
                        <label>Tên rule <span class="text-danger">*</span></label>
                        <input type="text" id="txtTenRule" class="form-control" placeholder="Nhập tên rule..." />
                    </div>

                    <div class="form-group">
                        <label>Điều kiện</label>
                        <textarea id="txtDieuKien" class="form-control" rows="3"
                            placeholder="Mô tả điều kiện áp dụng rule..."></textarea>
                    </div>

                    <div class="form-group">
                        <label>Hành động</label>
                        <textarea id="txtHanhDong" class="form-control" rows="3"
                            placeholder="Mô tả hành động khi điều kiện đúng..."></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/danh-muc-rule.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>