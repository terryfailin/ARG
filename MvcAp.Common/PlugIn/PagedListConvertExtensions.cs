using System.Linq;

namespace System.Collections.Generic
{
    public static class PagedListConvertExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, bool isEnable = true)
        {
            return new PagedList<T>(source, pageIndex, pageSize, isEnable);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount, bool isEnable = true)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount, isEnable);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool isEnable = true)
        {
            return new PagedList<T>(source, pageIndex, pageSize, isEnable);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalCount, bool isEnable = true)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount, isEnable);
        }
    }
}
