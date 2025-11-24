using System;
using AccountManagement.Models;
using AccountManagement.Services;
using System.Globalization;
using AccountManagement.Ultils;
using System.Linq;
using System.Security.Principal;
using System.Collections.Generic;

namespace AccountManagement
{
    class MenuItem { 
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

            ILoggerService logger = new LoggerService();
            IAccountService accountService = new AccountService(logger);
            ITransactionService transactionService = new TransactionService(accountService, logger);

            //SEED DATA 
            accountService.CreateMainAccount("MBBANK", out _);
            accountService.CreateSubAccount("MBBANK", new SavingsAccount("TIETKIEM", 1_000_000), out _);
            accountService.CreateSubAccount("MBBANK", new InvestmentAccount("DAUTUMBB", 900_000), out _);
            accountService.CreateSubAccount("MBBANK", new InvestmentAccount("DAUTUMBB2", 1_200_000), out _);

            accountService.CreateMainAccount("VIETIN", out _);
            accountService.CreateSubAccount("VIETIN", new SavingsAccount("VIETINTK", 500_000), out _);
            accountService.CreateSubAccount("VIETIN", new InvestmentAccount("VIETINDT", 1_500_000), out _);
            accountService.CreateSubAccount("VIETIN", new InvestmentAccount("VIETINTK2", 700_000), out _);

            accountService.CreateMainAccount("MOMOVN", out _);
            accountService.CreateSubAccount("MOMOVN", new SavingsAccount("MOMODT", 100_000), out _);
            accountService.CreateSubAccount("MOMOVN", new InvestmentAccount("MOMOTK", 500_000), out _);
            accountService.CreateSubAccount("MOMOVN", new InvestmentAccount("MOMOTK2", 200_000), out _);

            accountService.CreateMainAccount("VNPAYY", out _);
            accountService.CreateSubAccount("VNPAYY", new SavingsAccount("VNPAYDT", 700_000), out _);
            accountService.CreateSubAccount("VNPAYY", new InvestmentAccount("VNPAYTK", 5_000_000), out _);
            accountService.CreateSubAccount("VNPAYY", new InvestmentAccount("VNDT2", 100_000), out _);

            accountService.CreateMainAccount("VIETCOM", out _);
            accountService.CreateSubAccount("VIETCOM", new SavingsAccount("VIETCOMDT", 100_000), out _);
            accountService.CreateSubAccount("VIETCOM", new InvestmentAccount("VIETCOMTK", 500_000), out _);
            accountService.CreateSubAccount("VIETCOM", new InvestmentAccount("VIETCOMDT2", 600_000), out _);


