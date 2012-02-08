using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CoolCode.Web.Mvc.UI {
	public abstract class ViewComponentBuilder<TComponent, TBuilder> : IHtmlString, IFluentViewComponentBuilder<TComponent, TBuilder> //IViewComponentBuilder<TComponent>, IFluentInterface<TBuilder>
		where TComponent : ViewComponent
		where TBuilder : ViewComponentBuilder<TComponent, TBuilder> {
		public TComponent Component { get; private set; }

		public IViewDataContainer ViewDataContainer {
			get {
				return Component.ViewDataContainer;
			}
		}

		public ViewContext ViewContext {
			get {
				return Component.ViewContext;
			}
		}

		private AjaxHelper _ajaxHelper;

		protected AjaxHelper Helper {
			get {
				if (_ajaxHelper == null)
					_ajaxHelper = new AjaxHelper(this.ViewContext, this.ViewDataContainer);
				return _ajaxHelper;
			}
		}

		protected ViewComponentBuilder(TComponent component, ViewContext context, IViewDataContainer container) {
			if (context == null)
				throw new ArgumentNullException("context");
			Component = component;
			Component.ViewContext = context;
			Component.ViewDataContainer = container;
		}

		public TBuilder GenerateId() {
			if (string.IsNullOrEmpty(Component.Name)) {
				string prefix = Component.GetType().Name;
				string key = "Mvc_ViewComponent_IDSEQ_" + prefix;
				int seq = 1;

				if (ViewContext.HttpContext.Items.Contains(key)) {
					seq = (int)ViewContext.HttpContext.Items[key] + 1;
					ViewContext.HttpContext.Items[key] = seq;
				}
				else {
					ViewContext.HttpContext.Items.Add(key, seq);
				}
				Component.Name = prefix + seq.ToString();
			}

			return this as TBuilder;
		}

		public virtual TBuilder Name(string name) {
			Component.Name = name;
			return this as TBuilder;
		}

		public virtual TBuilder Css(string className) {
			Component.MergeAttribute("class", className, true);
			return this as TBuilder;
		}

		public virtual TBuilder Style(string cssText) {
			Component.MergeAttribute("style", cssText);
			return this as TBuilder;
		}

		public virtual TBuilder Width(int width) {
			Component.Width = width;
			return this as TBuilder;
		}

		public virtual TBuilder Height(int height) {
			Component.Height = height;
			return this as TBuilder;
		}

		public virtual TBuilder MergeAttribute(string key, object value) {
			Component.MergeAttribute(key, value.ToString());
			return this as TBuilder;
		}

		public virtual TBuilder HtmlAttributes(object htmlAttributes) {
			var attributes = htmlAttributes.ConvertTo<IDictionary<string, object>>();
			Component.MergeAttributes(attributes);
			return this as TBuilder;
		}

		public virtual TBuilder ToolTip(string title) {
			MergeAttribute("title", title);
			return this as TBuilder;
		}

		public virtual void Render() {
			RenderComponent();
		}

		protected void RenderComponent() {
			using (var writer = new HtmlTextWriter(ViewContext.Writer)) {
				Component.Render(writer);
			}
		}

		public virtual MvcHtmlString GetHtml() {
			return MvcHtmlString.Create(Component.ToHtmlString());
		}

		#region IHtmlString Members

		public string ToHtmlString() {
			return Component.ToHtmlString();
		}

		#endregion

		public override string ToString() {
			return ToHtmlString();
		}
	}
}
