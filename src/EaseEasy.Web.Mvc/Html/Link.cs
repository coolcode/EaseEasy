using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EaseEasy.Web.Mvc.UI;

namespace EaseEasy.Web.Mvc.Html {
	public static class LinkExtensions {
		public static LinkBuilder Link(this HtmlHelper helper) {
			return helper.Element<Link, LinkBuilder>();
		}
	}
}
