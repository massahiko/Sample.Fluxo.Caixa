using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sample.Fluxo.Caixa.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> andExpr)
        {
            var parameter = expr.Parameters[0];
            var visitor = new SubstExpressionVisitor();
            visitor.subst[andExpr.Parameters[0]] = parameter;
            var body = Expression.AndAlso(expr.Body, visitor.Visit(andExpr.Body));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> orExpr)
        {
            var parameter = expr.Parameters[0];
            var visitor = new SubstExpressionVisitor();
            visitor.subst[orExpr.Parameters[0]] = parameter;
            var body = Expression.Or(expr.Body, visitor.Visit(orExpr.Body));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }

    internal class SubstExpressionVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> subst = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (subst.TryGetValue(node, out var newValue))
            {
                return newValue;
            }

            return node;
        }

    }
}
