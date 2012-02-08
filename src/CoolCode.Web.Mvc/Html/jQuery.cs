using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Web;

namespace CoolCode.Web.Mvc.Html {
	/// <summary>
	/// jQuery扩展
	/// </summary>
	public static partial class jQueryExtensions {

		#region Get

		public static IHtmlString Get(this AjaxHelper helper, string linkText) {
			string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];
			return Get(helper, linkText, currentActionName);
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName) {
			return Get(helper, linkText, actionName, null);
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName, string controllerName) {
			return Get(helper, linkText, actionName, controllerName, null, new AjaxOptions());
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			string jsonParam) {
			return Get(helper, linkText, actionName, controllerName, null, jsonParam, new AjaxOptions());
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			string jsonParam, AjaxOptions ajaxOptions) {
			return Get(helper, linkText, actionName, controllerName, null, jsonParam, null, ajaxOptions);
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName,
		 object routeValues) {
			return Get(helper, linkText, actionName, routeValues, new AjaxOptions());
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName,
		 object routeValues, AjaxOptions ajaxOptions) {
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			return Get(helper, linkText, actionName, currentControllerName, routeValues, null, ajaxOptions);
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName, string controllerName,
		   object routeValues, AjaxOptions ajaxOptions) {
			return Get(helper, linkText, actionName, controllerName, routeValues, null, ajaxOptions);
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			object routeValues, object htmlAttributes, AjaxOptions ajaxOptions) {
			return Get(helper, linkText, actionName, controllerName, routeValues, null, htmlAttributes, ajaxOptions);
		}

		public static IHtmlString Get(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			object routeValues, string jsonParam, object htmlAttributes, AjaxOptions ajaxOptions) {
			return Ajax(helper, linkText, actionName, controllerName, routeValues, jsonParam, htmlAttributes, ajaxOptions, "get");
		}
		#endregion

		#region Post

