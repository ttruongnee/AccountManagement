using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Models
{
    public class SavingsAccount : SubAccount
    {
        public SavingsAccount(string subId, double initialBalance = 0) : base(subId, initialBalance)
        {
        }

        public override double InterestRate => 4.7;

        public override string Name => "Tài khoản tiết kiệm";
    }
}
