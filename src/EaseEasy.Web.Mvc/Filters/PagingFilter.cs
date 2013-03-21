using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using EaseEasy.Linq;
using EaseEasy.Linq.Dynamic;
using System.Collections;
using System.Collections.Generic;

namespace EaseEasy.Web.Mvc {
	/// <summary>
	/// 分页ActionFilter
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public class PagingFilterAttribute : ActionFilterAttribute {
		public string Sort { get; set; }
		public string SortFieldName { get; set; }
		public int PageSize { get; set; }
		public string PageFieldName { get; set; }
		public string ViewDataKey { get; set; }

		public PagingFilterAttribute() {
			PageFieldName = "page";
			Sort = string.Empty;
			SortFieldName = "sort";
			PageSize = 10;
		}

		private int pageIndex = 0;

		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			if (filterContext.Controller.ContainsKey(PageFieldName)) {
				pageIndex = filterContext.Controller.ValueOf<int>(PageFieldName);
			}

			if (filterContext.Controller.ContainsKey(SortFieldName)) {
				Sort = filterContext.Controller.ValueOf<string>(SortFieldName);
			}

			//base.OnActionExecuting(filterContext);            
		}

		public override void OnResultExecuting(ResultExecutingContext filterContext) {
			if (!(filterContext.Result is ViewResultBase)) {
				throw new NotSupportedException("filterContext.Result must be ViewResultBase type!");
			}

			ViewResultBase result = (ViewResultBase)filterContext.Result;

			object model = string.IsNullOrEmpty(ViewDataKey) ? result.ViewData.Model : result.ViewData.Eval(ViewDataKey);

			if (!(model is IQueryable)) {
				throw new NotSupportedException("Model must be IQueryable type!");
			}

			var query = (IQueryable<dynamic>)model;

			var queryHelper = new QueryHelper(query);

			var newModel = queryHelper.Wrap(Sort, pageIndex, PageSize);
			;
			if (!string.IsNullOrEmpty(ViewDataKey)) {
				result.ViewData[ViewDataKey] = newModel;
			}
			else {
				result.ViewData.Model = newModel;
			}
			//base.OnResultExecuting(filterContext);
		}

		class QueryHelper {

			private IQueryable<dynamic> _dataSource;

			public QueryHelper(IQueryable<dynamic> dataSource) {
				_dataSource = dataSource;
			}

			public IEnumerable<dynamic> Wrap(string sort, int pageIndex, int pageSize) {
				Type elementType = _dataSource.GetType().GetGenericArguments()[0];
				IQueryable<dynamic> orderedDataSource = _dataSource.OrderBy(sort);


				object pagedDataSource = Activator.CreateInstance(
						typeof(PaginatedList<>).MakeGenericType(elementType),
						new object[] { orderedDataSource, pageIndex, pageSize });
				;
				return pagedDataSource as IEnumerable<dynamic>;
			}
		}

	}
}
