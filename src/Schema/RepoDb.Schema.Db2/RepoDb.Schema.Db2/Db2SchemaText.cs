#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="Db2SchemaReader"/> to read the schema from the Db2 catalog views (<c>SYSCAT.*</c>).
    /// A schema is an actual schema: the reader resolves the current schema of the connection before it uses the texts, so the
    /// <c>:SchemaName</c> parameter is never blank.
    /// </summary>
    internal static class Db2SchemaText
    {
        #region Constants

        private const string CurrentSchema = "CURRENT SCHEMA";

        internal const string CurrentSchemaSql = "SELECT CURRENT SCHEMA FROM SYSIBM.SYSDUMMY1";

        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYSCAT.TABLES t
                WHERE t.TABSCHEMA = :SchemaName AND t.TABNAME = :TableName AND t.TYPE = 'T') THEN 1 ELSE 0 END
            FROM SYSIBM.SYSDUMMY1";

        internal const string ColumnsSql = @"SELECT c.COLNO + 1 AS Ordinal,
                c.COLNAME AS Name,
                c.TYPENAME AS TypeName,
                c.LENGTH AS MaxLength,
                c.LENGTH AS PrecisionValue,
                c.SCALE AS ScaleValue,
                CASE WHEN c.NULLS = 'Y' THEN 1 ELSE 0 END AS IsNullable,
                CASE WHEN c.IDENTITY = 'Y' THEN 1 ELSE 0 END AS IsIdentity,
                i.START AS IdentitySeed,
                i.INCREMENT AS IdentityIncrement,
                CASE WHEN c.IDENTITY = 'N' AND c.GENERATED = ' ' THEN c.DEFAULT END AS DefaultDefinition,
                CASE WHEN c.IDENTITY = 'N' AND c.GENERATED = 'A' THEN CAST(c.TEXT AS VARCHAR(4000)) END AS ComputedDefinition,
                CAST(NULL AS VARCHAR(128)) AS CollationName,
                CASE WHEN c.KEYSEQ IS NOT NULL AND c.KEYSEQ > 0 THEN 1 ELSE 0 END AS IsPrimary
            FROM SYSCAT.COLUMNS c
            LEFT JOIN SYSCAT.COLIDENTATTRIBUTES i ON i.TABSCHEMA = c.TABSCHEMA AND i.TABNAME = c.TABNAME AND i.COLNAME = c.COLNAME
            WHERE c.TABSCHEMA = :SchemaName AND c.TABNAME = :TableName
            ORDER BY c.COLNO";

        internal const string KeyConstraintSql = @"SELECT k.CONSTNAME AS ConstraintName,
                kc.COLNAME AS ColumnName,
                1 AS IsClustered
            FROM SYSCAT.TABCONST k
            INNER JOIN SYSCAT.KEYCOLUSE kc ON kc.TABSCHEMA = k.TABSCHEMA AND kc.TABNAME = k.TABNAME AND kc.CONSTNAME = k.CONSTNAME
            WHERE k.TABSCHEMA = :SchemaName AND k.TABNAME = :TableName AND k.TYPE = :KeyType
            ORDER BY k.CONSTNAME, kc.COLSEQ";

        internal const string IndexesSql = @"SELECT i.INDNAME AS IndexName,
                CASE WHEN i.UNIQUERULE = 'U' THEN 1 ELSE 0 END AS IsUnique,
                0 AS IsClustered,
                CAST(NULL AS VARCHAR(4000)) AS FilterDefinition,
                ic.COLNAME AS ColumnName,
                CASE WHEN ic.COLORDER = 'I' THEN 1 ELSE 0 END AS IsIncluded,
                CASE WHEN ic.COLORDER = 'D' THEN 1 ELSE 0 END AS IsDescending
            FROM SYSCAT.INDEXES i
            INNER JOIN SYSCAT.INDEXCOLUSE ic ON ic.INDSCHEMA = i.INDSCHEMA AND ic.INDNAME = i.INDNAME
            WHERE i.TABSCHEMA = :SchemaName AND i.TABNAME = :TableName
                AND i.SYSTEM_REQUIRED = 0 AND i.INDEXTYPE IN ('REG', 'CLUS')
            ORDER BY i.INDNAME, ic.COLSEQ";

        internal const string ForeignKeysSql = @"SELECT f.CONSTNAME AS ForeignKeyName,
                fc.COLNAME AS ColumnName,
                CASE WHEN f.REFTABSCHEMA = " + CurrentSchema + @" THEN NULL ELSE f.REFTABSCHEMA END AS ReferencedSchema,
                f.REFTABNAME AS ReferencedTable,
                rc.COLNAME AS ReferencedColumn,
                CASE f.UPDATERULE WHEN 'R' THEN 'RESTRICT' ELSE 'NO ACTION' END AS UpdateAction,
                CASE f.DELETERULE WHEN 'C' THEN 'CASCADE' WHEN 'N' THEN 'SET NULL' WHEN 'R' THEN 'RESTRICT' ELSE 'NO ACTION' END AS DeleteAction
            FROM SYSCAT.REFERENCES f
            INNER JOIN SYSCAT.KEYCOLUSE fc ON fc.TABSCHEMA = f.TABSCHEMA AND fc.TABNAME = f.TABNAME AND fc.CONSTNAME = f.CONSTNAME
            INNER JOIN SYSCAT.KEYCOLUSE rc ON rc.TABSCHEMA = f.REFTABSCHEMA AND rc.TABNAME = f.REFTABNAME AND rc.CONSTNAME = f.REFKEYNAME AND rc.COLSEQ = fc.COLSEQ
            WHERE f.TABSCHEMA = :SchemaName AND f.TABNAME = :TableName
            ORDER BY f.CONSTNAME, fc.COLSEQ";

        internal const string CheckConstraintsSql = @"SELECT k.CONSTNAME AS ConstraintName,
                k.TEXT AS Definition
            FROM SYSCAT.CHECKS k
            WHERE k.TABSCHEMA = :SchemaName AND k.TABNAME = :TableName AND k.TYPE = 'C'
            ORDER BY k.CONSTNAME";

        internal const string TablesSql = @"SELECT CASE WHEN t.TABSCHEMA = " + CurrentSchema + @" THEN NULL ELSE t.TABSCHEMA END AS SchemaName,
                t.TABNAME AS TableName
            FROM SYSCAT.TABLES t
            WHERE t.TABSCHEMA = :SchemaName AND t.TYPE = 'T'
            ORDER BY t.TABSCHEMA, t.TABNAME";

        internal const string ForeignKeyRelationshipsSql = @"SELECT DISTINCT CASE WHEN f.TABSCHEMA = " + CurrentSchema + @" THEN NULL ELSE f.TABSCHEMA END AS ChildSchema,
                f.TABNAME AS ChildTable,
                CASE WHEN f.REFTABSCHEMA = " + CurrentSchema + @" THEN NULL ELSE f.REFTABSCHEMA END AS ParentSchema,
                f.REFTABNAME AS ParentTable
            FROM SYSCAT.REFERENCES f
            WHERE f.TABSCHEMA NOT LIKE 'SYS%' AND f.REFTABSCHEMA NOT LIKE 'SYS%'";

        #endregion
    }
}
