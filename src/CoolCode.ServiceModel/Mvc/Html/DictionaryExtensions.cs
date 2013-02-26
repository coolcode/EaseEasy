using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CoolCode.ServiceModel.Services;
using CoolCode.Web.Mvc;
using CoolCode.Web.Mvc.UI;

namespace CoolCode.Web.Mvc.Html {
	public static class DictionaryExtensions {

		private static readonly IDictionaryService _systemService = DependencyResolver.Current.GetService<IDictionaryService>();

		public static IGridColumnBuilder<T> Dictionary<T>(this IGridColumnBuilder<T> column, string dictionaryName) where T : class {
			return column.Template(instance => {
				var key = Convert.ToString(column.Component.GetRawValue(instance));
				string value = _systemService.GetDictionaryText(dictionaryName, key);
				return value;
			});

		}

		public static IHtmlString DropDownListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier, string dictionaryName) {
			var source = _systemService.ListEnabledValueText(dictionaryName);
			return htmlHelper.DropDownListFor(propertySpecifier, source);
		}

		public static IHtmlString DropDownList<T>(this HtmlHelper<T> htmlHelper, string name, string dictionaryName, bool includeAll) {
			return htmlHelper.DropDownList(name, dictionaryName, includeAll, string.Empty);
		}

		public static IHtmlString DropDownList<T>(this HtmlHelper<T> htmlHelper, string name, string dictionaryName, bool includeAll, string allText) {
			var source = _systemService.ListValueText(dictionaryName);

			if (includeAll) {
				var list = new List<ValueText>(source);
				list.Insert(0, new ValueText(string.Empty, allText));
				source = list;
			}

			return htmlHelper.DropDownList(name, source.ToSelectList());
		}

		public static IHtmlString RadioButtonListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier, string dictionaryName) {
			var source = _systemService.ListValueText(dictionaryName);
			return htmlHelper.RadioButtonListFor(propertySpecifier, source.ToSelectList());
		}

		public static IHtmlString RadioButtonList<T>(this HtmlHelper<T> htmlHelper, string name, string dictionaryName, bool includeAll) {
			return htmlHelper.RadioButtonList(name, dictionaryName, includeAll, "全部");
		}

		public static IHtmlString RadioButtonList<T>(this HtmlHelper<T> htmlHelper, string name, string dictionaryName, bool includeAll, string allText) {
			var source = _systemService.ListValueText(dictionaryName);

			if (includeAll) {
				var list = new List<ValueText>(source);
				list.Insert(0, new ValueText(string.Empty, allText));
				source = list;
			}

			return htmlHelper.RadioButtonList(name, source.ToSelectList());
		}

        public static IHtmlString DictionaryText(this HtmlHelper htmlHelper, string dictionaryName, string key) {
            var text = _systemService.GetDictionaryText(dictionaryName, key);

            return MvcHtmlString.Create(text);
        }
	}
}
