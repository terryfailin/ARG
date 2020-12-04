using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Common
{
    public static class DateTimeExtensions
    {
        public static string ToDefaultFormat(this DateTime datetime)
        {
            return datetime.ToString("yyyy/MM/dd");
        }
    }
}
