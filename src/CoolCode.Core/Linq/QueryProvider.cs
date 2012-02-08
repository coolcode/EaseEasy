using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CoolCode.Reflection;

namespace CoolCode.Linq {
	public abstract class QueryProvider : IQueryProvider {
		protected QueryProvider() {

		}

		IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression) {
			return new Query<S>(this, expression);
		}

		IQueryable IQueryProvider.CreateQuery(Expression expression) {
			Type elementType = expression.Type.GetElementTypeOrSelf();
			try {
				return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
			}
			catch (System.Reflection.TargetInvocationException tie) {
				throw tie.InnerException;
			}
		}

		S IQueryProvider.Execute<S>(Expression expression) {

			return (S)this.Execute(expression);

		}

		object IQueryProvider.Execute(Expression expression) {

			return this.Execute(expression);

		}

		public abstract string GetQueryText(Expression expression);

		public abstract object Execute(Expression expression);

	}
}
