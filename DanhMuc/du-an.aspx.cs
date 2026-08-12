using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class du_an : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(du_an));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private static long GetCurrentCongTyID()
        {
            var val = System.Web.HttpContext.Current.Session["CongTyID"];
            if (val == null) return 0;
            return Convert.ToInt64(val);
        }

        [WebMethod(EnableSession = true)]
        public static object GetPhongBanOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", congTyId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAn_GetPhongBanOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { PhongBanID = dr["PhongBanID"], TenPhongBan = dr["TenPhongBan"].ToString() });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetPhongBanOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetLoaiCongTrinhOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", congTyId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAn_GetLoaiCongTrinhOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new { LoaiCongTrinhID = dr["LoaiCongTrinhID"], TenLoai = dr["TenLoai"].ToString() });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetLoaiCongTrinhOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, string phongBanId, string trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            log.Info($"GetList called with congTyId: {congTyId}, keyword: {keyword}, phongBanId: {phongBanId}, trangThai: {trangThai}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@PhongBanID", string.IsNullOrEmpty(phongBanId) ? DBNull.Value : (object)Convert.ToInt64(phongBanId) },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAn_GetList", pars);
                DataTable dt = ds.Tables[0];

                var scope = GetCurrentAccessScope();

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    long rowPhongBanId = Convert.ToInt64(dr["PhongBanID"]);
                    list.Add(new
                    {
                        DuAnID = dr["DuAnID"],
                        PhongBanID = rowPhongBanId,
                        TenPhongBan = dr["TenPhongBan"] == DBNull.Value ? "" : dr["TenPhongBan"].ToString(),
                        LoaiCongTrinhID = dr["LoaiCongTrinhID"] == DBNull.Value ? (object)null : dr["LoaiCongTrinhID"],
                        TenLoai = dr["TenLoai"] == DBNull.Value ? "" : dr["TenLoai"].ToString(),
                        MaDuAn = dr["MaDuAn"].ToString(),
                        TenDuAn = dr["TenDuAn"].ToString(),
                        ChuDauTu = dr["ChuDauTu"] == DBNull.Value ? "" : dr["ChuDauTu"].ToString(),
                        DiaDiem = dr["DiaDiem"] == DBNull.Value ? "" : dr["DiaDiem"].ToString(),
                        TienDo = dr["TienDo"],
                        TrangThaiDuAn = Convert.ToByte(dr["TrangThaiDuAn"]),
                        // Cho client biết dòng này có được Sửa/Xóa hay chỉ Xem, dựa theo Phòng ban
                        CanEditRow = scope.IsFullAccess || scope.PhongBanID == rowPhongBanId
                    });
                }

                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetList: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetById(long id)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            log.Info($"GetById called with id: {id}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@DuAnID", id } };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAn_GetById", pars);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        DuAnID = dr["DuAnID"],
                        PhongBanID = dr["PhongBanID"],
                        LoaiCongTrinhID = dr["LoaiCongTrinhID"] == DBNull.Value ? (object)null : dr["LoaiCongTrinhID"],
                        MaDuAn = dr["MaDuAn"].ToString(),
                        TenDuAn = dr["TenDuAn"].ToString(),
                        ChuDauTu = dr["ChuDauTu"] == DBNull.Value ? "" : dr["ChuDauTu"].ToString(),
                        DiaDiem = dr["DiaDiem"] == DBNull.Value ? "" : dr["DiaDiem"].ToString(),
                        MoTa = dr["MoTa"] == DBNull.Value ? "" : dr["MoTa"].ToString(),
                        NgayBatDau = dr["NgayBatDau"] == DBNull.Value ? null : (DateTime?)dr["NgayBatDau"],
                        NgayKetThucDuKien = dr["NgayKetThucDuKien"] == DBNull.Value ? null : (DateTime?)dr["NgayKetThucDuKien"],
                        TrangThaiDuAn = dr["TrangThaiDuAn"]
                    };
                    return new { success = true, data = data };
                }
                return new { success = false, message = "Không tìm thấy bản ghi." };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetById: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object SaveData(long duAnId, long phongBanId, object loaiCongTrinhId, string maDuAn, string tenDuAn,
                                       string chuDauTu, string diaDiem, string moTa, string ngayBatDau, string ngayKetThuc, byte trangThaiDuAn)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyID();
            if (congTyId == 0)
                return new { success = false, message = "Không xác định được Công ty của tài khoản. Vui lòng đăng nhập lại!" };

            // --- KIỂM TRA PHÂN QUYỀN THEO PHÒNG BAN ---
            if (!CanEdit(phongBanId))
            {
                return new { success = false, message = "Bạn không có quyền thêm/sửa Dự án thuộc Phòng ban này!" };
            }
            // --- HẾT KIỂM TRA ---

            if (phongBanId <= 0)
            {
                return new { success = false, message = "Vui lòng chọn Phòng ban!" };
            }

            log.Info($"SaveData called with duAnId: {duAnId}, congTyId: {congTyId}, phongBanId: {phongBanId}, maDuAn: {maDuAn}, tenDuAn: {tenDuAn}");
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@DuAnID", duAnId },
                    { "@CongTyID", congTyId },
                    { "@PhongBanID", phongBanId },
                    { "@LoaiCongTrinhID", loaiCongTrinhId == null ? DBNull.Value : (object)Convert.ToInt64(loaiCongTrinhId) },
                    { "@MaDuAn", maDuAn.Trim() },
                    { "@TenDuAn", tenDuAn.Trim() },
                    { "@ChuDauTu", string.IsNullOrEmpty(chuDauTu) ? DBNull.Value : (object)chuDauTu.Trim() },
                    { "@DiaDiem", string.IsNullOrEmpty(diaDiem) ? DBNull.Value : (object)diaDiem.Trim() },
                    { "@MoTa", string.IsNullOrEmpty(moTa) ? DBNull.Value : (object)moTa.Trim() },
                    { "@NgayBatDau", string.IsNullOrEmpty(ngayBatDau) ? DBNull.Value : (object)DateTime.Parse(ngayBatDau) },
                    { "@NgayKetThucDuKien", string.IsNullOrEmpty(ngayKetThuc) ? DBNull.Value : (object)DateTime.Parse(ngayKetThuc) },
                    { "@TrangThaiDuAn", trangThaiDuAn }
                };

                db.ExecuteDatasetStoredProcedure("sp_long_DuAn_Save", pars);
                return new { success = true, message = duAnId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteData(long id)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();

                // --- KIỂM TRA PHÂN QUYỀN THEO PHÒNG BAN ---
                var checkPars = new Dictionary<string, object> { { "@DuAnID", id } };
                DataSet dsCheck = db.ExecuteDatasetStoredProcedure("sp_long_DuAn_GetPhongBanID", checkPars);

                long targetPhongBanId = 0;
                if (dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0 && dsCheck.Tables[0].Rows[0]["PhongBanID"] != DBNull.Value)
                {
                    targetPhongBanId = Convert.ToInt64(dsCheck.Tables[0].Rows[0]["PhongBanID"]);
                }

                if (!CanEdit(targetPhongBanId))
                {
                    return new { success = false, message = "Bạn không có quyền xóa Dự án thuộc Phòng ban này!" };
                }
                // --- HẾT KIỂM TRA ---

                log.Info($"DeleteData called with id: {id}");
                var pars = new Dictionary<string, object> { { "@DuAnID", id } };
                db.ExecuteDatasetStoredProcedure("sp_long_DuAn_Delete", pars);

                return new { success = true, message = "Xóa thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DeleteData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}