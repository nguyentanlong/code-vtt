//using log4net;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Web;
//using System.Web.Configuration;
//namespace DongYWeb.libs { public class ConnectServer { public SqlConnection sqlConnect; ILog log = log4net.LogManager.GetLogger(typeof(ConnectServer)); public void OpenServer() { log.Info("ConnectServer => OpenServer"); if (sqlConnect != null) if (sqlConnect.State == ConnectionState.Open) sqlConnect.Close(); sqlConnect = new SqlConnection(); sqlConnect.ConnectionString = WebConfigurationManager.AppSettings["sqlconnection"]; sqlConnect.Open(); } public DataTable ShowTable(string strQuery) { log.Info("ConnectServer => ShowTable " + strQuery); DataTable table = new DataTable(); OpenServer(); SqlDataAdapter adapter = new SqlDataAdapter(strQuery, sqlConnect); try { adapter.Fill(table); } catch (Exception ex) { } sqlConnect.Close(); sqlConnect.Dispose(); sqlConnect = null; return table; } public string SaveInfo(string _strStore) { log.Info("ConnectServer => SaveInfo " + _strStore); try { OpenServer(); SqlCommand command = new SqlCommand(_strStore, sqlConnect); command.ExecuteNonQuery(); sqlConnect.Close(); sqlConnect.Dispose(); sqlConnect = null; return "1"; } catch (Exception ex) { return ""; } } } }

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace VTT.libs
{
    public class ConnectServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectServer));
        private readonly string connectionString = WebConfigurationManager.AppSettings["sqlconnection"];

        /// <summary>
        /// Tạo connection mới. Lưu ý: Không để biến SqlConnection ở cấp Class để tránh rò rỉ bộ nhớ.
        /// </summary>
        private SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// SELECT query đơn giản (trả về 1 DataTable)
        /// </summary>
        public DataTable ShowTable(string strQuery)
        {
            DataTable table = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.CommandTimeout = 60;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(table);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi ShowTable: " + strQuery, ex);
                throw; // Throw để WebMethod phía trên có thể bắt được lỗi
            }
            return table;
        }

        /// <summary>
        /// INSERT / UPDATE / DELETE 
        /// </summary>
        public string SaveInfo(string strSql)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(strSql, conn))
                    {
                        cmd.CommandTimeout = 60;
                        cmd.ExecuteNonQuery();
                    }
                }
                return "1";
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveInfo: " + strSql, ex);
                return "";
            }
        }

        /// <summary>
        /// EXEC SQL trả nhiều table (Multi Result Set) - Dùng Parameter để an toàn
        /// </summary>
        public DataSet ExecuteDataset(string sql, Dictionary<string, object> parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Tăng timeout lên 120s vì thường chứa nhiều Store Procedure nặng
                        cmd.CommandTimeout = 120;

                        if (parameters != null)
                        {
                            foreach (var p in parameters)
                            {
                                // Xử lý null: Nếu giá trị truyền vào là null thì gán DBNull.Value
                                cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                            }
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi ExecuteDataset: " + sql, ex);
                throw;
            }
            return ds;
        }
    }
}