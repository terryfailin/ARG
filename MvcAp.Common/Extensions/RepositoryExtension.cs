using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace MvcAp.Common
{
    public static class RepositoryExtension
    {
        public static IQueryable<T> Includes<T, TProperty>(this IQueryable<T> source, Expression<Func<T, TProperty>> path)
        {
            string pathString = path.Body.ToString();
            pathString = pathString.Substring(pathString.IndexOf('.') + 1);
            pathString = Regex.Replace(pathString, @"\.\w+\(\)", "");

            return source.Include(pathString);
        }
    }
}
