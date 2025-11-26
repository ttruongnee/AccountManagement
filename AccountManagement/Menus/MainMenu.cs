using AccountManagement.Handlers;
using AccountManagement.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Menus
{
    public class MainMenu
    {
        private readonly AccountHandlers _accountHandlers;
        private readonly SubAccountHandlers _subHandlers;
        private readonly RatingMenu _ratingMenu;

        public MainMenu(AccountHandlers accountHandlers, SubAccountHandlers subHandlers, RatingMenu ratingMenu)
        {
            _accountHandlers = accountHandlers;
            _subHandlers = subHandlers;
            _ratingMenu = ratingMenu;
        }

        public void Show()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Key = 1, Description = "Xem tài khoản", Action = _accountHandlers.PrintAllAccounts },
                new MenuItem { Key = 2, Description = "Tạo tài khoản chính", Action = _accountHandlers.CreateAccount },
                new MenuItem { Key = 3, Description = "Tạo tài khoản con", Action = _subHandlers.CreateSubAccount },
                new MenuItem { Key = 4, Description = "Xóa tài khoản chính", Action = _accountHandlers.DeleteAccount },
                new MenuItem { Key = 5, Description = "Xóa tài khoản con", Action = _subHandlers.DeleteSubAccount },
                new MenuItem { Key = 6, Description = "Nạp tiền", Action = _subHandlers.Deposit },
                new MenuItem { Key = 7, Description = "Rút tiền", Action = _subHandlers.Withdraw },
                //new MenuItem { Key = 8, Description = "Tính lãi (hiển thị)", Action = _subHandlers.ShowInterest },
                //new MenuItem { Key = 9, Description = "Thanh toán lãi", Action = _subHandlers.PayInterest },
                //new MenuItem { Key = 10, Description = "Xem lịch sử hoạt động", Action = _logHandlers.ShowLogs },
                new MenuItem { Key = 11, Description = "Xếp hạng account", Action = _ratingMenu.Show },
                new MenuItem { Key = 0, Description = "Thoát", Action = () => Environment.Exit(0) },
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== QUẢN LÝ TÀI KHOẢN ======");
                foreach (var item in menuItems)
                {
                    Console.WriteLine($"{item.Key}. {item.Description}");
                }

                int choice = InputHelper.ReadIntInRange("Chọn: ", 0, 11);
                Console.WriteLine();

                var selected = menuItems.FirstOrDefault(m => m.Key == choice);
                selected?.Action.Invoke();

                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }
    }
}
