using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DongYWeb.libs
{
    public class libs
    {
        ILog log = log4net.LogManager.GetLogger(typeof(libs));
        static ILog log1 = log4net.LogManager.GetLogger(typeof(libs));
        /// <summary>
        /// chuyen chuoi nhap vao ra 1 chuoi HEX lien tiep
        /// </summary>
        /// <param name="_strA"> chuoi can chuyen</param>
        /// <returns></returns>
        public string Encoding(string _strA)
        {
            log.Info("libs => Encoding");

            string hex = null;
            foreach (char c in _strA)
            {
                int tmp = c;
                hex += String.Format("{0:x0}", (uint)Convert.ToUInt32(tmp.ToString()) + (ulong)Convert.ToChar(tmp));
            }
            _strA = hex;
            hex = null;
            foreach (char c in _strA)
            {
                int tmp = c;
                hex += String.Format("{0:x0}", (uint)Convert.ToUInt32(tmp.ToString()) + (ulong)Convert.ToChar(tmp));
            }
            log.Info("libs => Encoding => " + hex);
            return hex;
        }

        /// <summary>
        /// lay thang nam ngay khoi tao co thoi gian
        /// </summary>
        /// <returns></returns>
        public string YMDDate()
        {
            log.Info("libs => YMDDate");
            return DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" +
                DateTime.Now.Day.ToString() + " " + DateTime.Now.Hour.ToString() + ":" +
                DateTime.Now.Minute.ToString() + ":000";
        }

        public string DataTableToJsonObj(DataTable dt)
        {
            log.Info("libs => DataTableToJsonObj");
            DataSet ds = new DataSet();
            ds.Merge(dt);
            StringBuilder JsonString = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        // Lấy giá trị ra, xóa sạch các ký tự \r và \n
                        string cellValue = ds.Tables[0].Rows[i][j].ToString()
                                             .Replace("\r", "")
                                             .Replace("\n", "");

                        // Nếu muốn thay thế xuống dòng bằng dấu phẩy hoặc khoảng trắng để dễ đọc:
                        // string cellValue = ds.Tables[0].Rows[i][j].ToString().Replace("\r\n", " ").Replace("\n", " ");

                        if (j < ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + cellValue + "\",");
                        }
                        else if (j == ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + cellValue + "\"");
                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                log.Info("libs => DataTableToJsonObj => " + JsonString);
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }

        public string DatasetToJsonObj(DataSet ds)
        {
            log.Info("libs => DatasetToJsonObj");

            StringBuilder JsonString = new StringBuilder();
            for (int k = 0; k < ds.Tables.Count; k++)
            {
                if (ds != null)
                {
                    JsonString.Append("[");
                    for (int i = 0; i < ds.Tables[k].Rows.Count; i++)
                    {
                        JsonString.Append("{");
                        for (int j = 0; j < ds.Tables[k].Columns.Count; j++)
                        {
                            if (j < ds.Tables[k].Columns.Count - 1)
                            {
                                JsonString.Append("\"" + ds.Tables[k].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[k].Rows[i][j].ToString() + "\",");
                            }
                            else if (j == ds.Tables[k].Columns.Count - 1)
                            {
                                JsonString.Append("\"" + ds.Tables[k].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[k].Rows[i][j].ToString() + "\"");
                            }
                        }
                        if (i == ds.Tables[k].Rows.Count - 1)
                        {
                            JsonString.Append("}");
                        }
                        else
                        {
                            JsonString.Append("},");
                        }
                    }
                    JsonString.Append("]#");
                }
            }
            log.Info("libs => DataTableToJsonObj => " + JsonString);

            return JsonString.ToString();
        }

        static Regex MobileCheck = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex MobileVersionCheck = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        public static bool fBrowserIsMobile()
        {
            log1.Info("libs => DatasetToJsonObj");

            Debug.Assert(HttpContext.Current != null);

            if (HttpContext.Current.Request != null && HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                var u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString();

                if (u.Length < 4)
                    return false;

                if (MobileCheck.IsMatch(u) || MobileVersionCheck.IsMatch(u.Substring(0, 4)))
                    return true;
            }
            log1.Info("libs => DatasetToJsonObj => false");

            return false;
        }

        public static bool checkQuyen(DataSet _dtsQuyen, string nameCol, string strLoai)
        {
            log1.Info("checkQuyen");
            int tableCount = _dtsQuyen.Tables.Count;
            string[] duocPhepArr = new string[tableCount-1];

            for (int i = 0; i < tableCount - 1; i++)
            {
                var rows = _dtsQuyen.Tables[i].AsEnumerable()
                    .Where(r => r.Field<string>(nameCol)?.Contains(strLoai) == true)
                    .ToList();

                if (rows.Any())
                    duocPhepArr[i] = rows[0]["DuocPhep"]?.ToString();
                else
                    duocPhepArr[i] = null; // không tìm thấy
            }

            // ----- XỬ LÝ LOGIC -----

            // Nếu cả 2 đều không có dữ liệu ⇒ coi như không được phép
            if (duocPhepArr.All(x => x == null))
                return false;

            // Nếu cả 2 = 1 → không (vì bạn return false)
            if (duocPhepArr.All(x => x == "1"))
                return false;

            // Nếu cả 2 = 0 → không
            if (duocPhepArr.All(x => x == "0"))
                return false;

            // Nếu 1 bảng 0, bảng kia 1 → return true
            return true;
        }

        public static string randomVersion()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() +
                DateTime.Now.Millisecond.ToString();
        }

        public static string RenderJS(params string[] files)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string file in files)
            {
                sb.AppendLine(
                    $"<script src='{file}?v={randomVersion()}'></script>"
                );
            }

            return sb.ToString();
        }

        public static string RenderCSS(params string[] files)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string file in files)
            {
                sb.AppendLine(
                    $"<link rel='stylesheet' href='{file}?v={randomVersion()}' />"
                );
            }

            return sb.ToString();
        }
    }
}