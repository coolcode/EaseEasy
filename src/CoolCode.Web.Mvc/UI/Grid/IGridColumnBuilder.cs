using System;

namespace CoolCode.Web.Mvc.UI {
	public interface IGridColumnBuilder<T> : IFluentViewComponentBuilder<GridColumn<T>, IGridColumnBuilder<T>> {
		IGridColumnBuilder<T> Header(string value);
		IGridColumnBuilder<T> Header(Func<GridColumn<T>, object> func);
		IGridColumnBuilder<T> Sortable(bool value);
		IGridColumnBuilder<T> Format(string value);
		IGridColumnBuilder<T> Template(Func<T, object> func);
		IGridColumnBuilder<T> Partial(string partialName);
		IGridColumnBuilder<T> HtmlEncode(bool value);
		IGridColumnBuilder<T> CheckBox();
		IGridColumnBuilder<T> Align(Alignment alignment);
	}
}
