using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Models
{
    public abstract class SubAccount
    {
        public string SubId { get; }
        public abstract string Name { get; }
        private double _balance;
        public double Balance
        {
            get => _balance;
            protected set => _balance = double.IsNaN(value) || value < 0 ? 0 : value;
        }

        //khởi tạo 
        protected SubAccount(string subId, double initialBalance = 0)
        {
            // ?? dùng để gán giá trị mặc định nếu biến null
            // nếu subId không null thì gán SubId = subId, nếu subId null thì gán bằng string.Emplty ("")
            SubId = (subId ?? string.Empty).Trim().ToLower();
            Balance = initialBalance;
        }

        //mỗi loại tài khoản sẽ phải override interest rate
        public abstract double InterestRate { get; }

        public double GetInterest() => Balance * InterestRate / 100;

        public virtual void Deposit(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Số tiền nạp phải lớn hơn 0");
            Balance += amount;
        }

        public virtual void Withdraw(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Số tiền muốn rút phải lớn hơn 0");
            if (Balance < amount) throw new InvalidOperationException("Số tiền muốn rút lớn hơn số tiền đang có");
            Balance -= amount;
        }
    }
}
