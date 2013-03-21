using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using EaseEasy.Linq.Expressions;

namespace EaseEasy.Linq {
	public static class IQueryableExtensions {
		public static IQueryable<T> Paging<T>(this IQueryable<T> source, int pageIndex, int pageSize) {
			return source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
		}

		public static IQueryable<T> Paging<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, int pageIndex, int pageSize) {
			return source.Where(predicate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
		}

		public static IQueryable<T> In<T, P>(this IQueryable<T> source, Expression<Func<T, P>> property, IList<P> values) {
			values.ThrowIfNullOrEmpty("value");
			Expression<Func<T, bool>> lambda = ExpressionTreeHelpers.BuildContainsExpression(property, values);

			return source.Where(lambda);
		}
	}
}
