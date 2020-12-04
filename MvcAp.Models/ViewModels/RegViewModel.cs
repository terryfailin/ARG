using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Models.ViewModels
{
    public class RegViewModel :EventRegist
    {
        public Dictionary<int,string>  ColValues{ get; set; }
        public string RegValues { get; set; }
    }
}
