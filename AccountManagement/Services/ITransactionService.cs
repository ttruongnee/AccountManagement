using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public interface ITransactionService
    {
        //gửi tiền
        bool Deposit(string accountId, string subId, double amount, out string message);
        
        //rút tiền
        bool Withdraw(string accountId, string subId, double amount, out string message);

        //trả lãi
        bool PayInterest(string accountId, string subId, out string message);
    }
}
