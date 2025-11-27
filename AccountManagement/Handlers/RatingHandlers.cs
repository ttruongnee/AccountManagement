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

        public List<(string accountId, double totalBalance)> GetAccountsWithTotalBalance()
        {
            var dict_acc = _accountService.GetAllAccounts();
            if (dict_acc.Count == 0) { return null; }

            var ranking = new List<(string accountId, double totalBalance)>();  //viết như này sẽ tạo ValueTuple<string, double>
            foreach (var account in dict_acc)
            {
                var dict_subacc = _subAccountService.GetByAccountId(account.Key);
                double TotalBalance = 0;
                if (dict_subacc != null)
                {
                    TotalBalance = dict_subacc.Values.Sum(s => s.Balance);
                }

                ranking.Add((account.Key, TotalBalance));
            }
            return ranking.OrderByDescending(a => a.totalBalance).ToList();
        }
        public void RankAccountsByBalance(List<(string accountId, double totalBalance)> ranking)
        {
            foreach (var acc in ranking)
            {
                if (ranking.Count == 0) { return; }

                Console.WriteLine($"- Tài khoản: {acc.accountId}\t Tổng số dư: {acc.totalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            }
        }

        public void Under1Million(List<(string accountId, double totalBalance)> ranking)
        {
            if (ranking.Count == 0) { return; }
            var result = ranking.Where(acc => acc.totalBalance < 1_000_000);

            foreach (var acc in result)
            {
                Console.WriteLine($"- Tài khoản: {acc.accountId}\t Tổng số dư: {acc.totalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            }
        }

        public void Top3(List<(string accountId, double totalBalance)> ranking)
        {
            if (ranking.Count == 0) { return; }
            var result = ranking.Take(3);

            foreach (var acc in result)
            {
                Console.WriteLine($"- Tài khoản: {acc.accountId}\t Tổng số dư: {acc.totalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            }
        }

        public void TotalBalanceOfInvestmentAccounts()
        {
            var dict_subacc = _subAccountService.GetAllSubAccounts();
            double totalBalance = dict_subacc.Values
                                    .Where(s => s.Type.Equals("DT"))
                                    .Sum(s => s.Balance);

            Console.WriteLine($"Tổng số dư tài khoản đầu tư của tất cả tài khoản là {totalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
        }
    }
}
