using System;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Expression"/> object.
    /// </summary>
    public static class ExpressionExtension
    {
        private readonly static ExpressionType[] m_extractableExpressionTypes = new[]
            {
                ExpressionType.Equal,
                ExpressionType.NotEqual,
                ExpressionType.GreaterThan,
                ExpressionType.GreaterThanOrEqual,
                ExpressionType.LessThan,
                ExpressionType.LessThanOrEqual
            };

        // CanBeExtracted

        /// <summary>
        /// Identify whether the expression can be extracted as <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="expression">The expression to be checked.</param>
        /// <returns>Returns true if the expression can be extracted as <see cref="QueryField"/> object.</returns>
        public static bool CanBeExtracted(this Expression expression)
        {
            return m_extractableExpressionTypes.Contains(expression.NodeType);
        }

        // CanBeGrouped

        /// <summary>
        /// Identify whether the expression can be grouped as <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="expression">The expression to be checked.</param>
        /// <returns>Returns true if the expression can be grouped as <see cref="QueryGroup"/> object.</returns>
        public static bool CanBeGrouped(this Expression expression)
        {
            return expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse;
        }

        // Binary

        public static bool IsMember(this Expression expression)
        {
            return expression is MemberExpression;
        }

        public static MemberExpression ToMember(this Expression expression)
        {
            return (MemberExpression)expression;
        }

        // Binary

        public static bool IsBinary(this Expression expression)
        {
            return expression is BinaryExpression;
        }

        public static BinaryExpression ToBinary(this Expression expression)
        {
            return (BinaryExpression)expression;
        }

        // Unary

        public static bool IsUnary(this Expression expression)
        {
            return expression is UnaryExpression;
        }

        public static UnaryExpression ToUnary(this Expression expression)
        {
            return (UnaryExpression)expression;
        }

        // Unary

        public static bool IsConstant(this Expression expression)
        {
            return expression is ConstantExpression;
        }

        public static ConstantExpression ToConstant(this Expression expression)
        {
            return (ConstantExpression)expression;
        }

        // Unary

        public static bool IsMethodCall(this Expression expression)
        {
            return expression is MethodCallExpression;
        }

        public static MethodCallExpression ToMethodCall(this Expression expression)
        {
            return (MethodCallExpression)expression;
        }


        // NewArray

        public static bool IsNewArray(this Expression expression)
        {
            return expression is NewArrayExpression;
        }

        public static NewArrayExpression ToNewArray(this Expression expression)
        {
            return (NewArrayExpression)expression;
        }

        // GetName

        public static string GetName(this BinaryExpression expression)
        {
            if (expression.Left.IsMember())
            {
                return expression.Left.ToMember().Member.Name;
            }
            return null;
        }

        // GetValue

        public static object GetValue(this Expression expression)
        {
            if (expression.IsBinary())
            {
                return expression.ToBinary().GetValue();
            }
            else if (expression.IsConstant())
            {
                return expression.ToConstant().GetValue();
            }
            else if (expression.IsUnary())
            {
                return expression.ToUnary().GetValue();
            }
            else if (expression.IsMethodCall())
            {
                return expression.ToMethodCall().GetValue();
            }
            else if (expression.IsMember())
            {
                return expression.ToMember().GetValue();
            }
            else if (expression.IsNewArray())
            {
                return expression.ToNewArray().GetValue();
            }
            else
            {
                return null;
            }
        }

        public static object GetValue(this BinaryExpression expression)
        {
            return expression.Right.GetValue();
        }

        public static object GetValue(this ConstantExpression expression)
        {
            return expression.Value;
        }

        public static object GetValue(this UnaryExpression expression)
        {
            return expression.Operand.GetValue();
        }

        public static object GetValue(this MethodCallExpression expression)
        {
            return expression.Method.Invoke(expression.Object.GetValue(),
                expression.Arguments?.Select(argExpression => argExpression.GetValue()).ToArray());
        }

        public static object GetValue(this MemberExpression expression)
        {
            if (expression.Expression != null)
            {
                return expression.Expression.GetValue();
            }
            else
            {
                if (expression.Member.IsFieldInfo())
                {
                    return expression.Member.AsFieldInfo().GetValue(null);
                }
                else if (expression.Member.IsPropertyInfo())
                {
                    return expression.Member.AsPropertyInfo().GetValue(null);
                }
                else if (expression.Member.IsMethodInfo())
                {
                    return expression.Member.AsMethodInfo().Invoke(null, null);
                }
            }
            return null;
        }

        public static object GetValue(this NewArrayExpression expression)
        {
            var arrayType = expression.Type.GetElementType();
            var array = Array.CreateInstance(arrayType, expression.Expressions.Count);
            expression.Expressions?.ToList().ForEach(item =>
            {
                var index = expression.Expressions.IndexOf(item);
                var value = item.GetValue();
                array.SetValue(value, index);
            });
            return array;
        }
    }
}
