#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Ahtola.Data.Sqlite;

namespace RepoDb.Attributes.Parameter.Ahtola
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
        /// <param name="sqliteType">A target <see cref="global::Ahtola.Data.Sqlite.SqliteType"/> value.</param>
        public SqliteTypeAttribute(SqliteType sqliteType)
            : base(typeof(SqliteParameter), nameof(SqliteParameter.SqliteType), sqliteType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::Ahtola.Data.Sqlite.SqliteType"/> value of the parameter.
        /// </summary>
        public SqliteType SqliteType => (SqliteType)Value;
    }
}