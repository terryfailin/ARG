using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MvcAp.Common;
using MvcAp.Common.Helper;
using MvcAp.Models;

namespace MvcAp.Services
{
    public class AgreementService :CommonService
    {
        public void Create(Agreement data)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    if (context.Agreement.Any(p => p.Version == data.Version))
                    {
                        throw new ExpectedException("Error:Version is exist");
                    }

                    int userid = BackendUserHelper.CurrentUserLiteData.ID;

                    if ((bool)data.IsPublish)
                    {
                        var currentAgree = context.Agreement.FirstOrDefault(p => (bool)p.IsPublish);
                        if (currentAgree!=null)
                        {
                            currentAgree.IsPublish = false;
                            context.SaveChanges();
                        }
                    }
                    Agreement vo = new Agreement()
                    {
                        Contents = data.Contents,
                        CreateDate = data.CreateDate,
                        CreateBy = userid,
                        IsPublish = data.IsPublish,
                        Version = data.Version
                    };

                    context.Agreement.Add(vo);
                    context.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void Update(Agreement data)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    if (context.Agreement.Any(p => p.Version == data.Version && p.ID != data.ID))
                    {
                        throw new ExpectedException("Error:Version is exist");
                    }

                    int userid = BackendUserHelper.CurrentUserLiteData.ID;
                    if ((bool)data.IsPublish)
                    {
                        var currentAgree = context.Agreement.FirstOrDefault(p => (bool)p.IsPublish);
                        if (currentAgree != null)
                        {
                            currentAgree.IsPublish = false;
                            context.SaveChanges();
                        }
                    }

                    Agreement vo = context.Agreement.FirstOrDefault(p => p.ID == data.ID);

                    vo.Contents = data.Contents;
                    vo.CreateDate = data.CreateDate;
                    vo.UpdatedDate = DateTime.Now;
                    vo.UpdatedBy = userid;
                    vo.IsPublish = data.IsPublish;
                    vo.Version = data.Version;

                    context.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void Delete(List<int> data)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    int userid = BackendUserHelper.CurrentUserLiteData.ID;

                    var dataList = context.Agreement.Where(p => data.Contains(p.ID)).Select(p => p.ID).ToList();

                    context.Agreement.RemoveRange(context.Agreement.Where(p => dataList.Contains(p.ID)).ToList());

                    context.SaveChanges();
                }
                scope.Complete();
            }
        }
    }
}
