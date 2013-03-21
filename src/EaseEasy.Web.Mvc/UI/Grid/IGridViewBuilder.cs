using System;
using System.Collections.Generic;

namespace EaseEasy.Web.Mvc.UI {
	public interface IGridViewBuilder<T> {
		IGridViewBuilder<T> Columns(Action<IGridColumnCollectionBuilder<T>> columnBuilder);

		IGridViewBuilder<T> Pager(
			bool pagable = true,
			int pageSize = 10,
			PagerModes mode = PagerModes.All,
			string pageFieldName = GridView.DefaultPageFieldName,
			string pageSizeFieldName = GridView.DefaultPageSizeFieldName,
			string firstText = "<<",
			string previousText = "<",
			string nextText = ">",
			string lastText = ">>",
			string summaryFormat = " 第 <b>{PageIndex}</b>/<b>{PageCount}</b> 页, 总数：{TotalRecords}");

		IGridViewBuilder<T> Sort(
			bool sortable = true,
			string defaultSort = null,
			string sortFieldName = GridView.DefaultSortFieldName,
			string sortDirFieldName = GridView.DefaultSortDirFieldName);

		IGridViewBuilder<T> Bind(IEnumerable<T> dataSource);

		IGridViewBuilder<T> Ajax(bool isAjax = true, string updateTargetId = "");
	}
}
