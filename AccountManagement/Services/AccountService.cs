using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public class AccountService : IAccountService
    {
        private readonly Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        private readonly ILoggerService _logger;
        public AccountService(ILoggerService logger)
        {
            _logger = logger;
        }

        //trả về 1 dictionary chỉ đọc chứa các tài khoản (chính)
        public IReadOnlyDictionary<string, Account> GetAllAccounts() => _accounts;

        //nhận vào id, kiểm tra nếu 
        public Account GetAccount(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId)) return null;

            //tìm key trong _accounts, nếu tìm thấy thì gán đối tượng tương ứng cho acc, không thì gán null
            //không ném lỗi khi key không tồn tại -> an toàn hơn _accounts[key]
            _accounts.TryGetValue(accountId.Trim().ToLower(), out var acc); //trả ra kiểu bool 
            return acc;
        }

        //tạo tài khoản chính, trả về kiểu bool và message thông báo
        public bool CreateMainAccount(string accountId, out string message)
        {
            //message = "";
            if (string.IsNullOrWhiteSpace(accountId))
            {
                message = "Mã tài khoản chính không hợp lệ.";
                return false;
            }
            accountId = accountId.Trim().ToLower();
            if (_accounts.ContainsKey(accountId))
            {
                message = "Tài khoản chính đã tồn tại.";
                return false;
            }
            var acc = new Account(accountId);
            _accounts.Add(accountId, acc);
            _logger.Log(new LogEntry(accountId, null, "Tạo tài khoản chính", null, true));
            message = "Tạo tài khoản chính thành công.";
            return true;
        }

        //xoá tài khoản chính khi truyền vào id tài khoản, trả về kiểu bool và message thông báo
        public bool DeleteMainAccount(string accountId, out string message)
        {
            message = "";
            var acc = GetAccount(accountId);
            if (acc == null)
            {
                message = "Không tồn tại tài khoản.";
                return false;
            }

            //kiểm tra số dư của tất cả tài khoản con của tài khoản muốn xoá, nếu có tài khoản con > 0 thì không xoá được
            //foreach (var s in acc.SubAccounts)
            //{
            //    if (s.Balance > 0)
            //    {
            //        message = $"Không thể xóa: tài khoản con {s.SubId} còn tiền.";
            //        _logger.Log(new LogEntry(accountId, s.SubId, "Xoá tài khoản chính", null, false, "Còn tiền trong tài khoản con"));
            //        return false;
            //    }
            //}

            //dùng any để kiểm tra số dư tài khoản con
            //bool check = acc.SubAccounts.Any(s => s.Balance > 0);
            //if (check)
            //{
            //    message = $"Không thể xóa: tài khoản con còn tiền.";
            //    _logger.Log(new LogEntry(accountId, null, "Xoá tài khoản chính", null, false, "Còn tiền trong tài khoản con"));
            //    return false;
            //}

            //dùng first or default để kiểm tra số dư tài khoản con
            var check = acc.SubAccounts.FirstOrDefault(s => s.Balance > 0);
            if (check != null)
            {
                message = $"Không thể xóa: tài khoản con {check.Name} còn tiền.";
                _logger.Log(new LogEntry(accountId, check.SubId, "Xoá tài khoản chính", null, false, "Còn tiền trong tài khoản con"));
                return false;
            }

            //xoá và kiểm tra trạng thái cho chắc chắn
            bool removed = _accounts.Remove(acc.AccountId);
            if (removed)
            {
                _logger.Log(new LogEntry(acc.AccountId, null, "Xoá tài khoản chính", null, true));
                message = "Xoá tài khoản thành công.";
                return true;
            }

            message = "Xoá thất bại.";
            _logger.Log(new LogEntry(acc.AccountId, null, "Xoá tài khoản chính", null, false));
            return false;
        }

        //tạo tài khoản con, truyền vào id tài khoản chính, tài khoản con; trả về kiểu bool và message thông báo
        public bool CreateSubAccount(string accountId, SubAccount subAccount, out string message)
        {
            message = "";
            var acc = GetAccount(accountId);
            if (acc == null)
            {
                message = "Không tồn tại tài khoản chính.";
                return false;
            }

            if (subAccount == null)
            {
                message = "Tài khoản con hợp lệ chưa được cung cấp.";
                return false;
            }
            //.Any trả về true nếu tồn tại ít nhất 1 phần tử thoả mãn điều kiện trong ngoặc (LINQ)
            //truyền vào StringComparison.OrdinalIgnoreCase trong Equals để so sánh không phân biệt hoa thường
            if (acc.SubAccounts.Any(s => s.SubId.Equals(subAccount.SubId, StringComparison.OrdinalIgnoreCase)))
            {
                message = "Tài khoản con đã tồn tại.";
                _logger.Log(new LogEntry(accountId, subAccount.SubId, "Tạo tài khoản con", null, false, "Trùng ID"));
                return false;
            }

            acc.SubAccounts.Add(subAccount);
            _logger.Log(new LogEntry(accountId, subAccount.SubId, "Tạo tài khoản con", null, true));
            message = "Tạo tài khoản con thành công.";
            return true;
        }
    }
}
