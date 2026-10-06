#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="MariaDbSchemaReader"/> to read the schema from the MariaDB catalog views.
    /// </summary>
    internal static class MariaDbSchemaText
    {
        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1
                FROM information_schema.TABLES t
                WHERE t.TABLE_SCHEMA = COALESCE(@Schema, DATABASE()) AND t.TABLE_NAME = @Table AND t.TABLE_TYPE = 'BASE TABLE') THEN 1 ELSE 0 END;";

        internal const string ColumnsSql = @"SELECT c.ORDINAL_POSITION AS Ordinal,
                c.COLUMN_NAME AS Name,
                c.DATA_TYPE AS TypeName,
                c.COLUMN_TYPE AS ColumnType,
                c.CHARACTER_MAXIMUM_LENGTH AS MaxLength,
                c.NUMERIC_PRECISION AS `Precision`,
                COALESCE(c.NUMERIC_SCALE, c.DATETIME_PRECISION) AS `Scale`,
                CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IsNullable,
                CASE WHEN c.EXTRA LIKE '%auto_increment%' THEN 1 ELSE 0 END AS IsIdentity,
                c.EXTRA AS Extra,
                c.COLLATION_NAME AS Collation,
                c.COLUMN_DEFAULT AS DefaultDefinition,
                NULLIF(c.GENERATION_EXPRESSION, '') AS ComputedDefinition,
                NULLIF(c.COLUMN_COMMENT, '') AS Comment,
                CASE WHEN c.COLUMN_KEY = 'PRI' THEN 1 ELSE 0 END AS IsPrimary
            FROM information_schema.COLUMNS c
            WHERE c.TABLE_SCHEMA = COALESCE(@Schema, DATABASE()) AND c.TABLE_NAME = @Table
            ORDER BY c.ORDINAL_POSITION;";

        internal const string KeyConstraintSql = @"SELECT s.INDEX_NAME AS ConstraintName,
                s.COLUMN_NAME AS ColumnName,
                1 AS IsClustered
            FROM information_schema.STATISTICS s
            WHERE s.TABLE_SCHEMA = COALESCE(@Schema, DATABASE()) AND s.TABLE_NAME = @Table
                AND ((@Type = 'PK' AND s.INDEX_NAME = 'PRIMARY') OR (@Type = 'UQ' AND s.NON_UNIQUE = 0 AND s.INDEX_NAME <> 'PRIMARY'))
                AND NOT EXISTS (SELECT 1 FROM information_schema.STATISTICS x
                    WHERE x.TABLE_SCHEMA = s.TABLE_SCHEMA AND x.TABLE_NAME = s.TABLE_NAME AND x.INDEX_NAME = s.INDEX_NAME AND x.COLUMN_NAME IS NULL)
            ORDER BY s.INDEX_NAME, s.SEQ_IN_INDEX;";

        internal const string IndexesSql = @"SELECT s.INDEX_NAME AS IndexName,
                0 AS IsUnique,
                0 AS IsClustered,
                NULL AS FilterDefinition,
                s.COLUMN_NAME AS ColumnName,
                0 AS IsIncluded,
                CASE WHEN s.COLLATION = 'D' THEN 1 ELSE 0 END AS IsDescending
            FROM information_schema.STATISTICS s
            WHERE s.TABLE_SCHEMA = COALESCE(@Schema, DATABASE()) AND s.TABLE_NAME = @Table
                AND s.NON_UNIQUE = 1
                AND s.INDEX_TYPE IN ('BTREE', 'HASH')
                AND NOT EXISTS (SELECT 1 FROM information_schema.STATISTICS x
                    WHERE x.TABLE_SCHEMA = s.TABLE_SCHEMA AND x.TABLE_NAME = s.TABLE_NAME AND x.INDEX_NAME = s.INDEX_NAME AND x.COLUMN_NAME IS NULL)
                AND NOT EXISTS (SELECT 1 FROM information_schema.KEY_COLUMN_USAGE k
                    WHERE k.TABLE_SCHEMA = s.TABLE_SCHEMA AND k.TABLE_NAME = s.TABLE_NAME AND k.CONSTRAINT_NAME = s.INDEX_NAME AND k.REFERENCED_TABLE_NAME IS NOT NULL)
            ORDER BY s.INDEX_NAME, s.SEQ_IN_INDEX;";

        internal const string ForeignKeysSql = @"SELECT kcu.CONSTRAINT_NAME AS ForeignKeyName,
                kcu.COLUMN_NAME AS ColumnName,
                CASE WHEN kcu.REFERENCED_TABLE_SCHEMA = DATABASE() THEN NULL ELSE kcu.REFERENCED_TABLE_SCHEMA END AS ReferencedSchema,
                kcu.REFERENCED_TABLE_NAME AS ReferencedTable,
                kcu.REFERENCED_COLUMN_NAME AS ReferencedColumn,
                rc.UPDATE_RULE AS UpdateAction,
                rc.DELETE_RULE AS DeleteAction
            FROM information_schema.KEY_COLUMN_USAGE kcu
            INNER JOIN information_schema.REFERENTIAL_CONSTRAINTS rc ON rc.CONSTRAINT_SCHEMA = kcu.CONSTRAINT_SCHEMA AND rc.TABLE_NAME = kcu.TABLE_NAME AND rc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
            WHERE kcu.TABLE_SCHEMA = COALESCE(@Schema, DATABASE()) AND kcu.TABLE_NAME = @Table AND kcu.REFERENCED_TABLE_NAME IS NOT NULL
            ORDER BY kcu.CONSTRAINT_NAME, kcu.ORDINAL_POSITION;";

        internal const string CheckConstraintsSql = @"SELECT tc.CONSTRAINT_NAME AS ConstraintName,
                cc.CHECK_CLAUSE AS Definition
            FROM information_schema.TABLE_CONSTRAINTS tc
            INNER JOIN information_schema.CHECK_CONSTRAINTS cc ON cc.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA AND cc.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
            WHERE tc.TABLE_SCHEMA = COALESCE(@Schema, DATABASE()) AND tc.TABLE_NAME = @Table AND tc.CONSTRAINT_TYPE = 'CHECK'
            ORDER BY tc.CONSTRAINT_NAME;";

        internal const string TablesSql = @"SELECT CASE WHEN t.TABLE_SCHEMA = DATABASE() THEN NULL ELSE t.TABLE_SCHEMA END AS SchemaName,
                t.TABLE_NAME AS TableName
            FROM information_schema.TABLES t
            WHERE t.TABLE_TYPE = 'BASE TABLE'
                AND ((@SchemaName IS NULL AND t.TABLE_SCHEMA = DATABASE()) OR t.TABLE_SCHEMA = @SchemaName)
            ORDER BY t.TABLE_SCHEMA, t.TABLE_NAME;";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT CASE WHEN kcu.TABLE_SCHEMA = DATABASE() THEN NULL ELSE kcu.TABLE_SCHEMA END AS ChildSchema,
                kcu.TABLE_NAME AS ChildTable,
                CASE WHEN kcu.REFERENCED_TABLE_SCHEMA = DATABASE() THEN NULL ELSE kcu.REFERENCED_TABLE_SCHEMA END AS ParentSchema,
                kcu.REFERENCED_TABLE_NAME AS ParentTable
            FROM information_schema.KEY_COLUMN_USAGE kcu
            WHERE kcu.REFERENCED_TABLE_NAME IS NOT NULL
                AND kcu.TABLE_SCHEMA NOT IN ('mysql', 'sys', 'information_schema', 'performance_schema')
                AND kcu.REFERENCED_TABLE_SCHEMA NOT IN ('mysql', 'sys', 'information_schema', 'performance_schema');";
    }
}
