using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountService _accountService;
        private readonly ILoggerService _logger;

        //hàm khởi tạo giao dịch
        public TransactionService(IAccountService accountService, ILoggerService logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public bool Deposit(string accountId, string subId, double amount, out string message)
        {
            message = "";
            try
            {
                if (amount <= 0)  //kiểm tra số tiền giao dịch <= 0
                {
                    message = "Số tiền giao dịch phải lớn hơn 0.";
                    _logger.Log(new LogEntry(accountId, subId, "Nạp tiền", amount, false, message));
                    return false;
                }

                //lấy ra tài khoản từ id truyền vào, nếu không tồn tại thì return false
                var acc = _accountService.GetAccount(accountId);
                if (acc == null) 
                { 
                    message = "Tài khoản chính không tồn tại."; 
                    _logger.Log(new LogEntry(accountId, subId, "Nạp tiền", amount, false, message)); 
                    return false; 
                }

                //dùng .Find để tìm và lấy ra phần tử thoả mãn điều kiện trong ngoặc
                //truyền vào StringComparison.OrdinalIgnoreCase trong Equals để so sánh không phân biệt hoa thường
                var sub = acc.SubAccounts.Find(s => s.SubId.Equals(subId, StringComparison.OrdinalIgnoreCase));
                if (sub == null) 
                {
                    message = "Tài khoản con không tồn tại."; 
                    _logger.Log(new LogEntry(accountId, subId, "Nạp tiền", amount, false, message)); 
                    return false; 
                }

                sub.Deposit(amount);
                _logger.Log(new LogEntry(accountId, subId, "Nạp tiền", amount, true));
                message = "Nạp tiền thành công.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi nạp tiền: " + ex.Message;
                _logger.Log(new LogEntry(accountId, subId, "Nạp tiền", amount, false, ex.Message));
                return false;
            }
        }

        public bool Withdraw(string accountId, string subId, double amount, out string message)
        {
            message = "";
            try
            {
                if (amount <= 0)
                {
                    message = "Số tiền phải lớn hơn 0.";
                    _logger.Log(new LogEntry(accountId, subId, "Rút tiền", amount, false, message));
                    return false;
                }

                var acc = _accountService.GetAccount(accountId);
                if (acc == null) 
                { 
                    message = "Tài khoản chính không tồn tại."; 
                    _logger.Log(new LogEntry(accountId, subId, "Rút tiền", amount, false, message)); 
                    return false; 
                }

                var sub = acc.SubAccounts.Find(s => s.SubId.Equals(subId, StringComparison.OrdinalIgnoreCase));
                if (sub == null) 
                { 
                    message = "Tài khoản con không tồn tại."; 
                    _logger.Log(new LogEntry(accountId, subId, "Rút tiền", amount, false, message)); 
                    return false; 
                }

                if (sub.Balance < amount)
                {
                    message = "Số dư không đủ.";
                    _logger.Log(new LogEntry(accountId, subId, "Rút tiền", amount, false, message));
                    return false;
                }

                sub.Withdraw(amount);
                _logger.Log(new LogEntry(accountId, subId, "Rút tiền", amount, true));
                message = "Rút tiền thành công.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi rút tiền: " + ex.Message;
                _logger.Log(new LogEntry(accountId, subId, "Rút tiền", amount, false, ex.Message));
                return false;
            }
        }

        public bool PayInterest(string accountId, string subId, out string message)
        {
            message = "";
            try
            {
                var acc = _accountService.GetAccount(accountId);
                if (acc == null) 
                { 
                    message = "Tài khoản chính không tồn tại."; 
                    _logger.Log(new LogEntry(accountId, subId, "Thanh toán lãi", null, false, message)); 
                    return false; 
                }

                var sub = acc.SubAccounts.Find(s => s.SubId.Equals(subId, StringComparison.OrdinalIgnoreCase));
                if (sub == null) 
                { 
                    message = "Tài khoản con không tồn tại."; 
                    _logger.Log(new LogEntry(accountId, subId, "Thanh toán lãi", null, false, message)); 
                    return false; 
                }

                double interest = sub.GetInterest();
                if (interest <= 0) 
                { 
                    message = "Không có lãi để thanh toán."; 
                    _logger.Log(new LogEntry(accountId, subId, "Thanh toán lãi", interest, false, message)); 
                    return false; 
                }

                sub.Deposit(interest); // deposit để cộng lãi
                _logger.Log(new LogEntry(accountId, subId, "Thanh toán lãi", interest, true));
                message = $"Đã thanh toán lãi {interest:N0}đ cho {sub.SubId}.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi thanh toán lãi: " + ex.Message;
                _logger.Log(new LogEntry(accountId, subId, "Thanh toán lãi", null, false, ex.Message));
                return false;
            }
        }
    }
}
