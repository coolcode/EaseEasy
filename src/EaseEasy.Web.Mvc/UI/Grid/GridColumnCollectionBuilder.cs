using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using EaseEasy.Linq.Expressions;

namespace EaseEasy.Web.Mvc.UI {
	public class GridColumnCollectionBuilder<T> : ViewComponentBuilder<GridColumnCollection<T>, GridColumnCollectionBuilder<T>>, IGridColumnCollectionBuilder<T> {
		public GridColumnCollectionBuilder(GridColumnCollection<T> component, ViewContext context, IViewDataContainer container)
			: base(component, context, container) {

		}

		public IGridColumnBuilder<T> Column(Expression<Func<T, object>> propertySpecifier) {
			var memberExpression = propertySpecifier.GetMemberExpression();
			var columnName = memberExpression == null ? null : memberExpression.Member.Name;

			var column = new GridColumn<T>();
			column.RawValueFunc = propertySpecifier.Compile();
			column.Name = columnName;

			var columnBuilder = new GridColumnBuilder<T>(column, ViewContext, ViewDataContainer);
			this.Component.Add(column);

			return columnBuilder;
		}

		public IGridColumnBuilder<T> Column(string name = "") {
			var column = new GridColumn<T>();
			column.Name = name;

			var columnBuilder = new GridColumnBuilder<T>(column, ViewContext, ViewDataContainer);
			this.Component.Add(column);

			return columnBuilder;
		}

		public IGridColumnBuilder<T> TemplateColumn(Func<T, object> template) {
			return this.Column().Template(template);
		}
	}
}
