using System;
using System.Linq.Expressions;

namespace EaseEasy.Web.Mvc.UI {
	public interface IGridColumnCollectionBuilder<T> {
		IGridColumnBuilder<T> Column(string name = "");
		IGridColumnBuilder<T> Column(Expression<Func<T, object>> propertySpecifier);
	}
}
