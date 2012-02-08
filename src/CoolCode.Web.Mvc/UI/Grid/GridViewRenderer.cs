using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using CoolCode.Linq;
using CoolCode.Web.Mvc.Html;

namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// 渲染Grid
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GridViewRenderer<T> : TableViewRenderer<T> {
		protected readonly HtmlHelper htmlHelper;

		public GridViewRenderer(ViewContext context, IViewDataContainer container)
			: base(context, container) {
			htmlHelper = new HtmlHelper(ViewContext, ViewDataContainer);
		}

		public IPageable PagerDataSource { get; set; }

		protected override void RenderHeaderText(GridColumn<T> column) {
			if (column.Sortable) {
				var sortLink = htmlHelper.Link()
					.Css(this.GridModel.IsAjax ? "ajax-headerLink" : "headerLink")
					.Href(CreateDefaultSortUrl(column))
					.Template(c => column.GetHeader());

				Dom.Write(sortLink.ToHtmlString());
			}
			else {
				base.RenderHeaderText(column);
			}
		}

		protected override void RenderGridStart() {
			DataSource = HandleDataSource(DataSource);
			base.RenderGridStart();
		}

		protected virtual IEnumerable<T> HandleDataSource(IEnumerable<T> dataSource) {
			IEnumerable<T> result = dataSource;

			var queryableDataSource = dataSource.AsQueryable();

			if (this.GridModel.Sortable) {
				Type elementType = GetElementType(queryableDataSource);
				result = Sort(queryableDataSource, elementType);
			}

			if (this.GridModel.Pagable) {
				result = Page(result);
			}

			return result;
		}

		protected internal static Type GetElementType(IQueryable<T> source) {
			Debug.Assert(source != null, "source cannot be null");
			Type sourceType = source.GetType();

			if (sourceType.IsArray) {
				return sourceType.GetElementType();
			}

			if (sourceType.GetGenericTypeDefinition() == typeof(EnumerableQuery<>)) {
				return source.Count() > 0 ? source.FirstOrDefault().GetType() : typeof(T);
			}

			Type elementType = sourceType.GetInterfaces().Select(GetGenericEnumerableType).FirstOrDefault(t => t != null);

			Debug.Assert(elementType != null);

			return elementType;
		}

		private static Type GetGenericEnumerableType(Type type) {
			Type enumerableType = typeof(IEnumerable<>);
			if (type.IsGenericType && enumerableType.IsAssignableFrom(type.GetGenericTypeDefinition())) {
				return type.GetGenericArguments()[0];
			}

			return null;
		}

		private IQueryable<T> Sort(IQueryable<T> dataSource, Type elementType) {
			var queryableDataSource = dataSource;

			string sortValue = ViewContext.Controller.ValueOf<string>(this.GridModel.SortFieldName);
			string sortDirValue = ViewContext.Controller.ValueOf<string>(this.GridModel.SortDirFieldName) ?? SortOrder.Ascending.ToString();

			//是否触发了排序
			bool tiggerSorting = !string.IsNullOrEmpty(sortValue);

			//如果没触发排序，取默认排序值
			if (!tiggerSorting) {
				if (!string.IsNullOrEmpty(this.GridModel.DefaultSort)) {
					sortValue = this.GridModel.DefaultSort;
				}
				else {
					//注意：EF分页必须先指定排序，因此给它默认按第一个属性排序
					//TODO：需要优化EF排序逻辑！默认按Key排序
					//判断是否已经排序过
					if (dataSource.Expression.Type.IsGenericType &&
						dataSource.Expression.Type.GetGenericTypeDefinition() != typeof(IOrderedQueryable<>)) {
						sortValue = elementType.GetProperties().First(p =>
								p.PropertyType.In(typeof(string), typeof(int), typeof(DateTime), typeof(Guid))
							).Name;
					}
				}
			}
			else {
				var sortColumn = this.GridModel.Columns.FirstOrDefault(c => c.Name == sortValue);
				if (sortColumn == null) {
					throw new ArgumentException("Cannot found the column:" + sortValue);
				}
				sortColumn.SortOptions.SortOrder =
					(sortDirValue == SortOrder.Ascending.ToString() ? SortOrder.Descending : SortOrder.Ascending);
			}


			if (string.IsNullOrEmpty(sortValue)) {
				return queryableDataSource;
			}

			var sourceParam = Expression.Parameter(elementType, "source");
			var sourceProp = Expression.Property(sourceParam, sortValue);
			Type propertyType = sourceProp.Type;

			//a) 如果是非IQueryable类型，如List,Array
			if (dataSource.GetType().GetGenericTypeDefinition() == typeof(EnumerableQuery<>)) {
				var listType = typeof(List<>).MakeGenericType(elementType);
				var listInstance = Activator.CreateInstance(listType);
				var listAddMethod = listType.GetMethod("Add");

				foreach (var row in dataSource) {
					listAddMethod.Invoke(listInstance, new object[] { row });
				}

				MethodInfo m = this.GetType().GetMethod("SortGenericExpression", BindingFlags.Static | BindingFlags.NonPublic);
				m = m.MakeGenericMethod(elementType, propertyType);

				var v = (IEnumerable<T>)m.Invoke(null, new object[] { listInstance, sourceProp, sourceParam, sortDirValue });

				queryableDataSource = v.AsQueryable();
			}
			else {
				//b) IQueryable类型
				//需要验证:当匿名类型情况下，dataSource.OrderBy 方法会报错，此时T是object类型
				string orderMethod = (sortDirValue == SortOrder.Ascending.ToString() ? "OrderBy" : "OrderByDescending");
				var orderExpression = Expression.Call(typeof(Queryable), orderMethod,
													  new Type[] { elementType, propertyType }, queryableDataSource.Expression,
													  Expression.Lambda(sourceProp, sourceParam));

				//注：这里不能使用Cast<T>()，匿名类型不支持Case<T> 
				queryableDataSource = (IOrderedQueryable<T>)queryableDataSource.Provider.CreateQuery(orderExpression);
			}

			return queryableDataSource;
		}

		//注：此方法在排序时反射调用
		private static IEnumerable<TElement> SortGenericExpression<TElement, TProperty>(
			IEnumerable<TElement> data,
			Expression body,
			ParameterExpression param,
			string sortDirection) {

			Debug.Assert(data != null);
			Debug.Assert(body != null);
			Debug.Assert(param != null);

			Expression<Func<TElement, TProperty>> lambdaExpr = Expression.Lambda<Func<TElement, TProperty>>(body, param);
			var lambda = lambdaExpr.Compile();

			return (sortDirection == SortOrder.Ascending.ToString() ?
				  data.OrderBy(lambda) :
				  data.OrderByDescending(lambda));
		}

		private IEnumerable<T> Page(IEnumerable<T> dataSource) {
			var pageParam = GetPageParam();
			var pagingSource = dataSource.Paging(pageParam);
			if (this.PagerDataSource == null) {
				this.PagerDataSource = pagingSource;
			}

			return pagingSource;
		}

		private PageParam GetPageParam() {
			int pageIndex = ViewContext.Controller.ValueOf<int>(this.GridModel.PageFieldName);

			return new PageParam(pageIndex, GridModel.PageSize);
		}

		protected override void RenderTableEnd() {
			if (GridModel.Pagable && PagerDataSource != null) {
				RenderPager();
			}

			base.RenderTableEnd();
		}

		protected virtual void RenderPager() {
			Dom.CreateTag("tfoot");
			Dom.CreateTag("tr");
			var td = Dom.CreateTag("td");
			td.Attributes.Add("colspan", "6");

			var pager = htmlHelper.Pager(PagerDataSource)
				.LinkCss(this.GridModel.IsAjax ? "ajax-pagerLink" : "pagerLink")
				.PageSize(this.GridModel.PageSize)
				.Mode(this.GridModel.PageMode)
				.PageFieldName(this.GridModel.PageFieldName)
				.First(this.GridModel.PageFirstText)
				.Previous(this.GridModel.PagePreviousText)
				.Next(this.GridModel.PageNextText)
				.Last(this.GridModel.PageLastText)
				.Summary(this.GridModel.PageSummaryFormat);

			Dom.Write(pager.ToHtmlString());

			Dom.Write("<div class=\"clear\"></div>");

			Dom.EndTag();
			Dom.EndTag();
			Dom.EndTag();
		}

		protected virtual string CreateDefaultSortUrl(GridColumn<T> column) {
			var routeValues = new Dictionary<string, object>();
			routeValues.Add(this.GridModel.SortFieldName, column.Name);
			routeValues.Add(this.GridModel.SortDirFieldName, column.SortOptions.SortOrder);

			return ViewContext.Url(routeValues);
		}
	}

}
