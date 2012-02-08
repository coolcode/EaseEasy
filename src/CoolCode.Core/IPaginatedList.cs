using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace CoolCode {
	/// <summary>
	/// 分页接口
	/// </summary>
	public interface IPageable {
		int PageIndex { get; }
		int PageSize { get; }
		int TotalRecords { get; }
		int PageCount { get; }
	}

	public interface IPaginatedList : IPageable {
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
	}

	public interface IPaginatedList<T> : IPaginatedList, IList<T>, IEnumerable<T> {

	}

	/// <summary>
	/// 分页参数
	/// </summary>
	public class PageParam {
		public int PageIndex { get; set; }

		public int PageSize { get; set; }

		public PageParam()
			: this(0) {
		}

		public PageParam(int pageIndex) : this(pageIndex, 10) { }

		public PageParam(int pageIndex, int pageSize) {
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
		}
	}


}
