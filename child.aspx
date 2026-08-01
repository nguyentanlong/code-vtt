<%@ Page Title="" Language="C#" MasterPageFile="~/DanhMuc/child.Master" AutoEventWireup="true" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <!-- server-side script moved to inline expressions to avoid declaration errors inside Content controls -->
        <section class="page" id="page-tong-quan">
            <div class="welcome">
                <div>
                    <h1>
                        <%= System.Web.HttpUtility.HtmlEncode("Xin chào, " + ((Session[" HoTen"] !=null &&
                            !string.IsNullOrWhiteSpace(Session["HoTen"].ToString())) ? Session["HoTen"].ToString() :
                            (Session["Username"] !=null && !string.IsNullOrWhiteSpace(Session["Username"].ToString()) ?
                            Session["Username"].ToString() : "Quản trị viên" ))) %>
                    </h1>
                    <p>Đây là tổng quan hoạt động của Tri Thức - VTT hôm nay.</p>
                </div>
                <div class="date-chip"><span id="currentTimeDisplay">
                        <%= System.Web.HttpUtility.HtmlEncode(System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")) %>
                    </span></div>
            </div>

            <div class="kpi-grid">
                <div class="kpi-card">
                    <div class="kpi-top">
                        <div class="kpi-icon blue">₹</div>
                        <span class="kpi-label">Doanh Thu</span>
                    </div>
                    <div class="kpi-value">12.450.000.000 đ</div>
                    <div class="kpi-sub up">↑ +12.5% so với tháng trước</div>
                </div>

                <div class="kpi-card">
                    <div class="kpi-top">
                        <div class="kpi-icon orange">◷</div>
                        <span class="kpi-label">Công Nợ Phải Thu</span>
                    </div>
                    <div class="kpi-value">4.320.000.000 đ</div>
                    <div class="kpi-sub down">↓ -8.3% so với tháng trước</div>
                </div>

                <div class="kpi-card">
                    <div class="kpi-top">
                        <div class="kpi-icon indigo">▣</div>
                        <span class="kpi-label">Dự Án Đang Thi Công</span>
                    </div>
                    <div class="kpi-value">18</div>
                    <div class="kpi-sub">+2 dự án mới</div>
                </div>

                <div class="kpi-card">
                    <div class="kpi-top">
                        <div class="kpi-icon green">!</div>
                        <span class="kpi-label">Việc Cần Làm Gấp</span>
                    </div>
                    <div class="kpi-value alert">7</div>
                    <div class="kpi-link">Xem chi tiết ›</div>
                </div>
            </div>

            <div class="charts-row">
                <div class="panel">
                    <div class="panel-head">
                        <h3>Doanh Thu Theo Tháng</h3>
                        <div class="select-chip">6 tháng gần đây</div>
                    </div>
                    <div class="legend">
                        <div class="legend-item"><span class="legend-dot bar"></span>Doanh thu (đ)</div>
                        <div class="legend-item"><span class="legend-dot line"></span>Lợi nhuận (đ)</div>
                    </div>
                    <svg class="chart-svg" viewBox="0 0 640 240" preserveAspectRatio="none" id="barChart"></svg>
                </div>

                <div class="panel">
                    <div class="panel-head">
                        <h3>Công Nợ</h3>
                    </div>
                    <div class="donut-wrap">
                        <svg width="180" height="180" viewBox="0 0 180 180" id="donutChart"></svg>
                        <div class="donut-legend">
                            <div class="donut-row">
                                <div class="donut-row-left"><span class="dot" style="background:#16a34a"></span>Chưa
                                    đến
                                    hạn</div>
                                <span class="donut-row-value">2.100M (48.6%)</span>
                            </div>
                            <div class="donut-row">
                                <div class="donut-row-left"><span class="dot" style="background:#f59e0b"></span>Đến
                                    hạn
                                    1-30 ngày</div>
                                <span class="donut-row-value">1.250M (28.9%)</span>
                            </div>
                            <div class="donut-row">
                                <div class="donut-row-left"><span class="dot" style="background:#f97316"></span>Đến
                                    hạn
                                    31-60 ngày</div>
                                <span class="donut-row-value">650M (15.0%)</span>
                            </div>
                            <div class="donut-row">
                                <div class="donut-row-left"><span class="dot" style="background:#dc2626"></span>Trên
                                    60
                                    ngày</div>
                                <span class="donut-row-value">320M (7.5%)</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="bottom-row">
                <div class="panel">
                    <div class="panel-head">
                        <h3>Tiến Độ Dự Án</h3>
                        <a class="see-all" href="#">Xem tất cả</a>
                    </div>
                    <div class="table-scroll">
                        <table>
                            <thead>
                                <tr>
                                    <th>Dự Án</th>
                                    <th>Khách Hàng</th>
                                    <th>Tiến Độ</th>
                                    <th>Kế Hoạch</th>
                                    <th>Trạng Thái</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class="proj-name">Khu Nhà Ở An Phú</td>
                                    <td>Cty An Phú</td>
                                    <td>
                                        <div class="progress-cell">
                                            <div class="progress-bar"><span style="width:65%"></span></div>65%
                                        </div>
                                    </td>
                                    <td>30/09/2025</td>
                                    <td><span class="badge progress">Đang thi công</span></td>
                                </tr>
                                <tr>
                                    <td class="proj-name">Nhà Máy Hưng Phát</td>
                                    <td>Cty Hưng Phát</td>
                                    <td>
                                        <div class="progress-cell">
                                            <div class="progress-bar"><span style="width:40%"></span></div>40%
                                        </div>
                                    </td>
                                    <td>15/08/2025</td>
                                    <td><span class="badge progress">Đang thi công</span></td>
                                </tr>
                                <tr>
                                    <td class="proj-name">Trung Tâm Thương Mại TS</td>
                                    <td>Cty Thành Sơn</td>
                                    <td>
                                        <div class="progress-cell">
                                            <div class="progress-bar"><span style="width:80%"></span></div>80%
                                        </div>
                                    </td>
                                    <td>20/07/2025</td>
                                    <td><span class="badge ontime">Đúng tiến độ</span></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div class="panel">
                    <div class="panel-head">
                        <h3>Việc Cần Làm Gấp</h3>
                        <a class="see-all" href="#">Xem tất cả</a>
                    </div>
                    <ul class="task-list">
                        <li class="task-item">
                            <div class="task-icon">!</div>
                            <div class="task-body">
                                <p>Duyệt thanh toán Đợt 2 dự án Nhà Máy Hưng Phát</p>
                            </div>
                            <div class="task-due">Hôm nay</div>
                        </li>
                        <li class="task-item">
                            <div class="task-icon">!</div>
                            <div class="task-body">
                                <p>Kiểm tra khối lượng Dự án Khu Nhà Ở An Phú</p>
                            </div>
                            <div class="task-due">Hôm nay</div>
                        </li>
                        <li class="task-item">
                            <div class="task-icon">!</div>
                            <div class="task-body">
                                <p>Gửi báo giá cho khách hàng Cty Thành Sơn</p>
                            </div>
                            <div class="task-due">Ngày mai</div>
                        </li>
                    </ul>
                </div>
            </div>
        </section>

        <section class="page" id="page-du-an" style="display:none">
            <div class="page-placeholder">
                <h2>Dự Án</h2>
                <p>Nội dung trang "Dự Án" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-doanh-thu" style="display:none">
            <div class="page-placeholder">
                <h2>Doanh Thu</h2>
                <p>Nội dung trang "Doanh Thu" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-nha-cung-cap" style="display:none">
            <div class="page-placeholder">
                <h2>Nhà Cung Cấp</h2>
                <p>Nội dung trang "Nhà Cung Cấp" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-khach-hang" style="display:none">
            <div class="page-placeholder">
                <h2>Khách Hàng</h2>
                <p>Nội dung trang "Khách Hàng" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-phong-thiet-ke" style="display:none">
            <div class="page-placeholder">
                <h2>Phòng Thiết Kế</h2>
                <p>Nội dung trang "Phòng Thiết Kế" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-phong-ke-toan" style="display:none">
            <div class="page-placeholder">
                <h2>Phòng Kế Toán</h2>
                <p>Nội dung trang "Phòng Kế Toán" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-tien-do" style="display:none">
            <div class="page-placeholder">
                <h2>Tiến Độ</h2>
                <p>Nội dung trang "Tiến Độ" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-hoan-thanh" style="display:none">
            <div class="page-placeholder">
                <h2>Hoàn Thành</h2>
                <p>Nội dung trang "Hoàn Thành" đang được xây dựng.</p>
            </div>
        </section>

        <section class="page" id="page-nguoi-dung" style="display:none">
            <div class="page-placeholder">
                <h2>Người Dùng</h2>
                <p>Nội dung trang "Người Dùng" đang được xây dựng.</p>
            </div>
        </section>
    </asp:Content>