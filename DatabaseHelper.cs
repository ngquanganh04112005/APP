using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace TBUProject
{
    public static class DatabaseHelper
    {
        // Mặc định kết nối SQL Server local instance (Windows Authentication)
        private static readonly string ConnectionString = "Server=.\\SQLEXPRESS;Database=QLDoAn_CNTT;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // Thực thi truy vấn SELECT và trả về DataTable
        public static DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Thực thi INSERT, UPDATE, DELETE và trả về số dòng bị ảnh hưởng
        public static int ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // Thực thi truy vấn trả về giá trị đơn lẻ (Ví dụ: COUNT, MAX)
        public static object? ExecuteScalar(string query, SqlParameter[]? parameters = null)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
