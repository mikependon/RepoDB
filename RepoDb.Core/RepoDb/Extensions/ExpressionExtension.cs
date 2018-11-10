using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> can be extracted as <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression can be extracted as <see cref="QueryField"/> object.</returns>
        public static bool CanBeExtracted(this Expression expression)
        {
            return m_extractableExpressionTypes.Contains(expression.NodeType);
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> can be grouped as <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression can be grouped as <see cref="QueryGroup"/> object.</returns>
        public static bool CanBeGrouped(this Expression expression)
        {
            return expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse;
        }

        /// <summary>
        /// Gets the name of the <see cref="MemberInfo"/> defines on the current instance of <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="BinaryExpression"/> to be checked.</param>
        /// <returns>The name of the <see cref="MemberInfo"/>.</returns>
        public static string GetName(this BinaryExpression expression)
        {
            if (expression.Left.IsMember())
            {
                return expression.Left.ToMember().Member.Name;
            }
            if (expression.Right.IsMember())
            {
                return expression.Right.ToMember().Member.Name;
            }
            return null;
        }

        #region GetValue

        /// <summary>
        /// Gets a value from the current instance of <see cref="Expression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="Expression"/> object.</returns>
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
            else if (expression.IsNew())
            {
                return expression.ToNew().GetValue();
            }
            else if (expression.IsMemberInit())
            {
                return expression.ToMemberInit().GetValue();
            }
            else if (expression.IsConditional())
            {
                return expression.ToConditional().GetValue();
            }
            else if (expression.IsParameter())
            {
                return expression.ToParameter().GetValue();
            }
            else if (expression.IsDefault())
            {
                return expression.ToDefault().GetValue();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="BinaryExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="BinaryExpression"/> object.</returns>
        public static object GetValue(this BinaryExpression expression)
        {
            return expression.GetValue(true);
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="BinaryExpression"/> object where the value is to be extracted.</param>
        /// <param name="includeLeft">True if the value at the left side will also be retrieved if the value at the right is NULL.</param>
        /// <returns>The extracted value from <see cref="BinaryExpression"/> object.</returns>
        public static object GetValue(this BinaryExpression expression, bool includeLeft = true)
        {
            return expression.Right.GetValue() ?? (includeLeft ? expression.Left.GetValue() : null);
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="ConstantExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="ConstantExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="ConstantExpression"/> object.</returns>
        public static object GetValue(this ConstantExpression expression)
        {
            return expression.Value;
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="UnaryExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="UnaryExpression"/> object.</returns>
        public static object GetValue(this UnaryExpression expression)
        {
            return expression.Operand.GetValue();
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MethodCallExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="MethodCallExpression"/> object.</returns>
        public static object GetValue(this MethodCallExpression expression)
        {
            return expression.Method.GetValue(expression.Object?.GetValue(),
                expression.Arguments?.Select(argExpression => argExpression.GetValue()).ToArray());
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MemberExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="MemberExpression"/> object.</returns>
        public static object GetValue(this MemberExpression expression)
        {
            return expression.Member.GetValue(expression.Expression?.GetValue());
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="NewArrayExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="NewArrayExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="NewArrayExpression"/> object.</returns>
        public static object GetValue(this NewArrayExpression expression)
        {
            var arrayType = expression.Type.GetElementType();
            var array = Array.CreateInstance(arrayType, expression.Expressions.Count);
            expression
                .Expressions?
                .ToList()
                .ForEach(item => array.SetValue(item.GetValue(), expression.Expressions.IndexOf(item)));
            return array;
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="NewExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="NewExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="NewExpression"/> object.</returns>
        public static object GetValue(this NewExpression expression)
        {
            if (expression.Arguments?.Any() == true)
            {
                return Activator.CreateInstance(expression.Constructor.DeclaringType,
                    expression.Arguments?.Select(arg => arg.GetValue()));
            }
            else
            {
                return Activator.CreateInstance(expression.Constructor.DeclaringType);
            }
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="MemberInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MemberInitExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="MemberInitExpression"/> object.</returns>
        public static object GetValue(this MemberInitExpression expression)
        {
            var instance = expression.NewExpression.GetValue();
            expression.Bindings.ToList().ForEach(member =>
                member.Member.SetValue(instance, member.GetValue()));
            return instance;
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="ConditionalExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="ConditionalExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="ConditionalExpression"/> object.</returns>
        public static object GetValue(this ConditionalExpression expression)
        {
            var ifTrue = expression.IfTrue.GetValue();
            return expression.Test.GetValue() == ifTrue ? ifTrue : expression.IfFalse.GetValue();
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="ParameterExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="ParameterExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="ParameterExpression"/> object.</returns>
        public static object GetValue(this ParameterExpression expression)
        {
            switch (expression.GetType().Name)
            {
                case "TypedParameterExpression":
                    return Activator.CreateInstance(expression.Type);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="DefaultExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="DefaultExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="DefaultExpression"/> object.</returns>
        public static object GetValue(this DefaultExpression expression)
        {
            return expression.Type.GetTypeInfo().IsValueType ? Activator.CreateInstance(expression.Type) : null;
        }

        #endregion

        #region Identification and Conversion

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="LambdaExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="LambdaExpression"/>.</returns>
        public static bool IsLambda(this Expression expression)
        {
            return expression is LambdaExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="LambdaExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="LambdaExpression"/> object.</returns>
        public static LambdaExpression ToLambda(this Expression expression)
        {
            return (LambdaExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="BinaryExpression"/>.</returns>
        public static bool IsBinary(this Expression expression)
        {
            return expression is BinaryExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="BinaryExpression"/> object.</returns>
        public static BinaryExpression ToBinary(this Expression expression)
        {
            return (BinaryExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="ConstantExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="ConstantExpression"/>.</returns>
        public static bool IsConstant(this Expression expression)
        {
            return expression is ConstantExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="ConstantExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="ConstantExpression"/> object.</returns>
        public static ConstantExpression ToConstant(this Expression expression)
        {
            return (ConstantExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="UnaryExpression"/>.</returns>
        public static bool IsUnary(this Expression expression)
        {
            return expression is UnaryExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="UnaryExpression"/> object.</returns>
        public static UnaryExpression ToUnary(this Expression expression)
        {
            return (UnaryExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="MethodCallExpression"/>.</returns>
        public static bool IsMethodCall(this Expression expression)
        {
            return expression is MethodCallExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MethodCallExpression"/> object.</returns>
        public static MethodCallExpression ToMethodCall(this Expression expression)
        {
            return (MethodCallExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="MemberExpression"/>.</returns>
        public static bool IsMember(this Expression expression)
        {
            return expression is MemberExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MemberExpression"/> object.</returns>
        public static MemberExpression ToMember(this Expression expression)
        {
            return (MemberExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="NewArrayExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="NewArrayExpression"/>.</returns>
        public static bool IsNewArray(this Expression expression)
        {
            return expression is NewArrayExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="NewArrayExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="NewArrayExpression"/> object.</returns>
        public static NewArrayExpression ToNewArray(this Expression expression)
        {
            return (NewArrayExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="NewExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="NewExpression"/>.</returns>
        public static bool IsNew(this Expression expression)
        {
            return expression is NewExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="NewExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="NewExpression"/> object.</returns>
        public static NewExpression ToNew(this Expression expression)
        {
            return (NewExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="MemberInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="MemberInitExpression"/>.</returns>
        public static bool IsMemberInit(this Expression expression)
        {
            return expression is MemberInitExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="MemberInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MemberInitExpression"/> object.</returns>
        public static MemberInitExpression ToMemberInit(this Expression expression)
        {
            return (MemberInitExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="ConditionalExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="ConditionalExpression"/>.</returns>
        public static bool IsConditional(this Expression expression)
        {
            return expression is ConditionalExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="ConditionalExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="ConditionalExpression"/> object.</returns>
        public static ConditionalExpression ToConditional(this Expression expression)
        {
            return (ConditionalExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="ParameterExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="ParameterExpression"/>.</returns>
        public static bool IsParameter(this Expression expression)
        {
            return expression is ParameterExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="ParameterExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="ParameterExpression"/> object.</returns>
        public static ParameterExpression ToParameter(this Expression expression)
        {
            return (ParameterExpression)expression;
        }

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="DefaultExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="DefaultExpression"/>.</returns>
        public static bool IsDefault(this Expression expression)
        {
            return expression is DefaultExpression;
        }

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="DefaultExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="DefaultExpression"/> object.</returns>
        public static DefaultExpression ToDefault(this Expression expression)
        {
            return (DefaultExpression)expression;
        }

        #endregion
    }
}
