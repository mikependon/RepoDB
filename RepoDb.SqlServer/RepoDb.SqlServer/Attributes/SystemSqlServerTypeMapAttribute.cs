using System;
using System.Data;
using System.Data.SqlClient;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="SqlDbType"/> value.
    /// </summary>
    public class SystemSqlServerTypeMapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SystemSqlServerTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="SqlDbType"/> value.</param>
        public SystemSqlServerTypeMapAttribute(SqlDbType dbType)
        {
            DbType = dbType;
            ParameterType = typeof(SqlParameter);
        }

        /// <summary>
        /// Gets a <see cref="SqlDbType"/> that is currently mapped.
        /// </summary>
        public SqlDbType DbType { get; }

        /// <summary>
        /// Gets the represented <see cref="Type"/> of the <see cref="SqlParameter"/>.
        /// </summary>
        public Type ParameterType { get; }
    }
}