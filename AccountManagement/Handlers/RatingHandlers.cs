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
    public class RatingHandlers
    {
        private readonly IAccountService _accountService;
        private readonly ISubAccountService _subAccountService;


        public RatingHandlers(IAccountService accountService, ISubAccountService subAccountService)
        {
            _accountService = accountService;
            _subAccountService = subAccountService;
        }
        //viết code vào service xong ở đây viết hàm k có para thôi
        public void RankAccountsByBalance(IAccountService accountService)
        {
            //var dict = accountService.GetAllAccounts();
            //if (dict.Count == 0) { return; }
            //var result = AccountHelpers.GetAccountsWithTotalBalance(accountService)
            //    .OrderByDescending(d => d.TotalBalance);

            //foreach (var item in result)
            //{
            //    Console.WriteLine($"- {item.MainId.ToUpper()}\t | Tổng số dư: {item.TotalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            //}
        }

        public void Under1Million(IAccountService accountService)
        {
            //var dict = accountService.GetAllAccounts();
            //if (dict.Count == 0) { return; }
            //var result = AccountHelpers.GetAccountsWithTotalBalance(accountService)
            //    .Where(acc => acc.TotalBalance < 1_000_000);

            //foreach (var item in result)
            //{
            //    Console.WriteLine($"- {item.MainId.ToUpper()}\t | Tổng số dư: {item.TotalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            //}
        }

        public void Top3(IAccountService accountService)
        {
            //var dict = accountService.GetAllAccounts();
            //if (dict.Count == 0) { return; }
            //var result = AccountHelpers.GetAccountsWithTotalBalance(accountService)
            //    .OrderByDescending(acc => acc.TotalBalance)
            //    .Take(3);

            //foreach (var item in result)
            //{
            //    Console.WriteLine($"- {item.MainId.ToUpper()}\t | Tổng số dư: {item.TotalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            //}
        }

        public void TotalBalanceOfInvestmentAccounts(IAccountService accountService)
        {
            //var dict = accountService.GetAllAccounts();
            //if (dict.Count == 0) { return; }
            //var result = dict.SelectMany(acc => acc.Value.SubAccounts)
            //    .Where(acc => acc.Name == "Tài khoản đầu tư")
            //    .Sum(acc => acc.Balance);

            //Console.WriteLine($"Tổng số dư tài khoản đầu tư của tất cả tài khoản là {result.ToString("N0", new CultureInfo("vi-VN"))}đ");
        }
    }
}
