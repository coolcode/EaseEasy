using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CoolCode.Web.Mvc.UI {
	public class SearchViewBuilder : TemplateViewBuilder<SearchView, SearchViewBuilder> {
		public SearchViewBuilder(SearchView component, ViewContext context, IViewDataContainer container)
			: base(component, context, container) {
		}

		public SearchViewBuilder Target(string target) {
			this.Component.Target = target;
			return this;
		}
	}
}
