using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Models
{
    public class SubAccount
    {
        public int Sub_Id { get; }
        public string Account_Id { get; }
        public string Name { get; }
        public string Type { get; }
        public double InterestRate { get; }

        private double _balance;
        public double Balance
        {
            get => _balance;
            protected set => _balance = double.IsNaN(value) || value < 0 ? 0 : value;
        }

        public SubAccount() { }

        //khởi tạo 
        public SubAccount(string account_Id, string name, string type, double initialBalance = 0)
        {
            Account_Id = account_Id;            
            Name = name.ToUpper();
            Type = type;
            Balance = initialBalance;
            InterestRate = Type.Equals("TK") ? 4.7 : 5.1;

        }

        public double GetInterest() => Balance * InterestRate / 100;

        public void Deposit(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Số tiền nạp phải lớn hơn 0");
            Balance += amount;
        }

        public void Withdraw(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Số tiền muốn rút phải lớn hơn 0");
            if (Balance < amount) throw new InvalidOperationException("Số tiền muốn rút lớn hơn số tiền đang có");
            Balance -= amount;
        }
    }
}
