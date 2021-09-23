using RepoDb.Enumerations;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// A functional-based <see cref="QueryField"/> object that is using the UPPER function.
    /// </summary>
    public sealed class UpperQueryField : FunctionalQueryField
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="UpperQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public UpperQueryField(string fieldName,
            object value)
            : this(fieldName, Operation.Equal, value)
        {
        }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public UpperQueryField(Field field,
            object value)
            : this(field, Operation.Equal, value)
        { }

        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public UpperQueryField(string fieldName,
            Operation operation,
            object value)
            : this(new Field(fieldName), operation, value)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public UpperQueryField(Field field,
            Operation operation,
            object value)
            : base(field, operation, value, "UPPER({0})")
        { }

        #endregion
    }
}