		public static IHtmlString Post(this AjaxHelper helper, string linkText) {
			string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];
			return Post(helper, linkText, currentActionName);
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName) {
			return Post(helper, linkText, actionName, null);
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName, string controllerName) {
			return Post(helper, linkText, actionName, controllerName, null, new AjaxOptions());
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			string jsonParam) {
			return Post(helper, linkText, actionName, controllerName, null, jsonParam, new AjaxOptions());
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			string jsonParam, AjaxOptions ajaxOptions) {
			return Post(helper, linkText, actionName, controllerName, null, jsonParam, null, ajaxOptions);
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName,
		 object routeValues) {
			return Post(helper, linkText, actionName, routeValues, new AjaxOptions());
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName,
		 object routeValues, AjaxOptions ajaxOptions) {
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			return Post(helper, linkText, actionName, currentControllerName, routeValues, null, ajaxOptions);
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName, string controllerName,
		   object routeValues, AjaxOptions ajaxOptions) {
			return Post(helper, linkText, actionName, controllerName, routeValues, null, ajaxOptions);
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			object routeValues, object htmlAttributes, AjaxOptions ajaxOptions) {
			return Post(helper, linkText, actionName, controllerName, routeValues, null, htmlAttributes, ajaxOptions);
		}

		public static IHtmlString Post(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			object routeValues, string jsonParam, object htmlAttributes, AjaxOptions ajaxOptions) {
			return Ajax(helper, linkText, actionName, controllerName, routeValues, jsonParam, htmlAttributes, ajaxOptions, "post");
		}

		#endregion

		#region Json

		public static IHtmlString Json(this AjaxHelper helper, string linkText) {
			string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];
			return Json(helper, linkText, currentActionName);
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName) {
			return Json(helper, linkText, actionName, null);
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName, string controllerName) {
			return Json(helper, linkText, actionName, controllerName, null, new AjaxOptions());
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			string jsonParam) {
			return Json(helper, linkText, actionName, controllerName, null, jsonParam, new AjaxOptions());
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			string jsonParam, AjaxOptions ajaxOptions) {
			return Json(helper, linkText, actionName, controllerName, null, jsonParam, null, ajaxOptions);
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName,
		 object routeValues) {
			return Json(helper, linkText, actionName, routeValues, new AjaxOptions());
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName,
		 object routeValues, AjaxOptions ajaxOptions) {
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			return Json(helper, linkText, actionName, currentControllerName, routeValues, null, ajaxOptions);
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName, string controllerName,
		   object routeValues, AjaxOptions ajaxOptions) {
			return Json(helper, linkText, actionName, controllerName, routeValues, null, ajaxOptions);
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			object routeValues, object htmlAttributes, AjaxOptions ajaxOptions) {
			return Json(helper, linkText, actionName, controllerName, routeValues, null, htmlAttributes, ajaxOptions);
		}

		public static IHtmlString Json(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			object routeValues, string jsonParam, object htmlAttributes, AjaxOptions ajaxOptions) {
			return Ajax(helper, linkText, actionName, controllerName, routeValues, jsonParam, htmlAttributes, ajaxOptions, "getJSON");
		}

		#endregion

		#region Ajax

		private static IHtmlString Ajax(this AjaxHelper helper, string linkText, string actionName, string controllerName,
			object routeValues, string jsonParam, object htmlAttributes, AjaxOptions ajaxOptions, string method) {
			string linkFormat = "<a href=\"{0}\" {1} {3}>{2}</a>";
			string attributes = string.Empty;
			string onclick = string.Empty;
			if (htmlAttributes != null) {
				attributes = ConvertObjectToAttribute(htmlAttributes);
			}
			UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);
			string action = routeValues == null ? url.Action(actionName, controllerName) : url.Action(actionName, controllerName, routeValues, "");

			if (string.IsNullOrEmpty(jsonParam)) {
				jsonParam = "$('form').serialize()";
			}

			if (ajaxOptions != null) {
				string confirmScript = string.Empty;
				if (!string.IsNullOrEmpty(ajaxOptions.Confirm)) {
					confirmScript = string.Format("var result = confirm('{0}'); if(!result)return false;", ajaxOptions.Confirm);
				}

				string updateScript = string.Empty;
				if (!string.IsNullOrEmpty(ajaxOptions.UpdateTargetId)) {
					updateScript = string.Format("$('{0}').html(data);", FormatElement(ajaxOptions.UpdateTargetId));
				}

				string showLoading = string.Empty;
				string hideLoading = string.Empty;
				if (!string.IsNullOrEmpty(ajaxOptions.LoadingElementId)) {
					string loading = FormatElement(ajaxOptions.LoadingElementId);
					showLoading = string.Format("$('{0}').show();", loading);
					hideLoading = string.Format("$('{0}').hide();", loading);
				}

				onclick = string.Format("onclick=\"{0}$.{4}('{1}',{2},{3});return false;\" ",
				   confirmScript + ajaxOptions.OnBegin + showLoading,
					action,
					jsonParam,
				   "function(data){" + updateScript + hideLoading + ajaxOptions.OnSuccess + "}",
				   method
				 );
			}

			string html = string.Format(CultureInfo.InvariantCulture, linkFormat, "#", attributes, linkText, onclick);

			return html.ToHtmlString();
		}

		#endregion

		#region Load

		public static IHtmlString Load(this AjaxHelper helper, string el) {
			return Load(helper, el, (string)helper.ViewContext.RouteData.Values["action"]);
		}

		public static IHtmlString Load(this AjaxHelper helper, string el, string actionName) {
			return Load(helper, el, actionName, null);
		}

		public static IHtmlString Load(this AjaxHelper helper, string el, string actionName, object routeValues) {
			string controllerName = (string)helper.ViewContext.RouteData.Values["controller"];

			return Load(helper, el, actionName, controllerName, routeValues);
		}

		public static IHtmlString Load(this AjaxHelper helper, string el, string actionName, string controllerName, object routeValues) {
			if (string.IsNullOrEmpty(el))
				throw new ArgumentNullException("el");

			UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);
			string action = routeValues == null ? url.Action(actionName, controllerName) : url.Action(actionName, controllerName, routeValues, "");

			el = FormatElement(el);
			string html = string.Format("$('{0}').load('{1}');", el, action);

			return html.ToHtmlString();
		}

		#endregion

		#region Toggle

		public static IHtmlString Toggle(this HtmlHelper helper, string linkText, string el, object htmlAttributes = null) {
			return helper.Toggle(linkText, linkText, el, htmlAttributes);
		}

		public static IHtmlString Toggle(this HtmlHelper helper, string showText, string hideText, string el, object htmlAttributes = null) {
			if (string.IsNullOrEmpty(el))
				throw new ArgumentNullException("el");

			string linkFormat = "<a href=\"#\" {1} onclick=\"$('{0}').toggle();{3}return false;\">{2}</a>";

			string attributes = null;
			if (htmlAttributes != null) {
				attributes = ConvertObjectToAttribute(htmlAttributes);
			}

			string html = string.Format(linkFormat, FormatElement(el), attributes, showText,
				(showText.Equals(hideText) ? "" : string.Format("if($(this).html()=='{0}'){{$(this).html('{1}');}}else{{$(this).html('{0}');}}",
						helper.Encode( showText), 
						helper.Encode(hideText)
				))
			);

			return html.ToHtmlString();
		}

		#endregion

		#region Common

		internal static string FormatElement(string el) {
			if (string.IsNullOrEmpty(el)) {
				return el;
			}
			/*如果以英文开头则认为是id*/
			if (el[0] > 'A' && el[0] < 'z') {
				el = "#" + el;
			}

			return el;
		}

		internal static string ConvertObjectToAttribute(this object value) {
			StringBuilder sb = new StringBuilder();
			if (value != null) {
				IDictionary<string, object> d = value as IDictionary<string, object>;
				if (d == null) {
					d = new RouteValueDictionary(value);
				}

				string resultFormat = "{0}=\"{1}\" ";
				foreach (string attribute in d.Keys) {
					object thisValue = d[attribute];
					if (d[attribute] is bool) {
						thisValue = d[attribute].ToString().ToLowerInvariant();
					}
					sb.AppendFormat(resultFormat, attribute.Replace("_", "").ToLowerInvariant(), thisValue);
				}
			}
			return sb.ToString();
		}

		#endregion
	}
}
