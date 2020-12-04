using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Models.ViewModels
{
    public class SystemSetting
    {
        public string SenderEmail { get; set; }
        public string SenderDisplayName { get; set; }
        public string SMTPHost { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPUserPassword { get; set; }
        public string DepList { get; set; }
        public int SmtpAuthType { get; set; }
        public int Port { get; set; }
    }
}
