using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcAp.Lang;

namespace MvcAp.Models.RequestModels
{
    public class LoginRqModel
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

        public string PostalID { get; set; }

        public string Catalogs { get; set; }

        public string Age { get; set; }

        public string Marriage { get; set; }
        public string PetType { get; set; }
        public string PetNum { get; set; }
        public string Gender { get; set; }
    }
}
