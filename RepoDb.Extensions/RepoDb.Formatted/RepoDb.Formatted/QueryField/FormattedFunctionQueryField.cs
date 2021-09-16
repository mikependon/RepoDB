using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Formatted.QueryField
{
    /// <summary>
    /// A base class for all the formatted-functional <see cref="RepoDb.QueryField"/> objects.
    /// </summary>
    public abstract class FormattedFunctionQueryField : RepoDb.QueryField
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FormattedFunctionQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FormattedFunctionQueryField(string fieldName,
            object value,
            string formattedFunction)
            : this(fieldName, Operation.Equal, value, formattedFunction)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FormattedFunctionQueryField(Field field,
            object value,
            string formattedFunction)
            : this(field, Operation.Equal, value, formattedFunction)
        { }

        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FormattedFunctionQueryField(string fieldName,
            Operation operation,
            object value,
            string formattedFunction)
            : this(new Field(fieldName), operation, value, formattedFunction)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="formattedFunction">The name of the formatted-function to be used.</param>
        public FormattedFunctionQueryField(Field field,
            Operation operation,
            object value,
            string formattedFunction)
            : base(field, operation, value, false)
        {
            FormattedFunction = formattedFunction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the formatted-function name of the current instance.
        /// </summary>
        public string FormattedFunction { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the string representations (column-value pairs) of the current <see cref="RepoDb.QueryField"/> object with the formatted-function transformations.
        /// </summary>
        /// <param name="index">The target index.</param>
        /// <param name="dbSetting">The database setting currently in used.</param>
        /// <returns>The string representations of the current <see cref="RepoDb.QueryField"/> object using the LOWER function.</returns>
        public override string GetString(int index,
            IDbSetting dbSetting) =>
            base.GetString(index, FormattedFunction, dbSetting);

        #endregion
    }
}
