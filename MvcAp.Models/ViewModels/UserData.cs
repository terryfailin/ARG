using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Models.ViewModels
{
    public class UserData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string RoleName { get; set; }
        public List<string> RoleList { get; set; }
    }
}
