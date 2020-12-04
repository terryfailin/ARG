using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Models.ViewModels
{
    public class UserViewModel : SystemUser
    {
        public string GroupName { get; set; }

        public string ConfirmPassword { get; set; }

        public int? RoleID { get; set; }
        public string DepartmentName { get; set; }

    }
}
