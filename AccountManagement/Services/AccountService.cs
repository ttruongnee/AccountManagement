using AccountManagement.Models;
using AccountManagement.Repositories;
using Oracle.ManagedDataAccess.Client;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public class AccountService : IAccountService
    {
        private readonly AccountRepository _accountRepo;
        private readonly SubAccountRepository _subAccountRepo;
        private readonly LogEntryRepository _loggerRepo;
        public AccountService(AccountRepository accountRepo, SubAccountRepository subAccountRepository, LogEntryRepository logEntryRepo)
        {
            _accountRepo = accountRepo;
            _subAccountRepo = subAccountRepository;
            _loggerRepo = logEntryRepo;
        }

        //trả về 1 dictionary chỉ đọc chứa các tài khoản (chính)
        public IReadOnlyDictionary<string, Account> GetAllAccounts() => _accountRepo.GetAllAccounts();


        public Account GetAccountById(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId.ToUpper())) return null;
            return _accountRepo.GetAccountById(accountId.ToUpper());
        }


        //tạo tài khoản chính, trả về kiểu bool và message thông báo
        public bool CreateAccount(string accountId, out string message)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                message = "Mã tài khoản chính không hợp lệ.";
                return false;
            }
            accountId = accountId.Trim().ToUpper();

            try
            {
                var result = _accountRepo.CreateAccount(new Account(accountId));
                if (!result)
                {
                    message = "Không thể tạo tài khoản chính.";
                    return false;
                }
                _loggerRepo.CreateLog(new LogEntry(accountId, null, "Tạo tài khoản chính", null, true, "Tạo tài khoản chính thành công"));

                message = "Tạo tài khoản chính thành công.";
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1:
                        message = "Tài khoản đã tồn tại.";
                        return false;

                    case 1400:
                        message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
                        return false;

                    case 2291:
                        message = "Tài khoản cha không tồn tại.";
                        return false;

                    case 904:
                        message = "Tên cột không hợp lệ.";
                        return false;

                    default:
                        message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}";
                        return false;
                }
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                return false;
            }

        }

        //xoá tài khoản chính khi truyền vào id tài khoản, trả về kiểu bool và message thông báo
        public bool DeleteAccount(string accountId, out string message)
        {
            var acc = _accountRepo.GetAccountById(accountId);
            if (acc == null)
            {
                message = "Không tồn tại tài khoản.";
                return false;
            }

            var subAccounts = _subAccountRepo.GetByAccountId(accountId);
            if (subAccounts != null)
            {
                //kiểm tra số dư của tất cả tài khoản con của tài khoản muốn xoá, nếu có tài khoản con > 0 thì không xoá được
                var blockedSub = subAccounts.Values.FirstOrDefault(s => s.Balance > 0);
                if (blockedSub != null)
                {
                    message = $"Không thể xóa: tài khoản con {blockedSub.Sub_Id} còn tiền.";
                    _loggerRepo.CreateLog(new LogEntry(blockedSub.Account_Id, blockedSub.Sub_Id, "Xoá tài khoản chính", null, false, "Còn tiền trong tài khoản con"));
                    return false;
                }
            }


            try
            {
                var result = _accountRepo.DeleteAccount(accountId);
                if (!result)
                {
                    message = $"Xoá tài khoản {accountId} thất bại.";
                    _loggerRepo.CreateLog(new LogEntry(acc.Account_Id, null, "Xoá tài khoản chính", null, false, message));
                    return false;
                }
                
                message = $"Xoá tài khoản {accountId} thành công.";
                _loggerRepo.CreateLog(new LogEntry(acc.Account_Id, null, "Xoá tài khoản chính", null, true, message));
                
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                { 
                    case 2292:
                        message = "Không thể xóa tài khoản: còn tài khoản con liên quan (FK constraint).";
                        return false;

                    default:
                        message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}";
                        return false;
                }
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                return false;
            }           
        }
    }
}
