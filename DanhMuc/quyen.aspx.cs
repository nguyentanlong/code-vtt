using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public class GrantItem
    {
        public int QuyenID { get; set; }
        public string DataScope { get; set; }
    }

    public partial class quyen : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(quyen));

        protected void Page_Load(object sender, EventArgs e) { }

        private static bool KiemTraAdmin(out object loi)
        {
            loi = null;
            if (!IsAuthenticated())
            {
                loi = new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };
                return false;
            }
            if (!PermissionHelper.IsAdminOwner())
            {
                loi = new { success = false, message = "Bạn không có quyền truy cập chức năng Phân quyền chi tiết!" };
                return false;
            }
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static object GetCongTyOptions()
        {
            if (!KiemTraAdmin(out object loi)) return loi;
            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_Quyen_GetCongTyOptions", new Dictionary<string, object>());
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new { CongTyID = dr["CongTyID"], TenCongTy = dr["TenCongTy"].ToString() });
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetCongTyOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetPhongBanByCongTy(int congTyId)
        {
            if (!KiemTraAdmin(out object loi)) return loi;
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", congTyId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_Quyen_GetPhongBanByCongTy", pars);
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new { PhongBanID = dr["PhongBanID"], TenPhongBan = dr["TenPhongBan"].ToString() });
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetPhongBanByCongTy: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetTaiKhoanByPhongBan(int phongBanId)
        {
            if (!KiemTraAdmin(out object loi)) return loi;
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@PhongBanID", phongBanId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_Quyen_GetTaiKhoanByPhongBan", pars);
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new { TaiKhoanID = dr["TaiKhoanID"], TenDangNhap = dr["TenDangNhap"].ToString(), HoTen = dr["HoTen"].ToString() });
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetTaiKhoanByPhongBan: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetTrangChucNangList()
        {
            if (!KiemTraAdmin(out object loi)) return loi;
            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_Quyen_GetTrangChucNangList", new Dictionary<string, object>());
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new
                    {
                        TrangID = dr["TrangID"],
                        MaTrang = dr["MaTrang"].ToString(),
                        TenTrang = dr["TenTrang"].ToString(),
                        ChucNangID = dr["ChucNangID"],
                        MaChucNang = dr["MaChucNang"].ToString(),
                        TenChucNang = dr["TenChucNang"].ToString(),
                        QuyenID = dr["QuyenID"] == DBNull.Value ? (object)null : dr["QuyenID"]
                    });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetTrangChucNangList: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetEffectiveByTaiKhoan(int taiKhoanId)
        {
            if (!KiemTraAdmin(out object loi)) return loi;
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_Quyen_GetEffectiveByTaiKhoan", pars);
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new
                    {
                        QuyenID = dr["QuyenID"],
                        GiaTri = dr["GiaTri"].ToString(),
                        DataScope = dr["DataScope"] == DBNull.Value ? null : dr["DataScope"].ToString(),
                        TuVaiTro = Convert.ToInt32(dr["TuVaiTro"]) == 1
                    });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetEffectiveByTaiKhoan: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object SaveData(string taiKhoanIdsJson, string grantsJson, string revokeQuyenIdsJson, string denyGrantsJson,
            int? vaiTroNhomQuyenId, int? phongBanIdChoVaiTro, string thoiHanLoai, int? soGioTuyChinh)
        {
            if (!KiemTraAdmin(out object loi)) return loi;

            try
            {
                var serializer = new JavaScriptSerializer();
                List<int> taiKhoanIds = serializer.Deserialize<List<int>>(taiKhoanIdsJson ?? "[]");
                List<GrantItem> grants = serializer.Deserialize<List<GrantItem>>(grantsJson ?? "[]");
                List<GrantItem> denies = serializer.Deserialize<List<GrantItem>>(denyGrantsJson ?? "[]");
                List<int> revokeQuyenIds = serializer.Deserialize<List<int>>(revokeQuyenIdsJson ?? "[]");

                if (taiKhoanIds == null || taiKhoanIds.Count == 0)
                    return new { success = false, message = "Vui lòng chọn ít nhất 1 Tài khoản!" };

                DateTime? denNgay = null;
                if (thoiHanLoai == "8h") denNgay = DateTime.UtcNow.AddMinutes(5);//.AddHours(8);
                else if (thoiHanLoai == "custom" && soGioTuyChinh.HasValue && soGioTuyChinh.Value > 0)
                    denNgay = DateTime.UtcNow.AddHours(soGioTuyChinh.Value);

                long nguoiCapId = PermissionHelper.GetCurrentUserId();
                string ip = System.Web.HttpContext.Current.Request.UserHostAddress;//xem log
                ConnectServer db = new ConnectServer();

                foreach (var taiKhoanId in taiKhoanIds)
                {
                    if (grants != null)
                    {
                        foreach (var g in grants)
                        {
                            var pars = new Dictionary<string, object>
                            {
                                { "@TaiKhoanID", taiKhoanId }, { "@QuyenID", g.QuyenID }, { "@DataScope", g.DataScope },
                                { "@NguoiCapID", nguoiCapId },
                                { "@DenNgay", denNgay.HasValue ? (object)denNgay.Value : DBNull.Value },
                                { "@GiaTri", "ALLOW" },
                                { "@IP", ip }
                            };
                            db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_Quyen_Grant", pars);
                        }
                    }

                    if (denies != null)
                    {
                        foreach (var d in denies)
                        {
                            var pars = new Dictionary<string, object>
                            {
                                { "@TaiKhoanID", taiKhoanId }, { "@QuyenID", d.QuyenID }, { "@DataScope", d.DataScope },
                                { "@NguoiCapID", nguoiCapId },
                                { "@DenNgay", denNgay.HasValue ? (object)denNgay.Value : DBNull.Value },
                                { "@GiaTri", "DENY" },
                                { "@IP", ip }
                            };
                            db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_Quyen_Grant", pars);
                        }
                    }

                    if (vaiTroNhomQuyenId.HasValue && vaiTroNhomQuyenId.Value > 0)
                    {
                        var dsNv = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetNhanVienID",
                            new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanId } });
                        if (dsNv.Tables.Count > 0 && dsNv.Tables[0].Rows.Count > 0)
                        {
                            long nhanVienId = Convert.ToInt64(dsNv.Tables[0].Rows[0]["NhanVienID"]);
                            var ganPars = new Dictionary<string, object>
                            {
                                { "@TaiKhoanID", taiKhoanId }, { "@NhanVienID", nhanVienId },
                                { "@NhomQuyenID", vaiTroNhomQuyenId.Value },
                                { "@PhongBanID", phongBanIdChoVaiTro.HasValue ? (object)phongBanIdChoVaiTro.Value : DBNull.Value },
                                { "@NguoiThucHienID", nguoiCapId },
                                { "@IP", ip }
                            };
                            db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GanVaiTro", ganPars);
                        }
                    }
                }

                /*if (revokeQuyenIds != null && revokeQuyenIds.Count > 0 && taiKhoanIds.Count == 1)
                {
                    foreach (var quyenId in revokeQuyenIds)
                    {
                        var pars = new Dictionary<string, object> { { "@TaiKhoanID", taiKhoanIds[0] }, { "@QuyenID", quyenId } };
                        db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_Quyen_Revoke", pars);
                    }
                }*/
                if (revokeQuyenIds != null && revokeQuyenIds.Count > 0 && taiKhoanIds.Count == 1)
                {
                    foreach (var quyenId in revokeQuyenIds)
                    {
                        var pars = new Dictionary<string, object>
                        {
                            { "@TaiKhoanID", taiKhoanIds[0] },
                            { "@QuyenID", quyenId },
                            { "@NguoiThucHienID", nguoiCapId },
                            { "@IP", ip }
                        };
                        db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_Quyen_Revoke", pars);
                    }
                }

                return new { success = true, message = "Cập nhật phân quyền thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetVaiTroOptions()
        {
            if (!KiemTraAdmin(out object loi)) return loi;
            try
            {
                ConnectServer db = new ConnectServer();
                // Admin không bị giới hạn cấp bậc -> truyền NULL để lấy toàn bộ Vai trò
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_TaiKhoan_GetVaiTroOptions",
                    new Dictionary<string, object> { { "@MinCapBacTuongUng", DBNull.Value } });
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new { NhomQuyenID = dr["NhomQuyenID"], TenNhomQuyen = dr["TenNhomQuyen"].ToString() });
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetVaiTroOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}