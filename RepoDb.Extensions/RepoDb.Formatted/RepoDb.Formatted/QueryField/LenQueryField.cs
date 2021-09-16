using RepoDb.Enumerations;

namespace RepoDb.Formatted.QueryField
{
    /// <summary>
    /// A functional-based <see cref="RepoDb.QueryField"/> object that is using the LEN function.
    /// This only works on SQL Server database provider.
    /// </summary>
    public sealed class LenQueryField : FormattedFunctionQueryField
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="LenQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public LenQueryField(string fieldName,
            int value)
            : this(fieldName, Operation.Equal, value)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public LenQueryField(Field field,
            int value)
            : this(field, Operation.Equal, value)
        { }

        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public LenQueryField(string fieldName,
            Operation operation,
            int value)
            : this(new Field(fieldName), operation, value)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public LenQueryField(Field field,
            Operation operation,
            int value)
            : base(field, operation, value, "len({0})")
        { }

        #endregion
    }
}
