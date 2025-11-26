using AccountManagement.Services;
using AccountManagement.Ultils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Handlers
{
    public class AccountHandlers
    {
        private readonly IAccountService _accountService;
        private readonly ISubAccountService _subAccountService;


        public AccountHandlers(IAccountService accountService, ISubAccountService subAccountService)
        {
            _accountService = accountService;
            _subAccountService = subAccountService;
        }

        public void CreateAccount()
        {
            string id = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ").ToUpper();
            if (_accountService.CreateAccount(id, out string msg))
                Console.WriteLine(msg);
            else
                Console.WriteLine("Lỗi: " + msg);
        }

        public void DeleteAccount()
        {
            string id = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính cần xóa: ").ToUpper();
            if (_accountService.DeleteAccount(id, out string msg))
                Console.WriteLine(msg);
            else
                Console.WriteLine("Lỗi: " + msg);
        }

        public void PrintAllAccounts()
        {
            var dict = _accountService.GetAllAccounts();
            if (dict.Count == 0) { Console.WriteLine("Không có tài khoản."); return; }
            foreach (var acc in dict)
            {
                Console.WriteLine($"\n=== Tài khoản chính: {acc.Key.ToUpper()} ===");
                var subaccs = _subAccountService.GetByAccountId(acc.Key);
                foreach (var s in subaccs)
                {
                    Console.WriteLine($"- {s.Value.Name}\t | Loại: {_subAccountService.GetSubAccountType(s.Value.Type)}\t | Số dư: {s.Value.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ");
                }
            }
        }
    }
}
