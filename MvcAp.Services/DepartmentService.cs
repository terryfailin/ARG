using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MvcAp.Models;
using Newtonsoft.Json;

namespace MvcAp.Services
{
    public class DepartmentService: GenericService
    {
        public void Edit(Department demartment)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    var model = context.Department.FirstOrDefault(p => p.ID ==demartment.ID);

                    if (model != null)
                    {
                        model.DepName = demartment.DepName;
                        context.SaveChanges();
                    }                    
                }
                scope.Complete();
            }
        }
        public string Detele(int sourceid)
        {
            string result = "Success";
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    var model = context.Department.FirstOrDefault(p => p.ID == sourceid);

                    if (model != null)
                    {
                        var users = context.SystemUser.Where(p => p.DepId == sourceid).ToList();
                        if (users!=null && users.Count()>0)
                        {
                            result = "Can't delete due to some users are belong to this department";
                        }
                        else
                        {
                            context.Department.Remove(model);
                            context.SaveChanges();
                        }
                    }
                }
                scope.Complete();
            }
            return result;
        }

        public void Add(Department demartment)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    Department dep=new Department();
                    dep.DepName = demartment.DepName;
                    context.Department.Add(dep);
                    context.SaveChanges();
                }
                scope.Complete();
            }
        }
    }
}
