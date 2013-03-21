using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using EaseEasy.Caching;

namespace EaseEasy {
	public static class InvokeMemberExtensions {
		private static readonly ICacheStrategy cache = new HashCacheStrategy();

		/// <summary>
		/// 给指定属性赋值 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">对象</param>
		/// <param name="propertyName">属性名</param>
		/// <param name="propertyValue">属性值</param>
		public static void SetValue<T>(this T obj, string propertyName, object propertyValue) where T : class {
			obj.ThrowIfNull();
			var type = obj.GetType();

			var setterDelegate = cache.Get(type.FullName + "_" + propertyName + "_SetValue",
				() => {
					ParameterExpression parameter = Expression.Parameter(type, "x");
					MethodInfo setter = type.GetMethod("set_" + propertyName);

					if (setter == null) {
						throw new MethodAccessException(string.Format("Cannot access setter of the property '{0}'", propertyName));
					}

					ParameterExpression value = Expression.Parameter(setter.GetParameters()[0].ParameterType, "propertyValue");
					MethodCallExpression call = Expression.Call(parameter, setter, value);
					LambdaExpression lambda = Expression.Lambda(call, parameter, value);
					var exp = lambda.Compile();
					return exp;
				});

			setterDelegate.DynamicInvoke(obj, propertyValue);
		}

		/// <summary>
		/// 获取指定属性的值 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">对象</param>
		/// <param name="propertyName">属性名</param>
		/// <returns></returns>
		public static object GetValue<T>(this T obj, string propertyName) where T : class {
			obj.ThrowIfNull();
			var type = obj.GetType();

			var getterDelegate = cache.Get(type.FullName + "_" + propertyName + "_GetValue",
				() => {
					ParameterExpression parameter = Expression.Parameter(type, "x");
					MethodInfo getter = type.GetMethod("get_" + propertyName);

					if (getter == null) {
						throw new MethodAccessException(string.Format("Cannot access getter of the property '{0}'", propertyName));
					}

					MethodCallExpression call = Expression.Call(parameter, getter);
					LambdaExpression lambda = Expression.Lambda(call, parameter);
					var exp = lambda.Compile();
					return exp;
				});

			return getterDelegate.DynamicInvoke(obj);
		}
	}
}
