#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="FirebirdSchemaReader"/> to read the schema from the Firebird system tables (<c>RDB$*</c>).
    /// Firebird has no schema, so a table is identified by its name only.
    /// </summary>
    internal static class FirebirdSchemaText
    {
        #region Constants

        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM RDB$RELATIONS r
                WHERE r.RDB$RELATION_NAME = @TableName AND r.RDB$VIEW_BLR IS NULL AND COALESCE(r.RDB$SYSTEM_FLAG, 0) = 0) THEN 1 ELSE 0 END
            FROM RDB$DATABASE";

        internal const string ColumnsSql = @"SELECT f.RDB$FIELD_POSITION + 1 AS Ordinal,
                TRIM(f.RDB$FIELD_NAME) AS Name,
                d.RDB$FIELD_TYPE AS FieldType,
                d.RDB$FIELD_SUB_TYPE AS FieldSubType,
                d.RDB$FIELD_LENGTH AS FieldLength,
                d.RDB$CHARACTER_LENGTH AS CharacterLength,
                d.RDB$CHARACTER_SET_ID AS CharacterSetId,
                d.RDB$FIELD_PRECISION AS PrecisionValue,
                d.RDB$FIELD_SCALE AS ScaleValue,
                CASE WHEN COALESCE(f.RDB$NULL_FLAG, 0) = 1 OR COALESCE(d.RDB$NULL_FLAG, 0) = 1 THEN 0 ELSE 1 END AS IsNullable,
                CASE WHEN f.RDB$IDENTITY_TYPE IS NOT NULL THEN 1 ELSE 0 END AS IsIdentity,
                g.RDB$INITIAL_VALUE AS IdentitySeed,
                g.RDB$GENERATOR_INCREMENT AS IdentityIncrement,
                CAST(COALESCE(f.RDB$DEFAULT_SOURCE, d.RDB$DEFAULT_SOURCE) AS VARCHAR(8000)) AS DefaultDefinition,
                CAST(d.RDB$COMPUTED_SOURCE AS VARCHAR(8000)) AS ComputedDefinition,
                CASE WHEN EXISTS (SELECT 1 FROM RDB$RELATION_CONSTRAINTS rc
                        INNER JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
                        WHERE rc.RDB$RELATION_NAME = f.RDB$RELATION_NAME AND rc.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY' AND s.RDB$FIELD_NAME = f.RDB$FIELD_NAME)
                    THEN 1 ELSE 0 END AS IsPrimary
            FROM RDB$RELATION_FIELDS f
            INNER JOIN RDB$FIELDS d ON d.RDB$FIELD_NAME = f.RDB$FIELD_SOURCE
            LEFT JOIN RDB$GENERATORS g ON g.RDB$GENERATOR_NAME = f.RDB$GENERATOR_NAME
            WHERE f.RDB$RELATION_NAME = @TableName
            ORDER BY f.RDB$FIELD_POSITION";

        internal const string KeyConstraintSql = @"SELECT TRIM(rc.RDB$CONSTRAINT_NAME) AS ConstraintName,
                TRIM(s.RDB$FIELD_NAME) AS ColumnName,
                1 AS IsClustered
            FROM RDB$RELATION_CONSTRAINTS rc
            INNER JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
            WHERE rc.RDB$RELATION_NAME = @TableName AND rc.RDB$CONSTRAINT_TYPE = @KeyType
            ORDER BY rc.RDB$CONSTRAINT_NAME, s.RDB$FIELD_POSITION";

        internal const string IndexesSql = @"SELECT TRIM(i.RDB$INDEX_NAME) AS IndexName,
                CASE WHEN i.RDB$UNIQUE_FLAG = 1 THEN 1 ELSE 0 END AS IsUnique,
                0 AS IsClustered,
                CAST(NULL AS VARCHAR(4000)) AS FilterDefinition,
                TRIM(s.RDB$FIELD_NAME) AS ColumnName,
                0 AS IsIncluded,
                CASE WHEN i.RDB$INDEX_TYPE = 1 THEN 1 ELSE 0 END AS IsDescending
            FROM RDB$INDICES i
            INNER JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = i.RDB$INDEX_NAME
            WHERE i.RDB$RELATION_NAME = @TableName AND i.RDB$EXPRESSION_SOURCE IS NULL AND COALESCE(i.RDB$SYSTEM_FLAG, 0) = 0
                AND NOT EXISTS (SELECT 1 FROM RDB$RELATION_CONSTRAINTS rc WHERE rc.RDB$INDEX_NAME = i.RDB$INDEX_NAME)
            ORDER BY i.RDB$INDEX_NAME, s.RDB$FIELD_POSITION";

        internal const string ForeignKeysSql = @"SELECT TRIM(rc.RDB$CONSTRAINT_NAME) AS ForeignKeyName,
                TRIM(s.RDB$FIELD_NAME) AS ColumnName,
                CAST(NULL AS VARCHAR(63)) AS ReferencedSchema,
                TRIM(pk.RDB$RELATION_NAME) AS ReferencedTable,
                TRIM(ps.RDB$FIELD_NAME) AS ReferencedColumn,
                TRIM(rf.RDB$UPDATE_RULE) AS UpdateAction,
                TRIM(rf.RDB$DELETE_RULE) AS DeleteAction
            FROM RDB$RELATION_CONSTRAINTS rc
            INNER JOIN RDB$REF_CONSTRAINTS rf ON rf.RDB$CONSTRAINT_NAME = rc.RDB$CONSTRAINT_NAME
            INNER JOIN RDB$RELATION_CONSTRAINTS pk ON pk.RDB$CONSTRAINT_NAME = rf.RDB$CONST_NAME_UQ
            INNER JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
            INNER JOIN RDB$INDEX_SEGMENTS ps ON ps.RDB$INDEX_NAME = pk.RDB$INDEX_NAME AND ps.RDB$FIELD_POSITION = s.RDB$FIELD_POSITION
            WHERE rc.RDB$RELATION_NAME = @TableName AND rc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'
            ORDER BY rc.RDB$CONSTRAINT_NAME, s.RDB$FIELD_POSITION";

        internal const string CheckConstraintsSql = @"SELECT TRIM(rc.RDB$CONSTRAINT_NAME) AS ConstraintName,
                CAST(t.RDB$TRIGGER_SOURCE AS VARCHAR(8000)) AS Definition
            FROM RDB$RELATION_CONSTRAINTS rc
            INNER JOIN RDB$CHECK_CONSTRAINTS cc ON cc.RDB$CONSTRAINT_NAME = rc.RDB$CONSTRAINT_NAME
            INNER JOIN RDB$TRIGGERS t ON t.RDB$TRIGGER_NAME = cc.RDB$TRIGGER_NAME
            WHERE rc.RDB$RELATION_NAME = @TableName AND rc.RDB$CONSTRAINT_TYPE = 'CHECK' AND t.RDB$TRIGGER_TYPE = 1
            ORDER BY rc.RDB$CONSTRAINT_NAME";

        internal const string TablesSql = @"SELECT CAST(NULL AS VARCHAR(63)) AS SchemaName,
                TRIM(r.RDB$RELATION_NAME) AS TableName
            FROM RDB$RELATIONS r
            WHERE r.RDB$VIEW_BLR IS NULL AND COALESCE(r.RDB$SYSTEM_FLAG, 0) = 0
            ORDER BY 2";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT CAST(NULL AS VARCHAR(63)) AS ChildSchema,
                TRIM(rc.RDB$RELATION_NAME) AS ChildTable,
                CAST(NULL AS VARCHAR(63)) AS ParentSchema,
                TRIM(pk.RDB$RELATION_NAME) AS ParentTable
            FROM RDB$RELATION_CONSTRAINTS rc
            INNER JOIN RDB$REF_CONSTRAINTS rf ON rf.RDB$CONSTRAINT_NAME = rc.RDB$CONSTRAINT_NAME
            INNER JOIN RDB$RELATION_CONSTRAINTS pk ON pk.RDB$CONSTRAINT_NAME = rf.RDB$CONST_NAME_UQ
            WHERE rc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'";

        #endregion
    }
}
