using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class nhan_vien : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(nhan_vien));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // 1. Lấy danh sách Nhân viên (Dùng Proc sp_chinh_DMNhanVien_GetList)
        [WebMethod]
        public static object GetList(long congTyId, string keyword, byte? trangThai)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword.Trim() },
                    { "@TrangThai", trangThai.HasValue ? (object)trangThai.Value : DBNull.Value }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMNhanVien_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        NhanVienID = dr["NhanVienID"],
                        MaNhanVien = dr["MaNhanVien"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        PhongBanID = dr["PhongBanID"] != DBNull.Value ? dr["PhongBanID"] : (object)null,
                        TenPhongBan = dt.Columns.Contains("TenPhongBan") && dr["TenPhongBan"] != DBNull.Value ? dr["TenPhongBan"].ToString() : "Chưa xếp",
                        BoPhanID = dt.Columns.Contains("BoPhanID") && dr["BoPhanID"] != DBNull.Value ? dr["BoPhanID"] : (object)null,
                        TenBoPhan = dt.Columns.Contains("TenBoPhan") && dr["TenBoPhan"] != DBNull.Value ? dr["TenBoPhan"].ToString() : "",
                        TrangThai = Convert.ToByte(dr["TrangThai"])
                    });
                }

                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMNhanVien GetList: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        // 2. Lấy danh sách Bộ Phận theo Phòng Ban (Dùng để cascade dropdown khi chọn Phòng Ban)
        [WebMethod]
        public static object GetBoPhanByPhongBan(long phongBanId)
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
                    { "@PhongBanID", phongBanId },
                    { "@Keyword", DBNull.Value }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMBoPhan_GetByPhongBan", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        BoPhanID = dr["BoPhanID"],
                        TenBoPhan = dr["TenBoPhan"].ToString(),
                        MaBoPhan = dr["MaBoPhan"].ToString()
                    });
                }

                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }

        // 3. Lưu thông tin Nhân viên (Thêm mới / Cập nhật)
        [WebMethod]
        public static object SaveData(long nhanVienId, long congTyId, string maNhanVien, string hoTen, long? phongBanId, byte trangThai)
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
                { "@NhanVienID", nhanVienId },
                { "@CongTyID", congTyId },
                { "@MaNhanVien", maNhanVien.Trim() },
                { "@HoTen", hoTen.Trim() },
                { "@PhongBanID", (phongBanId.HasValue && phongBanId.Value > 0) ? (object)phongBanId.Value : DBNull.Value },
                { "@TrangThai", trangThai }
            };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMNhanVien_Save", pars);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    // Kiểm tra an toàn sự tồn tại của cột để tránh crash
                    int responseCode = dr.Table.Columns.Contains("ResponseCode")
                        ? Convert.ToInt32(dr["ResponseCode"])
                        : 1;

                    string responseMsg = dr.Table.Columns.Contains("ResponseMessage")
                        ? dr["ResponseMessage"].ToString()
                        : "Lưu thành công!";

                    return new { success = (responseCode == 1), message = responseMsg };
                }

                return new { success = true, message = nhanVienId == 0 ? "Thêm nhân viên thành công!" : "Cập nhật nhân viên thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMNhanVien SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object DeleteData(long nhanVienId)
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
            { "@NhanVienID", nhanVienId }
        };

                // Thực thi Stored Procedure xóa mềm
                db.ExecuteDatasetStoredProcedure("sp_chinh_DMNhanVien_Delete", pars);

                return new { success = true, message = "Xóa nhân viên thành công!" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Lỗi khi xóa: " + ex.Message };
            }
        }
    }
}