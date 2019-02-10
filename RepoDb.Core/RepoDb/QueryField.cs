using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
                .GetTypeInfo()
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
            return string.Concat(Field.ToString(), " = ", Parameter.ToString());
        }

        // Static Methods

        #region Parse (Expression)

        /// <summary>
        /// Parse an instance of <see cref="BinaryExpression"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type</typeparam>
        /// <param name="expression">The instance of <see cref="BinaryExpression"/> to be parsed.</param>
        /// <returns>An instance of <see cref="QueryField"/> object.</returns>
        internal static QueryField Parse<TEntity>(BinaryExpression expression) where TEntity : class
        {
            // Only support the following expression type
            if (expression.IsExtractable() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Name
            var fieldName = expression.GetName();
            if (PropertyCache.Get<TEntity>().Any(property => property.PropertyInfo.Name == fieldName) == false)
            {
                throw new InvalidQueryExpressionException($"Invalid expression '{expression.ToString()}'. The property {fieldName} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }

            // Value
            var value = expression.GetValue();

            // Operation
            var operation = GetOperation(expression.NodeType);

            // Return the value
            return new QueryField(fieldName, operation, value);
        }

        // GetOperation
        internal static Operation GetOperation(ExpressionType expressionType)
        {
            switch (expressionType)
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
                    throw new NotSupportedException($"Operation: Expression '{expressionType.ToString()}' is currently not supported.");
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