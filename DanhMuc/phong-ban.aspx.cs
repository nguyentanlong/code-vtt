using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    // Kế thừa BasePage để tự động kiểm tra Login khi người dùng load trang
    public partial class phong_ban : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(phong_ban));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static object GetList(long congTyId, string keyword, string trangThai)
        {
            // Kiểm tra Authentication trước khi thực thi
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"DMPhongBan GetList called with congTyId: {congTyId}, keyword: {keyword}, trangThai: {trangThai}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword.Trim() },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMPhongBan_GetList", pars);
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        PhongBanID = dr["PhongBanID"],
                        CongTyID = dr["CongTyID"],
                        MaPhongBan = dr["MaPhongBan"].ToString(),
                        TenPhongBan = dr["TenPhongBan"].ToString(),
                        PhongBanChaID = dr["PhongBanChaID"] != DBNull.Value ? dr["PhongBanChaID"] : null,
                        TenPhongBanCha = dr["TenPhongBanCha"] != DBNull.Value ? dr["TenPhongBanCha"].ToString() : "",
                        CapDo = dr["CapDo"] != DBNull.Value ? Convert.ToInt32(dr["CapDo"]) : 1,
                        DuongDan = dr["DuongDan"] != DBNull.Value ? dr["DuongDan"].ToString() : "",
                        TruongPhongID = dr["TruongPhongID"] != DBNull.Value ? dr["TruongPhongID"] : null,
                        TenTruongPhong = dr["TenTruongPhong"] != DBNull.Value ? dr["TenTruongPhong"].ToString() : "",
                        ThuTu = dr["ThuTu"] != DBNull.Value ? Convert.ToInt32(dr["ThuTu"]) : 0,
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
                        NgayTaoText = dr["NgayTao"] != DBNull.Value ? Convert.ToDateTime(dr["NgayTao"]).ToString("dd/MM/yyyy HH:mm") : ""
                    });
                }

                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMPhongBan GetList: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object GetById(long id)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"DMPhongBan GetById called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@PhongBanID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMPhongBan_GetById", pars);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        PhongBanID = dr["PhongBanID"],
                        CongTyID = dr["CongTyID"],
                        MaPhongBan = dr["MaPhongBan"].ToString(),
                        TenPhongBan = dr["TenPhongBan"].ToString(),
                        PhongBanChaID = dr["PhongBanChaID"] != DBNull.Value ? dr["PhongBanChaID"] : null,
                        TruongPhongID = dr["TruongPhongID"] != DBNull.Value ? dr["TruongPhongID"] : null,
                        ThuTu = dr["ThuTu"] != DBNull.Value ? Convert.ToInt32(dr["ThuTu"]) : 0,
                        TrangThai = Convert.ToByte(dr["TrangThai"])
                    };
                    return new { success = true, data = data };
                }
                return new { success = false, message = "Không tìm thấy phòng ban." };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMPhongBan GetById: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        /*[WebMethod]
        public static object SaveData(long phongBanId, long congTyId, string maPhongBan, string tenPhongBan, long? phongBanChaId, long? truongPhongId, int thuTu, byte trangThai)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"DMPhongBan SaveData called: phongBanId={phongBanId}, congTyId={congTyId}, maPhongBan={maPhongBan}, tenPhongBan={tenPhongBan}, phongBanChaId={phongBanChaId}, truongPhongId={truongPhongId}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@PhongBanID", phongBanId },
                    { "@CongTyID", congTyId },
                    { "@MaPhongBan", maPhongBan.Trim() },
                    { "@TenPhongBan", tenPhongBan.Trim() },
                    { "@PhongBanChaID", (phongBanChaId.HasValue && phongBanChaId.Value > 0) ? (object)phongBanChaId.Value : DBNull.Value },
                    { "@TruongPhongID", (truongPhongId.HasValue && truongPhongId.Value > 0) ? (object)truongPhongId.Value : DBNull.Value },
                    { "@ThuTu", thuTu },
                    { "@TrangThai", trangThai }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMPhongBan_Save", pars);

                // Nếu Proc trả về bảng thông báo kết quả (ResponseCode, ResponseMessage)
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int responseCode = Convert.ToInt32(dr["ResponseCode"]);
                    string responseMsg = dr["ResponseMessage"].ToString();

                    return new { success = (responseCode == 1), message = responseMsg };
                }

                return new { success = true, message = phongBanId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMPhongBan SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod]
        public static object DeleteData(long id)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            log.Info($"DMPhongBan DeleteData called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@PhongBanID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMPhongBan_Delete", pars);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int responseCode = Convert.ToInt32(dr["ResponseCode"]);
                    string responseMsg = dr["ResponseMessage"].ToString();

                    return new { success = (responseCode == 1), message = responseMsg };
                }

                return new { success = true, message = "Xóa phòng ban thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMPhongBan DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }*/
// ham mới
        [WebMethod]
        public static object SaveData(long phongBanId, long congTyId, string maPhongBan, string tenPhongBan, long? phongBanChaId, long? truongPhongId, int thuTu, byte trangThai)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            // --- KIỂM TRA PHÂN QUYỀN THEO PHÒNG BAN ---
            // phongBanId = 0 (tạo mới) chỉ Admin/IT được phép; sửa phòng ban đã tồn tại thì đúng phòng ban mình mới được sửa
            if (!CanEdit(phongBanId))
            {
                return new { success = false, message = "Bạn không có quyền thêm/sửa Phòng ban này!" };
            }
            // --- HẾT KIỂM TRA ---

            log.Info($"DMPhongBan SaveData called: phongBanId={phongBanId}, congTyId={congTyId}, maPhongBan={maPhongBan}, tenPhongBan={tenPhongBan}, phongBanChaId={phongBanChaId}, truongPhongId={truongPhongId}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@PhongBanID", phongBanId },
                    { "@CongTyID", congTyId },
                    { "@MaPhongBan", maPhongBan.Trim() },
                    { "@TenPhongBan", tenPhongBan.Trim() },
                    { "@PhongBanChaID", (phongBanChaId.HasValue && phongBanChaId.Value > 0) ? (object)phongBanChaId.Value : DBNull.Value },
                    { "@TruongPhongID", (truongPhongId.HasValue && truongPhongId.Value > 0) ? (object)truongPhongId.Value : DBNull.Value },
                    { "@ThuTu", thuTu },
                    { "@TrangThai", trangThai }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMPhongBan_Save", pars);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int responseCode = Convert.ToInt32(dr["ResponseCode"]);
                    string responseMsg = dr["ResponseMessage"].ToString();
                    return new { success = (responseCode == 1), message = responseMsg };
                }

                return new { success = true, message = phongBanId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMPhongBan SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
        [WebMethod]
        public static object DeleteData(long id)
        {
            if (!IsAuthenticated())
            {
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
            }

            // --- KIỂM TRA PHÂN QUYỀN THEO PHÒNG BAN ---
            if (!CanEdit(id))
            {
                return new { success = false, message = "Bạn không có quyền xóa Phòng ban này!" };
            }
            // --- HẾT KIỂM TRA ---

            log.Info($"DMPhongBan DeleteData called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@PhongBanID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_chinh_DMPhongBan_Delete", pars);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int responseCode = Convert.ToInt32(dr["ResponseCode"]);
                    string responseMsg = dr["ResponseMessage"].ToString();
                    return new { success = (responseCode == 1), message = responseMsg };
                }

                return new { success = true, message = "Xóa phòng ban thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DMPhongBan DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}