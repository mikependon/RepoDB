using System;
using System.Data;
using System.Data.SqlClient;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="SqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the SqlParameterSqlDbTypeAttribute instead. The System.Data.SqlClient namespace will soon to be removed from this library.")]
    public class SystemSqlServerTypeMapAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MicrosoftSqlServerTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="SqlDbType"/> value.</param>
        public SystemSqlServerTypeMapAttribute(SqlDbType dbType)
            : base(typeof(SqlParameter), nameof(SqlParameter.SqlDbType), dbType)
        { }

        /// <summary>
        /// Gets a <see cref="SqlDbType"/> that is currently mapped.
        /// </summary>
        public SqlDbType DbType => (SqlDbType)Value;
    }
}