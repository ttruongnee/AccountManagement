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
            if (subaccs.Count == 0)
            {
                Console.WriteLine($"Tài khoản {account_id} có tài khoản con");
                return;
            }

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

        public void Deposit()
        {
            string account_id = InputHelper.ReadNonEmpty("Nhập id tài khoản cha: ");
            var subaccs = _subAccountService.GetByAccountId(account_id);
            if (subaccs.Count == 0)
            {
                Console.WriteLine($"Tài khoản {account_id} có tài khoản con");
                return;
            }
            foreach (var s in subaccs)
            {
                Console.WriteLine($"- ID: {s.Key}\t Mã: {s.Value.Name}\t | Loại: {_subAccountService.GetSubAccountType(s.Value.Type)}\t | Số dư: {s.Value.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ");

            }

            double id = InputHelper.ReadPositiveDouble("Nhập ID tài khoản con cần nạp: ");
            var subacc = _subAccountService.GetBySubAccountId((decimal)id);
            if (subacc == null)
            {
                Console.WriteLine($"Tài khoản con có ID {id} không tồn tại");
                return;
            }

            double amount = InputHelper.ReadPositiveDouble("Nhập số tiền cần nạp: ");
            if (_subAccountService.Deposit((decimal)id, amount, out string msg))
                Console.WriteLine(msg);
            else
                Console.WriteLine("Lỗi: " + msg);
        }

        public void Withdraw()
        {
            string account_id = InputHelper.ReadNonEmpty("Nhập id tài khoản cha: ");
            var subaccs = _subAccountService.GetByAccountId(account_id);
            if (subaccs.Count == 0)
            {
                Console.WriteLine($"Tài khoản {account_id} có tài khoản con");
                return;
            }
            foreach (var s in subaccs)
            {
                Console.WriteLine($"- ID: {s.Key}\t Mã: {s.Value.Name}\t | Loại: {_subAccountService.GetSubAccountType(s.Value.Type)}\t | Số dư: {s.Value.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ");

            }

            double id = InputHelper.ReadPositiveDouble("Nhập ID tài khoản con cần rút: ");
            var subacc = _subAccountService.GetBySubAccountId((decimal)id);
            if (subacc == null)
            {
                Console.WriteLine($"Tài khoản con có ID {id} không tồn tại");
                return;
            }

            double amount = InputHelper.ReadPositiveDouble("Nhập số tiền cần rút: ");

            if (subacc.Balance - amount < 0)
            {
                Console.WriteLine("Số dư không đủ!");
                return;
            }

            if (_subAccountService.Withdraw((decimal)id, amount, out string msg))
                Console.WriteLine(msg);
            else
                Console.WriteLine("Lỗi: " + msg);
        }

        public void ShowInterest()
        {
            string account_id = InputHelper.ReadNonEmpty("Nhập id tài khoản cha: ");
            var subaccs = _subAccountService.GetByAccountId(account_id);
            if (subaccs.Count == 0)
            {
                Console.WriteLine($"Tài khoản {account_id} có tài khoản con");
                return;
            }
            foreach (var s in subaccs)
            {
                Console.WriteLine($"- ID: {s.Key}\t Mã: {s.Value.Name}\t | Loại: {_subAccountService.GetSubAccountType(s.Value.Type)}\t | Số dư: {s.Value.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ\t Lãi: {s.Value.GetInterest().ToString("N0", new CultureInfo("vi-VN"))}đ");

            }
        }
        public void PayInterest()
        {
            ShowInterest();

            double id = InputHelper.ReadPositiveDouble("Nhập ID tài khoản con cần thanh toán lãi: ");
            var subacc = _subAccountService.GetBySubAccountId((decimal)id);
            if (subacc == null)
            {
                Console.WriteLine($"Tài khoản con có ID {id} không tồn tại");
                return;
            }

            if (_subAccountService.PayInterest((decimal)id, out string msg))
                Console.WriteLine(msg);
            else
                Console.WriteLine("Lỗi: " + msg);
        }
    }
}
