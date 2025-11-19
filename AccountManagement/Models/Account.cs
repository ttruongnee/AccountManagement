using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Models
{
    public class Account
    {
        public string AccountId { get; }
        public List<SubAccount> SubAccounts { get; }

        public Account(string accountId)
        {
            AccountId = (accountId ?? string.Empty).Trim().ToLower();
            SubAccounts = new List<SubAccount>();
        }
    }
}
