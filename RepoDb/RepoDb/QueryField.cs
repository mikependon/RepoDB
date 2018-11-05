using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// A class used to define the query expression for all repository operations. It holds the instances of field (<see cref="Field"/>),
    /// parameter (<see cref="QueryField"/>) and the target operation (<see cref="Operation"/>) of the query expression.
    /// </summary>
    public class QueryField : IEquatable<QueryField>
    {
        private const int HASHCODE_ISNULL = 128;
        private const int HASHCODE_ISNOTNULL = 256;
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object./
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName, object value)
            : this(fieldName, Operation.Equal, value)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object./
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName, Operation operation, object value)
            : this(fieldName, operation, value, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object./
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="appendParameterPrefix">
        /// The value to identify whether the underscope prefix will be appended to the parameter name.
        /// </param>
        internal QueryField(string fieldName, Operation operation, object value, bool appendParameterPrefix)
        {
            Field = new Field(fieldName);
            Operation = operation;
            Parameter = new Parameter(fieldName, value, appendParameterPrefix);
        }

        // Properties

        /// <summary>
        /// Gets the associated field object.
        /// </summary>
        public Field Field { get; }

        /// <summary>
        /// Gets the operation used by this instance.
        /// </summary>
        public Operation Operation { get; }

        /// <summary>
        /// Gets the associated parameter object.
        /// </summary>
        public Parameter Parameter { get; }

        // Methods

        /// <summary>
        /// Force to append prefix on the bound parameter object.
        /// </summary>
        internal void AppendParameterPrefix()
        {
            Parameter?.AppendPrefix();
        }

        /// <summary>
        /// Gets the text value of <see cref="TextAttribute"/> implemented at the <see cref="Operation"/> property value of this instance.
        /// </summary>
        /// <returns>A string instance containing the value of the <see cref="TextAttribute"/> text property.</returns>
        public string GetOperationText()
        {
            var textAttribute = typeof(Operation)
                .GetMembers()
                .First(member => member.Name.ToLower() == Operation.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
            return textAttribute.Text;
        }

        /// <summary>
        /// Stringify the current instance of this object. Will return the stringified format of field and parameter in combine.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Field.ToString()} = {Parameter.ToString()}";
        }

        // Static Methods

        #region Parse (Expression)

        internal static QueryField Parse<TEntity>(BinaryExpression expression) where TEntity : class
        {
            // Only support the following expression type
            if (expression.CanBeExtracted() == false)
            {
                throw new NotSupportedException($"Expression type '{expression.Left.NodeType.ToString()}' is currently not supported.");
            }

            // Needed variables for field
            var fieldName = GetNameFromExpression(expression);
            var operation = GetOperationFromExpression(expression);
            var value = GetValueFromExpression(expression);

            // Return the value
            return new QueryField(fieldName, operation, value);
        }

        // GetName

        private static string GetNameFromExpression(BinaryExpression expression)
        {
            // Check the left
            if (expression.Left.IsMember())
            {
                return expression.Left.ToMember().Member.Name;
            }

            // Check the right
            if (expression.Right.IsMember())
            {
                return expression.Right.ToMember().Member.Name;
            }

            // Throw an exception
            throw new InvalidOperationException($"Field name not found from expression '{expression.ToString()}'.");
        }

        // GetOperation
        private static Operation GetOperationFromExpression(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    return Operation.Equal;
                case ExpressionType.NotEqual:
                    return Operation.NotEqual;
                case ExpressionType.GreaterThan:
                    return Operation.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return Operation.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return Operation.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return Operation.LessThanOrEqual;
                default:
                    return Operation.Equal;
            }
        }

        private static object GetValueFromExpression(BinaryExpression expression)
        {
            // Check the contants
            if (expression.Right.IsConstant())
            {
                return GetValueFromExpression(expression.Right.ToConstant());
            }
            else if (expression.Left.IsConstant())
            {
                return GetValueFromExpression(expression.Left.ToConstant());
            }

            // Check the unaries
            else if (expression.Right.IsUnary())
            {
                return GetValueFromExpression(expression.Right.ToUnary());
            }
            else if (expression.Left.IsUnary())
            {
                return GetValueFromExpression(expression.Left.ToUnary());
            }

            // Check the member expression
            else if (expression.Right.IsMember())
            {
                return GetValueFromExpression(expression.Right.ToMember());
            }
            else if (expression.Left.IsMember())
            {
                return GetValueFromExpression(expression.Left.ToMember());
            }

            // Throw an exception
            throw new InvalidOperationException($"Value not found from expression '{expression.ToString()}'.");
        }

        private static object GetValueFromExpression(ConstantExpression expression)
        {
            return expression.Value;
        }

        private static object GetValueFromExpression(UnaryExpression expression)
        {
            if (expression.Operand.IsMember())
            {
                return GetValueFromExpression(expression.Operand.ToMember());
            }
            else if (expression.Operand.IsMethodCall())
            {
                return GetValueFromExpression(expression.Operand.ToMethodCall());
            }
            return null;
        }

        private static object GetValueFromExpression(MethodCallExpression expression)
        {
            if (expression.Arguments?.Any() == true)
            {
                return expression.Method.Invoke(expression.Object, expression.Arguments.Select(argExpression =>
                {
                    if (argExpression.IsConstant())
                    {
                        return GetValueFromExpression(argExpression.ToConstant());
                    }
                    else if (argExpression.IsMember())
                    {
                        return GetValueFromExpression(argExpression.ToMember());
                    }
                    throw new InvalidOperationException($"Value not found from expression '{argExpression.ToString()}'.");
                }).ToArray());
            }
            else
            {
                return expression.Method.Invoke(expression.Object, null);
            }
        }

        private static object GetValueFromExpression(MemberExpression expression)
        {
            if (expression.Expression.IsConstant())
            {
                if (expression.Member.IsFieldInfo())
                {
                    return expression.Member.AsFieldInfo().GetValue(expression.Expression.ToConstant().Value);
                }
                else if (expression.Member.IsPropertyInfo())
                {
                    return expression.Member.AsPropertyInfo().GetValue(expression.Expression.ToConstant().Value);
                }
            }
            else if (expression.Expression.IsMember())
            {
                if (expression.Member.IsFieldInfo())
                {
                    return expression.Member.AsFieldInfo().GetValue(GetValueFromExpression(expression.Expression.ToMember()));
                }
                else if (expression.Member.IsPropertyInfo())
                {
                    return expression.Member.AsPropertyInfo().GetValue(GetValueFromExpression(expression.Expression.ToMember()));
                }
            }
            else if (expression.Expression.IsMethodCall())
            {
                return GetValueFromExpression(expression.Expression.ToMethodCall());
            }
            return null;
        }

        #endregion

        #region Parse (Dynamics)

        internal static QueryField Parse(string fieldName, object value)
        {
            // The value must always be present
            if (value == null)
            {
                throw new ArgumentNullException($"The value must not be null for field ''{fieldName}''.");
            }

            // Another dynamic object type, get the 'Operation' property
            var properties = value.GetType().GetProperties();
            var operationProperty = properties?.FirstOrDefault(p => p.Name.ToLower() == StringConstant.Operation.ToLower());

            // The property 'Operation' must always be present
            if (operationProperty == null)
            {
                throw new InvalidQueryExpressionException($"Operation property must be present for field ''{fieldName}''.");
            }

            // The property operatoin must be of type 'RepoDb.Enumerations.Operation'
            if (operationProperty.PropertyType != typeof(Operation))
            {
                throw new InvalidQueryExpressionException($"The 'Operation' property for field ''{fieldName}'' must be of type '{typeof(Operation).FullName}'.");
            }

            // The 'Value' property must always be present
            var valueProperty = properties?.FirstOrDefault(p => p.Name.ToLower() == StringConstant.Value.ToLower());

            // Check for the 'Value' property
            if (valueProperty == null)
            {
                throw new InvalidQueryExpressionException($"The 'Value' property for dynamic type query must be present at field ''{fieldName}''.");
            }

            // Get the 'Operation' and the 'Value' value
            var operation = (Operation)operationProperty.GetValue(value);
            value = valueProperty.GetValue(value);

            // Identify the 'Operation' and parse the correct value
            if (operation == Operation.Between || operation == Operation.NotBetween)
            {
                // Special case: (Field.Name = new { Operation = Operation.<Between|NotBetween>, Value = new [] { value1, value2 })
                ValidateBetweenOperations(fieldName, operation, value);
            }
            else if (operation == Operation.In || operation == Operation.NotIn)
            {
                // Special case: (Field.Name = new { Operation = Operation.<In|NotIn>, Value = new [] { value1, value2 })
                ValidateInOperations(fieldName, operation, value);
            }
            else
            {
                // Other Operations
                ValidateOtherOperations(fieldName, operation, value);
            }

            // Return
            return new QueryField(fieldName, operation, value);
        }

        private static void ValidateBetweenOperations(string fieldName, Operation operation, object value)
        {
            var valid = false;

            // Make sure it is an Array
            if (value?.GetType().IsArray == true)
            {
                var values = ((Array)value)
                    .AsEnumerable()
                    .ToList();

                // The items must only be 2. There should be no NULL and no generic types
                if (values.Count == 2)
                {
                    valid = !values.Any(v => v == null || v?.GetType().IsGenericType == true);
                }

                // All type must be the same
                if (valid)
                {
                    var type = values.First().GetType();
                    values.ForEach(v =>
                    {
                        if (valid == false) return;
                        valid = v?.GetType() == type;
                    });
                }
            }

            // Throw an error if not valid
            if (valid == false)
            {
                throw new InvalidQueryExpressionException($"Invalid value for field '{fieldName}' for operation '{operation.ToString()}'. The value must be an array of 2 values with identitcal data types.");
            }
        }

        private static void ValidateInOperations(string fieldName, Operation operation, object value)
        {
            var valid = false;

            // Make sure it is an array
            if (value?.GetType().IsArray == true)
            {
                var values = ((Array)value)
                    .AsEnumerable()
                    .ToList();

                // Make sure there is not NULL and no generic types
                valid = !values.Any(v => v == null || v?.GetType().IsGenericType == true);

                // All type must be the same
                if (valid)
                {
                    var type = values.First().GetType();
                    values.ForEach(v =>
                    {
                        if (valid == false) return;
                        valid = v?.GetType() == type;
                    });
                }
            }

            // Throw an error if not valid
            if (valid == false)
            {
                throw new InvalidQueryExpressionException($"Invalid value for field '{fieldName}' for operation '{operation.ToString()}'. The value must be an array values with identitcal data types.");
            }
        }

        private static void ValidateOtherOperations(string fieldName, Operation operation, object value)
        {
            var valid = false;

            // Special for Equal and NonEqual
            if ((operation == Operation.Equal || operation == Operation.NotEqual) && value == null)
            {
                // Most likely new QueryField("Field.Name", null) or new { FieldName = (object)null }.
                // The SQL must be (@FieldName IS <NOT> NULL)
                valid = true;
            }
            else
            {
                // Must not be a generic
                valid = (value?.GetType().IsGenericType == false);
            }

            // Throw an error if not valid
            if (valid == false)
            {
                throw new InvalidQueryExpressionException($"Invalid value for field '{fieldName}' for operation '{operation.ToString()}'.");
            }
        }

        #endregion

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="QueryField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (!ReferenceEquals(null, m_hashCode))
            {
                return m_hashCode.Value;
            }

            // Use the non nullable for perf purposes
            var hashCode = 0;

            // Set in the combination of the properties
            hashCode += (Field.GetHashCode() + Operation.GetHashCode() + Parameter.GetHashCode());

            // The (IS NULL) affects the uniqueness of the object
            if (Operation == Operation.Equal && ReferenceEquals(null, Parameter.Value))
            {
                hashCode += HASHCODE_ISNULL;
            }
            // The (IS NOT NULL) affects the uniqueness of the object
            else if (Operation == Operation.NotEqual && ReferenceEquals(null, Parameter.Value))
            {
                hashCode += HASHCODE_ISNOTNULL;
            }
            // The parameter's length affects the uniqueness of the object
            else if ((Operation == Operation.In || Operation == Operation.NotIn) &&
                !ReferenceEquals(null, Parameter.Value) && Parameter.Value is Array)
            {
                hashCode += ((Array)Parameter.Value).Length.GetHashCode();
            }

            // Set back the value
            m_hashCode = hashCode;

            // Return the value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="QueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="QueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryField other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryField"/> object.</param>
        /// <param name="objB">The second <see cref="QueryField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(QueryField objA, QueryField objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryField"/> object.</param>
        /// <param name="objB">The second <see cref="QueryField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryField objA, QueryField objB)
        {
            return (objA == objB) == false;
        }
    }
}