using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Configuration;

namespace VTT.libs
{
    public class ConnectServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectServer));
        private readonly string connectionString = GetConnectionString();

        private static string GetConnectionString()
        {
            string rawConnectionString;
            var connection = WebConfigurationManager.ConnectionStrings["sqlconnection"];
            if (connection != null && !string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                rawConnectionString = connection.ConnectionString;
            }
            else
            {
                var appSetting = WebConfigurationManager.AppSettings["sqlconnection"];
                if (!string.IsNullOrWhiteSpace(appSetting))
                {
                    rawConnectionString = appSetting;
                }
                else
                {
                    throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'sqlconnection' trong Web.config.");
                }
            }

            var builder = new SqlConnectionStringBuilder(rawConnectionString);
            if (RunningInContainer() && IsLocalSqlServer(builder.DataSource))
            {
                builder.DataSource = "172.17.0.1,1433";
            }

            return builder.ConnectionString;
        }

        private static bool RunningInContainer()
        {
            if (File.Exists("/.dockerenv"))
            {
                return true;
            }

            try
            {
                string cgroup = File.ReadAllText("/proc/self/cgroup");
                return cgroup.Contains("docker") || cgroup.Contains("kubepods") || cgroup.Contains("containerd");
            }
            catch
            {
                return false;
            }
        }

        private static bool IsLocalSqlServer(string dataSource)
        {
            if (string.IsNullOrWhiteSpace(dataSource))
            {
                return false;
            }

            var normalized = dataSource.Trim().ToLowerInvariant();
            return normalized == "."
                || normalized == "(local)"
                || normalized == "localhost"
                || normalized == "127.0.0.1"
                || normalized.StartsWith("127.0.0.1,")
                || normalized.StartsWith("localhost,")
                || normalized.StartsWith(".,")
                || normalized.StartsWith("(local),");
        }

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

        /// <summary>
        /// Thực thi Stored Procedure trả về DataSet
        /// </summary>
        public DataSet ExecuteDatasetStoredProcedure(string spName, Dictionary<string, object> parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure; // Khai báo là Stored Procedure
                        cmd.CommandTimeout = 60;

                        if (parameters != null)
                        {
                            foreach (var p in parameters)
                            {
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
                log.Error("Lỗi ExecuteDatasetStoredProcedure: " + spName, ex);
                throw;
            }
            return ds;
        }
    }
}