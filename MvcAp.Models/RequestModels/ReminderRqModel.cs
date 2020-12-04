using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcAp.Lang;

namespace MvcAp.Models.RequestModels
{
    public class ReminderRqModel
    {       
        public int userid { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public int PetPageNum { get; set; }

        public string PetType { get; set; }

        public string DrugName { get; set; }

        public string PetWeightIndex { get; set; }

        public string StartDate { get; set; }

    }
}
