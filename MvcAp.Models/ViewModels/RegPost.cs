using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcAp.Models;

namespace MvcAp.Models.ViewModels

{
    public class RegPost 
    {
        public int EventId { get; set; }
        public string RegContent { get; set; }
        public string Email { get; set; }
    }
}