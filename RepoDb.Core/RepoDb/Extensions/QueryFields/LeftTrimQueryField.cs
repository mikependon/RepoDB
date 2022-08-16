using RepoDb.Enumerations;
using System.Data;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// A functional-based <see cref="QueryField"/> object that is using the LTRIM function.
    /// </summary>
    public sealed class LeftTrimQueryField : FunctionalQueryField
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="LeftTrimQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public LeftTrimQueryField(string fieldName,
            object value,
            DbType? dbType = null)
            : this(fieldName, Operation.Equal, value)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public LeftTrimQueryField(Field field,
            object value,
            DbType? dbType = null)
            : this(field, Operation.Equal, value, dbType)
        { }

        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public LeftTrimQueryField(string fieldName,
            Operation operation,
            object value,
            DbType? dbType = null)
            : this(new Field(fieldName), operation, value, dbType)
        { }

        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public LeftTrimQueryField(Field field,
            Operation operation,
            object value,
            DbType? dbType = null)
            : base(field, operation, value, dbType, "LTRIM({0})")
        { }

        #endregion
    }
}
