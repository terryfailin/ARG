namespace System.Collections.Generic
{
	public interface IPagedList<T> : IList<T>, IPagedList
	{
	}

	public interface IPagedList
	{
		int PageCount { get; }
		int TotalItemCount { get; }
		int PageIndex { get; }
		int PageNumber { get; }
		int PageSize { get; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
		bool IsFirstPage { get; }
		bool IsLastPage { get; }
	}



}