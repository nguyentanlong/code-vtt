<%@ Page Title="Chi tiết Quy trình" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="chi-tiet-quy-trinh.aspx.cs" Inherits="VTT.DanhMuc.chi_tiet_quy_trinh" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">Các Bước của Quy trình - QuyTrinhID=<span id="lblQuyTrinhID"></span></h2>
                <button type="button" id="btnAddNew" class="btn btn-primary" onclick="addNewRow()">
                    <i class="fa fa-plus"></i> Thêm Bước
                </button>
            </div>

            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 90px;">Thứ tự</th>
                            <th>Tên Bước</th>
                            <th style="width: 220px;">Vai trò duyệt</th>
                            <th style="width: 150px;">Hành động</th>
                            <th style="width: 140px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyBuoc"></tbody>
                </table>
            </div>
        </div>

        <div class="dm-container" style="margin-top:20px;">
            <div class="dm-header">
                <h2 class="dm-title">LỊCH SỬ DUYỆT (Chỉ xem)</h2>
            </div>
            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 150px;">Thời gian</th>
                            <th style="width: 160px;">Bước</th>
                            <th style="width: 150px;">Đối tượng</th>
                            <th style="width: 150px;">Người xử lý</th>
                            <th style="width: 100px;">Kết quả</th>
                            <th>Lý do</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyLichSu"></tbody>
                </table>
            </div>
        </div>

        <script
            src="../assets/js/danh-muc/chi-tiet-quy-trinh.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>