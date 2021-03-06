﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EaseEasy.Web.Mvc.UI;

namespace EaseEasy.Web.Mvc.Html {
	public static class SearchViewExtensions {
		public static SearchViewBuilder Search(this HtmlHelper helper) {
			return new SearchViewBuilder(new SearchView(), helper.ViewContext, helper.ViewDataContainer).GenerateId();
		}
	}
}
