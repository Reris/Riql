using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Riql.Transpiler
{
    public class ExpressionPath
    {
        public ExpressionPath([NotNull] Expression expression, [NotNull] IReadOnlyList<MemberExpression> expressionSteps)
        {
            this.Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            this.ExpressionSteps = expressionSteps ?? throw new ArgumentNullException(nameof(expressionSteps));
        }

        [NotNull]
        public Expression Expression { get; }

        [NotNull]
        public Type Type => this.Expression.Type;

        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<MemberExpression> ExpressionSteps { get; }
    }
}