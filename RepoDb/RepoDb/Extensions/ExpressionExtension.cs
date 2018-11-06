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
        internal static Func<Expression, bool> m_canBeExtracted;
        internal static Func<Expression, bool> m_canBeGrouped;

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

        // GetName

        internal static string GetName(this BinaryExpression expression)
        {
            if (expression.Left.IsMember())
            {
                return expression.Left.ToMember().Member.Name;
            }
            return null;
        }

        // GetValue

        internal static object GetValue(this BinaryExpression expression)
        {
            // Constants
            if (expression.Right.IsConstant())
            {
                return expression.Right.ToConstant().GetValue();
            }
            // Unaries
            else if (expression.Right.IsUnary())
            {
                return expression.Right.ToUnary().GetValue();
            }
            // MethodCall
            else if (expression.Right.IsMethodCall())
            {
                return expression.Right.ToMethodCall().GetValue();
            }
            // Member
            else if (expression.Right.IsMember())
            {
                return expression.Right.ToMember().GetValue();
            }
            // Others
            else
            {
                // Arrays
                if (expression.Right.NodeType == ExpressionType.NewArrayInit)
                {
                    var newArrayExpression = (NewArrayExpression)expression.Right;
                    var arrayType = Type.GetType(expression.Right.Type.FullName.Replace("[]", string.Empty));
                    var array = Array.CreateInstance(arrayType, newArrayExpression.Expressions.Count);
                    for (var i = 0; i < newArrayExpression.Expressions.Count; i++)
                    {
                        var currentExpression = newArrayExpression.Expressions[i];
                        var value = (object)null;
                        if (currentExpression.IsConstant())
                        {
                            value = currentExpression.ToConstant().GetValue();
                        }
                        else if (currentExpression.IsMember())
                        {
                            value = currentExpression.ToMember().GetValue();
                        }
                        else if (currentExpression.IsMethodCall())
                        {
                            value = currentExpression.ToMethodCall().GetValue();
                        }
                        array.SetValue(value, i);
                    }
                    return array;
                }
            }
            // Nothing
            return null;
        }

        internal static object GetValue(this ConstantExpression expression)
        {
            return expression.Value;
        }

        internal static object GetValue(this UnaryExpression expression)
        {
            if (expression.Operand.IsMember())
            {
                return expression.Operand.ToMember().GetValue();
            }
            else if (expression.Operand.IsMethodCall())
            {
                return expression.Operand.ToMethodCall().GetValue();
            }
            return null;
        }

        internal static object GetValue(this MethodCallExpression expression)
        {
            if (expression.Arguments?.Any() == true)
            {
                var instance = (object)null;
                if (expression.Object.IsMember())
                {
                    var member = expression.Object.ToMember();
                    if (member.Member.IsFieldInfo())
                    {
                        instance = member.Member.AsFieldInfo().GetValue(null);
                    }
                    else if (member.Member.IsPropertyInfo())
                    {
                        instance = member.Member.AsPropertyInfo().GetValue(null);
                    }
                    else if (member.Member.IsMethodInfo())
                    {
                        instance = member.Member.AsMethodInfo().Invoke(null, null);
                    }
                }
                return expression.Method.Invoke(instance, expression.Arguments.Select(argExpression =>
                {
                    if (argExpression.IsConstant())
                    {
                        return argExpression.ToConstant().GetValue();
                    }
                    else if (argExpression.IsMember())
                    {
                        return argExpression.ToMember().GetValue();
                    }
                    else if (argExpression.IsMethodCall())
                    {
                        return argExpression.ToMethodCall().GetValue();
                    }
                    return null;
                }).ToArray());
            }
            else
            {
                if (expression.Object != null)
                {
                    if (expression.Object.IsConstant())
                    {
                        return expression.Method.Invoke(expression.Object.ToConstant().GetValue(), null);
                    }
                    else if (expression.Object.IsMember())
                    {
                        return expression.Method.Invoke(expression.Object.ToMember().GetValue(), null);
                    }
                    else if (expression.Object.IsMethodCall())
                    {
                        return expression.Method.Invoke(expression.Object.ToMethodCall().GetValue(), null);
                    }
                }
                else
                {
                    return expression.Method.Invoke(null, null);
                }
            }
            return null;
        }

        internal static object GetValue(this MemberExpression expression)
        {
            if (expression.Expression != null)
            {
                if (expression.Expression.IsConstant())
                {
                    if (expression.Member.IsFieldInfo())
                    {
                        return expression.Member.AsFieldInfo().GetValue(expression.Expression.ToConstant().GetValue());
                    }
                    else if (expression.Member.IsPropertyInfo())
                    {
                        return expression.Member.AsPropertyInfo().GetValue(expression.Expression.ToConstant().GetValue());
                    }
                }
                else if (expression.Expression.IsMember())
                {
                    if (expression.Member.IsFieldInfo())
                    {
                        return expression.Member.AsFieldInfo().GetValue(expression.Expression.ToMember().GetValue());
                    }
                    else if (expression.Member.IsPropertyInfo())
                    {
                        return expression.Member.AsPropertyInfo().GetValue(expression.Expression.ToMember().GetValue());
                    }
                }
                else if (expression.Expression.IsMethodCall())
                {
                    return expression.Expression.ToMethodCall().GetValue();
                }
            }
            else
            {
                if (expression.Member != null && expression.Member.IsPropertyInfo())
                {
                    return expression.Member.AsPropertyInfo().GetValue(null);
                }
            }
            return null;
        }
    }
}
