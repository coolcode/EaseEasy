using System.Web.Mvc;

namespace EaseEasy.Web.Mvc.UI {
	public class LinkBuilder : TemplateViewBuilder<Link, LinkBuilder> {
		public LinkBuilder(Link component, ViewContext viewContext, IViewDataContainer viewContainer)
			: base(component, viewContext, viewContainer) {
		}

		public LinkBuilder OnClick(string script) {
			Component.MergeAttribute("onclick", script);
			return this;
		}

		public LinkBuilder Href(string value) {
			Component.MergeAttribute("href", value, true);
			return this;
		}
	}
}
