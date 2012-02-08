using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace CoolCode.Linq.Expressions {
	/// <summary>
	/// utilities for working with expression trees
	/// </summary>
	internal static class ExpressionTreeHelpers {
		/// <summary>
		/// test to see if expression is a binary expression that checks equality with a constant value
		/// - essentially, the caller wants to know if this is a well-formed expression with certain criteria
		/// </summary>
		/// <param name="exp">expression to check</param>
		/// <param name="declaringType">type containing member</param>
		/// <param name="memberName">member being checked</param>
		/// <returns>true if member is being checked for equality with value</returns>
		internal static bool IsMemberEqualsValueExpression(Expression exp, Type declaringType, string memberName) {
			if (exp.NodeType != ExpressionType.Equal)
				return false;

			BinaryExpression be = (BinaryExpression)exp;

			// Assert.
			if (IsSpecificMemberExpression(be.Left, declaringType, memberName) &&
				IsSpecificMemberExpression(be.Right, declaringType, memberName))
				throw new Exception("Cannot have 'member' == 'member' in an expression!");

			return (IsSpecificMemberExpression(be.Left, declaringType, memberName) ||
				IsSpecificMemberExpression(be.Right, declaringType, memberName));
		}

		/// <summary>
		/// verify that the type and member name in the expression are what is expected
		/// </summary>
		/// <param name="exp">expression to check</param>
		/// <param name="declaringType">expected type</param>
		/// <param name="memberName">expected member name</param>
		/// <returns>true if type and name in expression match expected type and name</returns>
		internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName) {
			// adjust for enums
			Expression tempExp =
				exp.NodeType == ExpressionType.Convert ?
					(exp as UnaryExpression).Operand :
					exp;

			return ((tempExp is MemberExpression) &&
				(((MemberExpression)tempExp).Member.DeclaringType == declaringType) &&
				(((MemberExpression)tempExp).Member.Name == memberName));
		}

		/// <summary>
		/// extracts the constant value from a binary equals expression
		/// - either the left or right side of the expression
		/// </summary>
		/// <param name="be">binary expression</param>
		/// <param name="memberDeclaringType">type of object</param>
		/// <param name="memberName">member to get value for</param>
		/// <returns>string representation of value</returns>
		internal static string GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType, string memberName) {
			if (be.NodeType != ExpressionType.Equal)
				throw new Exception("There is a bug in this program.");

			if (be.Left.NodeType == ExpressionType.MemberAccess ||
				be.Left.NodeType == ExpressionType.Convert) {
				// adjust for enums
				MemberExpression me =
					be.Left.NodeType == ExpressionType.Convert ?
						(be.Left as UnaryExpression).Operand as MemberExpression :
						be.Left as MemberExpression;

				if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName) {
					return GetValueFromExpression(be.Right);
				}
			}
			else if (be.Right.NodeType == ExpressionType.MemberAccess) {
				MemberExpression me = (MemberExpression)be.Right;

				if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName) {
					return GetValueFromExpression(be.Left);
				}
			}

			// We should have returned by now.
			throw new Exception("There is a bug in this program.");
		}

		/// <summary>
		/// converts constant expression to constant value
		/// </summary>
		/// <param name="expression">constant expression</param>
		/// <returns>constant value</returns>
		internal static string GetValueFromExpression(Expression expression) {
			if (expression.NodeType == ExpressionType.Constant)
				return (string)(((ConstantExpression)expression).Value.ToString());
			else
				throw new InvalidOperationException(
					String.Format("The client query is invalid: The expression type {0} is not supported to obtain a value.", expression.NodeType));
		}

		/// <summary>
		/// 创建Contains表达式
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="valueSelector"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(
		   Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values) {

			if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }

			if (null == values) { throw new ArgumentNullException("values"); }

			ParameterExpression p = valueSelector.Parameters[0];

			if (!values.Any()) {
				return e => false;
			}

			var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));

			var body = equals.Aggregate(Expression.Or);

			return Expression.Lambda<Func<TElement, bool>>(body, p);
		}
	}
}