            var menuItems = new List<MenuItem>
            {
                new MenuItem { Key = 1, Description = "Xem tài khoản", Action = () => PrintAllAccounts(accountService) },
                new MenuItem 
                { 
                    Key = 2, 
                    Description = "Tạo tài khoản chính", 
                    Action = () => 
                    {
                        string id = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                        if (accountService.CreateMainAccount(id, out var msg))
                            Console.WriteLine(msg);
                        else Console.WriteLine(msg);
                    } 
                },
                new MenuItem { 
                    Key = 3, 
                    Description = "Tạo tài khoản con", 
                    Action = () => 
                    { 
                        //tạo tài khoản con
                            string mainId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                            var acc = accountService.GetAccount(mainId);
                            if (acc == null)
                            {
                                Console.WriteLine("Tài khoản chính không tồn tại.");
                                return;
                            }

                            string subId = InputHelper.ReadNonEmpty("Nhập ID tài khoản con: ");
                            Console.WriteLine("Chọn loại: 1.Tiết kiệm  2.Đầu tư");
                            int type = InputHelper.ReadIntInRange("Loại: ", 1, 2);

                            Console.Write("Số dư khởi tạo (nhập 0 nếu không): ");
                            double initial = 0;
                            double.TryParse(Console.ReadLine(), out initial);

                            SubAccount sub;
                            if (type == 1)
                                sub = new SavingsAccount(subId, initial);
                            else
                                sub = new InvestmentAccount(subId, initial);

                            accountService.CreateSubAccount(mainId, sub, out var msg);
                            Console.WriteLine(msg);
                    }
                },
                new MenuItem { 
                    Key = 4, 
                    Description = "Xóa tài khoản chính", 
                    Action = () => 
                    { 
                        //xoá tài khoản chính
                        string id = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính cần xóa: ");
                        accountService.DeleteMainAccount(id, out var msg);
                        Console.WriteLine(msg);
                    } 
                },
                new MenuItem { 
                    Key = 5, 
                    Description = "Nạp tiền", 
                    Action = () => 
                    {
                        //nạp tiền
                        string accId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                        var acc = accountService.GetAccount(accId);
                        if (acc == null)
                        {
                            Console.WriteLine("Tài khoản chính không tồn tại.");
                            return;
                        }

                        string subId = InputHelper.ReadNonEmpty("Nhập ID tài khoản con: ");
                        double amount = InputHelper.ReadPositiveDouble("Nhập số tiền nạp: ");

                        transactionService.Deposit(accId, subId, amount, out var msg);
                        Console.WriteLine(msg);
                    } 
                },
                new MenuItem { 
                    Key = 6, 
                    Description = "Rút tiền", 
                    Action = () => 
                    {
                        //rút tiền
                        string accId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                        var acc = accountService.GetAccount(accId);
                        if (acc == null)
                        {
                            Console.WriteLine("Tài khoản chính không tồn tại.");
                            return;
                        }

                        string subId = InputHelper.ReadNonEmpty("Nhập ID tài khoản con: ");
                        double amount = InputHelper.ReadPositiveDouble("Nhập số tiền rút: ");

                        transactionService.Withdraw(accId, subId, amount, out var msg);
                        Console.WriteLine(msg);
                    } 
                },
                new MenuItem { 
                    Key = 7, 
                    Description = "Tính lãi (hiển thị)", 
                    Action = () =>
                    {
                        //hiển thị tài khoản kèm lãi
                        var dict = accountService.GetAllAccounts();
                        if (dict.Count == 0)
                        {
                            Console.WriteLine("Không có tài khoản.");
                            return;
                        }
                        foreach (var account in dict)
                        {
                            Console.WriteLine($"\n=== Tài khoản chính: {account.Key} ===");
                            foreach (var s in account.Value.SubAccounts)
                            {
                                Console.WriteLine($"{s.SubId.ToUpper()}\t | Loại: {s.Name}\t | Số dư: {s.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ\t | Lãi ({s.InterestRate}%): {s.GetInterest().ToString("N0", new CultureInfo("vi-VN"))}đ");
                            }
                        }
                    }  
                    
                },
                new MenuItem { 
                    Key = 8, 
                    Description = "Thanh toán lãi cho 1 tài khoản con", 
                    Action = () => 
                    {
                        //thanh toán lãi
                        string accId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                        var acc = accountService.GetAccount(accId);
                        if (acc == null)
                        {
                            Console.WriteLine("Tài khoản chính không tồn tại.");
                            return;
                        }
                        string subId = InputHelper.ReadNonEmpty("Nhập ID tài khoản con: ");

                        transactionService.PayInterest(accId, subId, out var msg);
                        Console.WriteLine(msg);
                    } 
                },
                new MenuItem { Key = 9, Description = "Xem lịch sử hoạt động", Action = () => logger.PrintAll() },
                new MenuItem { Key = 10, Description = "Xếp hạng account", Action = () => AccountRating(accountService) },
                new MenuItem { Key = 0, Description = "Thoát", Action = () => Environment.Exit(0) }
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== QUẢN LÝ TÀI KHOẢN ======");
                foreach (var item in menuItems)
                {
                    Console.WriteLine($"{item.Key}. {item.Description}");
                }

                int choice = InputHelper.ReadIntInRange("Chọn: ", 0, 10);
                Console.WriteLine();

                var selected = menuItems.FirstOrDefault(m => m.Key == choice);
                selected?.Action.Invoke();

                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }

