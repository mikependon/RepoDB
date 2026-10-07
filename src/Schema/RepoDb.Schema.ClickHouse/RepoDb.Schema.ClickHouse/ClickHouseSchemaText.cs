#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="ClickHouseSchemaReader"/> to read the schema from the ClickHouse system tables (<c>system.*</c>).
    /// A schema is a database: a blank schema parameter is the current database of the connection. ClickHouse has no foreign key and no unique constraint,
    /// so the texts of those return no row.
    /// </summary>
    internal static class ClickHouseSchemaText
    {
        private const string Database = "coalesce({Schema:Nullable(String)}, currentDatabase())";

        internal const string TableExistsSql = @"SELECT toUInt8(count() > 0) FROM system.tables t
            WHERE t.database = " + Database + @" AND t.name = {Table:String} AND NOT t.is_temporary AND t.engine NOT IN ('View', 'MaterializedView', 'Dictionary', 'LiveView')";

        internal const string ColumnsSql = @"SELECT toInt32(c.position) AS Ordinal,
                c.name AS Name,
                c.type AS TypeName,
                c.default_kind AS DefaultKind,
                c.default_expression AS DefaultDefinition,
                toUInt8(c.is_in_primary_key) AS IsPrimary
            FROM system.columns c
            WHERE c.database = " + Database + @" AND c.table = {Table:String}
            ORDER BY c.position";

        internal const string KeyConstraintSql = @"SELECT 'PRIMARY' AS ConstraintName,
                trim(BOTH ' `()' FROM column) AS ColumnName,
                1 AS IsClustered
            FROM system.tables t
            ARRAY JOIN splitByString(',', t.primary_key) AS column
            WHERE t.database = " + Database + @" AND t.name = {Table:String} AND {Type:String} = 'PK' AND trim(BOTH ' `()' FROM column) <> ''";

        internal const string IndexesSql = @"SELECT i.name AS IndexName,
                0 AS IsUnique,
                0 AS IsClustered,
                CAST(NULL AS Nullable(String)) AS FilterDefinition,
                trim(BOTH ' `()' FROM column) AS ColumnName,
                0 AS IsIncluded,
                0 AS IsDescending
            FROM system.data_skipping_indices i
            ARRAY JOIN splitByString(',', replaceRegexpAll(i.expr, '[()]', '')) AS column
            WHERE i.database = " + Database + @" AND i.table = {Table:String}";

        internal const string ForeignKeysSql = @"SELECT '' AS ForeignKeyName,
                '' AS ColumnName,
                CAST(NULL AS Nullable(String)) AS ReferencedSchema,
                '' AS ReferencedTable,
                '' AS ReferencedColumn,
                'NO ACTION' AS UpdateAction,
                'NO ACTION' AS DeleteAction
            WHERE 0";

        internal const string CheckConstraintsSql = @"SELECT t.create_table_query AS Definition
            FROM system.tables t
            WHERE t.database = " + Database + @" AND t.name = {Table:String}";

        internal const string TablesSql = @"SELECT if(t.database = currentDatabase(), CAST(NULL AS Nullable(String)), t.database) AS SchemaName,
                t.name AS TableName
            FROM system.tables t
            WHERE NOT t.is_temporary AND t.engine NOT IN ('View', 'MaterializedView', 'Dictionary', 'LiveView')
                AND t.database = coalesce({SchemaName:Nullable(String)}, currentDatabase())
            ORDER BY t.database, t.name";

        internal const string ForeignKeyRelationshipsSql = @"SELECT CAST(NULL AS Nullable(String)) AS ChildSchema,
                '' AS ChildTable,
                CAST(NULL AS Nullable(String)) AS ParentSchema,
                '' AS ParentTable
            WHERE 0";
    }
}
