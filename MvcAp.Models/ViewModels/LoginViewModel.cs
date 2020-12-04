using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcAp.Lang;

namespace MvcAp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Account", ResourceType = typeof(Language))]
        public string Account { get; set; }

        [Required]
        [Display(Name = "Password", ResourceType = typeof(Language))]
        public string Password { get; set; }

        public string AccessToken { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public int PostalID { get; set; }

        public string Catalogs { get; set; }

        public int Age { get; set; }

        public int Marriage { get; set; }
        public int PetType { get; set; }
        public int PetNum { get; set; }
        public int Gender { get; set; }
    }
}
