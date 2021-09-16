using RepoDb.Enumerations;

namespace RepoDb.Formatted.QueryField
{
    /// <summary>
    /// A dynamic functional-based <see cref="RepoDb.QueryField"/> object. This requires a
    /// formatted function string in order to work property (i.e.: 'LEN({0})', 'LTRIM({0})').
    /// </summary>
    public class FunctionQueryField : FormattedFunctionQueryField
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FunctionQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FunctionQueryField(string fieldName,
            object value,
            string formattedFunction)
            : this(fieldName, Operation.Equal, value, formattedFunction)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FunctionQueryField(Field field,
            object value,
            string formattedFunction)
            : this(field, Operation.Equal, value, formattedFunction)
        { }

        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FunctionQueryField(string fieldName,
            Operation operation,
            object value,
            string formattedFunction)
            : this(new Field(fieldName), operation, value, formattedFunction)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FunctionQueryField(Field field,
            Operation operation,
            object value,
            string formattedFunction)
            : base(field, operation, value, formattedFunction)
        { }

        #endregion
    }
}
