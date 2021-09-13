using Microsoft.Data.Sqlite;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqliteParameter.SqliteType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SqliteParameterDbTypeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqliteParameterDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="sqliteType">A target <see cref="Microsoft.Data.Sqlite.SqliteType"/> value.</param>
        public SqliteParameterDbTypeAttribute(SqliteType sqliteType)
            : base(typeof(SqliteParameter), nameof(SqliteParameter.SqliteType), sqliteType)
        { }

        /// <summary>
        /// Gets a <see cref="SqliteType"/> that is currently mapped.
        /// </summary>
        public SqliteType SqliteType => (SqliteType)Value;
    }
}