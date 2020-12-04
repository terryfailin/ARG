using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcAp.Models;

namespace MvcAp
{
    public class EventViewModel:Event
    {
        public int RegNum { get; set; }
        public int x1 { get; set; }
        public int y1 { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}