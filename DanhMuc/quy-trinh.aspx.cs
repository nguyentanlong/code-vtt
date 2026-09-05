using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class quy_trinh : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(quy_trinh));

        protected void Page_Load(object sender, EventArgs e) { }

        [WebMethod(EnableSession = true)]
        public static object GetPermission()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            bool canEdit = CheckPermission(Trang.QT, ChucNang.I, GetCurrentPhongBanId(), GetCurrentChiNhanhId());
            return new { success = true, data = new { canEdit = canEdit } };
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(string keyword, string trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (GetPermissionScope(Trang.QT, ChucNang.R) == null)
                return new { success = false, message = "Bạn không có quyền xem Danh mục Quy trình!" };

            long congTyId = GetCurrentCongTyId();
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@CongTyID", congTyId },
                    { "@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword },
                    { "@TrangThai", string.IsNullOrEmpty(trangThai) ? DBNull.Value : (object)Convert.ToByte(trangThai) }
                };

                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_QuyTrinh_GetList", pars);
                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new
                    {
                        QuyTrinhID = dr["QuyTrinhID"],
                        MaQuyTrinh = dr["MaQuyTrinh"].ToString(),
                        TenQuyTrinh = dr["TenQuyTrinh"].ToString(),
                        LoaiDoiTuong = dr["LoaiDoiTuong"].ToString(),
                        SoBuoc = dr["SoBuoc"],
                        TrangThai = Convert.ToByte(dr["TrangThai"])
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
                var pars = new Dictionary<string, object> { { "@QuyTrinhID", id } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_QuyTrinh_GetById", pars);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        QuyTrinhID = dr["QuyTrinhID"],
                        MaQuyTrinh = dr["MaQuyTrinh"].ToString(),
                        TenQuyTrinh = dr["TenQuyTrinh"].ToString(),
                        LoaiDoiTuong = dr["LoaiDoiTuong"].ToString(),
                        TrangThai = dr["TrangThai"]
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
        public static object SaveData(long quyTrinhId, string maQuyTrinh, string tenQuyTrinh, string loaiDoiTuong, byte trangThai)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            string maChucNang = quyTrinhId == 0 ? ChucNang.I : ChucNang.U;
            if (!CheckPermission(Trang.QT, maChucNang, GetCurrentPhongBanId(), GetCurrentChiNhanhId()))
                return new { success = false, message = "Bạn không có quyền thêm/sửa Quy trình! Chỉ Admin được phép." };

            long congTyId = GetCurrentCongTyId();
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@QuyTrinhID", quyTrinhId },
                    { "@CongTyID", congTyId },
                    { "@MaQuyTrinh", maQuyTrinh.Trim() },
                    { "@TenQuyTrinh", tenQuyTrinh.Trim() },
                    { "@LoaiDoiTuong", loaiDoiTuong },
                    { "@TrangThai", trangThai }
                };
                db.ExecuteDatasetStoredProcedure("sp_v2_QuyTrinh_Save", pars);
                return new { success = true, message = quyTrinhId == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!" };
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

            if (!CheckPermission(Trang.QT, ChucNang.D, GetCurrentPhongBanId(), GetCurrentChiNhanhId()))
                return new { success = false, message = "Bạn không có quyền xóa Quy trình! Chỉ Admin được phép." };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@QuyTrinhID", id } };
                db.ExecuteDatasetStoredProcedure("sp_v2_QuyTrinh_Delete", pars);
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