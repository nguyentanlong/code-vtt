<%@ Page Title="Quản lý Danh mục Công ty" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="nhan-vien.aspx.cs" Inherits="VTT.DanhMuc.nhan_vien" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../assets/css/selects.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
        <link href="../assets/css/danh-muc/nhan-vien.css?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"
            rel="stylesheet" />
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container-fluid mt-3">
            <!-- THANH TIÊU ĐỀ VÀ BỘ LỌC TÌM KIẾM -->
            <div class="card mb-3 shadow-sm">
                <div class="card-body">
                    <div class="row g-2 align-items-center">
                        <!-- Tiêu đề -->
                        <div class="col-md-3">
                            <h4 class="mb-0 text-primary fw-bold">
                                <i class="fa fa-users me-2"></i>Danh Mục Nhân Viên
                            </h4>
                        </div>

                        <!-- Ô Tìm kiếm -->
                        <div class="input-group">
                            <input type="text" id="txtSearch" class="form-control"
                                placeholder="Nhập mã hoặc tên nhân viên..." />
                            <button type="button" id="btnSearch" class="btn btn-primary"
                                onclick="loadData(); return false;">
                                <i class="fa fa-search"></i> Tìm kiếm
                            </button>
                        </div>

                        <!-- Nút Thêm Mới -->
                        <div class="col-md-3 text-end">
                            <button type="button" class="btn btn-primary px-3" onclick="openModalAdd()">
                                <i class="fa fa-plus-circle me-1"></i>Thêm Nhân Viên
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- BẢNG HIỂN THỊ DANH SÁCH NHÂN VIÊN -->
            <div class="card shadow-sm">
                <div class="card-body p-0">
                    <div class="table-responsive">
                        <table class="table table-hover table-striped align-middle mb-0">
                            <thead class="table-light">
                                <tr>
                                    <th class="text-center" style="width: 50px;">STT</th>
                                    <th style="width: 140px;">Mã Nhân Viên</th>
                                    <th>Họ Và Tên</th>
                                    <th>Phòng Ban</th>
                                    <th>Bộ Phận</th>
                                    <th class="text-center" style="width: 130px;">Trạng Thái</th>
                                    <th class="text-center" style="width: 100px;">Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody id="tbodyNhanVien">
                                <!-- JS loadData() sẽ render danh sách nhân viên vào đây -->
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <!-- MODAL THÊM / SỬA NHÂN VIÊN -->
        <div class="modal fade" id="modalNhanVien" tabindex="-1" aria-labelledby="modalTitle" aria-hidden="true"
            data-bs-backdrop="static">
            <div class="modal-dialog modal-dialog-centered modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white py-2">
                        <h5 class="modal-title fw-bold" id="modalTitle">Thêm Mới Nhân Viên</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"
                            aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="row g-3">
                            <!-- Mã Nhân Viên -->
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Mã nhân viên <span
                                        class="text-danger">*</span></label>
                                <input type="text" id="txtMaNhanVien" class="form-control" placeholder="Mã NV..." />
                            </div>

                            <!-- Họ Và Tên -->
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Họ và tên <span class="text-danger">*</span></label>
                                <input type="text" id="txtHoTen" class="form-control" placeholder="Họ và tên..." />
                            </div>

                            <!-- Chọn Phòng Ban (Dropdown) -->
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Thuộc Phòng ban</label>
                                <select id="ddlModalPhongBan" class="form-select">
                                    <option value="">-- Chọn Phòng Ban --</option>
                                </select>
                            </div>

                            <!-- Chọn Bộ Phận (Dropdown liên thông/Cascading theo Phòng ban) -->
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Thuộc Bộ phận</label>
                                <select id="ddlModalBoPhan" class="form-select">
                                    <option value="">-- Chọn Bộ Phận --</option>
                                </select>
                            </div>

                            <!-- Trạng Thái -->
                            <div class="col-12 mt-3">
                                <div class="form-check form-switch">
                                    <input class="form-check-input" type="checkbox" id="chkTrangThai" checked />
                                    <label class="form-check-label fw-bold" for="chkTrangThai">Đang làm việc (Hoạt
                                        động)</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer bg-light py-2">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal"><i
                                class="fa fa-times me-1"></i>Hủy</button>
                        <button type="button" class="btn btn-primary" id="btnSave"><i class="fa fa-save me-1"></i>Lưu Dữ
                            Liệu</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- SCRIPTS -->
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
        <script src='<%= ResolveUrl("~/assets/js/selects-v3.js") %>'></script>
        <script src='<%= ResolveUrl("~/assets/js/danh-muc/nhan-vien.js") %>'></script>
    </asp:Content>