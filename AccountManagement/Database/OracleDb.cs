using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace AccountManagement.Database
{
    public static class OracleDb
    {
        private static string _connectionString =
            "User Id=truong;Password=123;Data Source=localhost:1521/ORCLPDB";

        // Hàm trả về kết nối Oracle
        public static IDbConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }
    
    }
}
