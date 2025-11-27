using System;
using AccountManagement.Models;
using AccountManagement.Services;
using System.Globalization;
using AccountManagement.Ultils;
using System.Linq;
using System.Security.Principal;
using System.Collections.Generic;
using AccountManagement.Repositories;
using AccountManagement.Handlers;
using AccountManagement.Menus;

namespace AccountManagement
{
    class MenuItem
    {
        public int Key { get; set; }
        public string Description { get; set; }
        public Action Action { get; set; }
    }
    class Program
    {
        static void Main()
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;


            var accountRepo = new AccountRepository();
            var subRepo = new SubAccountRepository();
            var logRepo = new LogEntryRepository();

            var accountService = new AccountService(accountRepo, subRepo, logRepo);
            var subAccountService = new SubAccountService(accountRepo, subRepo, logRepo);
            var logService = new LoggerService(logRepo);

            var accountHandlers = new AccountHandlers(accountService, subAccountService);
            var subHandlers = new SubAccountHandlers(accountService, subAccountService);
            var logHandlers = new LogHandlers(logService);

            //khi nào sửa rating phải kiểm tra câu này xem nên tạo service cho rating k
            var ratingHandlers = new RatingHandlers(accountService, subAccountService);

            var ratingMenu = new RatingMenu(ratingHandlers);
            var mainMenu = new MainMenu(accountHandlers, subHandlers, logHandlers, ratingMenu);
            mainMenu.Show();
        }     
    }
}

