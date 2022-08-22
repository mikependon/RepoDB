using RepoDb.Enumerations;
using System;
using System.Data;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// A functional-based <see cref="QueryField"/> object that is using the LEFT function.
    /// </summary>
    public sealed class LeftQueryField : FunctionalQueryField
    {
        private int? hashCode = null;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="LeftQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public LeftQueryField(string fieldName,
            object value)
            : this(fieldName, Operation.Equal, value, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="LeftQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public LeftQueryField(string fieldName,
            object value,
            DbType? dbType)
            : this(fieldName, Operation.Equal, value, dbType, 0)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="LeftQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public LeftQueryField(string fieldName,
            Operation operation,
            object value)
            : this(fieldName, operation, value, null, 0)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="LeftQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public LeftQueryField(string fieldName,
            Operation operation,
            object value,
            DbType? dbType)
            : this(fieldName, operation, value, dbType, 0)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="LeftQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        /// <param name="charCount">The number of characters from the left to be evaluated.</param>
        private LeftQueryField(string fieldName,
            Operation operation,
            object value,
            DbType? dbType,
            int charCount = 0)
            : base(fieldName, operation, value, dbType, $"LEFT({{0}}, {(charCount > 0 ? charCount : (value?.ToString().Length).GetValueOrDefault())})")
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
        /// Returns the hashcode for this <see cref="LeftQueryField"/>.
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
