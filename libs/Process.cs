using log4net;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VTT.libs
{
    public class Process
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Process));
        /// <summary>
        /// chuyen chuoi nhap vao ra 1 chuoi HEX lien tiep
        /// </summary>
        /// <param name="_strA"> chuoi can chuyen</param>
        /// <returns></returns>
        public static string ProcessEncoding(string _strA)
        {
            string hex = null;
            foreach (char c in _strA)
            {
                int tmp = c;
                hex += String.Format("{0:x0}", (uint)System.Convert.ToUInt32(tmp.ToString()) + (ulong)System.Convert.ToChar(tmp));
            }
            _strA = hex;
            hex = null;
            foreach (char c in _strA)
            {
                int tmp = c;
                hex += String.Format("{0:x0}", (uint)System.Convert.ToUInt32(tmp.ToString()) + (ulong)System.Convert.ToChar(tmp));
            }
            return hex;
        }

        /// <summary>
        /// Gọi Stored Procedure sp_NhatKy_DangNhap để ghi log & update LastLogin
        /// </summary>
        public static void GhiNhatKyDangNhap(ConnectServer db, string loaiLog, string username, object taiKhoanId, object nhanVienId, string clientIP, string ghiChu)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "@LoaiLog", loaiLog },
            { "@Username", username },
            { "@TaiKhoanID", taiKhoanId ?? DBNull.Value },
            { "@NhanVienID", nhanVienId ?? DBNull.Value },
            { "@IPAddress", clientIP },
            { "@GhiChu", ghiChu }
        };

                db.ExecuteDatasetStoredProcedure("dbo.sp_chinh_NhatKy_DangNhap", parameters);
            }
            catch (Exception ex)
            {
                log.Error("Lỗi ghi nhật ký đăng nhập: ", ex);
            }
        }
    }
}
