using System;
using System.Globalization;
using System.Web.Mvc;

namespace CoolCode.Web.Mvc {
	public static class ControllerExtensions {
		public static bool ContainsKey(this System.Web.Mvc.ControllerBase controller, string key) {
			return controller.ValueProvider.GetValue(key) != null;
		}

		public static T ValueOf<T>(this System.Web.Mvc.ControllerBase controller, string key, T defaultValue = default(T)) {
			if (controller.ControllerContext.RouteData.Values.ContainsKey(key) &&
				controller.ControllerContext.RouteData.Values[key] != (object)string.Empty) {
				object value = controller.ControllerContext.RouteData.Values[key];
				return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentUICulture);
			}

			ValueProviderResult result = controller.ValueProvider.GetValue(key);

			if (result != null && result.RawValue != (object)string.Empty) {
				return (T)result.ConvertTo(typeof(T), CultureInfo.CurrentUICulture);
			}

			return defaultValue;
		}

		public static T AttemptedValueOf<T>(this System.Web.Mvc.ControllerBase controller, string key, T defaultValue = default(T)) {
			ValueProviderResult result = controller.ValueProvider.GetValue(key);

			if (result != null && !string.IsNullOrEmpty(result.AttemptedValue)) {
				return (T)Convert.ChangeType(result.AttemptedValue, typeof(T), CultureInfo.CurrentUICulture);
			}

			return defaultValue;
		}

		/// <summary> 
		/// 重定向到上一个Action. 即 header 的 "HTTP_REFERER"  (<c>Context.UrlReferrer</c>).
		/// </summary>
		public static RedirectResult RedirectToReferrer(this Controller controller) {
			return new RedirectResult(
				controller.Request.ServerVariables["HTTP_REFERER"]
				);
		}

		/// <summary> 
		/// Redirect 到站点根目录 (<c>Context.ApplicationPath + "/"</c>).
		/// </summary>
		public static RedirectResult RedirectToSiteRoot(this Controller controller) {
			return new RedirectResult(controller.Request.ApplicationPath + "/");
		}
	}
}
