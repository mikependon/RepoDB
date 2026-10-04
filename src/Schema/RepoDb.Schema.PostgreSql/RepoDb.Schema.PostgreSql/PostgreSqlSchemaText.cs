#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL texts that are used by the <see cref="PostgreSqlSchemaReader"/> to read the schema from the PostgreSQL catalogs.
    /// </summary>
    internal static class PostgreSqlSchemaText
    {
        internal const string ResolveSchemaSql = @"SELECT n.nspname
            FROM pg_class c
            INNER JOIN pg_namespace n ON n.oid = c.relnamespace
            WHERE c.relname = @TableName AND c.relkind IN ('r', 'p')
            ORDER BY (n.nspname = current_schema()) DESC, n.nspname
            LIMIT 1;";

        internal const string TableExistsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM pg_class
                WHERE oid = to_regclass(@FullName) AND relkind IN ('r', 'p')) THEN 1 ELSE 0 END;";

        internal const string ColumnsSql = @"SELECT a.attnum AS Ordinal,
                a.attname AS Name,
                format_type(a.atttypid, NULL) AS TypeName,
                a.atttypmod AS TypeModifier,
                NOT a.attnotnull AS IsNullable,
                a.attidentity <> '' AS IsIdentity,
                CASE WHEN a.attgenerated = '' THEN pg_get_expr(d.adbin, d.adrelid) END AS DefaultDefinition,
                CASE WHEN a.attgenerated <> '' THEN pg_get_expr(d.adbin, d.adrelid) END AS ComputedDefinition,
                s.seqstart AS IdentitySeed,
                s.seqincrement AS IdentityIncrement,
                col_description(a.attrelid, a.attnum) AS Comment,
                EXISTS (SELECT 1 FROM pg_index i WHERE i.indrelid = a.attrelid AND i.indisprimary AND a.attnum = ANY (i.indkey)) AS IsPrimary
            FROM pg_attribute a
            LEFT JOIN pg_attrdef d ON d.adrelid = a.attrelid AND d.adnum = a.attnum
            LEFT JOIN pg_sequence s ON s.seqrelid = NULLIF(pg_get_serial_sequence(@FullName, a.attname), '')::regclass
            WHERE a.attrelid = to_regclass(@FullName) AND a.attnum > 0 AND NOT a.attisdropped
            ORDER BY a.attnum;";

        internal const string KeyConstraintSql = @"SELECT c.conname AS ConstraintName,
                a.attname AS ColumnName
            FROM pg_constraint c
            CROSS JOIN LATERAL unnest(c.conkey) WITH ORDINALITY AS k(attnum, ord)
            INNER JOIN pg_attribute a ON a.attrelid = c.conrelid AND a.attnum = k.attnum
            WHERE c.conrelid = to_regclass(@FullName) AND c.contype::text = @Type
            ORDER BY c.conname, k.ord;";

        internal const string IndexesSql = @"SELECT ic.relname AS IndexName,
                i.indisunique AS IsUnique,
                pg_get_expr(i.indpred, i.indrelid) AS FilterDefinition,
                a.attname AS ColumnName,
                k.ord > i.indnkeyatts AS IsIncluded,
                (k.opt & 1) = 1 AS IsDescending
            FROM pg_index i
            INNER JOIN pg_class ic ON ic.oid = i.indexrelid
            INNER JOIN pg_am am ON am.oid = ic.relam AND am.amname = 'btree'
            CROSS JOIN LATERAL unnest(i.indkey::int2[], i.indoption::int2[]) WITH ORDINALITY AS k(attnum, opt, ord)
            INNER JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = k.attnum
            WHERE i.indrelid = to_regclass(@FullName)
                AND NOT i.indisprimary
                AND 0 <> ALL (i.indkey::int2[])
                AND NOT EXISTS (SELECT 1 FROM pg_constraint c WHERE c.conindid = i.indexrelid AND c.contype IN ('p', 'u', 'x'))
            ORDER BY ic.relname, k.ord;";

        internal const string ForeignKeysSql = @"SELECT c.conname AS ForeignKeyName,
                a.attname AS ColumnName,
                rn.nspname AS ReferencedSchema,
                rt.relname AS ReferencedTable,
                ra.attname AS ReferencedColumn,
                c.confupdtype::text AS UpdateAction,
                c.confdeltype::text AS DeleteAction
            FROM pg_constraint c
            CROSS JOIN LATERAL unnest(c.conkey, c.confkey) WITH ORDINALITY AS k(attnum, refattnum, ord)
            INNER JOIN pg_attribute a ON a.attrelid = c.conrelid AND a.attnum = k.attnum
            INNER JOIN pg_class rt ON rt.oid = c.confrelid
            INNER JOIN pg_namespace rn ON rn.oid = rt.relnamespace
            INNER JOIN pg_attribute ra ON ra.attrelid = c.confrelid AND ra.attnum = k.refattnum
            WHERE c.conrelid = to_regclass(@FullName) AND c.contype = 'f'
            ORDER BY c.conname, k.ord;";

        internal const string CheckConstraintsSql = @"SELECT c.conname AS ConstraintName,
                pg_get_expr(c.conbin, c.conrelid) AS Definition
            FROM pg_constraint c
            WHERE c.conrelid = to_regclass(@FullName) AND c.contype = 'c'
            ORDER BY c.conname;";

        internal const string TablesSql = @"SELECT n.nspname AS SchemaName,
                c.relname AS TableName
            FROM pg_class c
            INNER JOIN pg_namespace n ON n.oid = c.relnamespace
            WHERE c.relkind IN ('r', 'p')
                AND n.nspname NOT IN ('pg_catalog', 'information_schema')
                AND n.nspname NOT LIKE 'pg\_toast%'
                AND (CAST(@SchemaName AS text) IS NULL OR n.nspname = @SchemaName)
            ORDER BY n.nspname, c.relname;";
    }
}
