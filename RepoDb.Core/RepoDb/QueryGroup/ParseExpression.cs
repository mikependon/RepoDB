using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb
{
    public partial class QueryGroup
    {
        /// <summary>
        /// Parses a customized query expression.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type</typeparam>
        /// <param name="expression">The expression to be converted to a <see cref="QueryGroup"/> object.</param>
        /// <returns>An instance of the <see cref="QueryGroup"/> object that contains the parsed query expression.</returns>
        public static QueryGroup Parse<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            // Guard the presense of the expression
            if (expression == null)
            {
                throw new NullReferenceException("Expression cannot be null.");
            }

            // Parse the expression base on type
            var parsed = Parse<TEntity>(expression.Body);

            // Throw an unsupported exception if not parsed
            if (parsed == null)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Return the parsed values
            return parsed.Fix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(Expression expression)
            where TEntity : class
        {
            if (expression.IsLambda())
            {
                return Parse<TEntity>(expression.ToLambda().Body);
            }
            else if (expression.IsBinary())
            {
                return Parse<TEntity>(expression.ToBinary());
            }
            else if (expression.IsUnary())
            {
                return Parse<TEntity>(expression.ToUnary(), null, expression.NodeType, true);
            }
            else if (expression.IsMethodCall())
            {
                return Parse<TEntity>(expression.ToMethodCall(), false, true);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(BinaryExpression expression)
            where TEntity : class
        {
            var leftQueryGroup = (QueryGroup)null;
            var rightQueryGroup = (QueryGroup)null;
            var rightValue = (object)null;
            var skipRight = false;
            var isEqualsTo = true;

            // TODO: Refactor this

            /*
             * LEFT
             */

            // Get the value in the right
            if (expression.IsExtractable())
            {
                rightValue = expression.Right.GetValue();
                skipRight = true;
                if (rightValue is bool)
                {
                    isEqualsTo = Equals(rightValue, false) == false;
                }
            }

            // Binary
            if (expression.Left.IsBinary() == true)
            {
                leftQueryGroup = Parse<TEntity>(expression.Left.ToBinary());
                leftQueryGroup.SetIsNot(isEqualsTo == false);
            }
            // Unary
            else if (expression.Left.IsUnary() == true)
            {
                leftQueryGroup = Parse<TEntity>(expression.Left.ToUnary(), rightValue, expression.NodeType, isEqualsTo);
            }
            // MethodCall
            else if (expression.Left.IsMethodCall())
            {
                leftQueryGroup = Parse<TEntity>(expression.Left.ToMethodCall(), false, isEqualsTo);
            }
            else
            {
                // Extractable
                if (expression.IsExtractable())
                {
                    var queryField = QueryField.Parse<TEntity>(expression);
                    leftQueryGroup = new QueryGroup(queryField);
                    skipRight = true;
                }
            }

            // Identify the node type
            if (expression.NodeType == ExpressionType.NotEqual)
            {
                leftQueryGroup.SetIsNot(leftQueryGroup.IsNot == isEqualsTo);
            }

            /*
             * RIGHT
             */

            if (skipRight == false)
            {
                // Binary
                if (expression.Right.IsBinary() == true)
                {
                    rightQueryGroup = Parse<TEntity>(expression.Right.ToBinary());
                }
                // Unary
                else if (expression.Right.IsUnary() == true)
                {
                    rightQueryGroup = Parse<TEntity>(expression.Right.ToUnary(), null, expression.NodeType, true);
                }
                // MethodCall
                else if (expression.Right.IsMethodCall())
                {
                    rightQueryGroup = Parse<TEntity>(expression.Right.ToMethodCall(), false, true);
                }

                // Return both of them
                if (leftQueryGroup != null && rightQueryGroup != null)
                {
                    var conjunction = (expression.NodeType == ExpressionType.OrElse) ? Conjunction.Or : Conjunction.And;
                    return new QueryGroup(new[] { leftQueryGroup, rightQueryGroup }, conjunction);
                }
            }

            // Return either one of them
            return leftQueryGroup ?? rightQueryGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="rightValue"></param>
        /// <param name="expressionType"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(UnaryExpression expression,
            object rightValue,
            ExpressionType expressionType,
            bool isEqualsTo)
            where TEntity : class
        {
            if (expression.Operand?.IsMember() == true)
            {
                return Parse<TEntity>(expression.Operand.ToMember(), rightValue, expressionType, false, true);
            }
            else if (expression.Operand?.IsMethodCall() == true)
            {
                return Parse<TEntity>(expression.Operand.ToMethodCall(), (expression.NodeType == ExpressionType.Not), isEqualsTo);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="rightValue"></param>
        /// <param name="expressionType"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(MemberExpression expression,
            object rightValue,
            ExpressionType expressionType,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            var queryGroup = (QueryGroup)null;
            var value = rightValue;
            var isForBoolean = expression.Type == typeof(bool) &&
                (expressionType == ExpressionType.Not || expressionType == ExpressionType.AndAlso || expressionType == ExpressionType.OrElse);
            var ignoreIsNot = false;

            // Handle for boolean
            if (value == null)
            {
                if (isForBoolean)
                {
                    value = false;
                    ignoreIsNot = true;
                }
                else
                {
                    value = expression.GetValue();
                }
            }

            // Check if there are values
            if (value != null)
            {
                // Specialized for enum
                if (expression.Type.IsEnum)
                {
                    value = Enum.ToObject(expression.Type, value);
                }

                // Create a new field
                var field = (QueryField)null;

                if (isForBoolean)
                {
                    field = new QueryField(expression.Member.GetMappedName(),
                        value);
                    ignoreIsNot = true;
                }
                else
                {
                    field = new QueryField(expression.Member.GetMappedName(),
                        QueryField.GetOperation(expressionType),
                        value);
                }

                // Set the query group
                queryGroup = new QueryGroup(field);

                // Set the query group IsNot property
                if (ignoreIsNot == false)
                {
                    queryGroup.SetIsNot(isEqualsTo == false);
                }
            }

            // Return the result
            return queryGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // Check methods for the 'Like', both 'Array.<All|Any>()'
            if (expression.Method.Name == "All" || expression.Method.Name == "Any")
            {
                return ParseAllOrAnyForArrayOrAnyForList<TEntity>(expression, isNot, isEqualsTo);
            }

            // Check methods for the 'Like', both 'Array.Contains()' and 'StringProperty.Contains()'
            else if (expression.Method.Name == "Contains")
            {
                if (expression.Object?.IsMember() == true)
                {
                    // Cast to proper object
                    var member = expression.Object.ToMember();
                    if (member.Type == typeof(string))
                    {
                        // Check for the (p => p.Property.Contains("A")) for LIKE
                        return ParseContainsOrStartsWithOrEndsWithForStringProperty<TEntity>(expression, isNot, isEqualsTo);
                    }
                    else if (member.Type.IsConstructedGenericType == true)
                    {
                        // Check for the (p => list.Contains(p.Property)) or (p => (new List<int> { 1, 2 }).Contains(p.Property))
                        return ParseContainsForArrayOrList<TEntity>(expression, isNot, isEqualsTo);
                    }
                }
                else
                {
                    // Check for the (array.Contains(p.Property)) or (new [] { value1, value2 }).Contains(p.Property))
                    return ParseContainsForArrayOrList<TEntity>(expression, isNot, isEqualsTo);
                }
            }

            // Check methods for the 'Like', both 'StringProperty.StartsWith()' and 'StringProperty.EndsWith()'
            else if (expression.Method.Name == "StartsWith" || expression.Method.Name == "EndsWith")
            {
                if (expression.Object?.IsMember() == true)
                {
                    if (expression.Object.ToMember().Type == typeof(string))
                    {
                        return ParseContainsOrStartsWithOrEndsWithForStringProperty<TEntity>(expression, isNot, isEqualsTo);
                    }
                }
            }

            // Return null if not supported
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup ParseAllOrAnyForArrayOrAnyForList<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // TODO: Refactor this

            // Return null if there is no any arguments
            if (expression.Arguments?.Any() != true)
            {
                return null;
            }

            // Get the last property
            var last = expression
                .Arguments
                .LastOrDefault();

            // Make sure the last is a member
            if (last == null || last?.IsLambda() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure the last is a binary
            var lambda = last.ToLambda();
            if (lambda.Body.IsBinary() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure it is a member
            var binary = lambda.Body.ToBinary();
            if (binary.Left.IsMember() == false && binary.Right.IsMember() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported. Expression must contain a single condition to any property of type '{typeof(TEntity).FullName}'.");
            }

            // Make sure it is a property
            var member = binary.Left.IsMember() ? binary.Left.ToMember().Member : binary.Right.ToMember().Member;
            if (member.IsPropertyInfo() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure the property is in the entity
            var property = member.ToPropertyInfo();
            if (PropertyCache.Get<TEntity>().FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, property.Name, StringComparison.OrdinalIgnoreCase)) == null)
            {
                throw new InvalidExpressionException($"Invalid expression '{expression.ToString()}'. The property {property.Name} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }

            // Variables needed for fields
            var queryFields = new List<QueryField>();
            var conjunction = Conjunction.And;

            // Support only various methods
            if (expression.Method.Name == "Any")
            {
                conjunction = Conjunction.Or;
            }
            else if (expression.Method.Name == "All")
            {
                conjunction = Conjunction.And;
            }

            // Call the method
            var first = expression.Arguments.First();
            var values = (object)null;

            // Identify the type of the argument
            if (first.IsNewArray())
            {
                values = first.ToNewArray().GetValue();
            }
            else if (first.IsMember())
            {
                values = first.ToMember().GetValue();
            }

            // Values must be an array
            if (values is System.Collections.IEnumerable)
            {
                var operation = QueryField.GetOperation(binary.NodeType);
                foreach (var value in (System.Collections.IEnumerable)values)
                {
                    var queryField = new QueryField(PropertyMappedNameCache.Get(property), operation, value);
                    queryFields.Add(queryField);
                }
            }

            // Return the result
            return new QueryGroup(queryFields, conjunction, (isNot == isEqualsTo));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup ParseContainsForArrayOrList<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // TODO: Refactor this

            // Return null if there is no any arguments
            if (expression.Arguments?.Any() != true)
            {
                return null;
            }

            // Get the last arg
            var last = expression
                .Arguments
                .LastOrDefault();

            // Make sure the last arg is a member
            if (last == null || last?.IsMember() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure it is a property info
            var member = last.ToMember().Member;
            if (member.IsPropertyInfo() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Get the property
            var property = member.ToPropertyInfo();

            // Make sure the property is in the entity
            if (PropertyCache.Get<TEntity>().FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, property.Name, StringComparison.OrdinalIgnoreCase)) == null)
            {
                throw new InvalidExpressionException($"Invalid expression '{expression.ToString()}'. The property {property.Name} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }

            // Get the values
            var values = (object)null;

            // Array/List Separation
            if (expression.Object == null)
            {
                // Expecting an array
                values = expression.Arguments.First().GetValue();
            }
            else
            {
                // Expecting a list here
                values = expression.Object.GetValue();
            }

            // Add to query fields
            var operation = (isNot == false && isEqualsTo == true) ? Operation.In : Operation.NotIn;
            var queryField = new QueryField(PropertyMappedNameCache.Get(property), operation, values);

            // Return the result
            var queryGroup = new QueryGroup(queryField);

            // Set the IsNot value
            queryGroup.SetIsNot(isNot == true && isEqualsTo == false);

            // Return the instance
            return queryGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup ParseContainsOrStartsWithOrEndsWithForStringProperty<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // TODO: Refactor this

            // Return null if there is no any arguments
            if (expression.Arguments?.Any() != true)
            {
                return null;
            }

            // Get the value arg
            var value = Convert.ToString(expression.Arguments.FirstOrDefault()?.GetValue());

            // Make sure it has a value
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure it is a property info
            var member = expression.Object.ToMember().Member;
            if (member.IsPropertyInfo() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Get the property
            var property = member.ToPropertyInfo();

            // Make sure the property is in the entity
            if (PropertyCache.Get<TEntity>().FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, property.Name, StringComparison.OrdinalIgnoreCase)) == null)
            {
                throw new InvalidExpressionException($"Invalid expression '{expression.ToString()}'. The property {property.Name} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }

            // Add to query fields
            var operation = (isNot == isEqualsTo) ? Operation.NotLike : Operation.Like;
            var queryField = new QueryField(PropertyMappedNameCache.Get(property),
                operation,
                ConvertToLikeableValue(expression.Method.Name, value));

            // Return the result
            return new QueryGroup(queryField.AsEnumerable());
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
    }
}
