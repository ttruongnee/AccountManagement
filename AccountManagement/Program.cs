using System;
using AccountManagement.Models;
using AccountManagement.Services;
using System.Globalization;
using AccountManagement.Ultils;

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
            accountService.CreateSubAccount("MBBANK", new SavingsAccount("TIETKIEM", 1000), out _);
            accountService.CreateSubAccount("MBBANK", new InvestmentAccount("DAUTUMBB", 1000), out _);

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
                Console.WriteLine("0. Thoát");
                int choice = InputHelper.ReadIntInRange("Chọn: ", 0, 9);
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
    }
}
