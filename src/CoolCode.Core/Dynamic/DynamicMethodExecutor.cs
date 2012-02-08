using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CoolCode.Dynamic {
	/// <summary>
	/// 动态方法执行者
	/// <example> 
	/// <code>
	/// <para> MethodInfo methodInfo = typeof(Program).GetMethod("Call");</para>
	/// <para> DynamicMethodExecutor executor = new DynamicMethodExecutor(methodInfo);</para>
	/// <para> executor.Execute(program, parameters);</para>
	/// </code>
	/// </example>
	/// </summary>
	public class DynamicMethodExecutor {
		private Func<object, object[], object> _execute;

		public DynamicMethodExecutor(MethodInfo methodInfo) {
			this._execute = this.GetExecuteDelegate(methodInfo);
		}

		public DynamicMethodExecutor(Type type, string method) {
			this._execute = this.GetExecuteDelegate(type.GetMethod(method));
		}

		public object Execute(object instance, object[] parameters) {
			return this._execute(instance, parameters);
		}

		private Func<object, object[], object> GetExecuteDelegate(MethodInfo methodInfo) {
			// parameters to execute
			ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
			ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

			// build parameter list
			List<Expression> parameterExpressions = new List<Expression>();
			ParameterInfo[] paramInfos = methodInfo.GetParameters();
			for (int i = 0; i < paramInfos.Length; i++) {
				// (Ti)parameters[i]
				BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
				UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

				parameterExpressions.Add(valueCast);
			}

			// non-instance for static method, or ((TInstance)instance)
			Expression instanceCast = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType);

			// static invoke or ((TInstance)instance).Method
			MethodCallExpression methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

			// ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
			if (methodCall.Type == typeof(void)) {
				Expression<Action<object, object[]>> lambda = Expression.Lambda<Action<object, object[]>>(
						methodCall, instanceParameter, parametersParameter);

				Action<object, object[]> execute = lambda.Compile();
				return (instance, parameters) => {
					execute(instance, parameters);
					return null;
				};
			}
			else {
				UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
				Expression<Func<object, object[], object>> lambda = Expression.Lambda<Func<object, object[], object>>(
						castMethodCall, instanceParameter, parametersParameter);

				return lambda.Compile();
			}
		}
	}
}
