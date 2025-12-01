using Microsoft.Extensions.Configuration;
using System;

namespace AccountManagement.Utils
{
    public static class ConfigurationHelper
    {
        private static IConfigurationRoot configuration;  //IConfigurationRoot cho phép đọc file cấu hình 

        static ConfigurationHelper()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)  //đường dẫn tới chỗ chứa file appsettings.json (nó đang đến cùng cấp với .exe trong bin debug)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  //file phải tồn tại; nếu file thay đổi trong lúc chạy thì config tự cập nhật
                .Build(); //tạo đối tượng IConfigurationRoot có thể dùng để đọc giá trị
        }

        public static string GetEncryptedConnectionString()
        {
            return configuration["EncryptedConnectionString"];
        }

        public static string GetDecryptedConnectionString()
        {
            string encrypted = GetEncryptedConnectionString();
            return EncryptHelper.Decrypt(encrypted);
        }

        public static string GetConnectionString()
        {
            return configuration["ConnectionStrings:Default"];
        }
    }
}
