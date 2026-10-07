using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class chi_tiet_gd : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(chi_tiet_gd));

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private static bool LayThongTinDuAn(long duAnId, out long phongBanId, out long chiNhanhId, out long? nguoiTaoId)
        {
            phongBanId = 0; chiNhanhId = 0; nguoiTaoId = null;
            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_GetPhongBanChiNhanhID",
                new Dictionary<string, object> { { "@DuAnID", duAnId } });
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return false;

            DataRow dr = ds.Tables[0].Rows[0];
            phongBanId = dr["PhongBanID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["PhongBanID"]);
            chiNhanhId = dr["ChiNhanhID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ChiNhanhID"]);
            nguoiTaoId = dr["NguoiTaoID"] == DBNull.Value ? (long?)null : Convert.ToInt64(dr["NguoiTaoID"]);
            return true;
        }

        // Người đang có quyền duyệt đúng bước hiện tại của dự án cũng được xem Timeline
        private static bool LaNguoiDuyetHienTai(long duAnId)
        {
            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_KiemTraQuyenDuyet",
                new Dictionary<string, object> { { "@DuAnID", duAnId }, { "@TaiKhoanID", GetCurrentUserId() } });
            return ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0
                && Convert.ToBoolean(ds.Tables[0].Rows[0]["CoQuyenDuyet"]);
        }

        // maChucNang: ChucNang.R (xem) hoặc ChucNang.U (sửa)
        private static bool CoQuyen(string maChucNang, long duAnId, out string err)
        {
            err = "";
            long phongBanId, chiNhanhId;
            long? nguoiTaoId;
            if (!LayThongTinDuAn(duAnId, out phongBanId, out chiNhanhId, out nguoiTaoId))
            {
                err = "Không tìm thấy Dự án.";
                return false;
            }

            if (CheckPermission(Trang.DA, maChucNang, phongBanId, chiNhanhId, nguoiTaoId))
                return true;

            if (maChucNang == ChucNang.R && LaNguoiDuyetHienTai(duAnId))
                return true;

            err = maChucNang == ChucNang.R
                ? "Bạn không có quyền xem Timeline của Dự án này!"
                : "Bạn không có quyền chỉnh sửa Timeline của Dự án này!";
            return false;
        }

        private static bool LayDuAnIdCuaGiaiDoan(long duAnGiaiDoanId, out long duAnId)
        {
            duAnId = 0;
            ConnectServer db = new ConnectServer();
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_GetById",
                new Dictionary<string, object> { { "@DuAnGiaiDoanID", duAnGiaiDoanId } });
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return false;
            duAnId = Convert.ToInt64(ds.Tables[0].Rows[0]["DuAnID"]);
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(long duAnId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string err;
            if (!CoQuyen(ChucNang.R, duAnId, out err))
                return new { success = false, message = err };

            try
            {
                string errSua;
                bool canEdit = CoQuyen(ChucNang.U, duAnId, out errSua);

                ConnectServer db = new ConnectServer();
                db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_EnsureDefault", new Dictionary<string, object> { { "@DuAnID", duAnId } });

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_GetList", new Dictionary<string, object> { { "@DuAnID", duAnId } });
                DataTable dt = ds.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new
                    {
                        DuAnGiaiDoanID = dr["DuAnGiaiDoanID"],
                        MaGiaiDoan = dr["MaGiaiDoan"].ToString(),
                        TenGiaiDoan = dr["TenGiaiDoan"].ToString(),
                        ThuTuHienThi = dr["ThuTuHienThi"],
                        KeHoachBatDau = dr["KeHoachBatDau"] == DBNull.Value ? null : (DateTime?)dr["KeHoachBatDau"],
                        KeHoachKetThuc = dr["KeHoachKetThuc"] == DBNull.Value ? null : (DateTime?)dr["KeHoachKetThuc"],
                        ThucTeBatDau = dr["ThucTeBatDau"] == DBNull.Value ? null : (DateTime?)dr["ThucTeBatDau"],
                        ThucTeKetThuc = dr["ThucTeKetThuc"] == DBNull.Value ? null : (DateTime?)dr["ThucTeKetThuc"],
                        TienDo = dr["TienDo"],
                        TrangThai = Convert.ToByte(dr["TrangThai"]),
                        GhiChu = dr["GhiChu"] == DBNull.Value ? "" : dr["GhiChu"].ToString()
                    });
                }

                return new { success = true, data = list, canEdit = canEdit };
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

            try
            {
                ConnectServer db = new ConnectServer();
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_GetById",
                    new Dictionary<string, object> { { "@DuAnGiaiDoanID", id } });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return new { success = false, message = "Không tìm thấy bản ghi." };

                DataRow dr = ds.Tables[0].Rows[0];

                // Kiểm tra quyền xem theo dự án THẬT của giai đoạn này (không tin ID từ trình duyệt)
                string err;
                if (!CoQuyen(ChucNang.R, Convert.ToInt64(dr["DuAnID"]), out err))
                    return new { success = false, message = err };

                var data = new
                {
                    DuAnGiaiDoanID = dr["DuAnGiaiDoanID"],
                    DuAnID = dr["DuAnID"],
                    KeHoachBatDau = dr["KeHoachBatDau"] == DBNull.Value ? null : (DateTime?)dr["KeHoachBatDau"],
                    KeHoachKetThuc = dr["KeHoachKetThuc"] == DBNull.Value ? null : (DateTime?)dr["KeHoachKetThuc"],
                    ThucTeBatDau = dr["ThucTeBatDau"] == DBNull.Value ? null : (DateTime?)dr["ThucTeBatDau"],
                    ThucTeKetThuc = dr["ThucTeKetThuc"] == DBNull.Value ? null : (DateTime?)dr["ThucTeKetThuc"],
                    TienDo = dr["TienDo"],
                    TrangThai = dr["TrangThai"],
                    GhiChu = dr["GhiChu"] == DBNull.Value ? "" : dr["GhiChu"].ToString()
                };
                return new { success = true, data = data };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetById: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object SaveData(long duAnGiaiDoanId, long duAnId, string keHoachBatDau, string keHoachKetThuc,
                                       string thucTeBatDau, string thucTeKetThuc, decimal tienDo, byte trangThai, string ghiChu)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            // Giai đoạn phải thuộc đúng dự án mà trình duyệt khai báo, nếu không là gửi ID giả
            long duAnIdThat;
            if (!LayDuAnIdCuaGiaiDoan(duAnGiaiDoanId, out duAnIdThat))
                return new { success = false, message = "Không tìm thấy Giai đoạn của Dự án." };
            if (duAnIdThat != duAnId)
                return new { success = false, message = "Dữ liệu không hợp lệ!" };

            string err;
            if (!CoQuyen(ChucNang.U, duAnIdThat, out err))
                return new { success = false, message = err };

            if (tienDo < 0 || tienDo > 100)
                return new { success = false, message = "Tiến độ phải trong khoảng 0 - 100!" };
            if (trangThai > 2)
                return new { success = false, message = "Trạng thái không hợp lệ!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@DuAnGiaiDoanID", duAnGiaiDoanId },
                    { "@KeHoachBatDau", string.IsNullOrEmpty(keHoachBatDau) ? DBNull.Value : (object)DateTime.Parse(keHoachBatDau) },
                    { "@KeHoachKetThuc", string.IsNullOrEmpty(keHoachKetThuc) ? DBNull.Value : (object)DateTime.Parse(keHoachKetThuc) },
                    { "@ThucTeBatDau", string.IsNullOrEmpty(thucTeBatDau) ? DBNull.Value : (object)DateTime.Parse(thucTeBatDau) },
                    { "@ThucTeKetThuc", string.IsNullOrEmpty(thucTeKetThuc) ? DBNull.Value : (object)DateTime.Parse(thucTeKetThuc) },
                    { "@TienDo", tienDo },
                    { "@TrangThai", trangThai },
                    { "@GhiChu", string.IsNullOrEmpty(ghiChu) ? DBNull.Value : (object)ghiChu.Trim() }
                };

                db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_Save", pars);
                db.ExecuteDatasetStoredProcedure("sp_v2_DuAn_CapNhatTienDoTuGiaiDoan", new Dictionary<string, object> { { "@DuAnID", duAnIdThat } });
                return new { success = true, message = "Cập nhật tiến độ thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object Sync(long duAnId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string err;
            if (!CoQuyen(ChucNang.U, duAnId, out err))
                return new { success = false, message = err };

            try
            {
                ConnectServer db = new ConnectServer();
                db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_EnsureDefault", new Dictionary<string, object> { { "@DuAnID", duAnId } });
                return new { success = true, message = "Đã đồng bộ Timeline theo Danh mục Giai đoạn mới nhất!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi Sync: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }
    }
}