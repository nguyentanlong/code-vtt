<%@ Page Title="Quản lý Thiết bị đăng nhập" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="quan-ly-thiet-bi.aspx.cs" Inherits="VTT.DanhMuc.quan_ly_thiet_bi" %>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">THIẾT BỊ ĐÃ ĐĂNG NHẬP</h2>
            </div>

            <p style="color:#64748b; font-size:13.5px; margin-bottom:16px;">
                Tài khoản của bạn được phép đăng nhập tối đa <b>3 thiết bị</b> cùng lúc.
                Nếu bạn không nhận ra 1 thiết bị nào trong danh sách (ví dụ do mất máy hoặc dùng chung máy tính công
                cộng),
                hãy đăng xuất thiết bị đó ngay để bảo mật tài khoản.
            </p>

            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th>Thiết bị</th>
                            <th style="width: 150px;">Địa chỉ IP</th>
                            <th style="width: 160px;">Đăng nhập lần đầu</th>
                            <th style="width: 160px;">Hoạt động gần nhất</th>
                            <th style="width: 130px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyThietBi"></tbody>
                </table>
            </div>
        </div>

        <script src="../assets/js/ui-alert.js"></script>
        <script
            src="../assets/js/danh-muc/quan-ly-thiet-bi.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>