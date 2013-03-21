using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EaseEasy.Dynamic {
	public static class InstanceCreatorExtensions {
		/// <summary>
		/// 根据类型创建实例
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object CreateInstance(this Type type) {
			return ExpressionActivator.CreateInstance(type);
		}

		/// <summary>
		/// 根据类型和城市创建实例
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static object CreateInstance<T>(this Type type, T arg) {
			var creator = Singleton<InstanceCreator<T>>.GetInstance();
			return creator.CreateInstance(type, arg);
		}

		/// <summary>
		/// 根据类型和城市创建实例
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="type"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <returns></returns>
		public static object CreateInstance<T1, T2>(this Type type, T1 arg1, T2 arg2) {
			var creator = Singleton<InstanceCreator<T1, T2>>.GetInstance();
			return creator.CreateInstance(type, arg1, arg2);
		}

		#region InstanceCreators

		class ExpressionActivator {
			private readonly static Dictionary<Type, Func<object>> cache = new Dictionary<Type, Func<object>>();
			private readonly static object mutex = new object();

			public static object CreateInstance(Type type) {
				Func<object> value = null;
				if (!cache.TryGetValue(type, out value)) {
					lock (mutex) {
						if (!cache.TryGetValue(type, out value)) {
							value = CreateInstanceDelegate(type);
						}
					}
					cache[type] = value;
				}

				return value();
			}

			private static Func<object> CreateInstanceDelegate(Type type) {
				NewExpression newExp = Expression.New(type);
				Expression<Func<object>> lambdaExp = Expression.Lambda<Func<object>>(newExp, null);
				Func<object> func = lambdaExp.Compile();

				return func;
			}
		}

		/// <summary>
		/// 实例创建者（创建构造函数含1参数的对象）
		/// </summary>
		/// <typeparam name="T">参数</typeparam>
		class InstanceCreator<T> {
			private static readonly Dictionary<Type, Func<T, object>> cache = new Dictionary<Type, Func<T, object>>();

			public object CreateInstance(Type type, T arg) {
				Func<T, object> createInstanceDelegate = null;

				if (!cache.TryGetValue(type, out createInstanceDelegate)) {
					lock (this) {
						if (!cache.TryGetValue(type, out createInstanceDelegate)) {
							createInstanceDelegate = CreateInstanceDelegate(type);
							cache.Add(type, createInstanceDelegate);
						}
					}
				}

				return createInstanceDelegate(arg);
			}

			public static Func<T, object> CreateInstanceDelegate(Type type) {
				Type paramType = typeof(T);
				var constructor = type.GetConstructor(new Type[] { paramType });
				var param = new ParameterExpression[] { Expression.Parameter(paramType, "arg") };

				NewExpression newExp = Expression.New(constructor, param);
				Expression<Func<T, object>> lambdaExp =
					Expression.Lambda<Func<T, object>>(newExp, param);
				Func<T, object> func = lambdaExp.Compile();
				return func;
			}
		}

		/// <summary>
		/// 实例创建者（创建构造函数含2参数的对象）
		/// </summary>
		/// <typeparam name="T1">参数1</typeparam>
		/// <typeparam name="T2">参数2</typeparam>
		public class InstanceCreator<T1, T2> {
			private static readonly Dictionary<Type, Func<T1, T2, object>> cache = new Dictionary<Type, Func<T1, T2, object>>();

			public object CreateInstance(Type type, T1 arg1, T2 arg2) {
				Func<T1, T2, object> createInstanceDelegate = null;
				if (!cache.TryGetValue(type, out createInstanceDelegate)) {
					lock (this) {
						if (!cache.TryGetValue(type, out createInstanceDelegate)) {
							createInstanceDelegate = CreateInstanceDelegate(type);
							cache.Add(type, createInstanceDelegate);
						}
					}
				}
				return createInstanceDelegate(arg1, arg2);
			}

			public static Func<T1, T2, object> CreateInstanceDelegate(Type type) {
				Type paramType1 = typeof(T1);
				Type paramType2 = typeof(T2);
				var constructor = type.GetConstructor(new Type[] { paramType1, paramType2 });
				var param = new ParameterExpression[] { 
					Expression.Parameter(paramType1, "arg1"),
					Expression.Parameter(paramType2, "arg2")};

				NewExpression newExp = Expression.New(constructor, param);
				Expression<Func<T1, T2, object>> lambdaExp =
					Expression.Lambda<Func<T1, T2, object>>(newExp, param);
				Func<T1, T2, object> func = lambdaExp.Compile();
				return func;
			}
		}

		#endregion
	}
}
