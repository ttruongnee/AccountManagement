using AccountManagement.Models;
using AccountManagement.Repositories;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Services
{
    public class SubAccountService : ISubAccountService
    {
        private readonly AccountRepository _accountRepo;
        private readonly SubAccountRepository _subAccountRepo;
        private readonly LogEntryRepository _loggerRepo;

        //hàm khởi tạo giao dịch
        public SubAccountService(AccountRepository accountRepository, SubAccountRepository subAccountRepository, LogEntryRepository logEntryRepository)
        {
            _accountRepo = accountRepository;
            _subAccountRepo = subAccountRepository;
            _loggerRepo = logEntryRepository;
        }

        public Dictionary<decimal, SubAccount> GetAllSubAccounts()
        {
            return _subAccountRepo.GetAllSubAccounts();
        }

        //lấy ra dict subaccount từ accountid
        public Dictionary<decimal, SubAccount> GetByAccountId(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId)) return null;
            return _subAccountRepo.GetByAccountId(accountId);
        }

        //lấy ra subaccount theo id
        public SubAccount GetBySubAccountId(decimal sub_id)
        {
            if (sub_id <= 0) return null;
            return _subAccountRepo.GetBySubAccountId(sub_id);
        }

        //kiểm tra subaccount có tồn tại hay không
        public bool CheckSubAccountExists(decimal subId, out string message)
        {
            var sub = _subAccountRepo.GetBySubAccountId(subId);

            if (sub == null)
            {
                message = "Tài khoản con không tồn tại.";
                return false;
            }

            message = "Tài khoản con tồn tại.";
            return true;
        }

        //lấy ra type của tài khoản con để hiển thị
        public string GetSubAccountType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return type;

            if (type.Equals("TK")) return "Tài khoản tiết kiệm";
            return "Tài khoản đầu tư";
        }

        //tạo tài khoản con
        public bool CreateSubAccount(SubAccount subAccount, out string message)
        {
            try
            {
                var result = _subAccountRepo.CreateSubAccount(subAccount);
                if (!result)
                {
                    message = $"Tạo {GetSubAccountType(subAccount.Type).ToLower()} - {subAccount.Name.ToUpper()} thất bại.";
                    _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, "Tạo tài khoản con", null, false, message));
                    return false;
                }
                message = $"Tạo {GetSubAccountType(subAccount.Type).ToLower()} - {subAccount.Name.ToUpper()} thành công.";
                _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, "Tạo tài khoản con", null, true, message));
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1:
                        message = $"Không thể tạo tài khoản con: (account_id: {subAccount.Account_Id}, name: {subAccount.Name}, type: {subAccount.Type}) đã tồn tại.";
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

        //xoá tài khoản con
        public bool DeleteSubAccount(decimal subId, out string message)
        {
            if (!CheckSubAccountExists(subId, out message))
            {
                return false;
            }

            var subAccount = _subAccountRepo.GetBySubAccountId(subId);

            if (subAccount.Balance > 0)
            {
                message = $"{GetSubAccountType(subAccount.Type)} - {subAccount.Name.ToUpper()} vẫn còn tiền, không thể xoá.";
                _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, $"Xoá {GetSubAccountType(subAccount.Type).ToLower()}", null, false, message));
                return false;
            }

            try
            {
                var result = _subAccountRepo.DeleteSubAccount(subId);

                if (!result)
                {
                    message = $"Xoá {GetSubAccountType(subAccount.Type).ToLower()} - {subAccount.Name.ToUpper()} thất bại.";
                    _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, $"Xoá {GetSubAccountType(subAccount.Type).ToLower()}", null, false, message));
                    return false;
                }

                message = $"Xoá {GetSubAccountType(subAccount.Type).ToLower()} - {subAccount.Name.ToUpper()} thành công.";
                _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, $"Xoá {GetSubAccountType(subAccount.Type).ToLower()}", null, true, message));
                return true;
            }
            catch (OracleException ex)
            {
                if (ex.Number == 2292)
                {
                    message = "Không thể xóa: Có bản ghi liên quan (FK).";
                    return false;
                }

                message = $"Lỗi CSDL {ex.Number}: {ex.Message}";
                return false;
            }
        }

        //nạp tiền
        public bool Deposit(decimal subId, double amount, out string message)
        {
            try
            {
                if (!CheckSubAccountExists(subId, out message))
                {
                    return false;
                }

                //lấy ra sub_account
                var subAccount = _subAccountRepo.GetBySubAccountId(subId);

                //nạp tiền
                subAccount.Deposit(amount);

                //cập nhật vào db
                var result = _subAccountRepo.UpdateSubAccount(subAccount);
                if (!result)
                {
                    message = $"Nạp {amount.ToString("N0", new CultureInfo("vi-VN"))}đ vào tài khoản {subAccount.Name}";
                    _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subId, "Nạp tiền", amount, false, message));
                    return false;
                }
                message = $"Nạp {amount.ToString("N0", new CultureInfo("vi-VN"))}đ vào tài khoản {subAccount.Name}";
                _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subId, "Nạp tiền", amount, true, message));
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1400:
                        message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
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

        //rút tiền
        public bool Withdraw(decimal subId, double amount, out string message)
        {
            try
            {
                if (!CheckSubAccountExists(subId, out message))
                {
                    return false;
                }

                //lấy ra sub_account
                var subAccount = _subAccountRepo.GetBySubAccountId(subId);

                //rút tiền
                subAccount.Withdraw(amount);

                //cập nhật vào db
                var result = _subAccountRepo.UpdateSubAccount(subAccount);
                if (!result)
                {
                    message = $"Rút {amount.ToString("N0", new CultureInfo("vi-VN"))}đ khỏi tài khoản {subAccount.Name}";
                    _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subId, "Rút tiền", amount, false, message));
                    return false;
                }
                message = $"Rút {amount.ToString("N0", new CultureInfo("vi-VN"))}đ khỏi tài khoản {subAccount.Name}";
                _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subId, "Rút tiền", amount, true, message));
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1400:
                        message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
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

        //thanh toán lãi
        public bool PayInterest(decimal subId, out string message)
        {
            try
            {
                if (!CheckSubAccountExists(subId, out message))
                {
                    return false;
                }

                //lấy ra sub_account
                var subAccount = _subAccountRepo.GetBySubAccountId(subId);               

                double interest = subAccount.GetInterest();
                if (interest <= 0)
                {
                    message = "Không có lãi để thanh toán.";
                    _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, "Thanh toán lãi", interest, false, message));
                    return false;
                }

                subAccount.Deposit(interest); // dùng hàm nạp để cộng lãi
                var result = _subAccountRepo.UpdateSubAccount(subAccount);
                if (!result)
                {
                    message = $"Thanh toán {interest:N0}đ tiền lãi cho {subAccount.Name}.";
                    _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, "Thanh toán lãi", interest, false, message));
                    return false;
                }
                message = $"Thanh toán {interest:N0}đ tiền lãi cho {subAccount.Name}.";
                _loggerRepo.CreateLog(new LogEntry(subAccount.Account_Id, subAccount.Sub_Id, "Thanh toán lãi", interest, true, message));
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1400:
                        message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
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
    }
}
