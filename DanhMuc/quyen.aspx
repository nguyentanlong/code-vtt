<%@ Page Title="Phân quyền chi tiết" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="quyen.aspx.cs" Inherits="VTT.DanhMuc.quyen" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">PHÂN QUYỀN CHI TIẾT</h2>
            </div>

            <div class="dm-filter">
                <div class="filter-group">
                    <label>Công ty</label>
                    <select id="ddlCongTy" class="form-control"></select>
                </div>
                <div class="filter-group">
                    <label>Phòng ban</label>
                    <select id="ddlPhongBan" class="form-control" disabled>
                        <option value="">-- Chọn Công ty trước --</option>
                    </select>
                </div>
                <div class="filter-group">
                    <label>Tài khoản (chọn 1 để sửa quyền hiện có, chọn nhiều để cấp thêm hàng loạt)</label>
                    <select id="ddlTaiKhoan" class="form-control" multiple size="6" disabled>
                    </select>
                </div>
                <div class="filter-group">
                    <label>Vai trò (tùy chọn - gán cho tất cả tài khoản đã chọn)</label>
                    <select id="ddlVaiTro" class="form-control">
                        <option value="">-- Không gán Vai trò --</option>
                    </select>
                </div>
            </div>

            <div id="grantModeNote" class="dm-note" style="display:none; margin: 8px 0; font-style: italic;"></div>

            <div class="dm-table-wrapper">
                <table class="dm-table" id="tblQuyenGrid">
                    <thead>
                        <tr>
                            <th style="width: 40px;"></th>
                            <th>Trang</th>
                            <th style="width: 160px;">Xem</th>
                            <th style="width: 160px;">Thêm</th>
                            <th style="width: 160px;">Sửa</th>
                            <th style="width: 160px;">Xóa</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyQuyenGrid">
                        <tr>
                            <td colspan="6">Vui lòng chọn Công ty / Phòng ban / Tài khoản ở trên.</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- <div class="dm-footer" style="margin-top: 16px; text-align: right;">
                <button type="button" class="btn btn-success" id="btnSaveQuyen">
                    <i class="fa fa-save"></i> Lưu phân quyền
                </button>
            </div> -->
            <div class="dm-footer"
                style="margin-top: 16px; display:flex; align-items:center; justify-content:flex-end; gap:12px;">
                <label style="margin:0;">Thời hạn hiệu lực:</label>
                <select id="ddlThoiHan" class="form-control" style="width:180px;">
                    <option value="permanent">Vĩnh viễn</option>
                    <option value="8h">Tạm 8 giờ</option>
                    <option value="custom">Tùy chỉnh (giờ)</option>
                </select>
                <input type="number" id="txtSoGioTuyChinh" class="form-control" style="width:100px; display:none;"
                    min="1" placeholder="Số giờ" />
                <button type="button" class="btn btn-success" id="btnSaveQuyen">
                    <i class="fa fa-save"></i> Lưu phân quyền
                </button>
            </div>
        </div>

        <script src="../assets/js/danh-muc/quyen.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>