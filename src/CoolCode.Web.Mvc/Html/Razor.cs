using System;
using System.Collections.Generic;
using System.Web.WebPages;

namespace CoolCode.Web.Mvc.Html {
	public static class RazorExtensions {
		/// <summary>
		/// <example>
		/// <![CDATA[
		/// @items.List(@<li>@item</li>)
		/// <table>
		/// @items.List(
		///  @<tr>
		///      <td>@item.Title</td>
		///      <td>@item.Publisher</td>
		///  </tr>)
		/// </table>]]> 
		/// </example>
		/// <see cref="http://haacked.com/archive/2011/02/27/templated-razor-delegates.aspx"/>
		/// </summary> 
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="template"></param>
		/// <returns></returns>
		public static HelperResult List<T>(this IEnumerable<T> items, Func<T, HelperResult> template) {
			return new HelperResult(writer => {
				foreach (var item in items) {
					template(item).WriteTo(writer);
				}
			});
		}
	}
}
