#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="SqlServerSchemaReader"/> to read the schema from the SQL Server catalog views.
    /// </summary>
    internal static class SqlServerSchemaText
    {
        internal const string ResolveSchemaSql = @"SELECT TOP 1 SCHEMA_NAME(t.schema_id)
            FROM sys.tables t
            WHERE t.name = @TableName
            ORDER BY CASE WHEN t.schema_id = SCHEMA_ID() THEN 0 ELSE 1 END;";

        internal const string TableExistsSql = "SELECT CASE WHEN OBJECT_ID(@FullName, N'U') IS NULL THEN 0 ELSE 1 END;";

        internal const string ColumnsSql = @"SELECT c.column_id AS Ordinal,
                c.name AS Name,
                ty.name AS TypeName,
                c.max_length AS MaxLength,
                c.precision AS [Precision],
                c.scale AS [Scale],
                c.is_nullable AS IsNullable,
                c.is_identity AS IsIdentity,
                c.collation_name AS Collation,
                dc.definition AS DefaultDefinition,
                cc.definition AS ComputedDefinition,
                CAST(idc.seed_value AS bigint) AS IdentitySeed,
                CAST(idc.increment_value AS bigint) AS IdentityIncrement,
                CAST(ep.value AS nvarchar(4000)) AS Comment,
                CAST(CASE WHEN EXISTS (SELECT 1
                    FROM sys.indexes i
                    INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                    WHERE i.object_id = c.object_id AND i.is_primary_key = 1 AND ic.column_id = c.column_id) THEN 1 ELSE 0 END AS bit) AS IsPrimary
            FROM sys.columns c
            INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
            LEFT JOIN sys.default_constraints dc ON dc.object_id = c.default_object_id
            LEFT JOIN sys.computed_columns cc ON cc.object_id = c.object_id AND cc.column_id = c.column_id
            LEFT JOIN sys.identity_columns idc ON idc.object_id = c.object_id AND idc.column_id = c.column_id
            LEFT JOIN sys.extended_properties ep ON ep.class = 1 AND ep.major_id = c.object_id AND ep.minor_id = c.column_id AND ep.name = N'MS_Description'
            WHERE c.object_id = OBJECT_ID(@FullName)
            ORDER BY c.column_id;";

        internal const string KeyConstraintSql = @"SELECT kc.name AS ConstraintName,
                col.name AS ColumnName,
                CAST(CASE WHEN i.type = 1 THEN 1 ELSE 0 END AS bit) AS IsClustered
            FROM sys.key_constraints kc
            INNER JOIN sys.indexes i ON i.object_id = kc.parent_object_id AND i.index_id = kc.unique_index_id
            INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
            INNER JOIN sys.columns col ON col.object_id = ic.object_id AND col.column_id = ic.column_id
            WHERE kc.parent_object_id = OBJECT_ID(@FullName) AND kc.type = @Type
            ORDER BY kc.name, ic.key_ordinal;";

        internal const string IndexesSql = @"SELECT i.name AS IndexName,
                i.is_unique AS IsUnique,
                CAST(CASE WHEN i.type = 1 THEN 1 ELSE 0 END AS bit) AS IsClustered,
                i.filter_definition AS FilterDefinition,
                col.name AS ColumnName,
                ic.is_included_column AS IsIncluded,
                ic.is_descending_key AS IsDescending
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
            INNER JOIN sys.columns col ON col.object_id = ic.object_id AND col.column_id = ic.column_id
            WHERE i.object_id = OBJECT_ID(@FullName)
                AND i.is_primary_key = 0
                AND i.is_unique_constraint = 0
                AND i.type IN (1, 2)
                AND i.name IS NOT NULL
            ORDER BY i.name, ic.is_included_column, ic.key_ordinal, ic.index_column_id;";

        internal const string ForeignKeysSql = @"SELECT fk.name AS ForeignKeyName,
                pcol.name AS ColumnName,
                SCHEMA_NAME(rt.schema_id) AS ReferencedSchema,
                rt.name AS ReferencedTable,
                rcol.name AS ReferencedColumn,
                fk.update_referential_action AS UpdateAction,
                fk.delete_referential_action AS DeleteAction
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
            INNER JOIN sys.columns pcol ON pcol.object_id = fkc.parent_object_id AND pcol.column_id = fkc.parent_column_id
            INNER JOIN sys.tables rt ON rt.object_id = fkc.referenced_object_id
            INNER JOIN sys.columns rcol ON rcol.object_id = fkc.referenced_object_id AND rcol.column_id = fkc.referenced_column_id
            WHERE fk.parent_object_id = OBJECT_ID(@FullName)
            ORDER BY fk.name, fkc.constraint_column_id;";

        internal const string CheckConstraintsSql = @"SELECT cc.name AS ConstraintName,
                cc.definition AS Definition
            FROM sys.check_constraints cc
            WHERE cc.parent_object_id = OBJECT_ID(@FullName)
            ORDER BY cc.name;";

        internal const string TablesSql = @"SELECT SCHEMA_NAME(t.schema_id) AS SchemaName,
                t.name AS TableName
            FROM sys.tables t
            WHERE @SchemaName IS NULL OR SCHEMA_NAME(t.schema_id) = @SchemaName
            ORDER BY SCHEMA_NAME(t.schema_id), t.name;";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT SCHEMA_NAME(ct.schema_id) AS ChildSchema,
                ct.name AS ChildTable,
                SCHEMA_NAME(pt.schema_id) AS ParentSchema,
                pt.name AS ParentTable
            FROM sys.foreign_keys fk
            INNER JOIN sys.tables ct ON ct.object_id = fk.parent_object_id
            INNER JOIN sys.tables pt ON pt.object_id = fk.referenced_object_id;";

    }
}
