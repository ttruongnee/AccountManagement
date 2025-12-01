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
using AccountManagement.Utils;

namespace AccountManagement
{
    class Program
    {
        static void Main()
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //string conn = ConfigurationHelper.GetConnectionString();
            //string encrypted = EncryptHelper.Encrypt(conn);
            //Console.WriteLine(encrypted);
            //Console.ReadLine();

            var accountRepo = new AccountRepository();
            var subRepo = new SubAccountRepository();
            var logRepo = new LogEntryRepository();

            var accountService = new AccountService(accountRepo, subRepo, logRepo);
            var subAccountService = new SubAccountService(accountRepo, subRepo, logRepo);
            var logService = new LoggerService(logRepo);

            var accountHandlers = new AccountHandlers(accountService, subAccountService);
            var subHandlers = new SubAccountHandlers(accountService, subAccountService);
            var logHandlers = new LogHandlers(logService);
            var ratingHandlers = new RatingHandlers(accountService, subAccountService);

            var ratingMenu = new RatingMenu(ratingHandlers);
            var mainMenu = new MainMenu(accountHandlers, subHandlers, logHandlers, ratingMenu);
            mainMenu.Show();
        }     
    }
}

