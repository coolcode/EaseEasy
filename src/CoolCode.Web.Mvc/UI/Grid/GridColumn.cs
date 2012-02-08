using System;
using System.Web;
using System.Web.UI;
using System.Web.WebPages;

namespace CoolCode.Web.Mvc.UI {
	public class GridColumn<T> : ViewComponent {
		public string Header { get; set; }

		public string Format { get; set; }

		public GridColumnSortOptions SortOptions { get; set; }

		public string SortExpression { get; set; }

		public IHtmlAttributes HeaderAttributes { get; set; }

		public bool Visible { get; set; }

		public bool Sortable { get; set; }

		public bool HtmlEncode { get; set; }

		public Func<GridColumn<T>, object> HeaderFunc { get; set; }

		public Func<T, object> TemplateFunc { get; set; }

		public Func<T, object> RawValueFunc { get; set; }

		public GridColumn() {
			Visible = true;
			HtmlEncode = true;
			Sortable = false;
			HeaderAttributes = new HtmlAttributes();
			SortOptions = new GridColumnSortOptions();
		}

		/// <summary>
		/// 从指定行对象，获取列的内容
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public virtual object GetValue(GridRow row) {
			object value;

			//从列模板读取，如果有则直接返回相应内容
			if (TemplateFunc != null) {
				Func<dynamic, object> template;
				if (row.Value != null && ((Type)row.Value.GetType()).IsAnonymous()) {
					template = x => TemplateFunc(x);
				}
				else {
					template = x => TemplateFunc(x.Value);
				}

				return Template(template, row, HtmlEncode).ToHtmlString();
			}

			//通过Lambda表达式指定字段名：从委托获取属性值
			//通过字符串指定字段名：通过反射获取
			value = GetRawValue(row.Value) ?? DataBinder.Eval(row.Value, Name);

			//格式化，一般用于日期、数值等转换
			if (!string.IsNullOrEmpty(Format)) {
				value = string.Format(Format, value);
			}

			//Html编码
			if (HtmlEncode && value != null) {
				value = HttpUtility.HtmlEncode(value.ToString());
			}

			return value;
		}

		public virtual string GetHeader() {
			return HeaderFunc == null ? Header ?? Name : Template(x => HeaderFunc(x), this, false).ToHtmlString();
		}

		private static HelperResult Template(Func<dynamic, object> format, dynamic arg, bool htmlEncode = true) {
			var result = format(arg);
			return new HelperResult(tw => {
				var helper = result as HelperResult;
				if (helper != null) {
					helper.WriteTo(tw);
					return;
				}
				IHtmlString htmlString = result as IHtmlString;
				if (htmlString != null) {
					tw.Write(htmlString);
					return;
				}
				if (result != null) {
					if (htmlEncode) {
						result = HttpUtility.HtmlEncode(result);
					}
					tw.Write(result);
				}
			});
		}

		public object GetRawValue(dynamic instance) {
			return RawValueFunc == null ? null : RawValueFunc(instance);
		}
	}
}
