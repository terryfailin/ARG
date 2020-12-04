using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace MvcAp.Common.Classes
{
    public class RequestArguments
    {
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchType { get; set; }
        public string SearchText { get; set; }
        private NameValueCollection _wheres;
        public NameValueCollection Wheres { get { if (_wheres == null) _wheres = new NameValueCollection(); return _wheres; } }
    }
}
