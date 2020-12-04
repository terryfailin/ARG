using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcAp.Common
{
    /// <summary>
    /// 可預期的錯誤
    /// </summary>
    public class ExpectedException : ApplicationException
    {
        public ExpectedException()
            : this("預期的錯誤", null)
        {
        }

        public ExpectedException(string message)
            : this(message, null)
        {
        }

        public ExpectedException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
