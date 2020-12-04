using System.Text.RegularExpressions;

namespace System
{
    public static class ValidateHelper
    {

        /// <summary>
        /// 檢查字串是否為手機號碼
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsMobileNumber(object x)
        {
            return new Regex(@"^[0][9]\d{8}$").IsMatch(Convert.ToString(x));
        }

        /// <summary>
        /// 檢查是否為整數數值樣式
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsIntegerNumber(object x)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(
                Convert.ToString(x),
                System.Globalization.NumberStyles.Integer,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out retNum);

            return isNum;
        }

        /// <summary>
        /// 檢查是否為正確的Email格式.
        /// </summary>
        public static bool IsEmailFormat(string email)
        {
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4})$");
        }

        /// <summary>
        /// 判斷字串是否為GUID
        /// </summary>
        /// <param name="str">傳入的字串</param>
        /// <returns></returns>
        public static bool IsGuid(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            string Pattern = "^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$";
            return System.Text.RegularExpressions.Regex.Match(str, Pattern).Success;
        }
    }
}
