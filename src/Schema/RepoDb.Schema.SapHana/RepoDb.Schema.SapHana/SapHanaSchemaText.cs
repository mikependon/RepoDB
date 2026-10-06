#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="SapHanaSchemaReader"/> to read the schema from the SAP HANA system views (<c>SYS.*</c>).
    /// A schema is an actual schema: the reader resolves the current schema of the connection before it uses the texts, so the
    /// <c>:SchemaName</c> parameter is never blank.
    /// </summary>
    internal static class SapHanaSchemaText
    {
        #region Constants

        private const string CurrentSchema = "CURRENT_SCHEMA";

        internal const string CurrentSchemaSql = "SELECT CURRENT_SCHEMA FROM DUMMY";

        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYS.TABLES t
                WHERE t.SCHEMA_NAME = :SchemaName AND t.TABLE_NAME = :TableName AND t.IS_USER_DEFINED_TYPE = 'FALSE') THEN 1 ELSE 0 END
            FROM DUMMY";

        internal const string ColumnsSql = @"SELECT c.POSITION AS Ordinal,
                c.COLUMN_NAME AS Name,
                c.DATA_TYPE_NAME AS TypeName,
                c.LENGTH AS MaxLength,
                c.LENGTH AS PrecisionValue,
                c.SCALE AS ScaleValue,
                CASE WHEN c.IS_NULLABLE = 'TRUE' THEN 1 ELSE 0 END AS IsNullable,
                CASE WHEN c.GENERATION_TYPE LIKE '%IDENTITY' THEN 1 ELSE 0 END AS IsIdentity,
                CASE WHEN c.GENERATION_TYPE LIKE '%IDENTITY' THEN c.GENERATED_ALWAYS_AS END AS IdentityOptions,
                c.DEFAULT_VALUE AS DefaultDefinition,
                CASE WHEN c.GENERATION_TYPE = 'ALWAYS AS' THEN c.GENERATED_ALWAYS_AS END AS ComputedDefinition,
                CASE WHEN EXISTS (SELECT 1 FROM SYS.CONSTRAINTS k
                        WHERE k.SCHEMA_NAME = c.SCHEMA_NAME AND k.TABLE_NAME = c.TABLE_NAME AND k.COLUMN_NAME = c.COLUMN_NAME AND k.IS_PRIMARY_KEY = 'TRUE')
                    THEN 1 ELSE 0 END AS IsPrimary
            FROM SYS.TABLE_COLUMNS c
            WHERE c.SCHEMA_NAME = :SchemaName AND c.TABLE_NAME = :TableName
            ORDER BY c.POSITION";

        internal const string KeyConstraintSql = @"SELECT k.CONSTRAINT_NAME AS ConstraintName,
                k.COLUMN_NAME AS ColumnName,
                1 AS IsClustered
            FROM SYS.CONSTRAINTS k
            WHERE k.SCHEMA_NAME = :SchemaName AND k.TABLE_NAME = :TableName AND k.COLUMN_NAME IS NOT NULL
                AND k.IS_UNIQUE_KEY = 'TRUE' AND k.IS_PRIMARY_KEY = :IsPrimaryKey
            ORDER BY k.CONSTRAINT_NAME, k.POSITION";

        internal const string IndexesSql = @"SELECT i.INDEX_NAME AS IndexName,
                0 AS IsUnique,
                0 AS IsClustered,
                CAST(NULL AS NVARCHAR(4000)) AS FilterDefinition,
                ic.COLUMN_NAME AS ColumnName,
                0 AS IsIncluded,
                CASE WHEN ic.ASCENDING_ORDER = 'FALSE' THEN 1 ELSE 0 END AS IsDescending
            FROM SYS.INDEXES i
            INNER JOIN SYS.INDEX_COLUMNS ic ON ic.SCHEMA_NAME = i.SCHEMA_NAME AND ic.TABLE_NAME = i.TABLE_NAME AND ic.INDEX_NAME = i.INDEX_NAME
            WHERE i.SCHEMA_NAME = :SchemaName AND i.TABLE_NAME = :TableName AND i.CONSTRAINT IS NULL
                AND i.INDEX_NAME NOT LIKE '\_SYS%' ESCAPE '\'
            ORDER BY i.INDEX_NAME, ic.POSITION";

        internal const string ForeignKeysSql = @"SELECT f.CONSTRAINT_NAME AS ForeignKeyName,
                f.COLUMN_NAME AS ColumnName,
                CASE WHEN f.REFERENCED_SCHEMA_NAME = " + CurrentSchema + @" THEN NULL ELSE f.REFERENCED_SCHEMA_NAME END AS ReferencedSchema,
                f.REFERENCED_TABLE_NAME AS ReferencedTable,
                f.REFERENCED_COLUMN_NAME AS ReferencedColumn,
                f.UPDATE_RULE AS UpdateAction,
                f.DELETE_RULE AS DeleteAction
            FROM SYS.REFERENTIAL_CONSTRAINTS f
            WHERE f.SCHEMA_NAME = :SchemaName AND f.TABLE_NAME = :TableName
            ORDER BY f.CONSTRAINT_NAME, f.POSITION";

        internal const string CheckConstraintsSql = @"SELECT DISTINCT k.CONSTRAINT_NAME AS ConstraintName,
                k.CHECK_CONDITION AS Definition
            FROM SYS.CONSTRAINTS k
            WHERE k.SCHEMA_NAME = :SchemaName AND k.TABLE_NAME = :TableName AND k.CHECK_CONDITION IS NOT NULL
            ORDER BY k.CONSTRAINT_NAME";

        internal const string TablesSql = @"SELECT CASE WHEN t.SCHEMA_NAME = " + CurrentSchema + @" THEN NULL ELSE t.SCHEMA_NAME END AS SchemaName,
                t.TABLE_NAME AS TableName
            FROM SYS.TABLES t
            WHERE t.SCHEMA_NAME = :SchemaName AND t.IS_USER_DEFINED_TYPE = 'FALSE'
            ORDER BY t.SCHEMA_NAME, t.TABLE_NAME";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT CASE WHEN f.SCHEMA_NAME = " + CurrentSchema + @" THEN NULL ELSE f.SCHEMA_NAME END AS ChildSchema,
                f.TABLE_NAME AS ChildTable,
                CASE WHEN f.REFERENCED_SCHEMA_NAME = " + CurrentSchema + @" THEN NULL ELSE f.REFERENCED_SCHEMA_NAME END AS ParentSchema,
                f.REFERENCED_TABLE_NAME AS ParentTable
            FROM SYS.REFERENTIAL_CONSTRAINTS f
            WHERE f.SCHEMA_NAME NOT LIKE '\_SYS%' ESCAPE '\' AND f.SCHEMA_NAME <> 'SYS'
                AND f.REFERENCED_SCHEMA_NAME NOT LIKE '\_SYS%' ESCAPE '\' AND f.REFERENCED_SCHEMA_NAME <> 'SYS'";

        #endregion
    }
}
