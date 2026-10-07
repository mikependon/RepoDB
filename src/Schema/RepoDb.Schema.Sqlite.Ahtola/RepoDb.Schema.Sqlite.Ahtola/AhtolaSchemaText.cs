#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL text that is used by the <see cref="AhtolaSchemaReader"/> to read the schema of a SQLite database
    /// from the <c>sqlite_master</c> table and the <c>pragma</c> table-valued functions.
    /// </summary>
    internal static class AhtolaSchemaText
    {
        #region Constants

        internal static string ColumnsSql(string schema) => $@"SELECT name AS Name,
                type AS DeclaredType,
                ""notnull"" AS IsNotNull,
                dflt_value AS DefaultDefinition,
                pk AS PrimaryKeyPosition,
                hidden AS Hidden
            FROM pragma_table_xinfo(@Table, {schema})
            WHERE hidden IN (0, 2, 3)
            ORDER BY cid;";

        internal static string IndexListSql(string schema) => $@"SELECT name AS IndexName,
                ""unique"" AS IsUnique,
                origin AS Origin,
                partial AS IsPartial
            FROM pragma_index_list(@Table, {schema})
            ORDER BY name;";

        internal static string IndexColumnsSql(string schema) => $@"SELECT name AS ColumnName,
                ""desc"" AS IsDescending,
                ""key"" AS IsKey
            FROM pragma_index_xinfo(@Index, {schema})
            ORDER BY seqno;";

        internal static string ForeignKeysSql(string schema) => $@"SELECT id AS Id,
                ""table"" AS ReferencedTable,
                ""from"" AS ColumnName,
                ""to"" AS ReferencedColumn,
                on_update AS UpdateAction,
                on_delete AS DeleteAction
            FROM pragma_foreign_key_list(@Table, {schema})
            ORDER BY id, seq;";

        internal static string PrimaryKeyColumnsSql(string schema) => $@"SELECT name AS ColumnName
            FROM pragma_table_xinfo(@Table, {schema})
            WHERE pk > 0
            ORDER BY pk;";

        internal const string DatabasesSql = "PRAGMA database_list;";

        #endregion

        #region Methods

        /// <summary>
        /// Gets the string literal of the schema. The engine ignores a parameter that is the schema argument of the <c>pragma</c> functions, so it is inlined.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        internal static string Literal(string schema) =>
            "'" + schema.Replace("'", "''") + "'";

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
            $"SELECT name AS TableName FROM {schema}.sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite\\_%' ESCAPE '\\' AND name NOT LIKE '\\_\\_turso\\_internal%' ESCAPE '\\' ORDER BY name;";

        /// <summary>
        /// Gets the statement that reads the foreign keys of all the tables of the schema (a child table and its parent table).
        /// </summary>
        /// <param name="schema">The quoted name of the schema.</param>
        /// <returns>The SQL statement.</returns>
        internal static string RelationshipsSql(string schema, string schemaName) =>
            $"SELECT DISTINCT m.name AS ChildTable, f.\"table\" AS ParentTable FROM {schema}.sqlite_master m, pragma_foreign_key_list(m.name, {Literal(schemaName)}) f WHERE m.type = 'table';";

        #endregion
    }
}
