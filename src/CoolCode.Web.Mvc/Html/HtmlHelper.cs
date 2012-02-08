using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI.WebControls;
using CoolCode.Linq.Expressions;

namespace CoolCode.Web.Mvc
{ 
	public static class MenuItemHelper
	{
		public static IHtmlString MenuItem(this HtmlHelper helper, string linkText, string actionName, string controllerName)
		{
			return MenuItem(helper, linkText, actionName, controllerName, null, null);
		}

		public static IHtmlString MenuItem(this HtmlHelper helper, string linkText, string actionName, object routeValues)
		{
			return MenuItem(helper, linkText, actionName, null, routeValues, null);
		}

		public static IHtmlString MenuItem(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
		{
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];

			var builder = new TagBuilder("li");

			if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase) && currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))
			{
				// Add selected class
				builder.AddCssClass("selected");
				builder.InnerHtml = linkText;
			}
			else
			{
				// Add link
				MvcHtmlString mvcHtml = helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
				builder.InnerHtml = mvcHtml.ToHtmlString();
			}

			// Render Tag Builder
			return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
		}
	}

	public static class SelectedHelper
	{
		public static IHtmlString Selected<T>(this HtmlHelper helper, T value1, T value2)
		{
			if (value1.Equals(value2))
				return MvcHtmlString.Create("class=\"selected\"");
			return MvcHtmlString.Empty;
		}
	}


	public static class ImageActionLinkHelper
	{
		public static IHtmlString ImageActionLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, object routeValues, object htmlAttributes)
		{
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];

			return helper.ImageActionLink(imageUrl, altText, actionName, currentControllerName, routeValues, htmlAttributes); 
		}

		public static IHtmlString ImageActionLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, string controllerName, object routeValues, object htmlAttributes)
		{
			var builder = new TagBuilder("img");
			builder.MergeAttribute("src", imageUrl);
			builder.MergeAttribute("alt", altText);
			var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, htmlAttributes);
			var html = link.ToHtmlString();
			string s = html.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));

			return MvcHtmlString.Create(s);
		}

		public static IHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string actionName, object routeValues, AjaxOptions ajaxOptions)
		{
			var builder = new TagBuilder("img");
			builder.MergeAttribute("src", imageUrl);
			builder.MergeAttribute("alt", altText);
			var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions);
			var html = link.ToHtmlString();
			string s = html.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));

			return MvcHtmlString.Create(s);
		}
	}

	public static class TableHelper
	{
		/// <summary>
		/// Sample：<%=Html.Table("myTable", (IList)ViewData.Model, null) %>
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="name"></param>
		/// <param name="items"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public static IHtmlString Table(this HtmlHelper helper, string name, IList items, IDictionary<string, object> attributes)
		{
			if (items == null || items.Count == 0 || string.IsNullOrEmpty(name))
			{
				return MvcHtmlString.Empty;
			}

			return BuildTable(name, items, attributes);
		}

		private static IHtmlString BuildTable(string name, IList items, IDictionary<string, object> attributes)
		{
			StringBuilder sb = new StringBuilder();
			BuildTableHeader(sb, items[0].GetType());

			foreach (var item in items)
			{
				BuildTableRow(sb, item);
			}

			TagBuilder builder = new TagBuilder("table");
			builder.MergeAttributes(attributes);
			builder.MergeAttribute("name", name);
			builder.InnerHtml = sb.ToString();
			return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
		}

		private static void BuildTableRow(StringBuilder sb, object obj)
		{
			Type objType = obj.GetType();
			sb.AppendLine("\t<tr>");
			foreach (var property in objType.GetProperties())
			{
				sb.AppendFormat("\t\t<td>{0}</td>\n", property.GetValue(obj, null));
			}
			sb.AppendLine("\t</tr>");
		}

		private static void BuildTableHeader(StringBuilder sb, Type p)
		{
			sb.AppendLine("\t<tr>");
			foreach (var property in p.GetProperties())
			{
				sb.AppendFormat("\t\t<th>{0}</th>\n", property.Name);
			}
			sb.AppendLine("\t</tr>");
		}
	}


	/// <summary>
	/// Extension methods on IEnumerable.
	/// </summary>
	public static class EnumerableExtensions
	{
		static public List<ListItem> ToListItem<T>()
		{
			var li = new List<ListItem>();
			foreach (int s in Enum.GetValues(typeof(T)))
			{
				li.Add(new ListItem
				{
					Value = s.ToString(),
					Text = Enum.GetName(typeof(T), s)
				});
			}
			return li;
		}
		/// <summary>
		/// Converts an enumerable into a SelectList.
		/// </summary>
		/// <typeparam name="T">Type of item in the collection</typeparam>
		/// <typeparam name="TValueField">Type of the value field</typeparam>
		/// <param name="items">Items to convert into a select list</param>
		/// <param name="valueFieldSelector">Expression used to identify the data value field</param>
		/// <param name="textFieldSelector">Expression used to identify the data text field</param>
		/// <returns>A populated SelectList</returns>
		public static SelectList ToSelectList<T, TValueField>(this IEnumerable<T> items, Expression<Func<T, TValueField>> valueFieldSelector, Expression<Func<T, object>> textFieldSelector)
		{
			string textField = ExpressionToName(textFieldSelector);
			string valueField = ExpressionToName(valueFieldSelector);

			return new SelectList(items, valueField, textField);
		}

		/// <summary>
		/// Converts an enumerable into a SelectList.
		/// </summary>
		/// <typeparam name="T">Type of item in the collection</typeparam>
		/// <typeparam name="TValueField">Type of the value field</typeparam>
		/// <param name="items">Items to convert into a select list</param>
		/// <param name="valueFieldSelector">Expression used to identify the data value field</param>
		/// <param name="textFieldSelector">Expression used to identify the data text field</param>
		/// <param name="selectedValue">The selected value</param>
		/// <returns>A populated SelectList</returns>
		public static SelectList ToSelectList<T, TValueField>(this IEnumerable<T> items, Expression<Func<T, TValueField>> valueFieldSelector, Expression<Func<T, object>> textFieldSelector, TValueField selectedValue)
		{
			string textField = ExpressionToName(textFieldSelector);
			string valueField = ExpressionToName(valueFieldSelector);

			return new SelectList(items, valueField, textField, selectedValue);
		}

		/// <summary>
		/// Converts an enumerable into a SelectList.
		/// </summary>
		/// <typeparam name="T">Type of item in the collection</typeparam>
		/// <typeparam name="TValueField">Type of the value field</typeparam>
		/// <param name="items">Items to convert into a select list</param>
		/// <param name="valueFieldSelector">Expression used to identify the data value field</param>
		/// <param name="textFieldSelector">Expression used to identify the data text field</param>
		/// <param name="selectedValueSelector">A predicate that can be used to specify which values should be selected</param>
		/// <returns>A populated SelectList</returns>
		public static MultiSelectList ToSelectList<T, TValueField>(this IEnumerable<T> items, Expression<Func<T, TValueField>> valueFieldSelector, Expression<Func<T, object>> textFieldSelector, Func<T, bool> selectedValueSelector)
		{
			var selectedItems = items.Where(selectedValueSelector);
			string textField = ExpressionToName(textFieldSelector);
			string valueField = ExpressionToName(valueFieldSelector);

			return new MultiSelectList(items, valueField, textField, selectedItems);
		}

		private static string ExpressionToName<T, TValue>(Expression<Func<T, TValue>> expression)
		{
			var memberExpression = RemoveUnary(expression.Body) as MemberExpression;
			return memberExpression == null ? string.Empty : memberExpression.Member.Name;
		}

		private static Expression RemoveUnary(Expression body)
		{
			var unary = body as UnaryExpression;
			if (unary != null)
			{
				return unary.Operand;
			}
			return body;
		}
	}

	public static class DropDownListExtensions
	{
		/*
		public static IHtmlString DropDownListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier, string viewDataName)
		{
			viewDataName.ThrowIfNull();
			object value = htmlHelper.ViewData[viewDataName];
			value.ThrowIfNull(viewDataName);

			if (value as IEnumerable<SelectListItem> != null)
			{
				return htmlHelper.DropDownListFor(propertySpecifier, (IEnumerable<SelectListItem>)value);
			}
			else
			{
				return htmlHelper.DropDownListFor(propertySpecifier, (IEnumerable<ValueText>)value);
			}
		}
		*/

		public static IHtmlString DropDownListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier, IEnumerable<ValueText> source)
		{
			source.ThrowIfNull("source");
			/*
			return htmlHelper.DropDownListFor(propertySpecifier, source.ToSelectList());			
			 */
			string name = propertySpecifier.Body.GetMemberName();
			object value = htmlHelper.ViewData[name];

			IEnumerable<SelectListItem> listInfo = source.ToSelectList();

			SelectListItem selectedItem = listInfo.Where(c => c.Selected || c.Value == Convert.ToString(value)).FirstOrDefault();

			MvcHtmlString html = htmlHelper.DropDownList(name, listInfo);

			if (selectedItem != null)
			{
				string replaceText = string.Format("<option value=\"{0}\">", selectedItem.Value);
				string selectedText = string.Format("<option value=\"{0}\" selected=\"selected\">", selectedItem.Value);
				string correctHtml = html.ToHtmlString().Replace(replaceText, selectedText);
				html = MvcHtmlString.Create(correctHtml);
			}

			return html;
		}
	}

	/// <summary>
	/// TODO:与RadioButton同步
	/// </summary>
	public static class CheckBoxExtensions
	{
		public static IHtmlString CheckBoxListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier)
		{
			return htmlHelper.CheckBoxListFor(propertySpecifier, 0);
		}

		public static IHtmlString CheckBoxListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier, int repeatColumns)
		{
			string propertyName = propertySpecifier.Body.GetMemberName();
			return htmlHelper.CheckBoxList(propertyName, repeatColumns);
		}

		public static IHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name)
		{
			return htmlHelper.CheckBoxList(name, 0);
		}

		public static IHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, int repeatColumns)
		{
			object source = htmlHelper.ViewData[name];
			source.ThrowIfNull(name);

			IEnumerable<SelectListItem> listInfo = new List<SelectListItem>();

			if (source.GetType() == typeof(List<SelectListItem>))
			{
				listInfo = (List<SelectListItem>)source;
			}
			else if (source.GetType() == typeof(List<ValueText>))
			{
				listInfo = ((List<ValueText>)source).ToSelectList();
			}
			return htmlHelper.CheckBoxList(name, listInfo, repeatColumns);
		}

		public static IHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo, int repeatColumns)
		{
			listInfo.ThrowIfNull();
			return htmlHelper.CheckBoxList(name, listInfo, ((IDictionary<string, object>)null), repeatColumns);
		}

		public static IHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo, object htmlAttributes)
		{
			return htmlHelper.CheckBoxList(name, listInfo, ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)), 0);
		}

		public static IHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo, IDictionary<string, object> htmlAttributes, int repeatColumns)
		{
			return htmlHelper.InputList(name, listInfo, htmlAttributes, repeatColumns, "checkbox");
		}

		public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<ValueText> source)
		{
			IEnumerable<SelectListItem> listInfo = source
				.Select(c => new SelectListItem
				{
					Value = c.Value,
					Text = c.Text,
					Selected = c.Selected
				});
			return listInfo;
		}
	}

	public static class RadioButtonExtensions
	{
		public static IHtmlString RadioButtonListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier, IEnumerable<SelectListItem> listInfo)
		{
			string propertyName = propertySpecifier.Body.GetMemberName();
			return htmlHelper.RadioButtonList(propertyName, listInfo);
		}

		/*
		public static IHtmlString RadioButtonListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier)
		{
			return htmlHelper.RadioButtonListFor(propertySpecifier, 0);
		}

		public static IHtmlString RadioButtonListFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier, int repeatColumns)
		{
			string propertyName = propertySpecifier.Body.GetMemberName();
			return htmlHelper.RadioButtonList(propertyName, repeatColumns);
		}
		 * 
		public static IHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name)
		{
			return htmlHelper.RadioButtonList(name, 0);
		}

		public static IHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, int repeatColumns)
		{
			object source = htmlHelper.ViewData[name];
			source.ThrowIfNull(name);

			IEnumerable<SelectListItem> listInfo = new List<SelectListItem>();

			if (source.GetType() == typeof(List<SelectListItem>))
			{
				listInfo = (List<SelectListItem>)source;
			}
			else if (source.GetType() == typeof(List<ValueText>))
			{
				listInfo = ((List<ValueText>)source).ToSelectList();
			}
			return htmlHelper.RadioButtonList(name, listInfo, repeatColumns);
		}
		*/

		public static IHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo)
		{
			return htmlHelper.RadioButtonList(name, listInfo, 0);
		}

		public static IHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo, int repeatColumns)
		{
			listInfo.ThrowIfNull();
			return htmlHelper.RadioButtonList(name, listInfo, ((IDictionary<string, object>)null), repeatColumns);
		}

		public static IHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo, object htmlAttributes)
		{
			return htmlHelper.RadioButtonList(name, listInfo, ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)), 0);
		}

		public static IHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo, IDictionary<string, object> htmlAttributes, int repeatColumns)
		{
			return htmlHelper.InputList(name, listInfo, htmlAttributes, repeatColumns, "radio");
		}

		internal static IHtmlString InputList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> listInfo, IDictionary<string, object> htmlAttributes, int repeatColumns, string type)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("The argument must have a value", "name");
			if (listInfo == null)
				throw new ArgumentNullException("listInfo");
			if (string.IsNullOrEmpty(type))
				throw new ArgumentException("The argument must have a value", "type");

			string selectedValue = Convert.ToString(htmlHelper.ValueOf<object>(name));

			bool existSelectedValue = string.IsNullOrEmpty(selectedValue);

			Func<int, SelectListItem, string> buildRadioButton = (index, info) =>
			{
				TagBuilder RadioButtonBuilder = new TagBuilder("input");
				if ((info.Selected && !existSelectedValue) || info.Value == selectedValue)
				{
					RadioButtonBuilder.MergeAttribute("checked", "checked");
				}
				RadioButtonBuilder.MergeAttributes<string, object>(htmlAttributes);
				RadioButtonBuilder.MergeAttribute("type", type);
				RadioButtonBuilder.MergeAttribute("value", info.Value);
				RadioButtonBuilder.MergeAttribute("name", name);
				RadioButtonBuilder.GenerateId(name + "_" + index);

				TagBuilder labelBuilder = new TagBuilder("label");
				labelBuilder.MergeAttribute("for", RadioButtonBuilder.Attributes["id"]);
				labelBuilder.InnerHtml = info.Text;

				string result = RadioButtonBuilder.ToString(TagRenderMode.SelfClosing) + labelBuilder.ToString(TagRenderMode.Normal);
				return result;
			};

			int i = 0;
			StringBuilder sb = new StringBuilder();

			if (repeatColumns <= 0)
			{
				foreach (SelectListItem info in listInfo)
				{
					sb.Append(buildRadioButton(i++, info));
				}
			}
			else
			{
				int columnIndex = 0;
				sb.Append("<table border=\"0\">");
				foreach (SelectListItem info in listInfo)
				{
					i++;
					columnIndex++;
					bool isBeginColumn = columnIndex == 1;
					bool isEndColumn = columnIndex == repeatColumns;

					if (isBeginColumn)
					{
						sb.Append("<tr>");
					}

					sb.Append("<td>");
					sb.Append(buildRadioButton(i++, info));
					sb.Append("</td>");

					if (isEndColumn)
					{
						sb.Append("</tr>");
						columnIndex = 0;
					}
				}
				sb.Append("</table>");
			}

			return MvcHtmlString.Create(sb.ToString());
		}

		internal static T ValueOf<T>(this HtmlHelper htmlHelper, string key)
		{
			T result = htmlHelper.ViewContext.Controller.ValueOf<T>(key);

			if (result != null)
			{
				return result;
			}

			object model = htmlHelper.ViewData.Eval(key);
			if (model != null)
			{
				return (T)model;
			}

			return default(T);
		}
	}

	public static class DatePickerExtensions
	{
		public static IHtmlString DatePickerFor<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> propertySpecifier)
		{
			string propertyName = propertySpecifier.Body.GetMemberName();
			return htmlHelper.DatePicker(propertyName);
		}

		public static IHtmlString DatePicker(this HtmlHelper htmlHelper, string name)
		{
			return htmlHelper.DatePicker(name, "yyyy-MM-dd");
		}

		public static IHtmlString DatePicker(this HtmlHelper htmlHelper, string name, string format)
		{
			object source = htmlHelper.ValueOf<object>(name);
			string v = string.Empty;
			if (source != null && (DateTime)source != DateTime.MinValue)
			{
				v = ((DateTime)source).ToString(format);
			}
			return htmlHelper.TextBox(name, v, new { @class = "datepicker", onclick = "WdatePicker(); " });
		}
	}

	public static class OtherExtensions
	{
		internal static IHtmlString ToHtmlString(this string html) {
			return MvcHtmlString.Create(html);
		}

		public static IHtmlString Copyright(this HtmlHelper htmlHelper, int beginYear)
		{
			int currentYear = DateTime.Now.Year;
			if (currentYear > beginYear)
			{
				return MvcHtmlString.Create(string.Format("{0}-{1}", beginYear, currentYear));
			}
			return MvcHtmlString.Create(beginYear.ToString());
		}
	}
	 
}
