using AccountManagement.Handlers;
using AccountManagement.Services;
using AccountManagement.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Menus
{
    public class RatingMenu
    {
        private readonly RatingHandlers _ratingHandlers;

        public RatingMenu(RatingHandlers ratingHandlers)
        {
            _ratingHandlers = ratingHandlers;
        }

        public void Show()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem { 
                    Key = 1, 
                    Description = "Xếp hạng tài khoản theo số dư", 
                    Action =  () => _ratingHandlers.RankAccountsByBalance(
                                        _ratingHandlers.GetAccountsWithTotalBalance())
                },
                new MenuItem { 
                    Key = 2, 
                    Description = "Các tài khoản số dư dưới 1 triệu", 
                    Action = () => _ratingHandlers.Under1Million(
                                         _ratingHandlers.GetAccountsWithTotalBalance())
                },
                new MenuItem { 
                    Key = 3, 
                    Description = "Top 3 tài khoản có số dư thanh toán lớn nhất", 
                    Action = () => _ratingHandlers.Top3(
                                        _ratingHandlers.GetAccountsWithTotalBalance()) 
                },
                new MenuItem { Key = 4, 
                    Description = "Tổng số dư tài khoản đầu tư của tất cả tài khoản", 
                    Action = _ratingHandlers.TotalBalanceOfInvestmentAccounts 
                },
                new MenuItem { Key = 0, Description = "Thoát", Action = () => { } }

            };

            while (true)
            {
                Console.Clear();
                foreach (var item in menuItems)
                {
                    Console.WriteLine($"{item.Key}. {item.Description}");
                }

                int choice = InputHelper.ReadIntInRange("Chọn: ", 0, 4);
                if (choice == 0) return;
                Console.WriteLine();

                var selected = menuItems.FirstOrDefault(m => m.Key == choice);
                selected?.Action.Invoke();

                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }
    }
}
