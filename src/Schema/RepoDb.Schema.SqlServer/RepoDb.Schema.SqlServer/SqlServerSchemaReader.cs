#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Resolvers;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An <see cref="ISchemaReader"/> that reads the schema of the tables of a SQL Server database from its catalog views (<c>sys.*</c>).
    /// The connection (and the optional transaction) that is passed to the constructor is used by all the operations.
    /// </summary>
    public class SqlServerSchemaReader : ISchemaReader
    {
        #region Private Variables

        private const string DefaultSchema = "dbo";

        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly SqlServerDbTypeNameToClientTypeResolver _typeResolver = new SqlServerDbTypeNameToClientTypeResolver();

        private const string ResolveSchemaSql = @"SELECT TOP 1 SCHEMA_NAME(t.schema_id)
            FROM sys.tables t
            WHERE t.name = @TableName
            ORDER BY CASE WHEN t.schema_id = SCHEMA_ID() THEN 0 ELSE 1 END;";

        private const string TableExistsSql = "SELECT CASE WHEN OBJECT_ID(@FullName, N'U') IS NULL THEN 0 ELSE 1 END;";

        private const string ColumnsSql = @"SELECT c.column_id AS Ordinal,
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

        private const string KeyConstraintSql = @"SELECT kc.name AS ConstraintName,
                col.name AS ColumnName
            FROM sys.key_constraints kc
            INNER JOIN sys.index_columns ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
            INNER JOIN sys.columns col ON col.object_id = ic.object_id AND col.column_id = ic.column_id
            WHERE kc.parent_object_id = OBJECT_ID(@FullName) AND kc.type = @Type
            ORDER BY kc.name, ic.key_ordinal;";

        private const string IndexesSql = @"SELECT i.name AS IndexName,
                i.is_unique AS IsUnique,
                col.name AS ColumnName,
                ic.is_included_column AS IsIncluded
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
            INNER JOIN sys.columns col ON col.object_id = ic.object_id AND col.column_id = ic.column_id
            WHERE i.object_id = OBJECT_ID(@FullName)
                AND i.is_primary_key = 0
                AND i.is_unique_constraint = 0
                AND i.type > 0
                AND i.name IS NOT NULL
            ORDER BY i.name, ic.is_included_column, ic.key_ordinal, ic.index_column_id;";

        private const string ForeignKeysSql = @"SELECT fk.name AS ForeignKeyName,
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

        private const string CheckConstraintsSql = @"SELECT cc.name AS ConstraintName,
                cc.definition AS Definition
            FROM sys.check_constraints cc
            WHERE cc.parent_object_id = OBJECT_ID(@FullName)
            ORDER BY cc.name;";

        private const string TablesSql = @"SELECT SCHEMA_NAME(t.schema_id) AS SchemaName,
                t.name AS TableName
            FROM sys.tables t
            WHERE @SchemaName IS NULL OR SCHEMA_NAME(t.schema_id) = @SchemaName
            ORDER BY SCHEMA_NAME(t.schema_id), t.name;";

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SqlServerSchemaReader"/> class.
        /// </summary>
        /// <param name="connection">The connection to the SQL Server database to be read.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        public SqlServerSchemaReader(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _transaction = transaction;
        }

        #endregion

        #region Public Methods

        #region Sync

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        public TableSchema GetTableSchema(string tableName)
        {
            var (schema, table) = Resolve(tableName);
            var fullName = FullName(schema, table);

            return new TableSchema
            {
                TableName = table,
                SchemaName = schema,
                Columns = Query(ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, FullNameParameter(fullName)),
                PrimaryKey = MapPrimaryKey(Query(KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, FullNameParameter(fullName), Parameter("Type", "PK"))),
                Indexes = MapIndexes(Query(IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, FullNameParameter(fullName))),
                ForeignKeys = MapForeignKeys(Query(ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, FullNameParameter(fullName))),
                UniqueConstraints = MapUniqueConstraints(Query(KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, FullNameParameter(fullName), Parameter("Type", "UQ"))),
                CheckConstraints = Query(CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, FullNameParameter(fullName))
            };
        }

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public bool TableExists(string tableName)
        {
            var (schema, table) = ParseTableName(tableName);
            if (schema == null)
            {
                schema = ResolveSchemaName(table) ?? DefaultSchema;
            }
            return Query(TableExistsSql, SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), FullNameParameter(FullName(schema, table))).FirstOrDefault() == 1;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
        public string GetSchemaName(string tableName) =>
            Resolve(tableName).Schema;

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ColumnInfo"/> objects of the table.</returns>
        public IEnumerable<ColumnInfo> GetColumns(string tableName) =>
            Query(ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, FullNameParameter(Resolve(tableName)));

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public PrimaryKeyInfo GetPrimaryKey(string tableName) =>
            MapPrimaryKey(Query(KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, FullNameParameter(Resolve(tableName)), Parameter("Type", "PK")));

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="IndexInfo"/> objects of the table.</returns>
        public IEnumerable<IndexInfo> GetIndexes(string tableName) =>
            MapIndexes(Query(IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public IEnumerable<ForeignKeyInfo> GetForeignKeys(string tableName) =>
            MapForeignKeys(Query(ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<UniqueConstraintInfo> GetUniqueConstraints(string tableName) =>
            MapUniqueConstraints(Query(KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, FullNameParameter(Resolve(tableName)), Parameter("Type", "UQ")));

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<CheckConstraintInfo> GetCheckConstraints(string tableName) =>
            Query(CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, FullNameParameter(Resolve(tableName)));

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <returns>The names of the tables.</returns>
        public IEnumerable<string> GetTables(string schemaName = null) =>
            Query(TablesSql, SchemaTraceKeys.GetTables, r => $"{r.GetString(0)}.{r.GetString(1)}", Parameter("SchemaName", schemaName));

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that its foreign keys reference. Use the order to create the tables, and the reverse of it to drop them.
        /// </summary>
        /// <param name="tableNames">The names of the tables to be ordered.</param>
        /// <returns>The ordered names of the tables.</returns>
        public IEnumerable<string> GetDependencyOrder(IEnumerable<string> tableNames)
        {
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            var foreignKeys = names.ToDictionary(Key, n => (IList<ForeignKeyInfo>)GetForeignKeys(n).ToList());
            return Order(names, foreignKeys);
        }

        #endregion

        #region Async

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="TableSchema"/> of the table.</returns>
        public async Task<TableSchema> GetTableSchemaAsync(string tableName,
            CancellationToken cancellationToken = default)
        {
            var (schema, table) = await ResolveAsync(tableName, cancellationToken).ConfigureAwait(false);
            var fullName = FullName(schema, table);

            return new TableSchema
            {
                TableName = table,
                SchemaName = schema,
                Columns = await QueryAsync(ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false),
                PrimaryKey = MapPrimaryKey(await QueryAsync(KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, cancellationToken, FullNameParameter(fullName), Parameter("Type", "PK")).ConfigureAwait(false)),
                Indexes = MapIndexes(await QueryAsync(IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)),
                ForeignKeys = MapForeignKeys(await QueryAsync(ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)),
                UniqueConstraints = MapUniqueConstraints(await QueryAsync(KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, cancellationToken, FullNameParameter(fullName), Parameter("Type", "UQ")).ConfigureAwait(false)),
                CheckConstraints = await QueryAsync(CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)
            };
        }

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: <c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> TableExistsAsync(string tableName,
            CancellationToken cancellationToken = default)
        {
            var (schema, table) = ParseTableName(tableName);
            if (schema == null)
            {
                schema = await ResolveSchemaNameAsync(table, cancellationToken).ConfigureAwait(false) ?? DefaultSchema;
            }
            var result = await QueryAsync(TableExistsSql, SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), cancellationToken, FullNameParameter(FullName(schema, table))).ConfigureAwait(false);
            return result.FirstOrDefault() == 1;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
        public async Task<string> GetSchemaNameAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            (await ResolveAsync(tableName, cancellationToken).ConfigureAwait(false)).Schema;

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ColumnInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ColumnInfo>> GetColumnsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            await QueryAsync(ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public async Task<PrimaryKeyInfo> GetPrimaryKeyAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapPrimaryKey(await QueryAsync(KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false)), Parameter("Type", "PK")).ConfigureAwait(false));

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="IndexInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<IndexInfo>> GetIndexesAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapIndexes(await QueryAsync(IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ForeignKeyInfo>> GetForeignKeysAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapForeignKeys(await QueryAsync(ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<UniqueConstraintInfo>> GetUniqueConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapUniqueConstraints(await QueryAsync(KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false)), Parameter("Type", "UQ")).ConfigureAwait(false));

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<CheckConstraintInfo>> GetCheckConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            await QueryAsync(CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the tables.</returns>
        public async Task<IEnumerable<string>> GetTablesAsync(string schemaName = null,
            CancellationToken cancellationToken = default) =>
            await QueryAsync(TablesSql, SchemaTraceKeys.GetTables, r => $"{r.GetString(0)}.{r.GetString(1)}", cancellationToken, Parameter("SchemaName", schemaName)).ConfigureAwait(false);

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that its foreign keys reference. Use the order to create the tables, and the reverse of it to drop them.
        /// </summary>
        /// <param name="tableNames">The names of the tables to be ordered.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the ordered names of the tables.</returns>
        public async Task<IEnumerable<string>> GetDependencyOrderAsync(IEnumerable<string> tableNames,
            CancellationToken cancellationToken = default)
        {
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            var foreignKeys = new Dictionary<string, IList<ForeignKeyInfo>>();
            foreach (var name in names)
            {
                foreignKeys[Key(name)] = (await GetForeignKeysAsync(name, cancellationToken).ConfigureAwait(false)).ToList();
            }
            return Order(names, foreignKeys);
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static (string Schema, string Table) ParseTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            var parts = tableName.Split('.')
                .Select(p => p.Trim().Trim('[', ']', '"'))
                .Where(p => p.Length > 0)
                .ToArray();
            return parts.Length >= 2
                ? (parts[parts.Length - 2], parts[parts.Length - 1])
                : (null, parts[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string FullName(string schema, string table) =>
            $"[{schema.Replace("]", "]]")}].[{table.Replace("]", "]]")}]";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static string Key(string tableName)
        {
            var (schema, table) = ParseTableName(tableName);
            return $"{schema ?? DefaultSchema}.{table}".ToLowerInvariant();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private (string Schema, string Table) Resolve(string tableName)
        {
            var (schema, table) = ParseTableName(tableName);
            return (schema ?? ResolveSchemaName(table) ?? DefaultSchema, table);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<(string Schema, string Table)> ResolveAsync(string tableName,
            CancellationToken cancellationToken)
        {
            var (schema, table) = ParseTableName(tableName);
            return (schema ?? await ResolveSchemaNameAsync(table, cancellationToken).ConfigureAwait(false) ?? DefaultSchema, table);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> ResolveFullNameAsync(string tableName,
            CancellationToken cancellationToken)
        {
            var (schema, table) = await ResolveAsync(tableName, cancellationToken).ConfigureAwait(false);
            return FullName(schema, table);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private string ResolveSchemaName(string table) =>
            Query(ResolveSchemaSql, SchemaTraceKeys.ResolveSchemaName, r => r.GetString(0), Parameter("TableName", table)).FirstOrDefault();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> ResolveSchemaNameAsync(string table,
            CancellationToken cancellationToken) =>
            (await QueryAsync(ResolveSchemaSql, SchemaTraceKeys.ResolveSchemaName, r => r.GetString(0), cancellationToken, Parameter("TableName", table)).ConfigureAwait(false)).FirstOrDefault();

        // Parameters

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static KeyValuePair<string, object> Parameter(string name, object value) =>
            new KeyValuePair<string, object>(name, value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private static KeyValuePair<string, object> FullNameParameter(string fullName) =>
            Parameter("FullName", fullName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static KeyValuePair<string, object> FullNameParameter((string Schema, string Table) name) =>
            FullNameParameter(FullName(name.Schema, name.Table));

        // Execution

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static IDictionary<string, object> ToParameters(KeyValuePair<string, object>[] parameters) =>
            parameters.ToDictionary(p => p.Key, p => p.Value);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="traceKey"></param>
        /// <param name="map"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IList<T> Query<T>(string sql,
            string traceKey,
            Func<IDataRecord, T> map,
            params KeyValuePair<string, object>[] parameters)
        {
            using (var reader = _connection.ExecuteReader(sql,
                param: ToParameters(parameters),
                traceKey: traceKey,
                transaction: _transaction))
            {
                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(map(reader));
                }
                return list;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="traceKey"></param>
        /// <param name="map"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<IList<T>> QueryAsync<T>(string sql,
            string traceKey,
            Func<IDataRecord, T> map,
            CancellationToken cancellationToken,
            params KeyValuePair<string, object>[] parameters)
        {
            using (var reader = await _connection.ExecuteReaderAsync(sql,
                param: ToParameters(parameters),
                traceKey: traceKey,
                transaction: _transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                var list = new List<T>();
                if (reader is DbDataReader dbReader)
                {
                    while (await dbReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        list.Add(map(dbReader));
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        list.Add(map(reader));
                    }
                }
                return list;
            }
        }

        // Row mappers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Text(IDataRecord r, string name)
        {
            var value = r[name];
            return value == DBNull.Value ? null : Convert.ToString(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static int Int(IDataRecord r, string name)
        {
            var value = r[name];
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static long? NullableLong(IDataRecord r, string name)
        {
            var value = r[name];
            return value == DBNull.Value ? (long?)null : Convert.ToInt64(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool Flag(IDataRecord r, string name)
        {
            var value = r[name];
            return value != DBNull.Value && Convert.ToBoolean(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private ColumnInfo MapColumn(IDataRecord r)
        {
            var typeName = Text(r, "TypeName");
            var maxLength = Int(r, "MaxLength");

            // The max length is in bytes (-1 is MAX), so it is halved for the unicode types to get the number of characters
            var size = maxLength > 0 && (typeName == "nchar" || typeName == "nvarchar")
                ? maxLength / 2
                : maxLength;

            var field = new DbField(Text(r, "Name"),
                Flag(r, "IsPrimary"),
                Flag(r, "IsIdentity"),
                Flag(r, "IsNullable"),
                _typeResolver.Resolve(typeName) ?? typeof(object),
                size,
                (byte)Int(r, "Precision"),
                (byte)Int(r, "Scale"),
                typeName,
                Text(r, "DefaultDefinition") != null,
                "SqlServer");

            return new ColumnInfo
            {
                Field = field,
                Ordinal = Int(r, "Ordinal"),
                DefaultExpression = Text(r, "DefaultDefinition"),
                IdentitySeed = NullableLong(r, "IdentitySeed"),
                IdentityIncrement = NullableLong(r, "IdentityIncrement"),
                ComputedExpression = Text(r, "ComputedDefinition"),
                Collation = Text(r, "Collation"),
                Comment = Text(r, "Comment")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static KeyValuePair<string, string> MapKeyColumn(IDataRecord r) =>
            new KeyValuePair<string, string>(Text(r, "ConstraintName"), Text(r, "ColumnName"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static PrimaryKeyInfo MapPrimaryKey(IEnumerable<KeyValuePair<string, string>> rows)
        {
            var list = rows.ToList();
            return list.Count == 0
                ? null
                : new PrimaryKeyInfo { Name = list[0].Key, Columns = list.Select(x => x.Value).ToList() };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static IList<UniqueConstraintInfo> MapUniqueConstraints(IEnumerable<KeyValuePair<string, string>> rows) =>
            rows.GroupBy(x => x.Key)
                .Select(g => new UniqueConstraintInfo { Name = g.Key, Columns = g.Select(x => x.Value).ToList() })
                .ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static (string Name, bool IsUnique, string Column, bool IsIncluded) MapIndexColumn(IDataRecord r) =>
            (Text(r, "IndexName"), Flag(r, "IsUnique"), Text(r, "ColumnName"), Flag(r, "IsIncluded"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static IList<IndexInfo> MapIndexes(IEnumerable<(string Name, bool IsUnique, string Column, bool IsIncluded)> rows) =>
            rows.GroupBy(x => x.Name)
                .Select(g => new IndexInfo
                {
                    Name = g.Key,
                    IsUnique = g.First().IsUnique,
                    Columns = g.Where(x => !x.IsIncluded).Select(x => x.Column).ToList(),
                    IncludedColumns = g.Where(x => x.IsIncluded).Select(x => x.Column).ToList()
                })
                .ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static (string Name, string Column, string RefSchema, string RefTable, string RefColumn, int Update, int Delete) MapForeignKeyColumn(IDataRecord r) =>
            (Text(r, "ForeignKeyName"), Text(r, "ColumnName"), Text(r, "ReferencedSchema"), Text(r, "ReferencedTable"), Text(r, "ReferencedColumn"), Int(r, "UpdateAction"), Int(r, "DeleteAction"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static IList<ForeignKeyInfo> MapForeignKeys(IEnumerable<(string Name, string Column, string RefSchema, string RefTable, string RefColumn, int Update, int Delete)> rows) =>
            rows.GroupBy(x => x.Name)
                .Select(g => new ForeignKeyInfo
                {
                    Name = g.Key,
                    Columns = g.Select(x => x.Column).ToList(),
                    ReferencedTable = $"{g.First().RefSchema}.{g.First().RefTable}",
                    ReferencedColumns = g.Select(x => x.RefColumn).ToList(),
                    UpdateRule = ToRule(g.First().Update),
                    DeleteRule = ToRule(g.First().Delete)
                })
                .ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static ForeignKeyRule ToRule(int action)
        {
            switch (action)
            {
                case 1: return ForeignKeyRule.Cascade;
                case 2: return ForeignKeyRule.SetNull;
                case 3: return ForeignKeyRule.SetDefault;
                default: return ForeignKeyRule.NoAction;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static CheckConstraintInfo MapCheckConstraint(IDataRecord r) =>
            new CheckConstraintInfo { Name = Text(r, "ConstraintName"), Expression = Text(r, "Definition") };

        // Ordering

        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <param name="foreignKeys"></param>
        /// <returns></returns>
        private static IEnumerable<string> Order(IList<string> names,
            IDictionary<string, IList<ForeignKeyInfo>> foreignKeys)
        {
            var byKey = new Dictionary<string, string>();
            foreach (var name in names)
            {
                byKey[Key(name)] = name;
            }

            // Every table depends on the (other) tables its foreign keys reference
            var pending = byKey.Keys.ToDictionary(
                k => k,
                k => new HashSet<string>(foreignKeys[k]
                    .Select(fk => Key(fk.ReferencedTable))
                    .Where(d => d != k && byKey.ContainsKey(d))));

            var ordered = new List<string>();
            while (pending.Count > 0)
            {
                var ready = pending.Where(p => p.Value.Count == 0).Select(p => p.Key).ToList();
                if (ready.Count == 0)
                {
                    // A cycle: keep the remaining tables in their given order
                    ready = pending.Keys.ToList();
                }
                foreach (var key in ready)
                {
                    ordered.Add(byKey[key]);
                    pending.Remove(key);
                }
                foreach (var dependencies in pending.Values)
                {
                    dependencies.ExceptWith(ready);
                }
            }
            return ordered;
        }

        #endregion
    }
}
