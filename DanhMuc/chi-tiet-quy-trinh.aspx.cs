using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using VTT.libs;
using log4net;

namespace VTT.DanhMuc
{
    public partial class chi_tiet_quy_trinh : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(chi_tiet_quy_trinh));

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
        public static object GetNhomQuyenOptions()
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            long congTyId = GetCurrentCongTyId();
            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@CongTyID", congTyId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_BuocQuyTrinh_GetNhomQuyenOptions", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new { NhomQuyenID = dr["NhomQuyenID"], TenNhomQuyen = dr["TenNhomQuyen"].ToString() });
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetNhomQuyenOptions: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetList(long quyTrinhId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@QuyTrinhID", quyTrinhId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_BuocQuyTrinh_GetList", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new
                    {
                        BuocID = dr["BuocID"],
                        QuyTrinhID = dr["QuyTrinhID"],
                        TenBuoc = dr["TenBuoc"].ToString(),
                        ThuTu = dr["ThuTu"],
                        NhomQuyenDuyetID = dr["NhomQuyenDuyetID"] == DBNull.Value ? (object)null : dr["NhomQuyenDuyetID"],
                        TenNhomQuyen = dr["TenNhomQuyen"] == DBNull.Value ? "" : dr["TenNhomQuyen"].ToString(),
                        HanhDong = dr["HanhDong"] == DBNull.Value ? "" : dr["HanhDong"].ToString()
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
        public static object SaveData(long buocId, long quyTrinhId, string tenBuoc, int thuTu, object nhomQuyenDuyetId, string hanhDong)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (!CheckPermission(Trang.QT, ChucNang.U, GetCurrentPhongBanId(), GetCurrentChiNhanhId()))
                return new { success = false, message = "Bạn không có quyền chỉnh sửa Bước Quy trình! Chỉ Admin được phép." };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object>
                {
                    { "@BuocID", buocId },
                    { "@QuyTrinhID", quyTrinhId },
                    { "@TenBuoc", tenBuoc.Trim() },
                    { "@ThuTu", thuTu },
                    { "@NhomQuyenDuyetID", nhomQuyenDuyetId == null ? DBNull.Value : (object)Convert.ToInt32(nhomQuyenDuyetId) },
                    { "@HanhDong", string.IsNullOrEmpty(hanhDong) ? DBNull.Value : (object)hanhDong.Trim() }
                };
                db.ExecuteDatasetStoredProcedure("sp_v2_BuocQuyTrinh_Save", pars);
                return new { success = true, message = "Lưu Bước thành công!" };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveData: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetLichSu(long quyTrinhId)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@QuyTrinhID", quyTrinhId } };
                DataSet ds = db.ExecuteDatasetStoredProcedure("sp_v2_LichSuQuyTrinh_GetList", pars);

                List<object> list = new List<object>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new
                    {
                        LichSuID = dr["LichSuID"],
                        ObjectType = dr["ObjectType"].ToString(),
                        ObjectID = dr["ObjectID"],
                        TenBuoc = dr["TenBuoc"].ToString(),
                        TenNguoiXuLy = dr["TenNguoiXuLy"] == DBNull.Value ? "" : dr["TenNguoiXuLy"].ToString(),
                        KetQua = dr["KetQua"] == DBNull.Value ? (object)null : Convert.ToByte(dr["KetQua"]),
                        LyDo = dr["LyDo"] == DBNull.Value ? "" : dr["LyDo"].ToString(),
                        NgayXuLyText = dr["NgayXuLyText"].ToString()
                    });
                }
                return new { success = true, data = list };
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetLichSu: " + ex.Message, ex);
                return new { success = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteData(long id)
        {
            if (!IsAuthenticated())
                return new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" };

            if (!CheckPermission(Trang.QT, ChucNang.D, GetCurrentPhongBanId(), GetCurrentChiNhanhId()))
                return new { success = false, message = "Bạn không có quyền xóa Bước Quy trình! Chỉ Admin được phép." };

            try
            {
                ConnectServer db = new ConnectServer();
                var pars = new Dictionary<string, object> { { "@BuocID", id } };
                db.ExecuteDatasetStoredProcedure("sp_v2_BuocQuyTrinh_Delete", pars);
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