        static void PrintAllAccounts(IAccountService accountService)
        {
            var dict = accountService.GetAllAccounts();
            if (dict.Count == 0) { Console.WriteLine("Không có tài khoản."); return; }
            foreach (var kv in dict)
            {
                Console.WriteLine($"\n=== Tài khoản chính: {kv.Key.ToUpper()} ===");
                if (kv.Value.SubAccounts.Count == 0) Console.WriteLine("  (Chưa có tài khoản con)");
                foreach (var s in kv.Value.SubAccounts)
                {
                    Console.WriteLine($"- {s.SubId.ToUpper()}\t | Loại: {s.Name}\t | Số dư: {s.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ");
                }
            }
        }

        static void AccountRating(IAccountService accountService)
        {
            var subMenu = new List<MenuItem>
            {
                new MenuItem { Key = 1, Description = "Xếp hạng tài khoản theo số dư", Action = () => RankAccountsByBalance(accountService) },
                new MenuItem { Key = 2, Description = "Các tài khoản số dư dưới 1 triệu", Action = () => Under1Million(accountService) },
                new MenuItem { Key = 3, Description = "Top 3 tài khoản có số dư thanh toán lớn nhất", Action = () => Top3(accountService) },
                new MenuItem { Key = 4, Description = "Tổng số dư tài khoản đầu tư của tất cả tài khoản", Action = () => TotalBalanceOfInvestmentAccounts(accountService) },
                new MenuItem { Key = 0, Description = "Thoát", Action = () => { } }

            };
            while (true)
            {
                Console.Clear();
                foreach (var item in subMenu)
                {
                    Console.WriteLine($"{item.Key}. {item.Description}");
                }

                int choice = InputHelper.ReadIntInRange("Chọn: ", 0, 4);
                if (choice == 0) return;
                Console.WriteLine();


                var selected = subMenu.FirstOrDefault(x => x.Key == choice);
                selected?.Action.Invoke()
                    ;
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void RankAccountsByBalance(IAccountService accountService)
        {
            var dict = accountService.GetAllAccounts();
            if (dict.Count == 0) { return; }
            var result = AccountHelpers.GetAccountsWithTotalBalance(accountService)
                .OrderByDescending(d => d.TotalBalance);

            foreach (var item in result)
            {
                Console.WriteLine($"- {item.MainId.ToUpper()}\t | Tổng số dư: {item.TotalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            }
        }

        static void Under1Million(IAccountService accountService)
        {
            var dict = accountService.GetAllAccounts();
            if (dict.Count == 0) { return; }
            var result = AccountHelpers.GetAccountsWithTotalBalance(accountService)
                .Where(acc => acc.TotalBalance < 1_000_000);
            
            foreach (var item in result)
            {
                Console.WriteLine($"- {item.MainId.ToUpper()}\t | Tổng số dư: {item.TotalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            }
        }

        static void Top3(IAccountService accountService)
        {
            var dict = accountService.GetAllAccounts();
            if(dict.Count == 0) {return; }
            var result = AccountHelpers.GetAccountsWithTotalBalance(accountService)
                .OrderByDescending(acc => acc.TotalBalance)
                .Take(3);

            foreach (var item in result)
            {
                Console.WriteLine($"- {item.MainId.ToUpper()}\t | Tổng số dư: {item.TotalBalance.ToString("N0", new CultureInfo("vi-VN"))}đ");
            }
        }

        static void TotalBalanceOfInvestmentAccounts(IAccountService accountService)
        {
            var dict = accountService.GetAllAccounts();
            if (dict.Count == 0) { return ; }
            var result = dict.SelectMany(acc => acc.Value.SubAccounts)
                .Where(acc => acc.Name == "Tài khoản đầu tư")
                .Sum(acc => acc.Balance);

            Console.WriteLine($"Tổng số dư tài khoản đầu tư của tất cả tài khoản là {result.ToString("N0", new CultureInfo("vi-VN"))}đ");
        }        
    }
}
