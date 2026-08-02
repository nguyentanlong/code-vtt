<%@ Page Title="Quản lý Danh mục Công ty" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="phong-ban.aspx.cs" Inherits="VTT.DanhMuc.phong_ban" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../assets/css/danh-muc/phong-ban.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <!-- Hidden Fields lưu tham số hệ thống -->
        <input type="hidden" id="hdfCongTyID" value="1" /> <!-- Tạm cứng Công ty ID = 1 -->
        <input type="hidden" id="hdfPhongBanID" value="0" />

        <div class="container-fluid py-3">
            <!-- BAR THAO TÁC & TÌM KIẾM -->
            <div class="card mb-3 shadow-sm">
                <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
                    <div class="d-flex gap-2 align-items-center flex-grow-1">
                        <input type="text" id="txtKeyword" class="form-control" style="max-width: 300px;"
                            placeholder="Tìm theo mã, tên phòng..." />
                        <select id="ddlFilterTrangThai" class="form-select" style="max-width: 180px;">
                            <option value="">-- Tất cả trạng thái --</option>
                            <option value="1">Đang hoạt động</option>
                            <option value="0">Ngừng hoạt động</option>
                        </select>
                        <button type="button" class="btn btn-primary" onclick="loadData()">
                            <i class="bi bi-search"></i> Tìm kiếm
                        </button>
                    </div>
                    <button type="button" class="btn btn-success" onclick="openModalAdd()">
                        <i class="bi bi-plus-lg"></i> Thêm phòng ban
                    </button>
                </div>
            </div>

            <!-- BẢNG DỮ LIỆU -->
            <div class="card shadow-sm">
                <div class="card-body p-0">
                    <div class="table-responsive">
                        <table class="table table-hover table-striped mb-0 align-middle">
                            <thead class="table-light">
                                <tr>
                                    <th class="text-center" style="width: 50px;">STT</th>
                                    <th>Mã phòng</th>
                                    <th>Tên phòng ban</th>
                                    <th>Phòng ban cha</th>
                                    <th>Trưởng phòng</th>
                                    <th class="text-center">Cấp độ</th>
                                    <th class="text-center">Thứ tự</th>
                                    <th class="text-center">Trạng thái</th>
                                    <th class="text-center" style="width: 120px;">Thao tác</th>
                                </tr>
                            </thead>
                            <tbody id="tblDataPhongBan">
                                <!-- JS load dữ liệu động vào đây -->
                            </tbody>
                        </table>
                    </div>
                </div>
                <!-- PHÂN TRANG -->
                <div class="card-footer bg-white d-flex justify-content-between align-items-center">
                    <div id="lblPaginationInfo" class="text-muted small">Đang tải...</div>
                    <ul class="pagination pagination-sm mb-0" id="ulPagination"></ul>
                </div>
            </div>
        </div>

        <!-- MODAL THÊM / SỬA PHÒNG BAN -->
        <div class="modal fade" id="modalPhongBan" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="modalTitle">Thêm mới Phòng Ban</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <form id="frmPhongBan" onsubmit="return false;">
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label required">Mã phòng ban</label>
                                    <input type="text" id="txtMaPhongBan" class="form-control" placeholder="Mã PB..."
                                        required />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label required">Tên phòng ban</label>
                                    <input type="text" id="txtTenPhongBan" class="form-control" placeholder="Tên PB..."
                                        required />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Phòng ban cấp cha</label>
                                    <select id="ddlPhongBanCha" class="form-select">
                                        <option value="0">-- Là cấp cao nhất --</option>
                                    </select>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Trưởng phòng ID</label>
                                    <input type="number" id="txtTruongPhongID" class="form-control"
                                        placeholder="Mã NV làm trưởng phòng" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Thứ tự hiển thị</label>
                                    <input type="number" id="txtThuTu" class="form-control" value="0" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Trạng thái</label>
                                    <select id="ddlTrangThai" class="form-select">
                                        <option value="1">Đang hoạt động</option>
                                        <option value="0">Ngừng hoạt động</option>
                                    </select>
                                </div>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                        <button type="button" class="btn btn-primary" onclick="savePhongBan()">Lưu thông tin</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- SCRIPT REFERENCE -->
        <script
            src="../assets/js/danh-muc/phong-ban.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>