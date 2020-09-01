using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    public partial class QueryField
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        private static ClassProperty GetTargetProperty<TEntity>(Field field)
            where TEntity : class
        {
            var properties = PropertyCache.Get<TEntity>();

            // Failing at some point - for base interfaces
            var property = properties
                .FirstOrDefault(p =>
                    string.Equals(p.GetMappedName(), field.Name, StringComparison.OrdinalIgnoreCase));

            // Matches to the actual class properties
            if (property == null)
            {
                property = properties
                    .FirstOrDefault(p =>
                        string.Equals(p.PropertyInfo.Name, field.Name, StringComparison.OrdinalIgnoreCase));
            }

            // Return the value
            return property;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressionType"></param>
        /// <returns></returns>
        internal static Operation GetOperation(ExpressionType expressionType)
        {
            var value = default(Operation);
            if (Enum.TryParse(expressionType.ToString(), out value))
            {
                return value;
            }
            throw new NotSupportedException($"Operation: Expression '{expressionType}' is currently not supported.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="enumerable"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        private static QueryField ToIn(string fieldName,
            System.Collections.IEnumerable enumerable,
            ExpressionType? unaryNodeType = null)
        {
            var operation = unaryNodeType == ExpressionType.Not ? Operation.NotIn : Operation.In;
            return new QueryField(fieldName, operation, enumerable.OfType<object>().AsArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="enumerable"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        private static IEnumerable<QueryField> ToQueryFields(string fieldName,
            System.Collections.IEnumerable enumerable,
            ExpressionType? unaryNodeType = null)
        {
            var operation = (unaryNodeType == ExpressionType.Not || unaryNodeType == ExpressionType.NotEqual) ?
                Operation.NotEqual : Operation.Equal;
            foreach (var item in enumerable)
            {
                yield return new QueryField(fieldName, operation, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        private static QueryField ToLike(string fieldName,
            object value,
            ExpressionType? unaryNodeType = null)
        {
            var operation = unaryNodeType == ExpressionType.Not ? Operation.NotLike : Operation.Like;
            return new QueryField(fieldName, operation, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertToLikeableValue(string methodName,
            string value)
        {
            if (methodName == "Contains")
            {
                value = value.StartsWith("%") ? value : string.Concat("%", value);
                value = value.EndsWith("%") ? value : string.Concat(value, "%");
            }
            else if (methodName == "StartsWith")
            {
                value = value.EndsWith("%") ? value : string.Concat(value, "%");
            }
            else if (methodName == "EndsWith")
            {
                value = value.StartsWith("%") ? value : string.Concat("%", value);
            }
            return value;
        }

        /*
         * Binary
         */

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static IEnumerable<QueryField> Parse<TEntity>(BinaryExpression expression)
            where TEntity : class
        {
            // Only support the following expression type
            if (expression.IsExtractable() == false)
            {
                throw new NotSupportedException($"Expression '{expression}' is currently not supported.");
            }

            // Field
            var field = expression.GetField();
            var property = GetTargetProperty<TEntity>(field);

            // Check
            if (property == null)
            {
                throw new InvalidExpressionException($"Invalid expression '{expression}'. The property {field.Name} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }
            else
            {
                field = property.AsField();
            }

            // Value
            var value = expression.GetValue();

            // Operation
            var operation = GetOperation(expression.NodeType);

            // Enum
            if (property.PropertyInfo.PropertyType.IsEnum)
            {
                value = ToEnumValue(property.PropertyInfo.PropertyType, value);
            }

            // Return the value
            return new QueryField(field, operation, value).AsEnumerable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object ToEnumValue(Type enumType,
            object value) =>
            Enum.Parse(enumType, Enum.GetName(enumType, value));

        /*
         * Member
         */

        internal static IEnumerable<QueryField> Parse<TEntity>(MemberExpression expression,
            ExpressionType? unaryNodeType = null)
            where TEntity : class
        {
            // Property
            var property = GetProperty(expression);

            // Operation
            var operation = unaryNodeType == ExpressionType.Not ? Operation.NotEqual : Operation.Equal;

            // Value
            var value = (object)null;
            if (expression.Type == StaticType.Boolean)
            {
                value = true;
            }
            else
            {
                value = expression.GetValue();
            }

            // Return
            return new QueryField(PropertyMappedNameCache.Get(property), operation, value).AsEnumerable();
        }

        /*
         * MethodCall
         */

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        internal static IEnumerable<QueryField> Parse<TEntity>(MethodCallExpression expression,
        ExpressionType? unaryNodeType = null)
        where TEntity : class
        {
            if (expression.Method.Name == "Contains")
            {
                return ParseContains<TEntity>(expression, unaryNodeType)?.AsEnumerable();
            }
            else if (expression.Method.Name == "StartsWith" ||
                expression.Method.Name == "EndsWith")
            {
                return ParseWith<TEntity>(expression, unaryNodeType)?.AsEnumerable();
            }
            else if (expression.Method.Name == "All")
            {
                return ParseAll<TEntity>(expression, unaryNodeType);
            }
            else if (expression.Method.Name == "Any")
            {
                return ParseAny<TEntity>(expression, unaryNodeType)?.AsEnumerable();
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        internal static QueryField ParseContains<TEntity>(MethodCallExpression expression,
            ExpressionType? unaryNodeType = null)
            where TEntity : class
        {
            // Property
            var property = GetProperty(expression);

            // Value
            if (expression?.Object != null)
            {
                if (expression?.Object?.Type == StaticType.String)
                {
                    var likeable = ConvertToLikeableValue("Contains", Converter.ToType<string>(expression.Arguments.First().GetValue()));
                    return ToLike(PropertyMappedNameCache.Get(property), likeable, unaryNodeType);
                }
                else
                {
                    var enumerable = Converter.ToType<System.Collections.IEnumerable>(expression.Object.GetValue());
                    return ToIn(PropertyMappedNameCache.Get(property), enumerable, unaryNodeType);
                }
            }
            else
            {
                var enumerable = Converter.ToType<System.Collections.IEnumerable>(expression.Arguments.First().GetValue());
                return ToIn(PropertyMappedNameCache.Get(property), enumerable, unaryNodeType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        internal static QueryField ParseWith<TEntity>(MethodCallExpression expression,
            ExpressionType? unaryNodeType = null)
            where TEntity : class
        {
            // Property
            var property = GetProperty(expression);

            // Values
            var value = Converter.ToType<string>(expression.Arguments.First().GetValue());

            // Fields
            return ToLike(PropertyMappedNameCache.Get(property),
                ConvertToLikeableValue(expression.Method.Name, value), unaryNodeType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        internal static IEnumerable<QueryField> ParseAll<TEntity>(MethodCallExpression expression,
            ExpressionType? unaryNodeType = null)
            where TEntity : class
        {
            // Property
            var property = GetProperty(expression);

            // Value
            var enumerable = Converter.ToType<System.Collections.IEnumerable>(expression.Arguments.First().GetValue());
            return ToQueryFields(PropertyMappedNameCache.Get(property), enumerable, unaryNodeType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="unaryNodeType"></param>
        /// <returns></returns>
        internal static IEnumerable<QueryField> ParseAny<TEntity>(MethodCallExpression expression,
            ExpressionType? unaryNodeType = null)
            where TEntity : class
        {
            // Property
            var property = GetProperty(expression);

            // Value
            var enumerable = Converter.ToType<System.Collections.IEnumerable>(expression.Arguments.First().GetValue());
            return ToQueryFields(PropertyMappedNameCache.Get(property), enumerable, unaryNodeType);
        }

        #region GetProperty

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static PropertyInfo GetProperty(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            if (expression.IsLambda())
            {
                return GetProperty(expression.ToLambda());
            }
            else if (expression.IsBinary())
            {
                return GetProperty(expression.ToBinary());
            }
            else if (expression.IsMethodCall())
            {
                return GetProperty(expression.ToMethodCall());
            }
            else if (expression.IsMember())
            {
                return GetProperty(expression.ToMember());
            }
            //throw new PropertyNotFoundException($"Property is not found from the expression '{expression}'.");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static PropertyInfo GetProperty(LambdaExpression expression) =>
            GetProperty(expression.ToLambda().Body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static PropertyInfo GetProperty(BinaryExpression expression) =>
            GetProperty(expression.Left) ?? GetProperty(expression.Right);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static PropertyInfo GetProperty(MemberExpression expression) =>
            expression.Member.ToPropertyInfo();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static PropertyInfo GetProperty(MethodCallExpression expression)
        {
            if (expression?.Object?.Type == StaticType.String)
            {
                return GetProperty(expression.Object.ToMember());
            }
            else
            {
                return GetProperty(expression.Arguments.LastOrDefault());
            }
        }

        #endregion
    }
}
