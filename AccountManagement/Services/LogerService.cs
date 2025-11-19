using System;
using System.Collections.Generic;
using System.Globalization;
using AccountManagement.Models;

namespace AccountManagement.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly List<LogEntry> _entries = new List<LogEntry>();

        public void Log(LogEntry entry)
        {
            if (entry == null) return;
            _entries.Add(entry);
        }

        public IReadOnlyList<LogEntry> GetAll() => _entries.AsReadOnly();  //trả về phiên bản chỉ đọc từ list _entries

        public void PrintAll()
        {
            if (_entries.Count == 0)
            {
                Console.WriteLine("Chưa có hành động nào.");
                return;
            }

            Console.WriteLine("Thời gian\t\tTài khoản\tTài khoản con\tHành động\t\tSố tiền\t\tTrạng thái\tGhi chú");
            foreach (var e in _entries)
            {                
                string subId = e.SubId != null ? e.SubId.ToUpper() : "_\t";
                string amount = e.Amount.HasValue ? e.Amount.Value.ToString("N0", new CultureInfo("vi-VN")) + "đ" : "_\t";
                string status = e.Success ? "Thành công" : "Thất bại";
                Console.WriteLine($"{e.Time}\t{e.AccountId.ToUpper()}\t\t{subId}\t{e.Action}\t{amount}\t{status}\t{e.Note}");
            }
        }
    }
}
