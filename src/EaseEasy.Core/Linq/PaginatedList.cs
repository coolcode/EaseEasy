using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EaseEasy.Linq {
	public class PaginatedList<T> : List<T>, IPaginatedList, IPaginatedList<T> {
		public int PageIndex { get; private set; }
		public int PageSize { get; private set; }
		public int TotalRecords { get; private set; }
		public int PageCount { get; private set; }

		public PaginatedList(IQueryable source, int pageIndex, int pageSize)
			: this((IQueryable<T>)source, pageIndex, pageSize) { }

		public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize) {
			PageIndex = pageIndex < 0 ? 0 : pageIndex;
			PageSize = pageSize < 0 ? 10 : pageSize;
			TotalRecords = source.Count();
			PageCount = (int)Math.Ceiling(TotalRecords / (double)PageSize);
			var query = source.Skip(PageIndex * PageSize).Take(PageSize);
			this.AddRange(query.ToList());
		}

		public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize) {
			PageIndex = pageIndex < 0 ? 0 : pageIndex;
			PageSize = pageSize < 0 ? 10 : pageSize;
			TotalRecords = source.Count();
			PageCount = (int)Math.Ceiling(TotalRecords / (double)PageSize);

			this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
		}

        public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalRecords) {
            PageIndex = pageIndex < 0 ? 0 : pageIndex;
            PageSize = pageSize < 0 ? 10 : pageSize;
            TotalRecords = totalRecords;
            PageCount = (int)Math.Ceiling(TotalRecords / (double)PageSize);

            this.AddRange(source);
        }

		public PaginatedList(IQueryable<T> source, PageParam p)
			: this(source, p.PageIndex, p.PageSize) {
		}

		public PaginatedList(IEnumerable<T> source, PageParam p)
			: this(source, p.PageIndex, p.PageSize) {
		}

		public PaginatedList() {
			PageIndex = 0;
			PageSize = 10;
			TotalRecords = 0;
			PageCount = 0;
		}

		public PaginatedList(IPaginatedList<T> source) {
			PageIndex = source.PageIndex;
			PageSize = source.PageSize;
			TotalRecords = source.TotalRecords;
			PageCount = source.PageCount;
			this.AddRange(source);
		}

		public PaginatedList(IPageable source, IEnumerable<T> collection) {
			PageIndex = source.PageIndex;
			PageSize = source.PageSize;
			TotalRecords = source.TotalRecords;
			PageCount = source.PageCount;
			this.AddRange(collection);
		}

		public bool HasPreviousPage {
			get {
				return (PageIndex > 0);
			}
		}

		public bool HasNextPage {
			get {
				return (PageIndex + 1 < PageCount);
			}
		}
	}


	public static class PaginatedListExtension {
		public static IPaginatedList<T> Paging<T>(this IEnumerable<T> query, PageParam p) {
			return new PaginatedList<T>(query, p);
		}

		public static IPaginatedList<S> Paging<T, S>(this IEnumerable<T> query, Func<T, S> selector, PageParam p) {
			var q = query.Select(selector);
			return new PaginatedList<S>(q, p);
		}

		public static IPaginatedList<T> Paging<T>(this IQueryable<T> query, PageParam p) {
			return new PaginatedList<T>(query, p);
		}

		public static IPaginatedList<T> Paging<T, K>(this IQueryable<T> query, Expression<Func<T, K>> orderBy, PageParam p) {
			var q = query.OrderBy(orderBy);
			return new PaginatedList<T>(query, p);
		}

		public static IPaginatedList<S> Paging<T, K, S>(this IQueryable<T> query, Expression<Func<T, K>> orderBy, Expression<Func<T, S>> selector, PageParam p) {
			var q = query.OrderBy(orderBy).Select(selector);
			return new PaginatedList<S>(q, p);
		}

		public static IPaginatedList<TResult> Select<T, TResult>(this IPaginatedList<T> paginatedList, Func<T, TResult> selector) {
			IEnumerable<T> list = paginatedList;
			IEnumerable<TResult> target = list.Select(selector);
			IPaginatedList<TResult> result = new PaginatedList<TResult>(paginatedList, target);
			return result;
		}

		public static void ForEach<T>(this IPaginatedList<T> paginatedList, Action<T, int> action) {
			int i = 0;
			paginatedList.ForEach(c => action(c, (i++) + paginatedList.PageIndex * paginatedList.PageSize));
		}
	}
}
