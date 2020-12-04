using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using MvcAp.Common;
using MvcAp.Common.Helper;
using MvcAp.Models;
using MvcAp.Models.Enums;
using MvcAp.Models.ViewModels;
using Newtonsoft.Json;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace MvcAp.Services
{
    public class CommonService : GenericService
    {
        private static object lockObject = new object();
        private Object thisLock = new Object();
        private Object lockObj = new Object();

        public List<PostalData> GetsPostalData()
        {
            List<PostalData> result = new List<PostalData>();

            result = QueryFor<PostalData>().Where(p => (bool)p.IsEnable).ToList();

            return result;
        }

        public void SaveSystemSetting(SystemSetting data)
        {
            lock (thisLock)
            {
                using (var scope = new TransactionScope())
                {
                    using (var context = new Entities())
                    {
                        var model = context.DbConfig.FirstOrDefault(p => p.KeyName == "System" && p.Type == "Setting");

                        if (model == null)
                        {
                            model = new DbConfig()
                            {
                                IsEnable = true,
                                KeyName = "System",
                                Type = "Setting",
                            };
                            context.DbConfig.Add(model);
                        }
                        model.Value = JsonConvert.SerializeObject(data);
                        context.SaveChanges();
                    }
                    scope.Complete();
                }
            }
        }

        public void SavePage(string page1, string page2, string page3, string page4, string page5)
        {
            lock (lockObj)
            {
                using (var scope = new TransactionScope())
                {
                    using (var context = new Entities())
                    {
                        var Page1 = context.DbConfig.FirstOrDefault(p => p.KeyName == "Page1" && p.Type == "Dynamic");

                        if (Page1 == null)
                        {
                            Page1 = new DbConfig()
                            {
                                IsEnable = true,
                                KeyName = "Page1",
                                Type = "Dynamic",
                            };
                            context.DbConfig.Add(Page1);
                        }
                        Page1.Value = page1;

                        var Page2 = context.DbConfig.FirstOrDefault(p => p.KeyName == "Page2" && p.Type == "Dynamic");

                        if (Page2 == null)
                        {
                            Page2 = new DbConfig()
                            {
                                IsEnable = true,
                                KeyName = "Page2",
                                Type = "Dynamic",
                            };
                            context.DbConfig.Add(Page2);
                        }
                        Page2.Value = page2;

                        var Page3 = context.DbConfig.FirstOrDefault(p => p.KeyName == "Page3" && p.Type == "Dynamic");

                        if (Page3 == null)
                        {
                            Page3 = new DbConfig()
                            {
                                IsEnable = true,
                                KeyName = "Page3",
                                Type = "Dynamic",
                            };
                            context.DbConfig.Add(Page3);
                        }
                        Page3.Value = page3;

                        var Page4 = context.DbConfig.FirstOrDefault(p => p.KeyName == "Page4" && p.Type == "Dynamic");

                        if (Page4 == null)
                        {
                            Page4 = new DbConfig()
                            {
                                IsEnable = true,
                                KeyName = "Page4",
                                Type = "Dynamic",
                            };
                            context.DbConfig.Add(Page4);
                        }
                        Page4.Value = page4;

                        var Page5 = context.DbConfig.FirstOrDefault(p => p.KeyName == "Page5" && p.Type == "Dynamic");

                        if (Page5 == null)
                        {
                            Page5 = new DbConfig()
                            {
                                IsEnable = true,
                                KeyName = "Page5",
                                Type = "Dynamic",
                            };
                            context.DbConfig.Add(Page5);
                        }
                        Page5.Value = page5;

                        context.SaveChanges();
                    }
                    scope.Complete();
                }
            }
        }
    }
}
