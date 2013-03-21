using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace EaseEasy.Linq.Expressions
{
    public class PartialEvaluator : ExpressionVisitor
    {
        private Func<Expression, bool> _fnCanBeEvaluated;
        private HashSet<Expression> _candidates;

        public PartialEvaluator()
            : this(CanBeEvaluatedLocally)
        { }

        public PartialEvaluator(Func<Expression, bool> fnCanBeEvaluated)
        {
            this._fnCanBeEvaluated = fnCanBeEvaluated;
        }

        public Expression Eval(Expression exp)
        {
            this._candidates = new Nominator(this._fnCanBeEvaluated).Nominate(exp);

            return this.Visit(exp);
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

            if (this._candidates.Contains(exp))
            {
                return this.Evaluate(exp);
            }

            return base.Visit(exp);
        }

        private Expression Evaluate(Expression e)
        {
            if (e.NodeType == ExpressionType.Constant)
            {
                return e;
            }

            LambdaExpression lambda = Expression.Lambda(e);
            Delegate fn = lambda.Compile();

            return Expression.Constant(fn.DynamicInvoke(null), e.Type);
        }

        private static bool CanBeEvaluatedLocally(Expression exp)
        {
            return exp.NodeType != ExpressionType.Parameter;
        }

        #region Nominator

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private Func<Expression, bool> _fnCanBeEvaluated;
            private HashSet<Expression> _candidates;
            private bool _cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this._fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                this._candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this._candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this._cannotBeEvaluated;
                    this._cannotBeEvaluated = false;

                    base.Visit(expression);

                    if (!this._cannotBeEvaluated)
                    {
                        if (this._fnCanBeEvaluated(expression))
                        {
                            this._candidates.Add(expression);
                        }
                        else
                        {
                            this._cannotBeEvaluated = true;
                        }
                    }

                    this._cannotBeEvaluated |= saveCannotBeEvaluated;
                }

                return expression;
            }
        }

        #endregion
    }
}
