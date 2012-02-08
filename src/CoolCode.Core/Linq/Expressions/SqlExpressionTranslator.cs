using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace CoolCode.Linq.Expressions {
	/// <summary>
	/// 表达式转换成SQL语句
	/// </summary>
	public class SqlExpressionTranslator : ExpressionVisitor {
		private StringBuilder _sqlStringBuilder = new StringBuilder();

		public string Translate(Expression expression) {
			_sqlStringBuilder.Clear();
			Visit(expression);

			return _sqlStringBuilder.ToString();
		}

		private static Expression StripQuotes(Expression e) {
			while (e.NodeType == ExpressionType.Quote) {
				e = ((UnaryExpression)e).Operand;
			}
			return e;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m) {
			if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where") {
				_sqlStringBuilder.Append("SELECT * FROM (");
				Visit(m.Arguments[0]);
				_sqlStringBuilder.Append(") AS T WHERE ");
				LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
				Visit(lambda.Body);
				return m;
			}
			throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
		}

		protected override Expression VisitUnary(UnaryExpression u) {
			switch (u.NodeType) {
				case ExpressionType.Not:
					_sqlStringBuilder.Append(" NOT ");
					Visit(u.Operand);
					break;

				default:
					throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
			}
			return u;
		}

		protected override Expression VisitBinary(BinaryExpression b) {
			_sqlStringBuilder.Append("(");
			Visit(b.Left);
			switch (b.NodeType) {
				case ExpressionType.And:
					_sqlStringBuilder.Append(" AND ");
					break;

				case ExpressionType.Or:
					_sqlStringBuilder.Append(" OR");
					break;

				case ExpressionType.Equal:
					_sqlStringBuilder.Append(" = ");
					break;

				case ExpressionType.NotEqual:
					_sqlStringBuilder.Append(" <> ");
					break;

				case ExpressionType.LessThan:
					_sqlStringBuilder.Append(" < ");
					break;

				case ExpressionType.LessThanOrEqual:
					_sqlStringBuilder.Append(" <= ");
					break;

				case ExpressionType.GreaterThan:
					_sqlStringBuilder.Append(" > ");
					break;

				case ExpressionType.GreaterThanOrEqual:
					_sqlStringBuilder.Append(" >= ");
					break;

				default:
					throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
			}

			Visit(b.Right);
			_sqlStringBuilder.Append(")");
			return b;
		}

		protected override Expression VisitConstant(ConstantExpression c) {
			IQueryable q = c.Value as IQueryable;
			if (q != null) {
				// assume constant nodes w/ IQueryables are table references
				_sqlStringBuilder.Append("SELECT * FROM ");
				_sqlStringBuilder.Append(q.ElementType.Name);
			}
			else if (c.Value == null) {
				_sqlStringBuilder.Append("NULL");
			}
			else {
				switch (Type.GetTypeCode(c.Value.GetType())) {
					case TypeCode.Boolean:
						_sqlStringBuilder.Append(((bool)c.Value) ? 1 : 0);
						break;

					case TypeCode.String:
						_sqlStringBuilder.Append("'");
						_sqlStringBuilder.Append(c.Value);
						_sqlStringBuilder.Append("'");
						break;

					case TypeCode.Object:
						throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));

					default:
						_sqlStringBuilder.Append(c.Value);
						break;
				}
			}
			return c;
		}

		protected override Expression VisitMemberAccess(MemberExpression m) {
			if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter) {
				_sqlStringBuilder.Append(m.Member.Name);
				return m;
			}
			throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
		}
	}
}
