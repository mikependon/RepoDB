using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="SqlDbType"/> value.
    /// </summary>
    public class MicrosoftSqlServerTypeMapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MicrosoftSqlServerTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="SqlDbType"/> value.</param>
        public MicrosoftSqlServerTypeMapAttribute(SqlDbType dbType)
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