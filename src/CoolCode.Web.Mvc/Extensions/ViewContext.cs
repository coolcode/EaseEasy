using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace CoolCode.Web.Mvc
{
	public static class ViewContextExtensions
	{
		public static string Url(this ViewContext viewContext, IDictionary<string, object> routeValues = null, bool keepQueryString = true)
		{
			var newRouteValues = new RouteValueDictionary(viewContext.RouteData.Values);

			//保留查询字符到RouteValues
			if (keepQueryString)
			{
				var queryString = viewContext.HttpContext.Request.QueryString;
				foreach (string key in queryString.Keys)
				{
					newRouteValues[key] = queryString[key];
				}
			}

			//自定义参数
			if (routeValues != null)
			{
				routeValues.ForEach(v => newRouteValues[v.Key] = v.Value);
			}

			UrlHelper urlHelper = new UrlHelper(viewContext.RequestContext);
			string url = urlHelper.Action(viewContext.RouteData.Values["action"].ToString(), newRouteValues);

			return url;
		}
	}
}
