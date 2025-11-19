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
        bool CreateMainAccount(string accountId, out string message);

        //xoá tài khoản chính
        bool DeleteMainAccount(string accountId, out string message);

        //tạo tài khoản con
        bool CreateSubAccount(string accountId, SubAccount subAccount, out string message);

        //lấy ra tài khoản chính
        Account GetAccount(string accountId);
    }
}
