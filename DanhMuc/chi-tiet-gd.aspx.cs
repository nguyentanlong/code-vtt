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

        private static bool CanEditProject(long duAnId, out string errorMessage)
        {
            errorMessage = "";
            ConnectServer db = new ConnectServer();
            var pars = new Dictionary<string, object> { { "@DuAnID", duAnId } };
            DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_GetPhongBanID", pars);

            long targetPhongBanId = 0;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["PhongBanID"] != DBNull.Value)
            {
                targetPhongBanId = Convert.ToInt64(ds.Tables[0].Rows[0]["PhongBanID"]);
            }
            else
            {
                errorMessage = "Không tìm thấy Dự án.";
                return false;
            }

            if (!CanEdit(targetPhongBanId))
            {
                errorMessage = "Bạn không có quyền chỉnh sửa Timeline của Dự án thuộc Phòng ban này!";
                return false;
            }
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(long duAnId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();

                // Đảm bảo mọi giai đoạn trong Danh mục đã được khởi tạo cho dự án này
                db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_EnsureDefault", new Dictionary<string, object> { { "@DuAnID", duAnId } });

                var pars = new Dictionary<string, object> { { "@DuAnID", duAnId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_GetList", pars);
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

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@DuAnGiaiDoanID", id } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_long_DuAnGiaiDoan_GetById", pars);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
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
                return new { success = false, message = "Không tìm thấy bản ghi." };
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

            string err;
            if (!CanEditProject(duAnId, out err))
            {
                return new { success = false, message = err };
            }

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
            if (!CanEditProject(duAnId, out err))
            {
                return new { success = false, message = err };
            }

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