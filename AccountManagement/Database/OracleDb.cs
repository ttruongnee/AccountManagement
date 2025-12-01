using AccountManagement.Utils;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace AccountManagement.Database
{
    public static class OracleDb
    {
        // Lấy connection string đã giải mã từ appsettings.json
        private static readonly string _connectionString =
            ConfigurationHelper.GetDecryptedConnectionString();

        // Hàm trả về kết nối Oracle
        public static IDbConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }

    }
}
