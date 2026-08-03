<%@ Page Title="Quản lý Danh mục Vai trò Dự án" Language="C#" MasterPageFile="~/DanhMuc/child.Master"
    AutoEventWireup="true" CodeFile="vai-tro-du-an.aspx.cs" Inherits="VTT.DanhMuc.vai_tro_du_an" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../assets/css/danh-muc/cong-ty.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
        <link href="../assets/css/danh-muc/vai-tro-du-an.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <!-- Header Page -->
            <div class="dm-header">
                <h2 class="dm-title">DANH MỤC VAI TRÒ DỰ ÁN</h2>
                <button type="button" class="btn btn-primary" onclick="addNewRow()">
                    <i class="fa fa-plus"></i> Thêm mới Vai trò
                </button>
            </div>

            <!-- Filter Bar -->
            <div class="dm-filter">
                <div class="filter-group">
                    <input type="text" id="txtSearchKeyword" class="form-control"
                        placeholder="Tìm theo Mã hoặc Tên vai trò..." onkeyup="if(event.keyCode===13) loadData();" />
                </div>
                <div class="filter-group">
                    <select id="ddlSearchCongTy" class="form-control" onchange="loadData()">
                        <option value="">-- Tất cả công ty --</option>
                    </select>
                </div>
                <button type="button" class="btn btn-info" onclick="loadData()">
                    <i class="fa fa-search"></i> Tìm kiếm
                </button>
            </div>

            <!-- Data Table -->
            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 50px;">STT</th>
                            <th style="width: 130px;">Mã Vai trò</th>
                            <th>Tên Vai trò</th>
                            <th style="width: 220px;">Công ty</th>
                            <th style="width: 90px;">Thứ tự</th>
                            <th style="width: 140px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyVaiTro">
                        <!-- Dynamic rendering by JS -->
                    </tbody>
                </table>
            </div>
        </div>

        <!-- SCRIPT REFERENCE -->
        <script
            src="../assets/js/danh-muc/vai-tro-du-an.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>