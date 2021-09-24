﻿using RepoDb.Enumerations;

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
        public RightQueryField(string fieldName,
            object value)
            : this(fieldName, Operation.Equal, value)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightQueryField(Field field,
            object value)
            : this(field, Operation.Equal, value)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightQueryField(string fieldName,
            Operation operation,
            object value)
            : this(new Field(fieldName), operation, value)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightQueryField(Field field,
            Operation operation,
            object value)
            : this(field, operation, value, (value?.ToString()?.Length).GetValueOrDefault())
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="charCount">The number of characters from the right to be evaluated.</param>
        private RightQueryField(Field field,
            Operation operation,
            object value,
            int charCount)
            : base(field, operation, value, $"RIGHT({{0}}, {charCount})")
        {
            CharCount = charCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value that defines the number of characters from the right to be evaluated.
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
            hashCode += CharCount.GetHashCode();

            // Return
            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}