using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CoolCode.Web.Mvc.UI;

namespace CoolCode.Web.Mvc.Html {
	public static class GridExtensions {
		#region GridLoader

		public static MvcHtmlString GridLoader(this HtmlHelper helper, string actionName) {
			return GridLoader(helper, "g" + Guid.NewGuid().ToString("N"), actionName);
		}

		public static MvcHtmlString GridLoader(this HtmlHelper helper, string id, string actionName) {
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			return GridLoader(helper, id, actionName, currentControllerName);
		}

		public static MvcHtmlString GridLoader(this HtmlHelper helper, string id, string actionName, object routeValues) {
			string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			return GridLoader(helper, id, actionName, currentControllerName, routeValues);
		}

		public static MvcHtmlString GridLoader(this HtmlHelper helper, string id, string actionName, string controllerName) {
			return GridLoader(helper, id, actionName, controllerName, null);
		}

		public static MvcHtmlString GridLoader(this HtmlHelper helper, string id, string actionName, string controllerName, object routeValues) {
			TagBuilder builder = new TagBuilder("div");
			builder.Attributes["class"] = "gridview";
			builder.Attributes["id"] = id;

			UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
			builder.Attributes["url"] = urlHelper.Action(actionName, controllerName);

			builder.InnerHtml = helper.Action(actionName, controllerName, routeValues).ToHtmlString();

			string html = builder.ToString(TagRenderMode.Normal);

			return MvcHtmlString.Create(html);
		}

		#endregion

		#region Grid

		/// <summary>
		/// Creates a grid using the dynamic datasource.
		/// </summary> 
		/// <returns></returns>
		public static IGridViewBuilder<dynamic> Grid(this HtmlHelper helper) {
			/*
			var genericArguments = helper.ViewContext.ViewData.Model.GetType().GetGenericArguments();
			var component = Activator.CreateInstance(typeof(GridView<>).MakeGenericType(genericArguments));
			var builderType = typeof(GridViewBuilder<>).MakeGenericType(genericArguments);
			var builder = Activator.CreateInstance(builderType, component, helper.ViewContext, helper.ViewDataContainer);

			builderType.GetMethod("Bind").Invoke(builder, new object[] { helper.ViewContext.ViewData.Model });

			return (IGridViewBuilder)builder;
			*/
			return helper.Grid<dynamic>();
		}

		/// <summary>
		/// Creates a grid using the specified datasource.
		/// </summary>
		/// <typeparam name="T">Type of datasouce element</typeparam>
		/// <returns></returns>
		public static IGridViewBuilder<T> Grid<T>(this HtmlHelper helper) {
			var dataSource = helper.ViewContext.ViewData.Model as IEnumerable<T>;
			if (dataSource == null) {
				throw new InvalidOperationException(string.Format("Model in ViewData is not an IEnumerable of '{0}'", typeof(T).Name));
			}

			return helper.Grid(dataSource);
		}

		/// <summary>
		/// Creates a grid from an entry in the viewdata.
		/// </summary>
		/// <typeparam name="T">Type of element in the grid datasource.</typeparam>
		/// <param name="viewDataKey">The ViewData key for data source</param>
		/// <returns></returns>
		public static IGridViewBuilder<T> Grid<T>(this HtmlHelper helper, string viewDataKey) {
			var dataSource = helper.ViewContext.ViewData.Eval(viewDataKey) as IEnumerable<T>;

			if (dataSource == null) {
				throw new InvalidOperationException(string.Format("Item in ViewData with key '{0}' is not an IEnumerable of '{1}'.",
					viewDataKey,
					typeof(T).Name));
			}

			return helper.Grid(dataSource);
		}

		/// <summary>
		/// Creates a grid using the specified datasource.
		/// </summary>
		/// <typeparam name="T">Type of datasouce element</typeparam>
		/// <returns></returns>
		public static IGridViewBuilder<T> Grid<T>(this HtmlHelper helper, IEnumerable<T> dataSource) {
			return helper.Element<GridView<T>, GridViewBuilder<T>>().Bind(dataSource).Pager();
		}

		#endregion

		#region IGridViewBuilder

		public static IGridViewBuilder<T> AutoGenerateColumns<T>(this IGridViewBuilder<T> builder) {
			return builder.Columns(DynamicGenerateColumns);
		}

		private static void DynamicGenerateColumns<T>(IGridColumnCollectionBuilder<T> columnCollectionBuilder) {

			//TODO:去除EF外键关联的属性
			//bug:未支持匿名类型
			var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			/*from p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
							 where !p.GetCustomAttributes(true).Any(a => {
								 string attr = Convert.ToString(a);
								 return attr.Contains("Association");
							 })
							 select p;*/

			foreach (var property in properties) {
				var propertyExpression = PropertyToExpression<T>(property);
				columnCollectionBuilder.Column(propertyExpression).Sortable(true);
			}
		}

		private static Expression<Func<T, object>> PropertyToExpression<T>(PropertyInfo property) {
			var parameterExpression = Expression.Parameter(typeof(T), "x");
			Expression propertyExpression = Expression.Property(parameterExpression, property);

			if (property.PropertyType.IsValueType) {
				propertyExpression = Expression.Convert(propertyExpression, typeof(object));
			}

			var expression = Expression.Lambda<Func<T, object>>(
				propertyExpression,
				parameterExpression
			);

			return expression;
		}

		#endregion

	}
}
