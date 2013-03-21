using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EaseEasy.Web.Mvc.UI;

namespace EaseEasy.Web.Mvc.Html {
	public static class PagerExtensions {

		public static PagerBuilder Pager(this HtmlHelper helper, string viewDataKey) {
			var dataSource = helper.ViewContext.ViewData.Eval(viewDataKey) as IPageable;

			if (dataSource == null) {
				throw new InvalidOperationException(string.Format("Item in ViewData with key '{0}' is not an IPageable.",
																  viewDataKey));
			}

			return helper.Pager(dataSource);
		}

		public static PagerBuilder Pager(this HtmlHelper helper, int totalRecords, int pageSize = 10, int? pageIndex = null) {
			return helper.Pager(new InnerPageSource(pageIndex ?? -1, pageSize, totalRecords));
		}

		public static PagerBuilder Pager(this HtmlHelper helper, IPageable source) {
			return helper.Element<Pager, PagerBuilder>().Bind(source).Css("pager").Align(Alignment.Left);
		}

		internal class InnerPageSource : IPageable {
			public int PageIndex { get; private set; }
			public int PageSize { get; private set; }
			public int TotalRecords { get; private set; }
			public int PageCount { get; private set; }

			public InnerPageSource(int pageIndex, int pageSize, int totalRecords) {
				PageIndex = pageIndex;
				PageSize = pageSize;
				TotalRecords = totalRecords;
				PageCount = (int)Math.Ceiling(TotalRecords / (double)PageSize);
			}
		}
	}
}
