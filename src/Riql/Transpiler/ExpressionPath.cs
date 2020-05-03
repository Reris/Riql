using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Riql.Transpiler
{
    public class ExpressionPath
    {
        public ExpressionPath(Expression expression, IReadOnlyList<MemberExpression> expressionSteps)
        {
            this.Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            this.ExpressionSteps = expressionSteps ?? throw new ArgumentNullException(nameof(expressionSteps));
        }

        public Expression Expression { get; }
        public Type Type => this.Expression.Type;
        public IReadOnlyList<MemberExpression> ExpressionSteps { get; }
    }
}