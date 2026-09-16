<%@ Page Title="Thông tin tài khoản" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
        CodeFile="thong-tin-tai-khoan.aspx.cs" Inherits="VTT.thong_tin_tai_khoan" %>

        <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
                <div class="dm-container" style="max-width:640px; margin:0 auto;">
                        <div class="dm-header">
                                <h2 class="dm-title">THÔNG TIN TÀI KHOẢN</h2>
                        </div>

                        <div class="dm-form" style="display:flex; flex-direction:column; gap:20px;">

                                <div>
                                        <h3 style="margin-bottom:10px;">Thông tin cá nhân</h3>
                                        <div class="form-group"><label>Tên đăng nhập</label><input type="text"
                                                        id="txtTenDangNhap" class="form-control" disabled /></div>
                                        <div class="form-group"><label>Họ tên</label><input type="text" id="txtHoTen"
                                                        class="form-control" disabled /></div>
                                        <div class="form-group"><label>Mã nhân viên</label><input type="text"
                                                        id="txtMaNhanVien" class="form-control" disabled /></div>
                                        <div class="form-group"><label>Phòng ban</label><input type="text"
                                                        id="txtPhongBan" class="form-control" disabled /></div>
                                        <div class="form-group"><label>Chi nhánh</label><input type="text"
                                                        id="txtChiNhanh" class="form-control" disabled /></div>
                                        <div class="form-group"><label>Vai trò</label><input type="text" id="txtVaiTro"
                                                        class="form-control" disabled /></div>
                                </div>

                                <div style="border-top:1px solid #e2e8f0; padding-top:16px;">
                                        <h3 style="margin-bottom:10px;">Thông tin liên hệ</h3>
                                        <div class="form-group"><label>Số điện thoại</label><input type="text"
                                                        id="txtSoDienThoai" class="form-control" /></div>
                                        <div class="form-group"><label>Email</label><input type="email" id="txtEmail"
                                                        class="form-control" /></div>
                                        <button type="button" class="btn btn-primary" onclick="luuLienHe()"><i
                                                        class="fa fa-save"></i> Lưu
                                                thông tin liên hệ</button>
                                </div>

                                <div style="border-top:1px solid #e2e8f0; padding-top:16px;">
                                        <h3 style="margin-bottom:10px;">Đổi mật khẩu</h3>
                                        <div class="form-group"><label>Mật khẩu hiện tại</label><input type="password"
                                                        id="txtMatKhauCu" class="form-control"
                                                        autocomplete="current-password" /></div>
                                        <div class="form-group"><label>Mật khẩu mới</label><input type="password"
                                                        id="txtMatKhauMoi" class="form-control"
                                                        autocomplete="new-password" /></div>
                                        <div class="form-group"><label>Xác nhận mật khẩu mới</label><input
                                                        type="password" id="txtXacNhanMatKhau" class="form-control"
                                                        autocomplete="new-password" /></div>
                                        <button type="button" class="btn btn-primary" onclick="doiMatKhau()"><i
                                                        class="fa fa-key"></i> Đổi
                                                mật khẩu</button>
                                </div>

                        </div>
                </div>

                <script
                        src="../assets/js/danh-muc/thong-tin-tai-khoan.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
        </asp:Content>