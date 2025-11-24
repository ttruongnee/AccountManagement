using System;
using AccountManagement.Models;
using AccountManagement.Services;
using System.Globalization;
using AccountManagement.Ultils;
using System.Linq;
using System.Security.Principal;

namespace AccountManagement
{
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
            accountService.CreateSubAccount("VNPAYY", new InvestmentAccount("VNPAYDT2", 100_000), out _);

            accountService.CreateMainAccount("VIETCOM", out _);
            accountService.CreateSubAccount("VIETCOM", new SavingsAccount("VIETCOMDT", 100_000), out _);
            accountService.CreateSubAccount("VIETCOM", new InvestmentAccount("VIETCOMTK", 500_000), out _);
            accountService.CreateSubAccount("VIETCOM", new InvestmentAccount("VIETCOMDT2", 600_000), out _);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== QUẢN LÝ TÀI KHOẢN ======");
                Console.WriteLine("1. Xem tài khoản");
                Console.WriteLine("2. Tạo tài khoản chính");
                Console.WriteLine("3. Tạo tài khoản con");
                Console.WriteLine("4. Xóa tài khoản chính");
                Console.WriteLine("5. Nạp tiền");
                Console.WriteLine("6. Rút tiền");
                Console.WriteLine("7. Tính lãi (hiển thị)");
                Console.WriteLine("8. Thanh toán lãi cho 1 tài khoản con");
                Console.WriteLine("9. Xem lịch sử hoạt động");
                Console.WriteLine("10. Xếp hạng account");
                Console.WriteLine("0. Thoát");
                int choice = InputHelper.ReadIntInRange("Chọn: ", 0, 10);
                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        //hiển thị ra toàn bộ tài khoản
                        PrintAllAccounts(accountService);
                        break;
                    case 2:
                        {
                            //tạo tài khoản chính
                            string id = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                            if (accountService.CreateMainAccount(id, out var msg))
                                Console.WriteLine(msg);
                            else Console.WriteLine(msg);
                        }
                        break;
                    case 3:
                        {
                            //tạo tài khoản con
                            string mainId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                            var acc = accountService.GetAccount(mainId);
                            if (acc == null)
                            {
                                Console.WriteLine("Tài khoản chính không tồn tại.");
                                break;
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
                        break;
                    case 4:
                        {
                            //xoá tài khoản chính
                            string id = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính cần xóa: ");
                            accountService.DeleteMainAccount(id, out var msg);
                            Console.WriteLine(msg);
                        }
                        break;
                    case 5:
                        {
                            //nạp tiền
                            string accId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                            var acc = accountService.GetAccount(accId);
                            if (acc == null)
                            {
                                Console.WriteLine("Tài khoản chính không tồn tại.");
                                break;
                            }

                            string subId = InputHelper.ReadNonEmpty("Nhập ID tài khoản con: ");
                            double amount = InputHelper.ReadPositiveDouble("Nhập số tiền nạp: ");

                            transactionService.Deposit(accId, subId, amount, out var msg);
                            Console.WriteLine(msg);
                        }
                        break;
                    case 6:
                        {
                            //rút tiền
                            string accId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                            var acc = accountService.GetAccount(accId);
                            if (acc == null)
                            {
                                Console.WriteLine("Tài khoản chính không tồn tại.");
                                break;
                            }

                            string subId = InputHelper.ReadNonEmpty("Nhập ID tài khoản con: ");
                            double amount = InputHelper.ReadPositiveDouble("Nhập số tiền rút: ");

                            transactionService.Withdraw(accId, subId, amount, out var msg);
                            Console.WriteLine(msg);
                        }
                        break;
                    case 7:
                        {
                            //hiển thị tài khoản kèm lãi
                            var dict = accountService.GetAllAccounts();
                            if (dict.Count == 0)
                            {
                                Console.WriteLine("Không có tài khoản.");
                                break;
                            }
                            foreach (var account in dict)
                            {
                                Console.WriteLine($"\n=== Tài khoản chính: {account.Key} ===");
                                foreach (var s in account.Value.SubAccounts)
                                {
                                    Console.WriteLine($"{s.SubId.ToUpper()} | Loại: {s.Name} | Số dư: {s.Balance.ToString("N0", new CultureInfo("vi-VN"))}đ | Lãi ({s.InterestRate}%): {s.GetInterest().ToString("N0", new CultureInfo("vi-VN"))}đ");
                                }
                            }
                        }
                        break;
                    case 8:
                        {
                            //thanh toán lãi
                            string accId = InputHelper.ReadNonEmpty("Nhập ID tài khoản chính: ");
                            var acc = accountService.GetAccount(accId);
                            if (acc == null)
                            {
                                Console.WriteLine("Tài khoản chính không tồn tại.");
                                break;
                            }
                            string subId = InputHelper.ReadNonEmpty("Nhập ID tài khoản con: ");

                            transactionService.PayInterest(accId, subId, out var msg);
                            Console.WriteLine(msg);
                        }
                        break;
                    case 9:
                        logger.PrintAll();
                        break;
                    case 10:
                        AccountRating(accountService);
                        break;
                    case 0:
                        return;
                }

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
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Xếp hạng tài khoản theo số dư");
                Console.WriteLine("2. Các tài khoản số dư dưới 1 triệu");
                Console.WriteLine("3. Top 3 tài khoản có số dư thanh toán lớn nhất");
                Console.WriteLine("4. Tổng số dư tài khoản đầu tư của tất cả tài khoản");
                Console.WriteLine("0. Thoát");

                int choice = InputHelper.ReadIntInRange("Chọn: ", 0, 4);
                Console.WriteLine();
                switch (choice)
                {
                    case 1:
                        RankAccountsByBalance(accountService);
                        break;
                    case 2:
                        Under1Million(accountService);
                        break;
                    case 3:
                        Top3(accountService);
                        break;
                    case 4:
                        TotalBalanceOfInvestmentAccounts(accountService);
                        break;
                    case 0:
                        return;
                }
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void RankAccountsByBalance(IAccountService accountService)
        {
            var dict = accountService.GetAllAccounts();
            if (dict.Count == 0) { return; }
            var result = dict.Select(acc => new
            {
                MainId = acc.Key,
                TotalBalance = acc.Value.SubAccounts.Sum(s => s.Balance)
            })
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
            var result = dict.Select(acc => new
            {
                MainId = acc.Key,
                TotalBalance = acc.Value.SubAccounts.Sum(s => s.Balance)
            })
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
            var result = dict.Select(acc => new
            {
                MainId = acc.Key,
                TotalBalance = acc.Value.SubAccounts.Sum(s => s.Balance)
            })
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
