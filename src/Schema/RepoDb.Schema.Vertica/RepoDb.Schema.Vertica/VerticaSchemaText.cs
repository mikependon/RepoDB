#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="VerticaSchemaReader"/> to read the schema from the Vertica system views (<c>v_catalog.*</c>).
    /// A schema is an actual schema: the reader resolves the current schema of the connection before it uses the texts, so the
    /// <c>@SchemaName</c> parameter is never blank, and the <c>@CurrentSchema</c> parameter is the current schema (Vertica does not allow its
    /// <c>CURRENT_SCHEMA()</c> meta-function in a query of the system views). Vertica has no index (it has projections), so there is no text for the indexes.
    /// </summary>
    internal static class VerticaSchemaText
    {
        #region Constants

        private const string CurrentSchema = "@CurrentSchema";

        internal const string CurrentSchemaSql = "SELECT CURRENT_SCHEMA()";

        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM v_catalog.tables t
                WHERE t.table_schema = @SchemaName AND t.table_name = @TableName) THEN 1 ELSE 0 END";

        internal const string ColumnsSql = @"SELECT c.ordinal_position AS Ordinal,
                c.column_name AS Name,
                c.data_type AS TypeName,
                COALESCE(c.character_maximum_length, 0) AS MaxLength,
                COALESCE(c.numeric_precision, 0) AS PrecisionValue,
                COALESCE(c.numeric_scale, c.datetime_precision, 0) AS ScaleValue,
                CASE WHEN c.is_nullable THEN 1 ELSE 0 END AS IsNullable,
                CASE WHEN c.is_identity THEN 1 ELSE 0 END AS IsIdentity,
                s.minimum AS IdentitySeed,
                s.increment_by AS IdentityIncrement,
                CASE WHEN c.is_identity THEN NULL ELSE c.column_default END AS DefaultDefinition,
                CASE WHEN EXISTS (SELECT 1 FROM v_catalog.primary_keys k
                        WHERE k.table_schema = c.table_schema AND k.table_name = c.table_name AND k.column_name = c.column_name)
                    THEN 1 ELSE 0 END AS IsPrimary
            FROM v_catalog.columns c
            LEFT JOIN v_catalog.sequences s ON s.sequence_schema = c.table_schema AND s.identity_table_name = c.table_name
                AND s.sequence_name = c.table_name || '_' || c.column_name || '_seq'
            WHERE c.table_schema = @SchemaName AND c.table_name = @TableName
            ORDER BY c.ordinal_position";

        internal const string KeyConstraintSql = @"SELECT k.constraint_name AS ConstraintName,
                k.column_name AS ColumnName,
                1 AS IsClustered
            FROM v_catalog.constraint_columns k
            INNER JOIN v_catalog.columns c ON c.table_schema = k.table_schema AND c.table_name = k.table_name AND c.column_name = k.column_name
            LEFT JOIN v_catalog.primary_keys p ON p.table_schema = k.table_schema AND p.table_name = k.table_name
                AND p.constraint_name = k.constraint_name AND p.column_name = k.column_name
            WHERE k.table_schema = @SchemaName AND k.table_name = @TableName AND k.constraint_type = @KeyType
            ORDER BY k.constraint_name, COALESCE(p.ordinal_position, c.ordinal_position)";

        internal const string ForeignKeysSql = @"SELECT f.constraint_name AS ForeignKeyName,
                f.column_name AS ColumnName,
                CASE WHEN f.reference_table_schema = " + CurrentSchema + @" THEN NULL ELSE f.reference_table_schema END AS ReferencedSchema,
                f.reference_table_name AS ReferencedTable,
                f.reference_column_name AS ReferencedColumn,
                'NO ACTION' AS UpdateAction,
                'NO ACTION' AS DeleteAction
            FROM v_catalog.foreign_keys f
            WHERE f.table_schema = @SchemaName AND f.table_name = @TableName
            ORDER BY f.constraint_name, f.ordinal_position";

        internal const string CheckConstraintsSql = @"SELECT k.constraint_name AS ConstraintName,
                k.predicate AS Definition
            FROM v_catalog.table_constraints k
            INNER JOIN v_catalog.tables t ON t.table_id = k.table_id
            WHERE t.table_schema = @SchemaName AND t.table_name = @TableName AND k.constraint_type = 'c'
            ORDER BY k.constraint_name";

        internal const string TablesSql = @"SELECT CASE WHEN t.table_schema = " + CurrentSchema + @" THEN NULL ELSE t.table_schema END AS SchemaName,
                t.table_name AS TableName
            FROM v_catalog.tables t
            WHERE t.table_schema = @SchemaName
            ORDER BY t.table_schema, t.table_name";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT CASE WHEN f.table_schema = " + CurrentSchema + @" THEN NULL ELSE f.table_schema END AS ChildSchema,
                f.table_name AS ChildTable,
                CASE WHEN f.reference_table_schema = " + CurrentSchema + @" THEN NULL ELSE f.reference_table_schema END AS ParentSchema,
                f.reference_table_name AS ParentTable
            FROM v_catalog.foreign_keys f
            WHERE f.table_schema NOT IN ('v_catalog', 'v_monitor', 'v_internal', 'v_txtindex', 'v_func')
                AND f.reference_table_schema NOT IN ('v_catalog', 'v_monitor', 'v_internal', 'v_txtindex', 'v_func')";

        #endregion
    }
}
