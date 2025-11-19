using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Models
{
    public class LogEntry
    {
        public DateTime Time { get; } = DateTime.Now;
        public string AccountId { get; }
        //string thì ngầm hiểu là có thể null nên không cần ? ở đầu
        public string SubId { get; }
        public string Action { get; }
        public double? Amount { get; }
        public bool Success { get; }
        public string Note { get; }

        public LogEntry(string accountId, string subId, string action, double? amount, bool success, string note = "")
        {
            AccountId = accountId;
            SubId = subId;
            Action = action;
            Amount = amount;
            Success = success;
            Note = note;
        }
    }
}
