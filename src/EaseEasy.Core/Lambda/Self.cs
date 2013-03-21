using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace EaseEasy {
	/// <summary>
	/// 实现表达式和委托的定义和转化（注：免去对复杂类型定义和推测的麻烦）
	/// <see cref="http://tomasp.net/blog/dynamic-linq-queries.aspx"/> 
	/// </summary>
	public static class Self {
		// Returns the given anonymous method as a lambda expression
		public static Expression<Func<T, TResult>> Expr<T, TResult>(Expression<Func<T, TResult>> expr) {
			return expr;
		}

		// Returns the given anonymous method as a lambda expression
		public static Expression<Func<T1, T2, TResult>> Expr<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expr) {
			return expr;
		}

		// Returns the given anonymous function as a Func delegate
		public static Func<TResult> Func<TResult>(Func<TResult> func) {
			return func;
		}

		// Returns the given anonymous function as a Func delegate
		public static Func<T, TResult> Func<T, TResult>(Func<T, TResult> func) {
			return func;
		}

		/// <summary>
		/// Create a function delegate representing a unary operation
		/// </summary>
		/// <typeparam name="TArg1">The parameter type</typeparam>
		/// <typeparam name="TResult">The return type</typeparam>
		/// <param name="body">Body factory</param>
		/// <returns>Compiled function delegate</returns>
		public static Func<TArg1, TResult> Create<TArg1, TResult>(
			Func<Expression, UnaryExpression> body) {
			ParameterExpression inp = Expression.Parameter(typeof(TArg1), "inp");
			try {
				return Expression.Lambda<Func<TArg1, TResult>>(body(inp), inp).Compile();
			}
			catch (Exception ex) {
				string msg = ex.Message; // avoid capture of ex itself
				return delegate { throw new InvalidOperationException(msg); };
			}
		}

		/// <summary>
		/// Create a function delegate representing a binary operation
		/// </summary>
		/// <typeparam name="TArg1">The first parameter type</typeparam>
		/// <typeparam name="TArg2">The second parameter type</typeparam>
		/// <typeparam name="TResult">The return type</typeparam>
		/// <param name="body">Body factory</param>
		/// <returns>Compiled function delegate</returns>
		public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(
			Func<Expression, Expression, BinaryExpression> body) {
			return Create<TArg1, TArg2, TResult>(body, false);
		}

		/// <summary>
		/// Create a function delegate representing a binary operation
		/// </summary>
		/// <param name="castArgsToResultOnFailure">
		/// If no matching operation is possible, attempt to convert
		/// TArg1 and TArg2 to TResult for a match? For example, there is no
		/// "decimal operator /(decimal, int)", but by converting TArg2 (int) to
		/// TResult (decimal) a match is found.
		/// </param>
		/// <typeparam name="TArg1">The first parameter type</typeparam>
		/// <typeparam name="TArg2">The second parameter type</typeparam>
		/// <typeparam name="TResult">The return type</typeparam>
		/// <param name="body">Body factory</param>
		/// <returns>Compiled function delegate</returns>
		public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(
			Func<Expression, Expression, BinaryExpression> body, bool castArgsToResultOnFailure) {
			ParameterExpression lhs = Expression.Parameter(typeof(TArg1), "lhs");
			ParameterExpression rhs = Expression.Parameter(typeof(TArg2), "rhs");
			try {
				try {
					return Expression.Lambda<Func<TArg1, TArg2, TResult>>(body(lhs, rhs), lhs, rhs).Compile();
				}
				catch (InvalidOperationException) {
					if (castArgsToResultOnFailure && !(         // if we show retry                                                        
							typeof(TArg1) == typeof(TResult) &&  // and the args aren't
							typeof(TArg2) == typeof(TResult))) { // already "TValue, TValue, TValue"...
						// convert both lhs and rhs to TResult (as appropriate)
						Expression castLhs = typeof(TArg1) == typeof(TResult) ?
								(Expression)lhs :
								(Expression)Expression.Convert(lhs, typeof(TResult));
						Expression castRhs = typeof(TArg2) == typeof(TResult) ?
								(Expression)rhs :
								(Expression)Expression.Convert(rhs, typeof(TResult));

						return Expression.Lambda<Func<TArg1, TArg2, TResult>>(
							body(castLhs, castRhs), lhs, rhs).Compile();
					}
					else
						throw;
				}
			}
			catch (Exception ex) {
				string msg = ex.Message; // avoid capture of ex itself
				return delegate { throw new InvalidOperationException(msg); };
			}
		}

		/// <summary>
		/// Lambda实现递归
		/// <see cref="http://www.cnblogs.com/EaseEasy/archive/2009/10/12/Fix.html"/>  
		/// </summary>
		public static Action<T> Fix<T>(Action<Action<T>, T> f) {
			return x => f(Fix(f), x);
		}

		/// <summary>
		/// Lambda实现递归
		/// <see cref="http://www.cnblogs.com/EaseEasy/archive/2009/10/12/Fix.html"/>  
		/// </summary>
		public static Action<T1, T2> Fix<T1, T2>(Action<Action<T1, T2>, T1, T2> f) {
			return (x, y) => f(Fix(f), x, y); 
		}
	}
}
