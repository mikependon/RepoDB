using RepoDb.Exceptions;
using System;
using System.Collections.Generic;
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
        private readonly static HashSet<ExpressionType> extractableExpressionTypes = new()
        {
            ExpressionType.Equal,
            ExpressionType.NotEqual,
            ExpressionType.GreaterThan,
            ExpressionType.GreaterThanOrEqual,
            ExpressionType.LessThan,
            ExpressionType.LessThanOrEqual
        };

        private readonly static HashSet<ExpressionType> mathematicalExpressionTypes = new()
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
            return expression.Left switch
            {
                MemberExpression memberExpression => memberExpression.GetField(),
                UnaryExpression unaryExpression => unaryExpression.GetField(),
                _ => throw new NotSupportedException($"Expression '{expression}' is currently not supported.")
            };
        }

        /// <summary>
        /// Gets the <see cref="Field"/> defined on the current instance of <see cref="UnaryExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Field GetField(this UnaryExpression expression)
        {
            return expression.Operand switch
            {
                MethodCallExpression methodCallExpression => methodCallExpression.GetField(),
                MemberExpression memberExpression => memberExpression.GetField(),
                _ => throw new NotSupportedException($"Expression '{expression}' is currently not supported.")
            };
        }

        /// <summary>
        /// Gets the <see cref="Field"/> defined on the current instance of <see cref="MethodCallExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Field GetField(this MethodCallExpression expression)
        {
            if (expression.Object is MemberExpression objectMemberExpression)
            {
                return objectMemberExpression.GetField();
            }
            else
            {
                // Contains
                if (expression.Method.Name == "Contains")
                {
                    var last = expression.Arguments.Last();
                    if (last is MemberExpression memberExpression)
                    {
                        return memberExpression.GetField();
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
            return expression.Member switch
            {
                PropertyInfo propertyInfo => propertyInfo.AsField(),
                FieldInfo fieldInfo => new Field(fieldInfo.Name, fieldInfo.FieldType),
                _ => throw new NotSupportedException("Only fields and properties are currently supported.")
            };
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
            return expression.Left switch
            {
                MemberExpression memberExpression => memberExpression.Member.GetMappedName(),
                UnaryExpression unaryExpression => unaryExpression.GetName(),
                _ => throw new NotSupportedException($"Expression '{expression}' is currently not supported.")
            };
        }

        /// <summary>
        /// Gets the name of the operand defines on the current instance of <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="UnaryExpression"/> to be checked.</param>
        /// <returns>The name of the operand.</returns>
        public static string GetName(this UnaryExpression expression)
        {
            if (expression.Operand is MethodCallExpression methodCallExpression)
            {
                return methodCallExpression.GetName();
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
            if (expression.Object is MemberExpression objectMemberExpression)
            {
                return objectMemberExpression.GetName();
            }
            else
            {
                if (expression.Method.Name == "Contains" ||
                    expression.Method.Name == "StartsWith" ||
                    expression.Method.Name == "EndsWith")
                {
                    var last = expression.Arguments.Last();
                    if (last is MemberExpression memberExpression)
                    {
                        return memberExpression.Member.GetMappedName();
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
            return expression.Left switch
            {
                MemberExpression memberExpression => memberExpression.GetMemberType(),
                UnaryExpression unaryExpression => unaryExpression.GetMemberType(),
                _ => throw new NotSupportedException($"Expression '{expression}' is currently not supported.")
            };
        }

        /// <summary>
        /// Gets the type of the operand defines on the current instance of <see cref="UnaryExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="UnaryExpression"/> to be checked.</param>
        /// <returns>The type of the operand.</returns>
        public static Type GetMemberType(this UnaryExpression expression)
        {
            if (expression.Operand is MethodCallExpression methodCallExpression)
            {
                return methodCallExpression.GetMemberType();
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
            if (expression.Object is MemberExpression memberExpression)
            {
                return memberExpression.GetMemberType();
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
            return expression.Member switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                FieldInfo fieldInfo => fieldInfo.FieldType,
                _ => throw new NotSupportedException($"Expression '{expression}' is currently not supported.")
            };
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
            return expression switch
            {
                BinaryExpression binaryExpression => binaryExpression.GetValue(),
                ConstantExpression constantExpression => constantExpression.GetValue(),
                UnaryExpression unaryExpression => unaryExpression.GetValue(),
                MethodCallExpression methodCallExpression => methodCallExpression.GetValue(),
                MemberExpression memberExpression => memberExpression.GetValue(),
                NewArrayExpression newArrayExpression => newArrayExpression.GetValue(),
                ListInitExpression listInitExpression => listInitExpression.GetValue(),
                NewExpression newExpression => newExpression.GetValue(),
                MemberInitExpression memberInitExpression => memberInitExpression.GetValue(),
                ConditionalExpression conditionalExpression => conditionalExpression.GetValue(),
                ParameterExpression parameterExpression => parameterExpression.GetValue(),
                DefaultExpression defaultExpression => defaultExpression.GetValue(),
                _ => throw new NotSupportedException($"Expression '{expression}' is currently not supported.")
            };
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
        /// Converts the <see cref="Expression"/> object into <see cref="MemberExpression"/> object.
        /// </summary>
        /// <param name="expression">The instance of <see cref="Expression"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MemberExpression"/> object.</returns>
        public static MemberExpression ToMember(this Expression expression) =>
            (MemberExpression)expression;

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
            return expression.Body switch
            {
                UnaryExpression unaryExpression => GetProperty<T>(unaryExpression),
                MemberExpression memberExpression => GetProperty<T>(memberExpression),
                BinaryExpression binaryExpression => GetProperty<T>(binaryExpression),
                _ => throw new InvalidExpressionException($"Expression '{expression}' is not valid.")
            };
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
            return expression.Left switch
            {
                MemberExpression memberExpression => GetProperty<T>(memberExpression),
                UnaryExpression unaryExpression => GetProperty<T>(unaryExpression),
                _ => throw new InvalidExpressionException($"Expression '{expression}' is not valid.")
            };
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
            return expression.Operand switch
            {
                MemberExpression memberExpression => GetProperty<T>(memberExpression),
                BinaryExpression binaryExpression => GetProperty<T>(binaryExpression),
                _ => throw new InvalidExpressionException($"Expression '{expression}' is not valid.")
            };
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
            if (expression.Member is PropertyInfo propertyInfo)
            {
                return propertyInfo;
            }
            throw new InvalidExpressionException($"Expression '{expression}' is not valid.");
        }

        #endregion
    }
}
