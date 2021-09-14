using RepoDb.Attributes.Parameter.SqlServer;
using System;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="SqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the RepoDb.Attributes.SqlServer.SqlDbTypeAttribute instead.")]
    public class MicrosoftSqlServerTypeMapAttribute : SqlDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MicrosoftSqlServerTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="sqlDbType">The value of the target <see cref="SqlDbType"/>.</param>
        public MicrosoftSqlServerTypeMapAttribute(SqlDbType sqlDbType)
            : base(sqlDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="System.Data.SqlDbType"/> value of the property.
        /// </summary>
        public SqlDbType DbType => (SqlDbType)Value;
    }
}