using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public interface ILoggerService
    {
        void Log(LogEntry logEntry); //thêm mục nhập mới
        IReadOnlyList<LogEntry> GetAll(); //trả về danh sách chỉ đọc IReadOnlyList gồm các LogEntry
        void PrintAll();  //hiển thị ra toàn bộ nhật ký
    }
}
