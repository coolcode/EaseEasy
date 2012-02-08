using System;
using System.Collections.Generic;
using System.IO;
using CoolCode.Web.Mvc.Html;

namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// 分页控件
	/// </summary>
	public class Pager : ViewComponent, IPageable {
		private const int MiddlePageIndex = 5;
		private const int PageRange = MiddlePageIndex * 2;

		public PagerModes Mode { get; set; }

		public IPageable DataSource { get; set; }
		public string SummaryFormat { get; set; }
		public string FirstPageText { get; set; }
		public string PreviousPageText { get; set; }
		public string NextPageText { get; set; }
		public string LastPageText { get; set; }
		public string PageFieldName { get; set; }
		public bool SummaryVisible { get; set; }
		public Func<int, object> LinkUrlBuilder { get; set; }

		private LinkBuilder _link;
		public LinkBuilder Link {
			get {
				if (_link == null)
					_link = Html.Link().Css("pagerLink");
				return _link;
			}
		}

		#region IPageable Members

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

		public int PageCount {
			get {
				return (int)Math.Ceiling(TotalRecords / (double)PageSize);
			}
		}

		private int? _pageIndex;
		public int PageIndex {
			get {
				if (_pageIndex == null) {
					_pageIndex = DataSource.PageIndex >= 0 ? DataSource.PageIndex : ViewContext.Controller.ValueOf(PageFieldName, 0);
				}
				return _pageIndex.Value;
			}
		}

		private int _pageSize;
		public int PageSize {
			get { return DataSource.PageSize > 0 ? DataSource.PageSize : _pageSize; }
			internal set { _pageSize = value; }
		}

		public int TotalRecords {
			get { return DataSource.TotalRecords; }
		}

		#endregion

		public Pager() {
			/*页次：<b>{PageIndex}</b>/<b>{PageCount}</b>页 每页记录：<b>{PageSize}</b>条 记录总数：<b>{TotalRecords}</b>*/
			SummaryFormat = "<span>Page <b>{PageIndex}</b>/<b>{PageCount}</b>, Showing {From} - {To} of {TotalRecords} </span>";
			FirstPageText = "<<";
			PreviousPageText = "<";
			NextPageText = ">";
			LastPageText = ">>";
			PageFieldName = "page";
			Mode = PagerModes.All;
			SummaryVisible = true;
			LinkUrlBuilder = CreateDefaultPageUrl;
			PageSize = 10;
		}

		#region Render

		public override void RenderContent(TextWriter writer) {
			if (this.TotalRecords > 0) {
				if (ModeEnabled(PagerModes.FirstLast) && HasPreviousPage) {
					RenderPageLink(writer, 0, FirstPageText);
				}
				if (ModeEnabled(PagerModes.NextPrevious) && HasPreviousPage) {
					RenderPageLink(writer, PageIndex - 1, PreviousPageText);
				}

				if (ModeEnabled(PagerModes.Numeric)) {
					RenderNumericPages(writer);
				}

				if (ModeEnabled(PagerModes.NextPrevious) && HasNextPage) {
					RenderPageLink(writer, PageIndex + 1, NextPageText);
				}
				if (ModeEnabled(PagerModes.FirstLast) && HasNextPage) {
					RenderPageLink(writer, PageCount - 1, LastPageText);
				}
			}

			if (SummaryVisible) {
				RenderSummaryInfo(writer);
			}
		}

		protected virtual void RenderSummaryInfo(TextWriter writer) {
			string summaryHtml = this.SummaryFormat
				.Replace("{PageIndex}", (this.PageIndex + 1).ToString())
				.Replace("{PageCount}", this.PageCount.ToString())
				.Replace("{PageSize}", this.PageSize.ToString())
				.Replace("{TotalRecords}", this.TotalRecords.ToString())
				.Replace("{From}", (this.PageSize * this.PageIndex + 1).ToString())
				.Replace("{To}", ((this.PageIndex == this.PageCount - 1) ? this.TotalRecords : (this.PageSize * (this.PageIndex + 1))).ToString());

			writer.Write(summaryHtml);
		}

		protected virtual void RenderPageLink(TextWriter writer, int pageIndex, string displayText) {
			var link = Link.Href(LinkUrlBuilder(pageIndex).ToString())
				.Template(c => displayText);

			writer.WriteLine(link.ToHtmlString());
		}

		protected virtual void RenderNumericPages(TextWriter writer) {
			Func<int, int> clipIfMore = value => {
				if (value > this.PageCount - 1) {
					return this.PageCount - 1;
				}
				return value;
			};

			Func<int, int> clipIfLess = value => {
				if (value < 0) {
					return 0;
				}
				return value;
			};

			int maxpage = clipIfMore(this.PageIndex + MiddlePageIndex);
			int minpage = clipIfLess(this.PageIndex - MiddlePageIndex + 1);
			if (maxpage - minpage + 1 < PageRange) {
				if (minpage > 0) {
					minpage = clipIfLess(maxpage - PageRange + 1);
				}
				if (maxpage < this.PageCount - 1) {
					maxpage = clipIfMore(minpage + PageRange - 1);
				}
			}

			if (minpage > 0) {
				RenderSuspensionPoints(writer);
			}

			for (int i = minpage; i <= maxpage; i++) {
				bool isCurrentPage = (i == this.PageIndex);
				RenderPageLink(writer, i, isCurrentPage);
			}

			if (maxpage < this.PageCount - 1) {
				RenderSuspensionPoints(writer);
			}
		}

		protected virtual void RenderPageLink(TextWriter writer, int pageIndex, bool isCurrentPage) {
			string displayText = (pageIndex + 1).ToString();
			if (!isCurrentPage) {
				RenderPageLink(writer, pageIndex, displayText);
			}
			else {
				writer.Write("<span class=\"current\">{0}</span>", displayText);
			}
		}

		protected virtual void RenderSuspensionPoints(TextWriter writer) {
			writer.Write("...");
		}

		#endregion

		protected virtual string CreateDefaultPageUrl(int pageIndex) {
			var routeValues = new Dictionary<string, object>();
			routeValues.Add(PageFieldName, pageIndex);

			return ViewContext.Url(routeValues);
		}

		private bool ModeEnabled(PagerModes modeCheck) {
			return (Mode & modeCheck) == modeCheck;
		}
	}
}
