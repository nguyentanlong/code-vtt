<%@ Page Title="" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true" CodeBehind="bo-phan.aspx.cs" Inherits="VTT.DanhMuc.bo_phan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../assets/css/selects.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>" rel="stylesheet" />
    <link href="../assets/css/danh-muc/phong-ban.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>" rel="stylesheet" />
    <link href="../assets/css/danh-muc/bo-phan.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" id="hdfCongTyID" value="1" />
    <input type="hidden" id="hdfBoPhanID" value="0" />

    <div class="container-fluid py-3 page-container">
        <!-- BAR THAO TÁC & LỌC THEO PHÒNG BAN -->
        <div class="card mb-3 shadow-sm">
            <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
                <div class="d-flex gap-2 align-items-center flex-grow-1">
                    <label class="fw-bold me-1">Phòng ban:</label>
                    <select id="ddlFilterPhongBan" class="form-select" style="max-width: 280px;" onchange="loadData()">
                        <!-- Dropdown load từ danh sách phòng ban -->
                    </select>

                    <input type="text" id="txtKeyword" class="form-control ms-2" style="max-width: 250px;" placeholder="Tìm theo mã, tên bộ phận..." />
                    <button type="button" class="btn btn-primary" onclick="loadData()">
                        <i class="bi bi-search"></i> Tìm kiếm
                    </button>
                </div>
                <button type="button" class="btn btn-success" onclick="openModalAdd()">
                    <i class="bi bi-plus-lg"></i> Thêm bộ phận
                </button>
            </div>
        </div>

        <!-- BẢNG DỮ LIỆU -->
        <div class="card shadow-sm">
            <div class="card-body p-0">
                <div class="table-responsive">
                    <table class="table table-hover table-bordered mb-0 align-middle">
                        <thead>
                            <tr>
                                <th class="text-center" style="width: 50px;">STT</th>
                                <th>Mã bộ phận</th>
                                <th>Tên bộ phận</th>
                                <th>Trưởng bộ phận</th>
                                <th class="text-center">Thứ tự</th>
                                <th class="text-center">Trạng thái</th>
                                <th class="text-center" style="width: 100px;">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody id="tblDataBoPhan">
                            <!-- JS Render dữ liệu -->
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="card-footer bg-white text-muted small" id="lblPaginationInfo">
                Hiển thị 0 bộ phận
            </div>
        </div>
    </div>

    <!-- MODAL THÊM / SỬA BỘ PHẬN -->
    <div class="modal fade" id="modalBoPhan" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white py-2">
                    <h5 class="modal-title fs-6 fw-bold" id="modalTitle">Thêm mới Bộ Phận</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <form id="frmBoPhan" onsubmit="return false;">
                        <div class="row g-3">
                            <div class="col-md-12">
                                <label class="form-label required fw-bold text-dark">Thuộc Phòng ban</label>
                                <select id="ddlModalPhongBan" class="form-select"></select>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label required fw-bold text-dark">Mã bộ phận</label>
                                <input type="text" id="txtMaBoPhan" class="form-control" placeholder="Mã BP..." />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label required fw-bold text-dark">Tên bộ phận</label>
                                <input type="text" id="txtTenBoPhan" class="form-control" placeholder="Tên BP..." />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold text-dark">Trưởng bộ phận</label>
                                <div class="clSelect" 
                                     id="chonTruongBoPhan" 
                                     data-value="NhanVienID" 
                                     data-member="TenHienThi" 
                                     data-default="-1" 
                                     data-link="">
                                </div>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold text-dark">Thứ tự hiển thị</label>
                                <input type="number" id="txtThuTu" class="form-control" value="0" />
                            </div>
                            <div class="col-md-12">
                                <label class="form-label fw-bold text-dark">Trạng thái</label>
                                <select id="ddlTrangThai" class="form-select">
                                    <option value="1">Đang hoạt động</option>
                                    <option value="0">Ngừng hoạt động</option>
                                </select>
                            </div>
                        </div>
                    </form>
                </div>
                <div class="modal-footer py-2">
                    <button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Hủy</button>
                    <button type="button" class="btn btn-primary btn-sm" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="<%= ResolveUrl("~/assets/js/selects-v3.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/danh-muc/bo-phan.js") %>"></script>
</asp:Content>
