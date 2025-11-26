using System.Collections.Generic;
using Dapper;
using AccountManagement.Database;
using AccountManagement.Models;
using System.Linq;

namespace AccountManagement.Repositories
{
    public class SubAccountRepository
    {
        public Dictionary<decimal, SubAccount> GetByAccountId(string accountId)
        {
            using (var conn = OracleDb.GetConnection())
            {
                string sql = "select * from sub_accounts where account_id = :account_id";
                var subs = conn.Query(sql, new { account_id = accountId.ToUpper() });

                var dict = new Dictionary<decimal, SubAccount>();
                foreach (var sub in subs)
                {
                    dict.Add(sub.SUB_ID, new SubAccount(sub.ACCOUNT_ID, sub.NAME, sub.TYPE, sub.BALANCE));
                }
                return dict;
            }
        }

        public SubAccount GetBySubAccountId(decimal sub_id)
        {
            using (var conn = OracleDb.GetConnection())
            {
                string sql = "select * from sub_accounts where sub_id = :sub_id";
                var sub = conn.Query<SubAccount>(sql, new { sub_id });
                
                return sub.FirstOrDefault();
            }
        }

        public bool CreateSubAccount(SubAccount subAccount)
        {
            using (var conn = OracleDb.GetConnection())
            {
                string sql = "insert into sub_accounts(name, account_id, balance, type) VALUES (:Name, :Account_Id, :Balance, :Type)";
                var result = conn.Execute(sql, subAccount);  //tên property trong class phải trùng với tên parameter k thì phải new 1 đối tượng mới

                return result > 0;
            }
        }

        public bool UpdateSubAccount(SubAccount subAccount)
        {
            using (var conn = OracleDb.GetConnection())
            {
                string sql = "update sub_accounts set name = :Name, balance = :Balance, type = :Type where sub_id = :Sub_Id";
                var result = conn.Execute(sql, subAccount);

                return result > 0;
            }
        }

        
        public bool DeleteSubAccount(decimal subId)
        {
            using (var conn = OracleDb.GetConnection())
            {
                string sql = "delete from sub_accounts where sub_id = :subId";
                var result = conn.Execute(sql, new { subId });

                return result > 0;  
            }
        }
    }
}
