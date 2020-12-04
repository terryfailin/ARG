using System;
using System.Web;
using System.Configuration;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using MvcAp.Common.Helper;

namespace MvcAp.Common
{
    public static class AppUtility
    {
        private static Object lockObject = new Object();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static string SHA512SEncode(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                SHA512Managed SHA512 = new SHA512Managed();

                byte[] plainTextBytes = Encoding.UTF8.GetBytes(str);

                byte[] saltBytes = Convert.FromBase64String("a83ad7170bd6450295dd4e417571030f");

                byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

                for (int i = 0; i < plainTextBytes.Length; i++)
                { plainTextWithSaltBytes[i] = plainTextBytes[i]; }

                for (int i = 0; i < saltBytes.Length; i++)
                { plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i]; }

                byte[] hashBytes = SHA512.ComputeHash(plainTextWithSaltBytes);

                return Convert.ToBase64String(hashBytes);
            }
            return string.Empty;
        }
    }
}
