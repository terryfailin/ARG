using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MvcAp.Common.Helper
{
    public static class AppHelper
    {
        private static string PASSWORD_CHARS_LCASE = "abcdefghjkmnopqrstuvwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "0123456789";
        private static Object lockObject = new Object();

        public static String GetIP()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.UserHostAddress;
        }

        public static string GeneratePassword(int length)
        {
            return GeneratePassword(length, length);
        }

        public static string GeneratePassword(int minlength, int maxlength, bool onlynumber = false)
        {
            // Make sure that input parameters are valid.
            if (minlength <= 0 || maxlength <= 0 || minlength > maxlength)
                return null;

            char[][] charGroups;
            if (onlynumber)
            {
                charGroups = new char[][]
                {
                    PASSWORD_CHARS_NUMERIC.ToCharArray()
                };
            }
            else
            {
                charGroups = new char[][]
                {
                    PASSWORD_CHARS_LCASE.ToCharArray(),
                    PASSWORD_CHARS_UCASE.ToCharArray(),
                    PASSWORD_CHARS_NUMERIC.ToCharArray()
				    //, PASSWORD_CHARS_SPECIAL.ToCharArray()
			    };
            }


            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate產生亂數密碼 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7f) << 24 |
                        randomBytes[1] << 16 |
                        randomBytes[2] << 8 |
                        randomBytes[3];

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minlength < maxlength)
                password = new char[random.Next(minlength, maxlength + 1)];
            else
                password = new char[minlength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate產生亂數密碼 password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }

        /// <summary>
        /// 建立驗證碼
        /// </summary>
        /// <param name="captchaName">驗證碼對應文字</param>
        /// <returns></returns>
        public static byte[] GenerateCaptcha(string captchaName, bool onlynumber)
        {
            // (封裝 GDI+ 點陣圖) 新增一個 Bitmap 物件，並指定寬、高
            Bitmap _bmp = new Bitmap(64, 25);

            // (封裝 GDI+ 繪圖介面) 所有繪圖作業都需透過 Graphics 物件進行操作
            Graphics _graphics = Graphics.FromImage(_bmp);
            _graphics.Clear(Color.Black);
            // 如果想啟用「反鋸齒」功能，可以將以下這行取消註解
            _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // 設定要出現在圖片上的文字字型、大小與樣式
            Font _font = new Font("Arial", 14, FontStyle.Bold);

            // 產生一個 5 個字元的亂碼字串，並直接寫入 Session 裡
            // 請參考: http://www.obviex.com/Samples/Password.aspx )
            var newText = GeneratePassword(4, 4, onlynumber);
            HttpContext.Current.Session[captchaName] = newText;

            Random rdn = new Random();

            int x1, y1, x2, y2, x3, y3 = 0;
            int intNoiseWidth = 25;
            int intNoiseHeight = 15;

            //產生雜點
            for (int i = 0; i < 80; i++)
            {
                x1 = rdn.Next(0, _bmp.Width);
                y1 = rdn.Next(0, _bmp.Height);
                _bmp.SetPixel(x1, y1, Color.Brown);
            }
            //產生擾亂弧線
            for (int i = 0; i < 5; i++)
            {
                x1 = rdn.Next(_bmp.Width - intNoiseWidth);
                y1 = rdn.Next(_bmp.Height - intNoiseHeight);
                x2 = rdn.Next(1, intNoiseWidth);
                y2 = rdn.Next(1, intNoiseHeight);
                x3 = rdn.Next(0, 45);
                y3 = rdn.Next(-270, 270);
                _graphics.DrawArc(new Pen(Brushes.Gray), x1, y1, x2, y2, x3, y3);
            }
            // 將亂碼字串「繪製」到之前產生的 Graphics 「繪圖板」上
            _graphics.DrawString(newText, _font, Brushes.White, 6, 3);
            MemoryStream s = new MemoryStream();
            _bmp.Save(s, ImageFormat.Gif);

            // 釋放所有在 GDI+ 所佔用的記憶體空間 ( 非常重要!! )
            _font.Dispose();
            _graphics.Dispose();
            _bmp.Dispose();

            return s.GetBuffer();
        }

        public static string SiteUrl { get { return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority; } }

        public static string ProjectName
        {
            get
            {
                return "ＡＲＧ";
            }
        }
    }
}
