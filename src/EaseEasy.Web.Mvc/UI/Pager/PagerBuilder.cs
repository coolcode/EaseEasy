using System;
using System.Web.Mvc;

namespace EaseEasy.Web.Mvc.UI {
	/// <summary>
	/// 分页控件构造类
	/// </summary>
	public class PagerBuilder : ViewComponentBuilder<Pager, PagerBuilder> {
		public PagerBuilder(Pager component, ViewContext context, IViewDataContainer container)
			: base(component, context, container) {

		}

		public PagerBuilder PageSize(int value) {
			Component.PageSize = value;
			return this;
		}

		public PagerBuilder Align(Alignment alignment) {
			Component.MergeAttribute("align", alignment.GetAttachedString(), true);
			return this;
		}

		public PagerBuilder PageFieldName(string value) {
			Component.PageFieldName = value;
			return this;
		}

		public PagerBuilder PageSizeFieldName(string value) {
			Component.PageSizeFieldName = value;
			return this;
		}

		public PagerBuilder Summary(string format, bool visible = true) {
			Component.SummaryFormat = format;
			Component.SummaryVisible = visible;
			return this;
		}

		public PagerBuilder First(string value) {
			Component.FirstPageText = value;
			return this;
		}

		public PagerBuilder Previous(string value) {
			Component.PreviousPageText = value;
			return this;
		}

		public PagerBuilder Next(string value) {
			Component.NextPageText = value;
			return this;
		}

		public PagerBuilder Last(string value) {
			Component.LastPageText = value;
			return this;
		}

		/// <summary>
		/// Uses a lambda expression to generate the URL for the page links.
		/// </summary>
		/// <param name="urlBuilder">Lambda expression for generating the URL used in the page links</param>
		public PagerBuilder LinkUrl(Func<int, object> urlBuilder) {
			Component.LinkUrlBuilder = urlBuilder;
			return this;
		}

		public PagerBuilder LinkCss(string value) {
			Component.LinkCss = value;
			return this;
		}

		public PagerBuilder Mode(PagerModes mode) {
			Component.Mode = mode;
			return this;
		}

		public PagerBuilder OnClick(string script) {
			Component.Link.OnClick(script);
			return this;
		}

		internal PagerBuilder Bind(IPageable source) {
			Component.DataSource = source;
			return this;
		}

		public PagerBuilder UpdateTarget(string updateTargetId) {
			Component.UpdateTargetId = updateTargetId;
			return this;
		}

		public PagerBuilder TemplateName(string templateName) {
			Component.TemplateName = templateName;
			return this;
		}
		 
	}
}
