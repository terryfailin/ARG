using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Common
{
    public static class StringExtensions
    {
        public static List<int> ToIntList(this string val, char splitStr = ',')
        {
            List<int> result = new List<int>();

            if (!string.IsNullOrWhiteSpace(val))
            {
                int parseObj;
                foreach (var str in val.Split(splitStr).ToList())
                {
                    if (int.TryParse(str, out parseObj))
                    {
                        result.Add(parseObj);
                    }
                }
            }

            return result;
        }
    }
}
