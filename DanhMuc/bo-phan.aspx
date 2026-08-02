<%@ Page Title="Quản lý Danh mục Công ty" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="bo-phan.aspx.cs" Inherits="VTT.DanhMuc.bo_phan" %>
    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../assets/css/selects.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
        <link href="../assets/css/danh-muc/phong-ban.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
        <link href="../assets/css/danh-muc/bo-phan.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <input type="hidden" id="hdfCongTyID" value="1" />
        <input type="hidden" id="hdfBoPhanID" value="0" />

        <h1>Test long</h1>

        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
        <script src='<%= ResolveUrl("~/assets/js/selects-v3.js") %>'></script>
        <script src='<%= ResolveUrl("~/assets/js/danh-muc/bo-phan.js") %>'></script>

    </asp:Content>