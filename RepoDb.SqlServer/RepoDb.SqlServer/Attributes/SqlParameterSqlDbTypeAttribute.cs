using Microsoft.Data.SqlClient;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.SqlDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SqlParameterSqlDbTypeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlParameterSqlDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="sqlDbType">The value of the target <see cref="System.Data.SqlDbType"/>.</param>
        public SqlParameterSqlDbTypeAttribute(SqlDbType sqlDbType)
            : base(typeof(SqlParameter), nameof(SqlParameter.SqlDbType), sqlDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="System.Data.SqlDbType"/> value.
        /// </summary>
        public SqlDbType SqlDbType => (SqlDbType)Value;
    }
}