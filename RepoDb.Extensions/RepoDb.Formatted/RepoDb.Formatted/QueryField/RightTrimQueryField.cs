using RepoDb.Enumerations;

namespace RepoDb.Formatted.QueryField
{
    /// <summary>
    /// A functional-based <see cref="RepoDb.QueryField"/> object that is using the LTRIM function.
    /// </summary>
    public sealed class RightTrimQueryField : FormattedFunctionQueryField
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="RightTrimQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightTrimQueryField(string fieldName,
            object value)
            : this(fieldName, Operation.Equal, value)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightTrimQueryField(Field field,
            object value)
            : this(field, Operation.Equal, value)
        { }

        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightTrimQueryField(string fieldName,
            Operation operation,
            object value)
            : this(new Field(fieldName), operation, value)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public RightTrimQueryField(Field field,
            Operation operation,
            object value)
            : base(field, operation, value, "TRIM({0})")
        { }

        #endregion
    }
}
