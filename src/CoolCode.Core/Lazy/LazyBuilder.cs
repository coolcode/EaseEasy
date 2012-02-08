using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CoolCode {
	/// <summary>
	/// Make property's lazy loading as normal setting.
	/// </summary>
	public static class LazyBuilder {
		public static LazyBuilder<T> Create<T>() {
			return new LazyBuilder<T>();
		}
		/*
        public static LazyBuilder<T> Create<T>(Expression<Func<T>> expr)
        {
            throw new NotImplementedException();
        }*/
	}

	/// <summary>
	/// Make property's lazy loading as normal setting.
	/// </summary>
	public class LazyBuilder<T> {
		private readonly static LazyProxy<T> _lazyProxy = new LazyProxy<T>();
		private readonly static Func<T> _defaultCreator = null;

		public T Instance { get; private set; }

		static LazyBuilder() {
			if (_lazyProxy.HasDefaultConstructor) {
				var lambdaExpr = Expression.Lambda<Func<T>>(Expression.New(_lazyProxy.ProxyType));
				_defaultCreator = lambdaExpr.Compile();
			}
		}

		public LazyBuilder() {
			if (_defaultCreator == null) {
				string message = string.Format("No default constructor defined for type {0}.", typeof(T));
				throw new InvalidOperationException(message);
			}

			this.Instance = _defaultCreator();
		}

		public LazyBuilder<T> Setup<TProperty>(Expression<Func<T, TProperty>> expr, Func<TProperty> loader) {
			var property = (expr.Body as MemberExpression).Member as PropertyInfo;
			_lazyProxy.SetLazyLoader(property, this.Instance, loader);
			return this;
		}
	}
	/*Sample:
     
		public void LazyLoading(){
			var lazyBuilder = LazyBuilder.Create<LazyTypeTestClass>();
			var instance = lazyBuilder.Instance;

			int count = 0;
			instance.NonVirtualProperty = count;
			lazyBuilder.Setup(c => c.VirtualProperty, () => count);
			count = 10;

			Assert.Equal(0, instance.NonVirtualProperty);
			Assert.Equal(10, instance.VirtualProperty);

			count = 100;
			Assert.Equal(10, instance.VirtualProperty);
		}
	 */
}
