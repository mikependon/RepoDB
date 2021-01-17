using RepoDb.Exceptions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Extensions
{
    /**
     * Though we know that throwing an exception in the extension is not advisable, but I tend to do it to ensure that the
     * parsing of the Linq expressions are properly handled. Please be guided about this extension that it somehow throws
     * and exception at some scenarios.
     */

    /// <summary>
    /// Contains the extension methods for <see cref="Expression"/> object.
    /// </summary>
    public static class ExpressionExtension
    {
        private readonly static ExpressionType[] extractableExpressionTypes = new[]
        {
            ExpressionType.Equal,
            ExpressionType.NotEqual,
            ExpressionType.GreaterThan,
            ExpressionType.GreaterThanOrEqual,
            ExpressionType.LessThan,
            ExpressionType.LessThanOrEqual
        };

        private readonly static ExpressionType[] mathematicalExpressionTypes = new[]
        {
            ExpressionType.Add,
            ExpressionType.Subtract,
            ExpressionType.Multiply,
            ExpressionType.Divide
        };

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> can be extracted as <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression can be extracted as <see cref="QueryField"/> object.</returns>
        internal static bool IsExtractable(this Expression expression) =>
            extractableExpressionTypes.Contains(expression.NodeType);

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> can be grouped as <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression can be grouped as <see cref="QueryGroup"/> object.</returns>
        internal static bool IsGroupable(this Expression expression) =>
            expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is using the <see cref="Math"/> object operations.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is using the <see cref="Math"/> object operations.</returns>
        internal static bool IsMathematical(this Expression expression) =>
            mathematicalExpressionTypes.Contains(expression.NodeType);

        #region GetField

        /// <summary>
        /// Gets the <see cref="Field"/> defined on the current instance of <see cref="BinaryExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Field GetField(this BinaryExpression expression)
        {
            if (expression.Left.IsMember())
            {
                return expression.Left.ToMember().GetField();
            }
            else if (expression.Left.IsUnary())
            {
                return expression.Left.ToUnary().GetField();
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the <see cref="Field"/> defined on the current instance of <see cref="UnaryExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Field GetField(this UnaryExpression expression)
        {
            if (expression.Operand.IsMethodCall())
            {
                return expression.Operand.ToMethodCall().GetField();
            }
            else if (expression.Operand.IsMember())
            {
                return expression.Operand.ToMember().GetField();
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the <see cref="Field"/> defined on the current instance of <see cref="MethodCallExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Field GetField(this MethodCallExpression expression)
        {
            if (expression.Object?.IsMember() == true)
            {
                return expression.Object.ToMember().GetField();
            }
            else
            {
                // Contains
                if (expression.Method.Name == "Contains")
                {
                    var last = expression.Arguments.Last();
                    if (last?.IsMember() == true)
                    {
                        return last.ToMember().GetField();
                    }
                }
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the <see cref="Field"/> defined on the current instance of <see cref="MemberExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Field GetField(this MemberExpression expression)
        {
            if (expression.Member.IsPropertyInfo())
            {
                return expression.Member.ToPropertyInfo().AsField();
            }
            else if (expression.Member.IsFieldInfo())
            {
                var fieldInfo = expression.Member.ToFieldInfo();
                return new Field(fieldInfo.Name, fieldInfo.FieldType);
            }
            throw new NotSupportedException($"Only fields and properties are currently supported.");
        }

        #endregion

        #region GetName

        /// <summary>
        /// Gets the name of the <see cref="MemberInfo"/> defines on the current instance of <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="BinaryExpression"/> to be checked.</param>
        /// <returns>The name of the <see cref="MemberInfo"/>.</returns>
        public static string GetName(this BinaryExpression expression)
        {
            if (expression.Left.IsMember())
            {
                return expression.Left.ToMember().Member.GetMappedName();
            }
            else if (expression.Left.IsUnary())
            {
                return expression.Left.ToUnary().GetName();
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the name of the operand defines on the current instance of <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="UnaryExpression"/> to be checked.</param>
        /// <returns>The name of the operand.</returns>
        public static string GetName(this UnaryExpression expression)
        {
            if (expression.Operand.IsMethodCall())
            {
                return expression.Operand.ToMethodCall().GetName();
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the name of the operand defines on the current instance of <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MethodCallExpression"/> to be checked.</param>
        /// <returns>The name of the operand.</returns>
        public static string GetName(this MethodCallExpression expression)
        {
            if (expression.Object?.IsMember() == true)
            {
                return expression.Object.ToMember().GetName();
            }
            else
            {
                if (expression.Method.Name == "Contains" ||
                    expression.Method.Name == "StartsWith" ||
                    expression.Method.Name == "EndsWith")
                {
                    var last = expression.Arguments.Last();
                    if (last?.IsMember() == true)
                    {
                        return last.ToMember().Member.GetMappedName();
                    }
                }
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the name of the <see cref="MemberInfo"/> defines on the current instance of <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MemberExpression"/> to be checked.</param>
        /// <returns>The name of the <see cref="MemberInfo"/>.</returns>
        public static string GetName(this MemberExpression expression) =>
            expression.Member.GetMappedName();

        #endregion

        #region GetMemberType

        /// <summary>
        /// Gets the type of the <see cref="MemberInfo"/> defines on the current instance of <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="BinaryExpression"/> to be checked.</param>
        /// <returns>The type of the <see cref="MemberInfo"/>.</returns>
        public static Type GetMemberType(this BinaryExpression expression)
        {
            if (expression.Left.IsMember())
            {
                return expression.Left.ToMember().GetMemberType();
            }
            else if (expression.Left.IsUnary())
            {
                return expression.Left.ToUnary().GetMemberType();
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the type of the operand defines on the current instance of <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="UnaryExpression"/> to be checked.</param>
        /// <returns>The type of the operand.</returns>
        public static Type GetMemberType(this UnaryExpression expression)
        {
            if (expression.Operand.IsMethodCall())
            {
                return expression.Operand.ToMethodCall().GetMemberType();
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the type of the operand defines on the current instance of <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MethodCallExpression"/> to be checked.</param>
        /// <returns>The type of the operand.</returns>
        public static Type GetMemberType(this MethodCallExpression expression)
        {
            if (expression.Object?.IsMember() == true)
            {
                return expression.Object.ToMember().GetMemberType();
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets the type of the <see cref="MemberInfo"/> defines on the current instance of <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MemberExpression"/> to be checked.</param>
        /// <returns>The type of the <see cref="MemberInfo"/>.</returns>
        public static Type GetMemberType(this MemberExpression expression)
        {
            var member = expression.Member;
            if (member.IsPropertyInfo())
            {
                return member.ToPropertyInfo().PropertyType;
            }
            else if (member.IsFieldInfo())
            {
                return member.ToFieldInfo().FieldType;
            }
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        #endregion

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
            else if (expression.IsListInit())
            {
                return expression.ToListInit().GetValue();
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
            throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="BinaryExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="BinaryExpression"/> object.</returns>
        public static object GetValue(this BinaryExpression expression)
        {
            if (IsMathematical(expression))
            {
                throw new NotSupportedException($"A mathematical expression '{expression}' is currently not supported.");
            }
            return expression.Right.GetValue();
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="ConstantExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="ConstantExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="ConstantExpression"/> object.</returns>
        public static object GetValue(this ConstantExpression expression) =>
            expression.Value;

        /// <summary>
        /// Gets a value from the current instance of <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="UnaryExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="UnaryExpression"/> object.</returns>
        public static object GetValue(this UnaryExpression expression) =>
            expression.Operand.GetValue();

        /// <summary>
        /// Gets a value from the current instance of <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MethodCallExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="MethodCallExpression"/> object.</returns>
        public static object GetValue(this MethodCallExpression expression) =>
            expression.Method.GetValue(expression.Object?.GetValue(), expression.Arguments.Select(argExpression => argExpression.GetValue()).ToArray());

        /// <summary>
        /// Gets a value from the current instance of <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="MemberExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="MemberExpression"/> object.</returns>
        public static object GetValue(this MemberExpression expression) =>
            expression.Member.GetValue(expression.Expression?.GetValue());

        /// <summary>
        /// Gets a value from the current instance of <see cref="NewArrayExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="NewArrayExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="NewArrayExpression"/> object.</returns>
        public static object GetValue(this NewArrayExpression expression)
        {
            var arrayType = expression.Type.HasElementType ? expression.Type.GetElementType() : expression.Type;
            var array = Array.CreateInstance(arrayType, expression.Expressions.Count);
            for (var i = 0; i < expression.Expressions.Count; i++)
            {
                array.SetValue(expression.Expressions[i].GetValue(), i);
            }
            return array;
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="ListInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="ListInitExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="ListInitExpression"/> object.</returns>
        public static object GetValue(this ListInitExpression expression)
        {
            var list = Activator.CreateInstance(expression.Type);
            foreach (var item in expression.Initializers)
            {
                item.AddMethod.Invoke(list, new[] { item.Arguments.FirstOrDefault().GetValue() });
            }
            return list;
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="NewExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="NewExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="NewExpression"/> object.</returns>
        public static object GetValue(this NewExpression expression)
        {
            if (expression.Arguments.Count > 0)
            {
                return Activator.CreateInstance(expression.Type,
                    expression.Arguments.Select(arg => arg.GetValue()));
            }
            else
            {
                return Activator.CreateInstance(expression.Type);
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
            foreach (var binding in expression.Bindings)
            {
                binding.Member.SetValue(instance, binding.GetValue());
            }
            return instance;
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="ConditionalExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="ConditionalExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="ConditionalExpression"/> object.</returns>
        public static object GetValue(this ConditionalExpression expression)
        {
            var test = expression.Test.GetValue();
            var trueValue = expression.IfTrue.GetValue();
            if (expression.Test.NodeType == ExpressionType.Equal)
            {
                return test == trueValue ? trueValue : expression.IfFalse.GetValue();
            }
            else if (expression.Test.NodeType == ExpressionType.NotEqual)
            {
                return test != trueValue ? trueValue : expression.IfFalse.GetValue();
            }
            else if (expression.Test.NodeType > ExpressionType.GreaterThan)
            {
                return test.ToNumber() > trueValue.ToNumber() ? trueValue : expression.IfFalse.GetValue();
            }
            else if (expression.Test.NodeType > ExpressionType.GreaterThanOrEqual)
            {
                return test.ToNumber() >= trueValue?.ToNumber() ? trueValue : expression.IfFalse.GetValue();
            }
            else if (expression.Test.NodeType > ExpressionType.LessThan)
            {
                return test.ToNumber() < trueValue?.ToNumber() ? trueValue : expression.IfFalse.GetValue();
            }
            else if (expression.Test.NodeType > ExpressionType.LessThanOrEqual)
            {
                return test.ToNumber() <= trueValue?.ToNumber() ? trueValue : expression.IfFalse.GetValue();
            }
            throw new NotSupportedException($"The operation '{expression.NodeType}' at expression '{expression}' is currently not supported.");
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="ParameterExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="ParameterExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="ParameterExpression"/> object.</returns>
        public static object GetValue(this ParameterExpression expression)
        {
            if (expression.Type.GetConstructors().Any(e => e.GetParameters().Length == 0))
            {
                return Activator.CreateInstance(expression.Type);
            }
            throw new InvalidExpressionException($"The default constructor for expression '{expression}' is not found.");
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="DefaultExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="DefaultExpression"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="DefaultExpression"/> object.</returns>
        public static object GetValue(this DefaultExpression expression) =>
            expression.Type.IsValueType ? Activator.CreateInstance(expression.Type) : null;

        #endregion

        #region Identification and Conversion

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="LambdaExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="LambdaExpression"/>.</returns>
        public static bool IsLambda(this Expression expression) =>
            expression is LambdaExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="LambdaExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="LambdaExpression"/> object.</returns>
        public static LambdaExpression ToLambda(this Expression expression) =>
            (LambdaExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="BinaryExpression"/>.</returns>
        public static bool IsBinary(this Expression expression) =>
            expression is BinaryExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="BinaryExpression"/> object.</returns>
        public static BinaryExpression ToBinary(this Expression expression) =>
            (BinaryExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="ConstantExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="ConstantExpression"/>.</returns>
        public static bool IsConstant(this Expression expression) =>
            expression is ConstantExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="ConstantExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="ConstantExpression"/> object.</returns>
        public static ConstantExpression ToConstant(this Expression expression) =>
            (ConstantExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="UnaryExpression"/>.</returns>
        public static bool IsUnary(this Expression expression) =>
            expression is UnaryExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="UnaryExpression"/> object.</returns>
        public static UnaryExpression ToUnary(this Expression expression) =>
            (UnaryExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="MethodCallExpression"/>.</returns>
        public static bool IsMethodCall(this Expression expression) =>
            expression is MethodCallExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="MethodCallExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MethodCallExpression"/> object.</returns>
        public static MethodCallExpression ToMethodCall(this Expression expression) =>
            (MethodCallExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="MemberExpression"/>.</returns>
        public static bool IsMember(this Expression expression) =>
            expression is MemberExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MemberExpression"/> object.</returns>
        public static MemberExpression ToMember(this Expression expression) =>
            (MemberExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="NewArrayExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="NewArrayExpression"/>.</returns>
        public static bool IsNewArray(this Expression expression) =>
            expression is NewArrayExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="NewArrayExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="NewArrayExpression"/> object.</returns>
        public static NewArrayExpression ToNewArray(this Expression expression) =>
            (NewArrayExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="ListInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="ListInitExpression"/>.</returns>
        public static bool IsListInit(this Expression expression) =>
            expression is ListInitExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="ListInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="ListInitExpression"/> object.</returns>
        public static ListInitExpression ToListInit(this Expression expression) =>
            (ListInitExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="NewExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="NewExpression"/>.</returns>
        public static bool IsNew(this Expression expression) =>
            expression is NewExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="NewExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="NewExpression"/> object.</returns>
        public static NewExpression ToNew(this Expression expression) =>
            (NewExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="MemberInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="MemberInitExpression"/>.</returns>
        public static bool IsMemberInit(this Expression expression) =>
            expression is MemberInitExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="MemberInitExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MemberInitExpression"/> object.</returns>
        public static MemberInitExpression ToMemberInit(this Expression expression) =>
            (MemberInitExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="ConditionalExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="ConditionalExpression"/>.</returns>
        public static bool IsConditional(this Expression expression) =>
            expression is ConditionalExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="ConditionalExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="ConditionalExpression"/> object.</returns>
        public static ConditionalExpression ToConditional(this Expression expression) =>
            (ConditionalExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="ParameterExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="ParameterExpression"/>.</returns>
        public static bool IsParameter(this Expression expression) =>
            expression is ParameterExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="ParameterExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="ParameterExpression"/> object.</returns>
        public static ParameterExpression ToParameter(this Expression expression) =>
            (ParameterExpression)expression;

        /// <summary>
        /// Identify whether the instance of <see cref="Expression"/> is a <see cref="DefaultExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="DefaultExpression"/>.</returns>
        public static bool IsDefault(this Expression expression) =>
            expression is DefaultExpression;

        /// <summary>
        /// Converts the <see cref="Expression"/> object into <see cref="DefaultExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="DefaultExpression"/> object.</returns>
        public static DefaultExpression ToDefault(this Expression expression) =>
            (DefaultExpression)expression;

        #endregion

        #region Helper

        /*
         * GetProperty
         */

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on expression.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="expression">The expression to be extracted.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
            where T : class
        {
            if (expression.Body.IsUnary())
            {
                return GetProperty<T>(expression.Body.ToUnary());
            }
            else if (expression.Body.IsMember())
            {
                return GetProperty<T>(expression.Body.ToMember());
            }
            else if (expression.Body.IsBinary())
            {
                return GetProperty<T>(expression.Body.ToBinary());
            }
            throw new InvalidExpressionException($"Expression '{expression}' is not valid.");
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="expression">The expression to be extracted.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty<T>(BinaryExpression expression)
            where T : class
        {
            if (expression.Left.IsMember())
            {
                return GetProperty<T>(expression.Left.ToMember());
            }
            else if (expression.Left.IsUnary())
            {
                return GetProperty<T>(expression.Left.ToUnary());
            }
            throw new InvalidExpressionException($"Expression '{expression}' is not valid.");
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="expression">The expression to be extracted.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty<T>(UnaryExpression expression)
            where T : class
        {
            if (expression.Operand.IsMember())
            {
                return GetProperty<T>(expression.Operand.ToMember());
            }
            else if (expression.Operand.IsBinary())
            {
                return GetProperty<T>(expression.Operand.ToBinary());
            }
            throw new InvalidExpressionException($"Expression '{expression}' is not valid.");
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on <see cref="MemberExpression"/> object.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="expression">The expression to be extracted.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty<T>(MemberExpression expression)
            where T : class
        {
            if (expression.Member.IsPropertyInfo())
            {
                return expression.Member.ToPropertyInfo();
            }
            throw new InvalidExpressionException($"Expression '{expression}' is not valid.");
        }

        #endregion
    }
}
