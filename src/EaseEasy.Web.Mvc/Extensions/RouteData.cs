using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace EaseEasy.Web.Mvc {

	public static class RouteDataExtensions {
		public static string ToQueryString(this  RouteData routeData) {
			if (routeData.Values.Count == 0) {
				return string.Empty;
			}

			var b = new StringBuilder();
			foreach (var data in routeData.Values) {
				b.AppendFormat("{0}={1}&", data.Key, data.Value);
			}

			return b.ToString(0, b.Length - 1);
		}
	}
}
