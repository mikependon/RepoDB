using System;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Expression"/> object.
    /// </summary>
    internal static class ExpressionExtension
    {
        private static Func<Expression, bool> m_canBeExtracted;
        private static Func<Expression, bool> m_canBeGrouped;

        static ExpressionExtension()
        {
            m_canBeExtracted = (Expression expression) =>
            {
                return (new[]
                    {
                        ExpressionType.Equal,
                        ExpressionType.NotEqual,
                        ExpressionType.GreaterThan,
                        ExpressionType.GreaterThanOrEqual,
                        ExpressionType.LessThan,
                        ExpressionType.LessThanOrEqual
                    })
                    .Contains(expression.NodeType);
            };
            m_canBeGrouped = (Expression expression) =>
            {
                return expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse;
            };
        }

        // CanBeExtracted

        internal static bool CanBeExtracted(this Expression expression)
        {
            return m_canBeExtracted(expression);
        }

        // CanBeGrouped

        internal static bool CanBeGrouped(this Expression expression)
        {
            return m_canBeGrouped(expression);
        }

        // Binary

        internal static bool IsMember(this Expression expression)
        {
            return expression is MemberExpression;
        }

        internal static MemberExpression ToMember(this Expression expression)
        {
            return (MemberExpression)expression;
        }

        // Binary

        internal static bool IsBinary(this Expression expression)
        {
            return expression is BinaryExpression;
        }

        internal static BinaryExpression ToBinary(this Expression expression)
        {
            return (BinaryExpression)expression;
        }

        // Unary

        internal static bool IsUnary(this Expression expression)
        {
            return expression is UnaryExpression;
        }

        internal static UnaryExpression ToUnary(this Expression expression)
        {
            return (UnaryExpression)expression;
        }

        // Unary

        internal static bool IsConstant(this Expression expression)
        {
            return expression is ConstantExpression;
        }

        internal static ConstantExpression ToConstant(this Expression expression)
        {
            return (ConstantExpression)expression;
        }

        // Unary

        internal static bool IsMethodCall(this Expression expression)
        {
            return expression is MethodCallExpression;
        }

        internal static MethodCallExpression ToMethodCall(this Expression expression)
        {
            return (MethodCallExpression)expression;
        }
    }
}
