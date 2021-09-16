using RepoDb.Enumerations;

namespace RepoDb.Formatted.QueryField
{
    /// <summary>
    /// A functional-based <see cref="RepoDb.QueryField"/> object that is using the RIGHT function.
    /// </summary>
    public sealed class RightQueryField : FormattedFunctionQueryField
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightQueryField(string fieldName,
            string value)
            : this(fieldName, Operation.Equal, value)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RightQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightQueryField(Field field,
            string value)
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
            string value)
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
            string value)
            : this(field, operation, value, (value?.Length).GetValueOrDefault())
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="charCount">The number of characters from the right to be evaluated.</param>
        private RightQueryField(Field field,
            Operation operation,
            string value,
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
    }
}
