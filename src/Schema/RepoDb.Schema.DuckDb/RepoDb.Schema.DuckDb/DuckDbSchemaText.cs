#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="DuckDbSchemaReader"/> to read the schema from the DuckDB catalog functions (<c>duckdb_*()</c>).
    /// A schema is an actual schema of the database of the connection: the reader resolves the current schema of the connection before it uses the texts,
    /// so the <c>$SchemaName</c> parameter is never blank. DuckDB does not keep the names of the constraints, so it generates them.
    /// </summary>
    internal static class DuckDbSchemaText
    {
        #region Constants

        private const string CurrentSchema = "current_schema()";

        internal const string CurrentSchemaSql = "SELECT current_schema()";

        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM duckdb_tables() t
                WHERE t.database_name = current_database() AND t.schema_name = $SchemaName AND t.table_name = $TableName) THEN 1 ELSE 0 END";

        internal const string ColumnsSql = @"SELECT c.column_index AS Ordinal,
                c.column_name AS Name,
                c.data_type AS TypeName,
                COALESCE(c.character_maximum_length, 0) AS MaxLength,
                COALESCE(c.numeric_precision, 0) AS PrecisionValue,
                COALESCE(c.numeric_scale, 0) AS ScaleValue,
                CASE WHEN c.is_nullable THEN 1 ELSE 0 END AS IsNullable,
                CASE WHEN s.sequence_name IS NOT NULL THEN 1 ELSE 0 END AS IsIdentity,
                s.start_value AS IdentitySeed,
                s.increment_by AS IdentityIncrement,
                CASE WHEN s.sequence_name IS NULL THEN c.column_default END AS DefaultDefinition,
                CASE WHEN EXISTS (SELECT 1 FROM duckdb_constraints() k
                        WHERE k.database_name = c.database_name AND k.schema_name = c.schema_name AND k.table_name = c.table_name
                            AND k.constraint_type = 'PRIMARY KEY' AND list_contains(k.constraint_column_names, c.column_name))
                    THEN 1 ELSE 0 END AS IsPrimary
            FROM (SELECT c0.*, regexp_extract(c0.column_default, 'nextval\(''(?:[^''.]*[.])?""?([^""'']+)""?''\)', 1) AS sequence_name
                    FROM duckdb_columns() c0
                    WHERE c0.database_name = current_database() AND c0.schema_name = $SchemaName AND c0.table_name = $TableName) c
            LEFT JOIN duckdb_sequences() s ON s.database_name = c.database_name AND s.schema_name = c.schema_name
                AND s.sequence_name = c.sequence_name AND c.sequence_name <> ''
            ORDER BY c.column_index";

        internal const string KeyConstraintSql = @"SELECT k.constraint_name AS ConstraintName,
                k.constraint_column_names[r.pos] AS ColumnName,
                1 AS IsClustered
            FROM duckdb_constraints() k, range(1, 65) r(pos)
            WHERE k.database_name = current_database() AND k.schema_name = $SchemaName AND k.table_name = $TableName
                AND k.constraint_type = $KeyType AND r.pos <= len(k.constraint_column_names)
            ORDER BY k.constraint_name, r.pos";

        internal const string IndexesSql = @"SELECT i.index_name AS IndexName,
                CASE WHEN i.is_unique THEN 1 ELSE 0 END AS IsUnique,
                i.expressions AS Expressions
            FROM duckdb_indexes() i
            WHERE i.database_name = current_database() AND i.schema_name = $SchemaName AND i.table_name = $TableName
            ORDER BY i.index_name";

        internal const string ForeignKeysSql = @"SELECT k.constraint_name AS ForeignKeyName,
                k.constraint_column_names[r.pos] AS ColumnName,
                CASE WHEN k.schema_name = " + CurrentSchema + @" THEN NULL ELSE k.schema_name END AS ReferencedSchema,
                k.referenced_table AS ReferencedTable,
                k.referenced_column_names[r.pos] AS ReferencedColumn,
                'NO ACTION' AS UpdateAction,
                'NO ACTION' AS DeleteAction
            FROM duckdb_constraints() k, range(1, 65) r(pos)
            WHERE k.database_name = current_database() AND k.schema_name = $SchemaName AND k.table_name = $TableName
                AND k.constraint_type = 'FOREIGN KEY' AND r.pos <= len(k.constraint_column_names)
            ORDER BY k.constraint_name, r.pos";

        internal const string CheckConstraintsSql = @"SELECT k.constraint_name AS ConstraintName,
                k.constraint_text AS Definition
            FROM duckdb_constraints() k
            WHERE k.database_name = current_database() AND k.schema_name = $SchemaName AND k.table_name = $TableName AND k.constraint_type = 'CHECK'
            ORDER BY k.constraint_name";

        internal const string TablesSql = @"SELECT CASE WHEN t.schema_name = " + CurrentSchema + @" THEN NULL ELSE t.schema_name END AS SchemaName,
                t.table_name AS TableName
            FROM duckdb_tables() t
            WHERE t.database_name = current_database() AND t.schema_name = $SchemaName AND NOT t.internal AND NOT t.temporary
            ORDER BY t.schema_name, t.table_name";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT CASE WHEN k.schema_name = " + CurrentSchema + @" THEN NULL ELSE k.schema_name END AS ChildSchema,
                k.table_name AS ChildTable,
                CASE WHEN k.schema_name = " + CurrentSchema + @" THEN NULL ELSE k.schema_name END AS ParentSchema,
                k.referenced_table AS ParentTable
            FROM duckdb_constraints() k
            WHERE k.database_name = current_database() AND k.constraint_type = 'FOREIGN KEY'";

        #endregion
    }
}
