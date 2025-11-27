using AccountManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Handlers
{
    public class LogHandlers
    {
        private readonly ILoggerService _logService;

        public LogHandlers(ILoggerService logService)
        {
            _logService = logService;
        }

        public void ShowLogs()
        {
            _logService.ShowLogs();
        }
    }
}
