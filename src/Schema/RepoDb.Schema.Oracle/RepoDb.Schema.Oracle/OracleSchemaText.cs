#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="OracleSchemaReader"/> to read the schema from the Oracle data dictionary views (<c>ALL_*</c>).
    /// A schema is an owner: a blank schema parameter (<c>' '</c>) is the current schema of the connection.
    /// </summary>
    internal static class OracleSchemaText
    {
        #region Constants

        private const string Owner = "COALESCE(NULLIF(:SchemaName, ' '), SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA'))";

        private const string CurrentSchema = "SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA')";

        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM ALL_TABLES t
                WHERE t.OWNER = " + Owner + @" AND t.TABLE_NAME = :TableName) THEN 1 ELSE 0 END
            FROM DUAL";

        internal const string ColumnsSql = @"SELECT c.COLUMN_ID AS Ordinal,
                c.COLUMN_NAME AS Name,
                c.DATA_TYPE AS TypeName,
                CASE WHEN c.CHAR_USED = 'C' THEN c.CHAR_LENGTH ELSE c.DATA_LENGTH END AS MaxLength,
                c.DATA_PRECISION AS PrecisionValue,
                c.DATA_SCALE AS ScaleValue,
                CASE WHEN c.NULLABLE = 'Y' THEN 1 ELSE 0 END AS IsNullable,
                CASE WHEN c.IDENTITY_COLUMN = 'YES' THEN 1 ELSE 0 END AS IsIdentity,
                TO_NUMBER(REGEXP_SUBSTR(i.IDENTITY_OPTIONS, 'START WITH: (-?[0-9]+)', 1, 1, NULL, 1)) AS IdentitySeed,
                TO_NUMBER(REGEXP_SUBSTR(i.IDENTITY_OPTIONS, 'INCREMENT BY: (-?[0-9]+)', 1, 1, NULL, 1)) AS IdentityIncrement,
                CASE WHEN c.VIRTUAL_COLUMN = 'NO' AND c.IDENTITY_COLUMN = 'NO' THEN c.DATA_DEFAULT_VC END AS DefaultDefinition,
                CASE WHEN c.VIRTUAL_COLUMN = 'YES' THEN c.DATA_DEFAULT_VC END AS ComputedDefinition,
                CASE WHEN c.COLLATION IS NULL OR c.COLLATION = 'USING_NLS_COMP' THEN NULL ELSE c.COLLATION END AS CollationName,
                CASE WHEN EXISTS (SELECT 1 FROM ALL_CONSTRAINTS k
                        INNER JOIN ALL_CONS_COLUMNS kc ON kc.OWNER = k.OWNER AND kc.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                        WHERE k.OWNER = c.OWNER AND k.TABLE_NAME = c.TABLE_NAME AND k.CONSTRAINT_TYPE = 'P' AND kc.COLUMN_NAME = c.COLUMN_NAME)
                    THEN 1 ELSE 0 END AS IsPrimary
            FROM ALL_TAB_COLS c
            LEFT JOIN ALL_TAB_IDENTITY_COLS i ON i.OWNER = c.OWNER AND i.TABLE_NAME = c.TABLE_NAME AND i.COLUMN_NAME = c.COLUMN_NAME
            WHERE c.OWNER = " + Owner + @" AND c.TABLE_NAME = :TableName AND c.HIDDEN_COLUMN = 'NO'
            ORDER BY c.COLUMN_ID";

        internal const string KeyConstraintSql = @"SELECT k.CONSTRAINT_NAME AS ConstraintName,
                kc.COLUMN_NAME AS ColumnName,
                1 AS IsClustered
            FROM ALL_CONSTRAINTS k
            INNER JOIN ALL_CONS_COLUMNS kc ON kc.OWNER = k.OWNER AND kc.CONSTRAINT_NAME = k.CONSTRAINT_NAME
            WHERE k.OWNER = " + Owner + @" AND k.TABLE_NAME = :TableName AND k.CONSTRAINT_TYPE = :KeyType
            ORDER BY k.CONSTRAINT_NAME, kc.POSITION";

        internal const string IndexesSql = @"SELECT i.INDEX_NAME AS IndexName,
                CASE WHEN i.UNIQUENESS = 'UNIQUE' THEN 1 ELSE 0 END AS IsUnique,
                0 AS IsClustered,
                CAST(NULL AS VARCHAR2(4000)) AS FilterDefinition,
                CASE WHEN h.HIDDEN_COLUMN = 'YES' THEN TRIM(BOTH '""' FROM h.DATA_DEFAULT_VC) ELSE ic.COLUMN_NAME END AS ColumnName,
                0 AS IsIncluded,
                CASE WHEN ic.DESCEND = 'DESC' THEN 1 ELSE 0 END AS IsDescending
            FROM ALL_INDEXES i
            INNER JOIN ALL_IND_COLUMNS ic ON ic.INDEX_OWNER = i.OWNER AND ic.INDEX_NAME = i.INDEX_NAME
            LEFT JOIN ALL_TAB_COLS h ON h.OWNER = i.TABLE_OWNER AND h.TABLE_NAME = i.TABLE_NAME AND h.COLUMN_NAME = ic.COLUMN_NAME
            WHERE i.TABLE_OWNER = " + Owner + @" AND i.TABLE_NAME = :TableName
                AND i.INDEX_TYPE IN ('NORMAL', 'FUNCTION-BASED NORMAL', 'BITMAP')
                AND NOT EXISTS (SELECT 1 FROM ALL_CONSTRAINTS k
                    WHERE k.OWNER = i.TABLE_OWNER AND k.TABLE_NAME = i.TABLE_NAME AND k.INDEX_NAME = i.INDEX_NAME AND k.CONSTRAINT_TYPE IN ('P', 'U'))
                AND NOT EXISTS (SELECT 1 FROM ALL_IND_COLUMNS x
                    INNER JOIN ALL_TAB_COLS xh ON xh.OWNER = i.TABLE_OWNER AND xh.TABLE_NAME = i.TABLE_NAME AND xh.COLUMN_NAME = x.COLUMN_NAME
                    WHERE x.INDEX_OWNER = i.OWNER AND x.INDEX_NAME = i.INDEX_NAME AND xh.HIDDEN_COLUMN = 'YES'
                        AND NOT REGEXP_LIKE(xh.DATA_DEFAULT_VC, '^""[^""]+""$'))
            ORDER BY i.INDEX_NAME, ic.COLUMN_POSITION";

        internal const string ForeignKeysSql = @"SELECT f.CONSTRAINT_NAME AS ForeignKeyName,
                fc.COLUMN_NAME AS ColumnName,
                CASE WHEN r.OWNER = " + CurrentSchema + @" THEN NULL ELSE r.OWNER END AS ReferencedSchema,
                r.TABLE_NAME AS ReferencedTable,
                rc.COLUMN_NAME AS ReferencedColumn,
                'NO ACTION' AS UpdateAction,
                f.DELETE_RULE AS DeleteAction
            FROM ALL_CONSTRAINTS f
            INNER JOIN ALL_CONS_COLUMNS fc ON fc.OWNER = f.OWNER AND fc.CONSTRAINT_NAME = f.CONSTRAINT_NAME
            INNER JOIN ALL_CONSTRAINTS r ON r.OWNER = f.R_OWNER AND r.CONSTRAINT_NAME = f.R_CONSTRAINT_NAME
            INNER JOIN ALL_CONS_COLUMNS rc ON rc.OWNER = r.OWNER AND rc.CONSTRAINT_NAME = r.CONSTRAINT_NAME AND rc.POSITION = fc.POSITION
            WHERE f.OWNER = " + Owner + @" AND f.TABLE_NAME = :TableName AND f.CONSTRAINT_TYPE = 'R'
            ORDER BY f.CONSTRAINT_NAME, fc.POSITION";

        internal const string CheckConstraintsSql = @"SELECT k.CONSTRAINT_NAME AS ConstraintName,
                k.SEARCH_CONDITION_VC AS Definition
            FROM ALL_CONSTRAINTS k
            WHERE k.OWNER = " + Owner + @" AND k.TABLE_NAME = :TableName AND k.CONSTRAINT_TYPE = 'C'
                AND NOT REGEXP_LIKE(k.SEARCH_CONDITION_VC, '^""[^""]+"" IS NOT NULL$')
            ORDER BY k.CONSTRAINT_NAME";

        internal const string TablesSql = @"SELECT CASE WHEN t.OWNER = " + CurrentSchema + @" THEN NULL ELSE t.OWNER END AS SchemaName,
                t.TABLE_NAME AS TableName
            FROM ALL_TABLES t
            WHERE t.OWNER = " + Owner + @" AND t.TEMPORARY = 'N' AND t.NESTED = 'NO' AND t.IOT_NAME IS NULL
                AND t.TABLE_NAME NOT LIKE 'BIN$%' AND t.TABLE_NAME NOT LIKE 'DR$%' AND t.TABLE_NAME NOT LIKE 'MLOG$%'
            ORDER BY t.OWNER, t.TABLE_NAME";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT CASE WHEN f.OWNER = " + CurrentSchema + @" THEN NULL ELSE f.OWNER END AS ChildSchema,
                f.TABLE_NAME AS ChildTable,
                CASE WHEN r.OWNER = " + CurrentSchema + @" THEN NULL ELSE r.OWNER END AS ParentSchema,
                r.TABLE_NAME AS ParentTable
            FROM ALL_CONSTRAINTS f
            INNER JOIN ALL_CONSTRAINTS r ON r.OWNER = f.R_OWNER AND r.CONSTRAINT_NAME = f.R_CONSTRAINT_NAME
            WHERE f.CONSTRAINT_TYPE = 'R'
                AND f.OWNER IN (SELECT USERNAME FROM ALL_USERS WHERE ORACLE_MAINTAINED = 'N')
                AND r.OWNER IN (SELECT USERNAME FROM ALL_USERS WHERE ORACLE_MAINTAINED = 'N')";

        #endregion
    }
}
