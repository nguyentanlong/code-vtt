using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    // Kế thừa BasePage để tự động kiểm tra Login khi người dùng load trang
    public partial class bo_phan : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(bo_phan));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static object GetList(long phongBanId, string keyword)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"DMBoPhan GetList called with phongBanId: {phongBanId}, keyword: {keyword}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@PhongBanID", phongBanId },
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword.Trim() }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMBoPhan_GetByPhongBan", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        BoPhanID = dr["BoPhanID"],
                        PhongBanID = dr["PhongBanID"],
                        MaBoPhan = dr["MaBoPhan"].ToString(),
                        TenBoPhan = dr["TenBoPhan"].ToString(),
                        TruongBoPhanID = dr["TruongBoPhanID"] != DBNull.Value ? dr["TruongBoPhanID"] : null,
                        TenTruongBoPhan = dr["TenTruongBoPhan"] != DBNull.Value ? dr["TenTruongBoPhan"].ToString() : "",
                        ThuTu = dr["ThuTu"] != DBNull.Value ? Convert.ToInt32(dr["ThuTu"]) : 0,
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
                        NgayTaoText = dr["NgayTao"] != DBNull.Value ? Convert.ToDateTime(dr["NgayTao"]).ToString("dd/MM/yyyy HH:mm") : ""
                    });
                }

                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMBoPhan GetList: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        /*[WebMethod]
        public static object SaveData(long boPhanId, long phongBanId, string maBoPhan, string tenBoPhan, long? truongBoPhanId, int thuTu, byte trangThai)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"DMBoPhan SaveData called: boPhanId={boPhanId}, phongBanId={phongBanId}, maBoPhan={maBoPhan}, tenBoPhan={tenBoPhan}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@BoPhanID", boPhanId },
                    { "@PhongBanID", phongBanId },
                    { "@MaBoPhan", maBoPhan.Trim() },
                    { "@TenBoPhan", tenBoPhan.Trim() },
                    { "@TruongBoPhanID", (truongBoPhanId.HasValue && truongBoPhanId.Value > 0) ? (object)truongBoPhanId.Value : DBNull.Value },
                    { "@ThuTu", thuTu },
                    { "@TrangThai", trangThai }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMBoPhan_Save", pars);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int responseCode = Convert.ToInt32(dr["ResponseCode"]);
                    string responseMsg = dr["ResponseMessage"].ToString();

                    return new { success = (responseCode == 1), message = responseMsg };
                }

                return new { success = false, message = "Lưu thất bại, không nhận được phản hồi từ cơ sở dữ liệu!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMBoPhan SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }*/

        [WebMethod]
        public static object SaveData(long boPhanId, long phongBanId, string maBoPhan, string tenBoPhan, long? truongBoPhanId, int thuTu, byte trangThai)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            // --- KIỂM TRA PHÂN QUYỀN THEO PHÒNG BAN ---
            if (!CanEdit(phongBanId))
            {
                return new { success = false, message = "Bạn không có quyền thêm/sửa Bộ phận thuộc Phòng ban này!" };
            }
            // --- HẾT KIỂM TRA ---

            log.Info($"DMBoPhan SaveData called: boPhanId={boPhanId}, phongBanId={phongBanId}, maBoPhan={maBoPhan}, tenBoPhan={tenBoPhan}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@BoPhanID", boPhanId },
                    { "@PhongBanID", phongBanId },
                    { "@MaBoPhan", maBoPhan.Trim() },
                    { "@TenBoPhan", tenBoPhan.Trim() },
                    { "@TruongBoPhanID", (truongBoPhanId.HasValue && truongBoPhanId.Value > 0) ? (object)truongBoPhanId.Value : DBNull.Value },
                    { "@ThuTu", thuTu },
                    { "@TrangThai", trangThai }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMBoPhan_Save", pars);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int responseCode = Convert.ToInt32(dr["ResponseCode"]);
                    string responseMsg = dr["ResponseMessage"].ToString();
                    return new { success = (responseCode == 1), message = responseMsg };
                }

                return new { success = false, message = "Lưu thất bại, không nhận được phản hồi từ cơ sở dữ liệu!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMBoPhan SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
//hàm mới
        [WebMethod]
        public static object DeleteData(long boPhanId)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            try
            {
                ConnectServer db = new ConnectServer();

                // --- KIỂM TRA PHÂN QUYỀN THEO PHÒNG BAN ---
                var checkPars = new Dictionary<string, object> { { "@BoPhanID", boPhanId } };
                DataSet dsCheck = db.ExecuteDatasetStoredProcedure("sp_long_DMBoPhan_GetPhongBanID", checkPars);

                long targetPhongBanId = 0;
                if (dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0 && dsCheck.Tables[0].Rows[0]["PhongBanID"] != DBNull.Value)
                {
                    targetPhongBanId = Convert.ToInt64(dsCheck.Tables[0].Rows[0]["PhongBanID"]);
                }

                if (!CanEdit(targetPhongBanId))
                {
                    return new { success = false, message = "Bạn không có quyền xóa Bộ phận thuộc Phòng ban này!" };
                }
                // --- HẾT KIỂM TRA ---

                var pars = new Dictionary<string, object> { { "@BoPhanID", boPhanId } };
                db.ExecuteDatasetStoredProcedure("sp_long_DMBoPhan_Delete", pars);

                return new { success = true, message = "Xóa bộ phận thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMBoPhan DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object GetListNhanVien(long congTyId)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn!" };
            }

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", DBNull.Value },
                    { "@TrangThai", (byte)1 } // Chỉ lấy nhân viên đang làm việc
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMNhanVien_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    string maNV = dr["MaNhanVien"].ToString();
                    string hoTen = dr["HoTen"].ToString();
                    string tenPhong = dt.Columns.Contains("TenPhongBan") && dr["TenPhongBan"] != DBNull.Value ? dr["TenPhongBan"].ToString() : "Chưa thuộc PB";
                    string tenBoPhan = dt.Columns.Contains("TenBoPhan") && dr["TenBoPhan"] != DBNull.Value ? dr["TenBoPhan"].ToString() : "";

                    string tenBoPhanText = !string.IsNullOrEmpty(tenBoPhan) ? $" - {tenBoPhan}" : "";
                    string tenHienThi = $"{tenPhong}{tenBoPhanText} - {hoTen} ({maNV})";

                    list.Add(new
                    {
                        NhanVienID = dr["NhanVienID"],
                        TenHienThi = tenHienThi
                    });
                }

                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object GetListPhongBan(long congTyId)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn!" };
            }

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", DBNull.Value },
                    { "@TrangThai", (byte)1 } // Chỉ lấy phòng ban đang hoạt động
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMPhongBan_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        PhongBanID = dr["PhongBanID"],
                        TenPhongBan = dr["TenPhongBan"].ToString(),
                        MaPhongBan = dr["MaPhongBan"].ToString()
                    });
                }

                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }
    }
}