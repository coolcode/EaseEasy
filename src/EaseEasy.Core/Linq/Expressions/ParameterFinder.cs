using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace EaseEasy.Linq.Expressions {
	/// <summary>
	/// extracts parameters from an expression called for extracting parameters and values on where clauses
	/// </summary>
	/// <typeparam name="T">type to get parameters for</typeparam>
	internal class ParameterFinder<T> : ExpressionVisitor {
		/// <summary>
		/// expression being searched
		/// </summary>
		private Expression _expression;

		/// <summary>
		/// parameters to search for
		/// </summary>
		private Dictionary<string, string> _parameters;

		/// <summary>
		/// keep track of expression and parameter list
		/// </summary>
		/// <param name="exp">expression to search</param>
		/// <param name="parameters">parameters to search for</param>
		public ParameterFinder(Expression exp, List<string> parameters) {
			_expression = exp;
			ParameterNames = parameters;
		}

		/// <summary>
		/// name/value pairs of parameters and their values
		/// </summary>
		public Dictionary<string, string> Parameters {
			get {
				if (_parameters == null) {
					_parameters = new Dictionary<string, string>();
					Visit(_expression);
				}
				return _parameters;
			}
		}

		/// <summary>
		/// names of input parameters
		/// </summary>
		public List<string> ParameterNames { get; set; }

		/// <summary>
		/// extracts values from equality expressions that match parameter names
		/// </summary>
		/// <param name="be">binary expression to evaluate</param>
		/// <returns>binary expression - supports recursive tree traversal in visitor</returns>
		protected override Expression VisitBinary(BinaryExpression be) {
			if (be.NodeType == ExpressionType.Equal) {
				foreach (var param in ParameterNames) {
					if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(T), param)) {
						_parameters.Add(param, ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(T), param));
						return be;
					}
				}

				return base.VisitBinary(be);
			}
			else
				return base.VisitBinary(be);
		}
	}
}
