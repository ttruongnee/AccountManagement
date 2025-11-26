using AccountManagement.Models;
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
    public class SubAccountHandlers
    {
        private readonly IAccountService _accountService;
        private readonly ISubAccountService _subAccountService;

        public SubAccountHandlers(IAccountService accountService, ISubAccountService subAccountService)
        {
            _accountService = accountService;
            _subAccountService = subAccountService;
        }

        public void CreateSubAccount()
        {
            string accountId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ").ToUpper();
            //kiểm tra tồn tại tài khoản cha
            var acc = _accountService.GetAccountById(accountId);
            if (acc == null)
            {
                Console.WriteLine($"Tài khoản {accountId} không tồn tại!");
                return;
            }

            Console.WriteLine("Chọn loại: 1.Tiết kiệm  2.Đầu tư");
            int choice = InputHelper.ReadIntInRange("Loại: ", 1, 2);
            string type = choice == 1 ? "TK" : "ĐT";

            string name = InputHelper.ReadNonEmpty("Nhập tài khoản con: ");

            Console.Write("Số dư khởi tạo (nhập 0 nếu không): ");
            double.TryParse(Console.ReadLine(), out double initial);

            SubAccount new_sub = new SubAccount(accountId, name.ToUpper(), type, initial);

            //tạo tài khoản con
            _subAccountService.CreateSubAccount(new_sub, out var message);
            Console.WriteLine(message);
        }

        public void DeleteSubAccount()
        {
            string account_id = InputHelper.ReadNonEmpty("Nhập id tài khoản cha: ");
            var subaccs = _subAccountService.GetByAccountId(account_id);

            foreach (var s in subaccs)
            {
                Console.WriteLine($"- ID: {s.Key}\t Mã: {s.Value.Name}\t | Loại: {_subAccountService.GetSubAccountType(s.Value.Type)}\t | Số dư: {s.Value.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ");

            }

            double id = InputHelper.ReadPositiveDouble("Nhập ID tài khoản con cần xóa: ");
            if (_subAccountService.DeleteSubAccount((decimal)id, out string msg))
                Console.WriteLine(msg);
            else
                Console.WriteLine("Lỗi: " + msg);
        }
    }
}
