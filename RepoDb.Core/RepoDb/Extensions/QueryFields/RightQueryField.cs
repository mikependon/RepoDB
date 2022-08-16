using RepoDb.Enumerations;
using System;
using System.Data;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// A functional-based <see cref="QueryField"/> object that is using the RIGHT function.
    /// </summary>
    public sealed class RightQueryField : FunctionalQueryField
    {
        private int? hashCode = null;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public RightQueryField(string fieldName,
            object value,
            DbType? dbType = null)
            : this(fieldName, Operation.Equal, value, dbType)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public RightQueryField(Field field,
            object value,
            DbType? dbType = null)
            : this(field, Operation.Equal, value, dbType)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public RightQueryField(string fieldName,
            Operation operation,
            object value,
            DbType? dbType = null)
            : this(new Field(fieldName), operation, value, dbType)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public RightQueryField(Field field,
            Operation operation,
            object value,
            DbType? dbType = null)
            : this(field, operation, value, dbType, (value?.ToString()?.Length).GetValueOrDefault())
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        /// <param name="charCount">The number of characters from the left to be evaluated.</param>
        private RightQueryField(Field field,
            Operation operation,
            object value,
            DbType? dbType = null,
            int charCount = 0)
            : base(field, operation, value, dbType, $"RIGHT({{0}}, {charCount})")
        {
            CharCount = charCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value that defines the number of characters from the left to be evaluated.
        /// </summary>
        public int CharCount { get; }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="RightQueryField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Base
            var hashCode = base.GetHashCode();

            // CharCount
            hashCode = HashCode.Combine(hashCode, CharCount);

            // Return
            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
