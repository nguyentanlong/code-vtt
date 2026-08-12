<%@ Page Title="Timeline Dự án" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true"
    CodeFile="chi-tiet-gd.aspx.cs" Inherits="VTT.DanhMuc.chi_tiet_gd" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="dm-container">
            <div class="dm-header">
                <h2 class="dm-title">Timeline dự án - DuAnID=<span id="lblDuAnID"></span></h2>
                <button type="button" id="btnSync" class="btn btn-primary" onclick="syncTimeline()">
                    <i class="fa fa-rotate"></i> Đồng bộ Timeline
                </button>
            </div>

            <div class="dm-table-wrapper">
                <table class="dm-table">
                    <thead>
                        <tr>
                            <th style="width: 50px;">Thứ tự</th>
                            <th style="width: 160px;">Giai đoạn</th>
                            <th style="width: 150px;">Kế hoạch</th>
                            <th style="width: 150px;">Thực tế</th>
                            <th style="width: 150px;">Tiến độ</th>
                            <th style="width: 100px;">Trạng thái</th>
                            <th>Ghi chú</th>
                            <th style="width: 90px; text-align: center;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyTimeline"></tbody>
                </table>
            </div>
        </div>

        <div id="modalGiaiDoan" class="modal-backdrop" style="display: none;">
            <div class="modal-box">
                <div class="modal-header">
                    <h3 id="modalTitle">Chỉnh sửa Giai đoạn</h3>
                    <span class="modal-close" onclick="closeModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hddDuAnGiaiDoanID" value="0" />

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Kế hoạch bắt đầu</label>
                            <input type="text" id="txtKeHoachBatDau" class="form-control" placeholder="dd/MM/yyyy" />
                        </div>
                        <div class="form-group col-6">
                            <label>Kế hoạch kết thúc</label>
                            <input type="text" id="txtKeHoachKetThuc" class="form-control" placeholder="dd/MM/yyyy" />
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Thực tế bắt đầu</label>
                            <input type="text" id="txtThucTeBatDau" class="form-control" placeholder="dd/MM/yyyy" />
                        </div>
                        <div class="form-group col-6">
                            <label>Thực tế kết thúc</label>
                            <input type="text" id="txtThucTeKetThuc" class="form-control" placeholder="dd/MM/yyyy" />
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Tiến độ (%)</label>
                            <input type="number" id="txtTienDo" class="form-control" min="0" max="100" value="0" />
                        </div>
                        <div class="form-group col-6">
                            <label>Trạng thái</label>
                            <select id="ddlTrangThai" class="form-control">
                                <option value="0">Chưa bắt đầu</option>
                                <option value="1">Đang thực hiện</option>
                                <option value="2">Hoàn thành</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Ghi chú</label>
                        <textarea id="txtGhiChu" class="form-control" rows="2"
                            placeholder="Ghi chú tiến độ..."></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Hủy bỏ</button>
                    <button type="button" class="btn btn-success" onclick="saveData()">Lưu thông tin</button>
                </div>
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
        <script
            src="../assets/js/danh-muc/chi-tiet-gd.js?v=<% Response.Write(VTT.libs.libs.randomVersion()); %>"></script>
    </asp:Content>