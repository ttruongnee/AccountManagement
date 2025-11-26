using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public interface IAccountService
    {
        //trả về 1 dictionary chỉ đọc chứa các account
        IReadOnlyDictionary<string, Account> GetAllAccounts();
        
        //tạo tài khoản chính
        bool CreateAccount(string accountId, out string message);

        //xoá tài khoản chính
        bool DeleteAccount(string accountId, out string message);

        //lấy ra tài khoản chính
        Account GetAccountById(string accountId);
    }
}
