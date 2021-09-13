using Microsoft.Data.Sqlite;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="SqliteType"/> value.
    /// </summary>
    public class SqliteParameterDbTypeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqliteParameterDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="value">A target <see cref="SqliteType"/> value.</param>
        public SqliteParameterDbTypeAttribute(SqliteType value)
            : base(typeof(SqliteParameter), nameof(SqliteParameter.SqliteType), value)
        { }

        /// <summary>
        /// Gets a <see cref="SqliteType"/> that is currently mapped.
        /// </summary>
        public SqliteType SqliteType => (SqliteType)Value;
    }
}