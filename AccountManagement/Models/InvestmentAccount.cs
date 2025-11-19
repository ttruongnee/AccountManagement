using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Models
{
    public class InvestmentAccount : SubAccount
    {
        public InvestmentAccount(string subId, double initialBalance = 0) : base(subId, initialBalance)
        {
        }

        public override double InterestRate => 5.1;

        public override string Name => "Tài khoản đầu tư";
    }
}
