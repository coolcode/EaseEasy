using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EaseEasy.Web.Mvc {
	public static class UrlHelperExtensions {
		public static string FullAction(this UrlHelper helper, string actionName) {
			return GetSiteRoot(helper.RequestContext.HttpContext.Request) + helper.Action(actionName);
		}

		public static string FullAction(this UrlHelper helper, string actionName, object routeValues) {
			return GetSiteRoot(helper.RequestContext.HttpContext.Request) + helper.Action(actionName, routeValues);
		}

		public static string FullAction(this UrlHelper helper, string actionName, RouteValueDictionary routeValues) {
			return GetSiteRoot(helper.RequestContext.HttpContext.Request) + helper.Action(actionName, routeValues);
		}

		public static string FullAction(this UrlHelper helper, string actionName, string controllerName) {
			return GetSiteRoot(helper.RequestContext.HttpContext.Request) + helper.Action(actionName, actionName, controllerName);
		}

		public static string FullAction(this UrlHelper helper, string actionName, string controllerName, object routeValues) {
			return GetSiteRoot(helper.RequestContext.HttpContext.Request) + helper.Action(actionName, controllerName, routeValues);
		}

		public static string FullAction(this UrlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues) {
			return GetSiteRoot(helper.RequestContext.HttpContext.Request) + helper.Action(actionName, controllerName, routeValues);
		}

		private static string  GetSiteRoot(HttpRequestBase request)
        { 
            if (!request.Url.IsDefaultPort)
            {
                return string.Format("http://{0}:{1}", request.Url.Host, request.Url.Port );
            }
            return "http://"+request.Url.Host;
        } 
	}
}
