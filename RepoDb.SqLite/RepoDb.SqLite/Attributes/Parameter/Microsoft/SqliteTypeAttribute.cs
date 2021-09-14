using Microsoft.Data.Sqlite;

namespace RepoDb.Attributes.Parameter.Sqlite
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqliteParameter.SqliteType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SqliteTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqliteTypeAttribute"/> class.
        /// </summary>
        /// <param name="sqliteType">A target <see cref="Microsoft.Data.Sqlite.SqliteType"/> value.</param>
        public SqliteTypeAttribute(SqliteType sqliteType)
            : base(typeof(SqliteParameter), nameof(SqliteParameter.SqliteType), sqliteType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="Microsoft.Data.Sqlite.SqliteType"/> value of the parameter.
        /// </summary>
        public SqliteType SqliteType => (SqliteType)Value;
    }
}