using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CoolCode.Web.Mvc.UI {
	public class GridView<T> : GridView, IGridView<T> {
		private IEnumerable<T> _dataSource;
		public new IEnumerable<T> DataSource {
			get { return _dataSource ?? (base.DataSource as IEnumerable<T>); }
			set {
				_dataSource = value;
				base.DataSource = value;
			}
		}

		public GridColumnCollection<T> Columns { get; set; }

		public GridView()
			: base() {
			Columns = new GridColumnCollection<T>();
		}

		public override void Render(TextWriter writer) {
			IGridViewRenderer<T> renderer = new GridViewRenderer<T>(ViewContext, ViewDataContainer);

			renderer.Render(this, writer);

			base.Render(writer);
		}
	}

	public class GridView : ViewComponent, IGridView {
		protected internal const string DefaultPageFieldName = "page";
		protected internal const string DefaultPageSizeFieldName = "pageSize";
		protected internal const string DefaultSortFieldName = "sort";
		protected internal const string DefaultSortDirFieldName = "sortdir";
		protected internal const string DefaultEmptyText = "没有找到相关数据.";

		public string EmptyText { get; set; }
		public IHtmlAttributes Attributes { get; set; }
		public bool IsAjax { get; set; } 
		public string UpdateTargetId{ get; set; }

		public bool Pagable { get; set; }
		public int PageSize { get; set; }
		public string PageFieldName { get; set; }
		public string PageSizeFieldName { get; set; }
		public PagerModes PageMode { get; set; }
		public string PageFirstText { get; set; }
		public string PagePreviousText { get; set; }
		public string PageNextText { get; set; }
		public string PageLastText { get; set; }
		public string PageSummaryFormat { get; set; }

		public bool Sortable { get; set; }
		public string DefaultSort { get; set; }
		public string SortFieldName { get; set; }
		public string SortDirFieldName { get; set; }

		public IEnumerable DataSource { get; set; }

		public GridView() {
			Attributes = new HtmlAttributes();
			EmptyText = DefaultEmptyText;
			IsAjax = true;
			Sortable = true;
			SortFieldName = DefaultSortFieldName;
			SortDirFieldName = DefaultSortDirFieldName;
			Pagable = true;
			PageSize = 10;
			PageFieldName = DefaultPageFieldName;
			PageSizeFieldName = DefaultPageSizeFieldName;
		}

		public override void Render(TextWriter writer) {

		}
	}
}
