using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace EaseEasy.Linq.Expressions {
	internal class ConditionBuilder : ExpressionVisitor {
		private List<object> _arguments;
		private Stack<string> _conditionParts;

		public string Condition { get; private set; }

		public object[] Arguments { get; private set; }

		public void Build(Expression expression) {
			PartialEvaluator evaluator = new PartialEvaluator();
			Expression evaluatedExpression = evaluator.Eval(expression);

			this._arguments = new List<object>();
			this._conditionParts = new Stack<string>();

			this.Visit(evaluatedExpression);

			this.Arguments = this._arguments.ToArray();
			this.Condition = this._conditionParts.Count > 0 ? this._conditionParts.Pop() : null;
		}

		protected override Expression VisitBinary(BinaryExpression b) {
			if (b == null)
				return b;

			string opr;
			switch (b.NodeType) {
				case ExpressionType.Equal:
					opr = "=";
					break;
				case ExpressionType.NotEqual:
					opr = "<>";
					break;
				case ExpressionType.GreaterThan:
					opr = ">";
					break;
				case ExpressionType.GreaterThanOrEqual:
					opr = ">=";
					break;
				case ExpressionType.LessThan:
					opr = "<";
					break;
				case ExpressionType.LessThanOrEqual:
					opr = "<=";
					break;
				case ExpressionType.AndAlso:
					opr = "AND";
					break;
				case ExpressionType.OrElse:
					opr = "OR";
					break;
				case ExpressionType.Add:
					opr = "+";
					break;
				case ExpressionType.Subtract:
					opr = "-";
					break;
				case ExpressionType.Multiply:
					opr = "*";
					break;
				case ExpressionType.Divide:
					opr = "/";
					break;
				default:
					throw new NotSupportedException(b.NodeType + "is not supported.");
			}

			this.Visit(b.Left);
			this.Visit(b.Right);

			string right = this._conditionParts.Pop();
			string left = this._conditionParts.Pop();

			string condition = string.Format("({0} {1} {2})", left, opr, right);
			this._conditionParts.Push(condition);

			return b;
		}

		protected override Expression VisitConstant(ConstantExpression c) {
			if (c == null)
				return c;

			this._arguments.Add(c.Value);
			this._conditionParts.Push(String.Format("{{{0}}}", this._arguments.Count - 1));

			return c;
		}

		protected override Expression VisitMemberAccess(MemberExpression m) {
			if (m == null)
				return m;

			PropertyInfo propertyInfo = m.Member as PropertyInfo;
			if (propertyInfo == null)
				return m;

			this._conditionParts.Push(string.Format("[{0}]", propertyInfo.Name));

			return m;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m) {
			if (m == null)
				return m;

			string format;
			switch (m.Method.Name) {
				case "StartsWith":
					format = "({0} LIKE {1}+'%')";
					break;

				case "Contains":
					format = "({0} LIKE '%'+{1}+'%')";
					break;

				case "EndsWith":
					format = "({0} LIKE '%'+{1})";
					break;

				default:
					throw new NotSupportedException(m.NodeType + " is not supported!");
			}

			this.Visit(m.Object);
			this.Visit(m.Arguments[0]);
			string right = this._conditionParts.Pop();
			string left = this._conditionParts.Pop();
			this._conditionParts.Push(string.Format(format, left, right));

			return m;
		}
	}
}
