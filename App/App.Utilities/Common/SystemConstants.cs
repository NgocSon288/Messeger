using System;
using System.Collections.Generic;
using System.Text;

namespace App.Utilities.Common
{
    public static class SystemConstants
    {
        public static class Database
        {
            public static string MessengerConnectionString = "MessengerConnectionString";
        }

        public static class FileUploadPath
        {
            public static string UserAvatars = "Avatars";
        }

        public static class AppSettings
        {
            public static string SystemSettingKey = "SystemSetting";
            public static string BaseAddressServer = "BaseAddressServer";
            public static string TokensIssuer = "Tokens:Issuer";
            public static string TokensKey = "Tokens:Key";
            public static string Token = "Token";
        }
    }
}
