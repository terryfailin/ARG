using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using MvcAp.Models;

namespace MvcAp.Services
{
    public class GenericService
    {
        public IQueryable<TModel> QueryFor<TModel>() where TModel : class
        {
            var context = new Entities();

            return context.Set<TModel>();
        }

        public IPagedList<TModel> GetsPagedList<TModel>(IQueryable<TModel> predicate, string orderBy, string orderType, int pageIndex, int pageSize, Expression<Func<TModel, object>> includeExpr = null) where TModel : class
        {
            using (Entities context = new Entities())
            {
                if (includeExpr != null)
                {
                    string includeStr = includeExpr.ToString();
                    var firstPeriod = includeStr.IndexOf('.');

                    if (firstPeriod != -1)
                    {
                        includeStr = includeStr.Substring(firstPeriod + 1);
                    }
                    return predicate.Include(includeStr).OrderBy(orderBy, orderType).ToPagedList(pageIndex, pageSize);
                }
                else
                {
                    return predicate.OrderBy(orderBy, orderType).ToPagedList(pageIndex, pageSize);
                }
            }
        }
    }
}
