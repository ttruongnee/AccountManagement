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
        void CreateLog(LogEntry logEntry); 
        IReadOnlyList<LogEntry> GetAll(); 
        void ShowLogs();  
    }
}
