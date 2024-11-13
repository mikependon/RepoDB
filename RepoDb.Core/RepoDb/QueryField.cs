using System;
using System.Collections;
using System.Data;
using System.Linq;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to define a field expression for the query operation. It holds the instances of the <see cref="RepoDb.Field"/>,
    /// <see cref="RepoDb.Parameter"/> and the <see cref="Enumerations.Operation"/> objects of the query expression.
    /// </summary>
    public partial class QueryField : IEquatable<QueryField>
    {
        private const int HASHCODE_ISNULL = 128;
        private const int HASHCODE_ISNOTNULL = 256;
        private int? hashCode = null;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName,
            object? value)
            : this(fieldName, Operation.Equal, value, null, false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName,
            Operation operation,
            object? value)
            : this(fieldName, operation, value, null, false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public QueryField(string fieldName,
            Operation operation,
            object? value,
            DbType? dbType)
            : this(fieldName, operation, value, dbType, false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        /// <param name="prependUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        public QueryField(string fieldName,
            Operation operation,
            object? value,
            DbType? dbType,
            bool prependUnderscore = false)
            : this(new Field(fieldName), operation, value, dbType, false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="field">The instance of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        /// <param name="prependUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        internal QueryField(Field field,
            Operation operation,
            object value,
            DbType? dbType,
            bool prependUnderscore = false)
        {
            Field = field;
            Operation = operation;
            Parameter = new Parameter(field.Name, value, dbType, prependUnderscore);
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

        /// <summary>
        /// Gets the in-used instance of database parameter object.
        /// </summary>
        public IDbDataParameter DbParameter { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the string representations (column-value pairs) of the current <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="dbSetting">The database setting currently in used.</param>
        /// <returns>The string representations of the current <see cref="QueryField"/> object.</returns>
        public virtual string GetString(IDbSetting dbSetting) =>
            GetString(0, dbSetting);

        /// <summary>
        /// Gets the string representations (column-value pairs) of the current <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="index">The target index.</param>
        /// <param name="dbSetting">The database setting currently in used.</param>
        /// <returns>The string representations of the current <see cref="QueryField"/> object.</returns>
        public virtual string GetString(int index,
            IDbSetting dbSetting) =>
            GetString(index, null, dbSetting);

        /// <summary>
        /// Gets the string representations (column-value pairs) of the current <see cref="QueryField"/> object with the formatted-function transformations.
        /// </summary>
        /// <param name="index">The target index.</param>
        /// <param name="functionFormat">The properly constructed format of the target function to be used.</param>
        /// <param name="dbSetting">The database setting currently in used.</param>
        /// <returns>The string representations of the current <see cref="QueryField"/> object using the LOWER function.</returns>
        protected virtual string GetString(int index,
            string functionFormat,
            IDbSetting dbSetting)
        {
            // = AND NULL
            if (Operation == Operation.Equal && Parameter.Value == null)
            {
                return string.Concat(this.AsField(functionFormat, dbSetting), " IS NULL");
            }

            // <> AND NULL
            else if (Operation == Operation.NotEqual && Parameter.Value == null)
            {
                return string.Concat(this.AsField(functionFormat, dbSetting), " IS NOT NULL");
            }

            // BETWEEN @LeftValue AND @RightValue
            else if (Operation == Operation.Between || Operation == Operation.NotBetween)
            {
                return this.AsFieldAndParameterForBetween(index, functionFormat, dbSetting);
            }

            // IN (@Value1, @Value2)
            else if (Operation == Operation.In || Operation == Operation.NotIn)
            {
                return this.AsFieldAndParameterForIn(index, functionFormat, dbSetting);
            }

            // [Column] = @Column
            else
            {
                return string.Concat(this.AsField(functionFormat, dbSetting), " ", Operation.GetText(), " ", this.AsParameter(index /*, functionFormat*/, dbSetting));
            }
        }

        /// <summary>
        /// Returns the name of the <see cref="Field"/> object current in used.
        /// </summary>
        public string GetName() =>
            Field?.Name;

        /// <summary>
        /// Returns the value of the <see cref="Parameter"/> object currently in used. However, if this instance of object has already been used as a database parameter 
        /// with <see cref="DbParameter.Direction"/> equals to <see cref="System.Data.ParameterDirection.Output"/> via <see cref="DirectionalQueryField"/> 
        /// object, then the value of the in-used <see cref="IDbDataParameter"/> object will be returned.
        /// </summary>
        /// <returns>The value of the <see cref="Parameter"/> object.</returns>
        public object GetValue() =>
            GetValue<object>();

        /// <summary>
        /// Returns the value of the <see cref="Parameter"/> object currently in used. However, if this instance of object has already been used as a database parameter 
        /// with <see cref="DbParameter.Direction"/> equals to <see cref="System.Data.ParameterDirection.Output"/> via <see cref="DirectionalQueryField"/> 
        /// object, then the value of the in-used <see cref="IDbDataParameter"/> object will be returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The value of the converted <see cref="Parameter"/> object.</returns>
        public T GetValue<T>() =>
            Converter.ToType<T>(DbParameter?.Value ?? Parameter?.Value);

        /// <summary>
        /// Make the current instance of <see cref="QueryField"/> object to become an expression for 'Update' operations.
        /// </summary>
        public void IsForUpdate()
        {
            this.PrependAnUnderscoreAtParameter();
        }

        /// <summary>
        /// Resets the <see cref="QueryField"/> back to its default state (as is newly instantiated).
        /// </summary>
        public void Reset()
        {
            Parameter?.Reset();
            DbParameter = null;
            hashCode = null;
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

            // Set in the combination of the properties
            var hashCode = HashCode.Combine(Field, Operation, Parameter);

            // The (IS NULL) affects the uniqueness of the object
            if (Operation == Operation.Equal &&
                Parameter.Value == null)
            {
                hashCode = HashCode.Combine(hashCode, HASHCODE_ISNULL);
            }
            // The (IS NOT NULL) affects the uniqueness of the object
            else if (Operation == Operation.NotEqual && Parameter.Value == null)
            {
                hashCode = HashCode.Combine(hashCode, HASHCODE_ISNOTNULL);
            }
            // The parameter's length affects the uniqueness of the object
            else if (Operation is Operation.In or Operation.NotIn &&
                Parameter.Value is IEnumerable enumerable)
            {
                hashCode = HashCode.Combine(hashCode, enumerable.WithType<object>().Count());
            }
            // The string representation affects the collision
            // var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Value != 1);
            // var objB = QueryGroup.Parse<EntityClass>(c => c.Id != 1 && c.Value == 1);
            hashCode = HashCode.Combine(hashCode, Field.Name, Operation.GetText());

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="QueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return obj.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="QueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryField? other)
        {
            if (other is null) return false;

            return other.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryField"/> object.</param>
        /// <param name="objB">The second <see cref="QueryField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(QueryField? objA,
            QueryField? objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryField"/> object.</param>
        /// <param name="objB">The second <see cref="QueryField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryField? objA,
            QueryField? objB) =>
            (objA == objB) == false;

        #endregion
    }
}
