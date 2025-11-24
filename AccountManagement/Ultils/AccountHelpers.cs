using AccountManagement.Services;
using System.Collections.Generic;
using System.Linq;

internal static class AccountHelpers
{
    public static IEnumerable<(string MainId, double TotalBalance)> GetAccountsWithTotalBalance(IAccountService accountService)
    {
        var dict = accountService.GetAllAccounts();
        if (dict.Count == 0) return Enumerable.Empty<(string, double)>();

        return dict.Select(acc => (
            MainId: acc.Key,
            TotalBalance: acc.Value.SubAccounts.Sum(s => s.Balance)
        ));
    }
}