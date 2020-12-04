using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Common.Classes
{
    public class AjaxResult
    {
        #region Properties
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 回傳資料
        /// </summary>
        public Object Data { get; set; }
        /// <summary>
        /// 是否有權限
        /// </summary>
        public bool IsAuthorize { get; set; }
        #endregion

        #region Ctor
        public AjaxResult() { }

        public AjaxResult(bool success, string message, Object data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public AjaxResult(bool success, string message, object data, bool isAuthorize) : this(success, message, data)
        {
            IsAuthorize = isAuthorize;
        }
        #endregion
    }
}
