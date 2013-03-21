using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using EaseEasy.Reflection;

namespace EaseEasy.Linq.Expressions {
	public static class ExpressionExtensions {
		#region Invoke

		public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr) {
			return expr.Compile().Invoke();
		}

		public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1) {
			return expr.Compile().Invoke(arg1);
		}

		public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2) {
			return expr.Compile().Invoke(arg1, arg2);
		}

		public static TResult Invoke<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3) {
			return expr.Compile().Invoke(arg1, arg2, arg3);
		}

		public static TResult Invoke<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
			return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
		}

		#endregion

		#region 提供表达式的扩展方法，方便快速构建表达式

		/// <summary>
		/// 获取指定类型的Nullable&lt;T&gt;表达式
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ConstantExpression AsNullConstant(this Type type) {
			return Expression.Constant(null, type.GetNullAssignableType());
		}

		/// <summary>
		/// 构建参数表达式
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static ParameterExpression AsParameter(this Type type, string name) {
			return Expression.Parameter(type, name);
		}

		/// <summary>
		/// 构建属性表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static MemberExpression Property(this Expression expression, string propertyName) {
			return Expression.Property(expression, propertyName);
		}

		/// <summary>
		/// 构建方法表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="method"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static MethodCallExpression Call(this Expression expression, MethodInfo method, params Expression[] arguments) {
			return Expression.Call(expression, method, arguments);
		}

		/// <summary>
		/// 构键“相等”表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static BinaryExpression Equal(this Expression expression, Expression other) {
			return Expression.Equal(expression, other);
		}


		/// <summary>
		/// 构键“与”表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static BinaryExpression AndAlso(this Expression expression, Expression other) {
			return Expression.AndAlso(expression, other);
		}

		/// <summary>
		/// 构键“或”表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static BinaryExpression OrElse(this Expression expression, Expression other) {
			return Expression.OrElse(expression, other);
		}

		/// <summary>
		/// 获取Lambda表达式的参数表达式
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="S"></typeparam>
		/// <param name="expr"></param>
		/// <returns></returns>
		public static ParameterExpression[] GetParameters<T, S>(this Expression<Func<T, S>> expr) {
			return expr.Parameters.ToArray();
		}

		#endregion

		public static bool IsMemberExpression(this Expression expression) {
			return GetMemberExpression(expression) != null;
		}

		public static string GetMemberName(this Expression expression) {
			MemberExpression tempExp = GetMemberExpression(expression);
			if (tempExp == null) {
				return string.Empty;
			}

			string memberName = string.Empty;

			do {
				memberName = (memberName == string.Empty ? tempExp.Member.Name : tempExp.Member.Name + "." + memberName);
				tempExp = tempExp.Expression as MemberExpression;
			} while (tempExp != null);

			return memberName;
		}

		public static MemberExpression GetMemberExpression(this Expression expression) {
			Expression tempExp = (expression.NodeType == ExpressionType.Convert ?
					(expression as UnaryExpression).Operand :
					expression);

			return tempExp as MemberExpression;
		}

		public static MemberExpression GetMemberExpression(this LambdaExpression expression) {
			return RemoveUnary(expression.Body) as MemberExpression;
		}

		public static Type GetTypeFromMemberExpression(this MemberExpression memberExpression) {
			if (memberExpression == null)
				return null;

			var dataType = GetTypeFromMemberInfo(memberExpression.Member, (PropertyInfo p) => p.PropertyType);
			if (dataType == null)
				dataType = GetTypeFromMemberInfo(memberExpression.Member, (MethodInfo m) => m.ReturnType);
			if (dataType == null)
				dataType = GetTypeFromMemberInfo(memberExpression.Member, (FieldInfo f) => f.FieldType);

			return dataType;
		}

		private static Type GetTypeFromMemberInfo<TMember>(MemberInfo member, Func<TMember, Type> func) where TMember : MemberInfo {
			if (member is TMember) {
				return func((TMember)member);
			}
			return null;
		}

		private static Expression RemoveUnary(Expression body) {
			var unary = body as UnaryExpression;
			if (unary != null) {
				return unary.Operand;
			}
			return body;
		}

	}

}
