using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace MvcAp.Common
{
    public static class NLogExtensions
    {
        public static void CusotmerLog(this Logger logger, Exception ex, string prefix = "異常")
        {
            logger.Error(ex, prefix + "：" + ex.Message + "=>" + ex.InnerException);
        }
    }
}
