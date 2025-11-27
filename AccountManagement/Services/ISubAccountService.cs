using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public interface ISubAccountService
    {
        //lấy ra dict subaccounts theo account_id
        Dictionary<decimal, SubAccount> GetByAccountId(string accountId);

        //lấy ra subaccount theo sub_id
        SubAccount GetBySubAccountId(decimal sub_id);

        //lấy ra string kiểu tài khoản để hiển thị
        string GetSubAccountType(string type);

        //thêm
        bool CreateSubAccount(SubAccount subAccount, out string message);
        
        //xoá
        bool DeleteSubAccount(decimal subId, out string message);


        //gửi tiền
        bool Deposit(decimal subId, double amount, out string message);
        
        //rút tiền
        bool Withdraw(decimal subId, double amount, out string message);

        //trả lãi
        bool PayInterest(decimal subId, out string message);
    }
}
