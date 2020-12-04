using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Helpers;
using Microsoft.Security.Application;
using MvcAp.Common;
using MvcAp.Common.Helper;
using MvcAp.Models;
using MvcAp.Models.ViewModels;
using Newtonsoft.Json;
using NLog.LayoutRenderers.Wrappers;

namespace MvcAp.Services
{
    public class AnnounceService : GenericService
    {
        public void RegistEvent(RegPost data)
        {
            Event ev=null;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {                    
                    EventRegist eReg = new EventRegist();
                    eReg.EventId = data.EventId;
                    eReg.CreateDate = DateTime.Now;
                    eReg.Text01 = data.RegContent;
                    eReg.IsAttend = false;
                    eReg.IsContact = false;
                    eReg.OrderNum = 0;
                    context.EventRegist.Add(eReg);

                    context.SaveChanges();
                    ev = context.Event.FirstOrDefault(p => p.ID == data.EventId);
                }
                scope.Complete();                
            }
            if (NotifyHelper.IsValidEmail(data.Email))
            {                
                if (ev != null)
                {
                    if (!string.IsNullOrEmpty(ev.EmailContent))
                    {
                        string mailMsg = ev.EmailContent;
                        mailMsg = mailMsg.Replace(@"{thisname}", ev.Name).Replace(@"{address}", ev.Address);
                        SystemSetting setting = NotifyHelper.GetSystemSetting();
                        Task.Factory.StartNew(() =>
                        {
                            NotifyHelper.SendMail(setting, data.Email, "Activity Application been Received", mailMsg, true);
                        });
                    }
                }
            }
        }

        public bool hasCurrentEmail(RegPost data)
        {
            bool result = false;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    var eReg =
                    (from er in context.EventRegist
                        where er.EventId == data.EventId && er.Text01.Contains(data.Email)
                        select er).ToList();
                    if (eReg.Count()>0)
                    {
                        result = true;
                    }
                }
                scope.Complete();
            }
            return result;
        }
    }
}
