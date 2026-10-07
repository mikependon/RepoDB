#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL text that is used by the <see cref="TursoSchemaReader"/> to read the schema of a SQLite database
    /// from the <c>sqlite_master</c> table and the <c>pragma</c> table-valued functions.
    /// </summary>
    internal static class TursoSchemaText
    {
        #region Constants

        internal const string ColumnsSql = @"SELECT name AS Name,
                type AS DeclaredType,
                ""notnull"" AS IsNotNull,
                dflt_value AS DefaultDefinition,
                pk AS PrimaryKeyPosition,
                hidden AS Hidden
            FROM pragma_table_xinfo(@Table)
            WHERE hidden IN (0, 2, 3)
            ORDER BY cid;";

        internal const string IndexListSql = @"SELECT name AS IndexName,
                ""unique"" AS IsUnique,
                origin AS Origin,
                partial AS IsPartial
            FROM pragma_index_list(@Table)
            ORDER BY name;";

        internal const string IndexColumnsSql = @"SELECT name AS ColumnName,
                ""desc"" AS IsDescending,
                ""key"" AS IsKey
            FROM pragma_index_xinfo(@Index)
            ORDER BY seqno;";

        internal const string ForeignKeysSql = @"SELECT id AS Id,
                ""table"" AS ReferencedTable,
                ""from"" AS ColumnName,
                ""to"" AS ReferencedColumn,
                on_update AS UpdateAction,
                on_delete AS DeleteAction
            FROM pragma_foreign_key_list(@Table)
            ORDER BY id, seq;";

        internal const string PrimaryKeyColumnsSql = @"SELECT name AS ColumnName
            FROM pragma_table_xinfo(@Table)
            WHERE pk > 0
            ORDER BY pk;";

        internal const string DatabasesSql = @"SELECT name AS SchemaName
            FROM pragma_database_list
            WHERE name <> 'temp'
            ORDER BY seq;";

        internal const string ForeignKeyTablesSql = @"SELECT DISTINCT ""table"" AS ParentTable
            FROM pragma_foreign_key_list(@Table);";

        #endregion

        #region Methods

        /// <summary>
        /// Gets the statement that reads the SQL text of the table (the table is case insensitive).
        /// </summary>
        /// <param name="schema">The quoted name of the schema.</param>
        /// <returns>The SQL statement.</returns>
        internal static string TableSql(string schema) =>
            $"SELECT name AS TableName, sql AS Definition FROM {schema}.sqlite_master WHERE type = 'table' AND name = @Table COLLATE NOCASE;";

        /// <summary>
        /// Gets the statement that reads the SQL text of the indexes of the table.
        /// </summary>
        /// <param name="schema">The quoted name of the schema.</param>
        /// <returns>The SQL statement.</returns>
        internal static string IndexDefinitionsSql(string schema) =>
            $"SELECT name AS IndexName, sql AS Definition FROM {schema}.sqlite_master WHERE type = 'index' AND tbl_name = @Table COLLATE NOCASE;";

        /// <summary>
        /// Gets the statement that checks whether the table exists (it returns <c>1</c> or <c>0</c>).
        /// </summary>
        /// <param name="schema">The quoted name of the schema.</param>
        /// <returns>The SQL statement.</returns>
        internal static string TableExistsSql(string schema) =>
            $"SELECT COUNT(*) FROM {schema}.sqlite_master WHERE type = 'table' AND name = @Table COLLATE NOCASE;";

        /// <summary>
        /// Gets the statement that reads the names of the tables of the schema (the tables of the engine are not included).
        /// </summary>
        /// <param name="schema">The quoted name of the schema.</param>
        /// <returns>The SQL statement.</returns>
        internal static string TablesSql(string schema) =>
            $"SELECT name AS TableName, sql AS Definition FROM {schema}.sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite\\_%' ESCAPE '\\' AND name NOT LIKE '\\_\\_turso\\_internal%' ESCAPE '\\' ORDER BY name;";

        /// <summary>
        /// Gets the statement that reads the foreign keys of all the tables of the schema (a child table and its parent table).
        /// </summary>
        /// <param name="schema">The quoted name of the schema.</param>
        /// <returns>The SQL statement.</returns>

        #endregion
    }
}
