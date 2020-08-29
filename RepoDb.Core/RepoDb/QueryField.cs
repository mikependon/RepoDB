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
    /// A class that is used to define a field expression for the query operations. It holds the instances of field <see cref="RepoDb.Field"/>,
    /// parameter <see cref="RepoDb.Parameter"/> and the target operation <see cref="RepoDb.Enumerations.Operation"/> of the query expression.
    /// </summary>
    public class QueryField : IEquatable<QueryField>
    {
        private const int HASHCODE_ISNULL = 128;
        private const int HASHCODE_ISNOTNULL = 256;
        private int? hashCode = null;
        private TextAttribute operationTextAttribute = null;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName,
            object value)
            : this(fieldName,
                  Operation.Equal,
                  value)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName,
            Operation operation,
            object value)
            : this(fieldName,
                  operation,
                  value,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(Field field,
            object value)
            : this(field,
                  Operation.Equal,
                  value,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(Field field,
            Operation operation,
            object value)
            : this(field,
                  operation,
                  value,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="appendUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        internal QueryField(string fieldName,
            Operation operation,
            object value,
            bool appendUnderscore)
            : this(new Field(fieldName),
                  operation,
                  value,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="appendUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        internal QueryField(Field field,
            Operation operation,
            object value,
            bool appendUnderscore)
        {
            Field = field;
            Operation = operation;
            Parameter = new Parameter(field.Name, value, appendUnderscore);
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        /// <summary>
        /// Prepend an underscore on the bound parameter object.
        /// </summary>
        internal void PrependAnUnderscoreAtParameter()
        {
            Parameter?.PrependAnUnderscore();
        }

        /// <summary>
        /// Make the current instance of <see cref="QueryField"/> object to become an expression for 'Update' operations.
        /// </summary>
        public void IsForUpdate()
        {
            PrependAnUnderscoreAtParameter();
        }

        /// <summary>
        /// Resets the <see cref="QueryField"/> back to its default state (as is newly instantiated).
        /// </summary>
        public void Reset()
        {
            Parameter?.SetName(Field.Name);
            operationTextAttribute = null;
            hashCode = null;
        }

        /// <summary>
        /// Gets the text value of <see cref="TextAttribute"/> implemented at the <see cref="Operation"/> property value of this instance.
        /// </summary>
        /// <returns>A string instance containing the value of the <see cref="TextAttribute"/> text property.</returns>
        public string GetOperationText()
        {
            if (operationTextAttribute == null)
            {
                operationTextAttribute = StaticType.Operation
                    .GetMembers()
                    .First(member => string.Equals(member.Name, Operation.ToString(), StringComparison.OrdinalIgnoreCase))
                    .GetCustomAttribute<TextAttribute>();
            }
            return operationTextAttribute.Text;
        }

        /// <summary>
        /// Stringify the current instance of this object. Will return the stringified format of field and parameter in combine.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(Field.ToString(), " = ", Parameter.ToString());
        }

        #endregion

        #region Static Methods

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

            // Return the value
            return new QueryField(field, operation, value);
        }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="QueryField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            var hashCode = 0;

            // Set in the combination of the properties
            hashCode += (Field.GetHashCode() + (int)Operation + Parameter.GetHashCode());

            // The (IS NULL) affects the uniqueness of the object
            if (Operation == Operation.Equal && Parameter.Value == null)
            {
                hashCode += HASHCODE_ISNULL;
            }
            // The (IS NOT NULL) affects the uniqueness of the object
            else if (Operation == Operation.NotEqual && Parameter.Value == null)
            {
                hashCode += HASHCODE_ISNOTNULL;
            }
            // The parameter's length affects the uniqueness of the object
            else if ((Operation == Operation.In || Operation == Operation.NotIn) &&
                Parameter.Value != null && Parameter.Value is System.Collections.IEnumerable)
            {
                var items = ((System.Collections.IEnumerable)Parameter.Value);
                hashCode += items
                    .OfType<object>()
                    .Count()
                    .GetHashCode();
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="QueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="QueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryField other) =>
            other?.GetHashCode() == GetHashCode();

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
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryField"/> object.</param>
        /// <param name="objB">The second <see cref="QueryField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryField objA, QueryField objB) =>
            (objA == objB) == false;

        #endregion
    }
}
