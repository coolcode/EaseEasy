using System.Collections;
using System.Collections.Generic;

namespace CoolCode.Web.Mvc.UI {
	public interface IGridView<T> : IGridView {
		GridColumnCollection<T> Columns { get; set; }
		new IEnumerable<T> DataSource { get; set; }
	}

	public interface IGridView {
		string Id { get; }
		string EmptyText { get; set; }
		IHtmlAttributes Attributes { get; set; }
		bool IsAjax { get; set; }
		bool ShowScrollBar { get; set; }
		bool Pagable { get; set; }
		int PageSize { get; set; }
		string PageFieldName { get; set; }
		PagerModes PageMode { get; set; }
		string PageFirstText { get; set; }
		string PagePreviousText { get; set; }
		string PageNextText { get; set; }
		string PageLastText { get; set; }
		string PageSummaryFormat { get; set; }
		bool Sortable { get; set; }
		string DefaultSort { get; set; }
		string SortFieldName { get; set; }
		string SortDirFieldName { get; set; }
		IEnumerable DataSource { get; set; }
	}
}
