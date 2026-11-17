#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System.Diagnostics.CodeAnalysis;
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
    /// Reads Ahtola catalog metadata using operation connections.
    /// </summary>
    public class AhtolaSchemaReader : ISchemaReader
    {
        #region Private Variables

        private readonly AhtolaDbTypeNameToClientTypeResolver _typeResolver = new AhtolaDbTypeNameToClientTypeResolver();
        #endregion

        #region Public Methods

        #region Sync

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        public TableSchema GetTableSchema(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var (schema, table) = Resolve(tableName);
            return Build(connection,
                ReadTable(connection,
                    transaction,
                    schema,
                    table));
        }

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public bool TableExists(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var (schema, table) = Resolve(tableName);
            return Query(connection,
                transaction,
                AhtolaSchemaText.TableExistsSql(Schema(schema)),
                SchemaTraceKeys.TableExists,
                r => Convert.ToInt32(r[0]),
                Parameter("Table",
                    table)).FirstOrDefault() > 0;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The name of the owning schema, or <c>null</c> if the table is in the main database.</returns>
        public string GetSchemaName(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return Resolve(tableName).Schema;
        }

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="ColumnInfo"/> objects of the table.</returns>
        public IEnumerable<ColumnInfo> GetColumns(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return GetTableSchema(connection,
                tableName,
                transaction: transaction).Columns;
        }

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public PrimaryKeyInfo GetPrimaryKey(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return GetTableSchema(connection,
                tableName,
                transaction: transaction).PrimaryKey;
        }

        /// <summary>
        /// Gets the indexes of the table, except the ones that back the primary key and the unique constraints.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="IndexInfo"/> objects of the table.</returns>
        public IEnumerable<IndexInfo> GetIndexes(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return GetTableSchema(connection,
                tableName,
                transaction: transaction).Indexes;
        }

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return GetTableSchema(connection,
                tableName,
                transaction: transaction).ForeignKeys;
        }

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<UniqueConstraintInfo> GetUniqueConstraints(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return GetTableSchema(connection,
                tableName,
                transaction: transaction).UniqueConstraints;
        }

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<CheckConstraintInfo> GetCheckConstraints(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return GetTableSchema(connection,
                tableName,
                transaction: transaction).CheckConstraints;
        }

        /// <summary>
        /// Reads the database schemas.
        /// </summary>
        public IEnumerable<SchemaInfo> GetSchemas(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return Query(connection,
                transaction,
                AhtolaSchemaText.DatabasesSql,
                SchemaTraceKeys.GetSchemas,
                r => new SchemaInfo(Text(r,
                        "name")));
        }

        /// <summary>
        /// Gets the names of the tables of a schema (the tables of the engine are not included).
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="schemaName">The name of the schema (an attached database) to be read. The default is <c>null</c>, which reads the main database.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The names of the tables.</returns>
        public IEnumerable<string> GetTables(IDbConnection connection,
            string schemaName = null,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var schema = NormalizeSchema(schemaName);
            if (schema != null && !Query(connection,
                transaction,
                AhtolaSchemaText.DatabasesSql,
                SchemaTraceKeys.GetTables,
                r => Text(r,
                    "name")).Contains(schema,
                StringComparer.OrdinalIgnoreCase))
            {
                return new List<string>();
            }

            return Query(connection,
                transaction,
                AhtolaSchemaText.TablesSql(Schema(schema)),
                SchemaTraceKeys.GetTables,
                r => AhtolaSchemaHelper.FormatTableName(schema,
                    Text(r,
                        "TableName")));
        }

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that its foreign keys reference. Use the order to create the tables, and the reverse of it to drop them.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables to be ordered.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The ordered names of the tables.</returns>
        public IEnumerable<RelationshipInfo> GetDependencyOrder(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            return Order(names.Select((value0) => GetTableSchema(connection,
                        value0,
                        transaction: transaction)).ToList());
        }

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="RelatedTableBehavior.TableOnly"/> returns only the given tables.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        public IEnumerable<string> GetRelatedTables(IDbConnection connection,
            IEnumerable<string> tableNames,
            RelatedTableBehavior relationshipBehavior,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var tables = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).Select(name =>
            {
                var (schema, table) = Resolve(name);
                return NewTable(connection,
                        table,
                        schema);
            }).ToList();
            var foreignKeys = new List<(TableInfo Child, TableInfo Parent)>();
            if (relationshipBehavior != RelatedTableBehavior.TableOnly)
            {
                foreach (var schema in Query(connection,
                    transaction,
                    AhtolaSchemaText.DatabasesSql,
                    SchemaTraceKeys.GetTables,
                    r => NormalizeSchema(Text(r,
                            "name"))))
                {
                    foreignKeys.AddRange(Query(connection,
                            transaction,
                            AhtolaSchemaText.RelationshipsSql(Schema(schema),
                                schema ?? AhtolaSchemaHelper.MainSchema),
                            SchemaTraceKeys.GetRelationships,
                            r => (NewTable(connection,
                                Text(r,
                                    "ChildTable"),
                                schema), NewTable(connection,
                                Text(r,
                                    "ParentTable"),
                                schema))));
                }
            }

            return RelatedTableExpander.Expand(tables,
                foreignKeys,
                relationshipBehavior,
                Key).Select(table => AhtolaSchemaHelper.FormatTableName(table.Schema,
                    table.Name)).ToList();
        }

        #endregion

        #region Async

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="TableSchema"/> of the table.</returns>
        public async Task<TableSchema> GetTableSchemaAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var (schema, table) = Resolve(tableName);
            return Build(connection,
                await ReadTableAsync(connection,
                    transaction,
                    schema,
                    table,
                    cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: <c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> TableExistsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var (schema, table) = Resolve(tableName);
            var result = await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.TableExistsSql(Schema(schema)),
                SchemaTraceKeys.TableExists,
                r => Convert.ToInt32(r[0]),
                cancellationToken,
                Parameter("Table",
                    table)).ConfigureAwait(false);
            return result.FirstOrDefault() > 0;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the name of the owning schema, or <c>null</c> if the table is in the main database.</returns>
        public Task<string> GetSchemaNameAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return Task.FromResult(Resolve(tableName).Schema);
        }

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ColumnInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ColumnInfo>> GetColumnsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return (await GetTableSchemaAsync(connection,
                tableName,
                cancellationToken: cancellationToken,
                transaction: transaction).ConfigureAwait(false)).Columns;
        }

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public async Task<PrimaryKeyInfo> GetPrimaryKeyAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return (await GetTableSchemaAsync(connection,
                tableName,
                cancellationToken: cancellationToken,
                transaction: transaction).ConfigureAwait(false)).PrimaryKey;
        }

        /// <summary>
        /// Gets the indexes of the table, except the ones that back the primary key and the unique constraints.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="IndexInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<IndexInfo>> GetIndexesAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return (await GetTableSchemaAsync(connection,
                tableName,
                cancellationToken: cancellationToken,
                transaction: transaction).ConfigureAwait(false)).Indexes;
        }

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ForeignKeyInfo>> GetForeignKeysAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return (await GetTableSchemaAsync(connection,
                tableName,
                cancellationToken: cancellationToken,
                transaction: transaction).ConfigureAwait(false)).ForeignKeys;
        }

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<UniqueConstraintInfo>> GetUniqueConstraintsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return (await GetTableSchemaAsync(connection,
                tableName,
                cancellationToken: cancellationToken,
                transaction: transaction).ConfigureAwait(false)).UniqueConstraints;
        }

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<CheckConstraintInfo>> GetCheckConstraintsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return (await GetTableSchemaAsync(connection,
                tableName,
                cancellationToken: cancellationToken,
                transaction: transaction).ConfigureAwait(false)).CheckConstraints;
        }

        /// <summary>
        /// Reads the database schemas.
        /// </summary>
        public async Task<IEnumerable<SchemaInfo>> GetSchemasAsync(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.DatabasesSql,
                SchemaTraceKeys.GetSchemas,
                r => new SchemaInfo(Text(r,
                        "name")),
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the names of the tables of a schema (the tables of the engine are not included).
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="schemaName">The name of the schema (an attached database) to be read. The default is <c>null</c>, which reads the main database.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the tables.</returns>
        public async Task<IEnumerable<string>> GetTablesAsync(IDbConnection connection,
            string schemaName = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var schema = NormalizeSchema(schemaName);
            if (schema != null)
            {
                var schemas = await QueryAsync(connection,
                    transaction,
                    AhtolaSchemaText.DatabasesSql,
                    SchemaTraceKeys.GetTables,
                    r => Text(r,
                        "name"),
                    cancellationToken).ConfigureAwait(false);
                if (!schemas.Contains(schema,
                    StringComparer.OrdinalIgnoreCase))
                {
                    return new List<string>();
                }
            }

            return await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.TablesSql(Schema(schema)),
                SchemaTraceKeys.GetTables,
                r => AhtolaSchemaHelper.FormatTableName(schema,
                    Text(r,
                        "TableName")),
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that its foreign keys reference. Use the order to create the tables, and the reverse of it to drop them.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables to be ordered.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the ordered names of the tables.</returns>
        public async Task<IEnumerable<RelationshipInfo>> GetDependencyOrderAsync(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            var schemas = new List<TableSchema>();
            foreach (var name in names)
            {
                schemas.Add(await GetTableSchemaAsync(connection,
                        name,
                        cancellationToken: cancellationToken,
                        transaction: transaction).ConfigureAwait(false));
            }

            return Order(schemas);
        }

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="RelatedTableBehavior.TableOnly"/> returns only the given tables.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        public async Task<IEnumerable<string>> GetRelatedTablesAsync(IDbConnection connection,
            IEnumerable<string> tableNames,
            RelatedTableBehavior relationshipBehavior,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            var tables = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).Select(name =>
            {
                var (schema, table) = Resolve(name);
                return NewTable(connection,
                        table,
                        schema);
            }).ToList();
            var foreignKeys = new List<(TableInfo Child, TableInfo Parent)>();
            if (relationshipBehavior != RelatedTableBehavior.TableOnly)
            {
                var schemas = await QueryAsync(connection,
                    transaction,
                    AhtolaSchemaText.DatabasesSql,
                    SchemaTraceKeys.GetTables,
                    r => NormalizeSchema(Text(r,
                            "name")),
                    cancellationToken).ConfigureAwait(false);
                foreach (var schema in schemas)
                {
                    var rows = await QueryAsync(connection,
                        transaction,
                        AhtolaSchemaText.RelationshipsSql(Schema(schema),
                            schema ?? AhtolaSchemaHelper.MainSchema),
                        SchemaTraceKeys.GetRelationships,
                        r => (NewTable(connection,
                            Text(r,
                                "ChildTable"),
                            schema), NewTable(connection,
                            Text(r,
                                "ParentTable"),
                            schema)),
                        cancellationToken).ConfigureAwait(false);
                    foreignKeys.AddRange(rows);
                }
            }

            return RelatedTableExpander.Expand(tables,
                foreignKeys,
                relationshipBehavior,
                Key).Select(table => AhtolaSchemaHelper.FormatTableName(table.Schema,
                    table.Name)).ToList();
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the provider of the operation connection.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        private static string GetProvider(IDbConnection connection) => DbSettingMapper.Get(connection)?.Provider;

        /// <summary>
        /// Maps the catalog rows.
        /// </summary>
        /// <typeparam name="T">The mapped type.</typeparam>
        /// <param name="reader">The catalog reader.</param>
        /// <param name="map">The record mapper.</param>
        /// <returns>The mapped rows.</returns>
        private static IList<T> ReadRows<T>(IDataReader reader,
            Func<IDataRecord, T> map)
        {
            var rows = new List<T>();
            while (reader.Read())
                rows.Add(map(reader));
            return rows;
        }

        /// <summary>
        /// Maps the catalog rows asynchronously.
        /// </summary>
        /// <typeparam name="T">The mapped type.</typeparam>
        /// <param name="reader">The catalog reader.</param>
        /// <param name="map">The record mapper.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The mapped rows.</returns>
        private static async Task<IList<T>> ReadRowsAsync<T>(IDataReader reader,
            Func<IDataRecord, T> map,
            CancellationToken cancellationToken)
        {
            if (!(reader is DbDataReader dbReader))
                return ReadRows(reader,
                    map);
            var rows = new List<T>();
            while (await dbReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                rows.Add(map(dbReader));
            return rows;
        }

        /// <summary>
        /// Creates the identity of a table that is read from the connection: its name, its schema, the database of the connection and the type of the provider.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The name of the schema that owns the table.</param>
        /// <returns>The identity of the table.</returns>
        private TableInfo NewTable(IDbConnection connection,
            string name,
            string schema) => new TableInfo(name,
            schema,
            connection.Database,
            GetProvider(connection));
        // Names
        /// <summary>
        /// Gets the key of the table: the names of SQLite are case insensitive.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The key of the table.</returns>
        private static string Key(TableInfo table) => $"{table.Schema}\u0001{table.Name}".ToLowerInvariant();

        /// <summary>
        /// Gets the schema of a name: <c>null</c> for the main database.
        /// </summary>
        /// <param name="schema">The name of the schema.</param>
        /// <returns>The name of the schema, or <c>null</c> for the main schema.</returns>
        private static string NormalizeSchema(string schema) => string.IsNullOrWhiteSpace(schema) || string.Equals(schema,
            AhtolaSchemaHelper.MainSchema,
            StringComparison.OrdinalIgnoreCase) ? null : schema;

        /// <summary>
        /// Gets the quoted name of the schema, to be used in the SQL text.
        /// </summary>
        /// <param name="schema">The name of the schema, or <c>null</c> for the main schema.</param>
        /// <returns>The quoted name of the schema.</returns>
        private static string Schema(string schema) => AhtolaSchemaHelper.Quote(schema ?? AhtolaSchemaHelper.MainSchema);

        /// <summary>
        /// Resolves the schema and the name of the table: the schema is the one of the name, or the one that the table is found in.
        /// </summary>
        /// <param name="tableName">The name of the table, with or without its schema.</param>
        /// <returns>The schema and the name of the table.</returns>
        private static (string Schema, string Table) Resolve(string tableName) => AhtolaSchemaHelper.ParseSchemaAndTable(tableName);
        // Parameters
        /// <summary>
        /// Creates a named parameter of a statement.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The parameter.</returns>
        private static KeyValuePair<string, object> Parameter(string name,
            object value) => new KeyValuePair<string, object>(name,
            value);
        // Execution
        /// <summary>
        /// Converts the parameters into the parameter object that is passed to the statement.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The parameter object.</returns>
        private static IDictionary<string, object> ToParameters(KeyValuePair<string, object>[] parameters) => parameters.ToDictionary(p => p.Key,
            p => p.Value);

        /// <summary>
        /// Executes a statement and maps every record of its result.
        /// </summary>
        /// <typeparam name="T">The type of the result of the map function.</typeparam>
        /// <param name="connection">The operation connection.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="traceKey">The tracking key of the statement.</param>
        /// <param name="map">The function that maps a record into the result.</param>
        /// <param name="parameters">The parameters of the statement.</param>
        /// <returns>The mapped records.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "The parameter object passed to the RepoDB execute methods is either null or a Dictionary<string, object>, which is not reflected.")]
        private IList<T> Query<T>(IDbConnection connection,
            IDbTransaction transaction,
            string sql,
            string traceKey,
            Func<IDataRecord, T> map,
            params KeyValuePair<string, object>[] parameters)
        {
            using (var reader = connection.ExecuteReader(sql,
                param: ToParameters(parameters),
                traceKey: SchemaReadContext.Current?.TraceKey ?? traceKey,
                commandTimeout: SchemaReadContext.Current?.CommandTimeout,
                trace: SchemaReadContext.Current?.Trace,
                transaction: transaction))
            {
                return ReadRows(reader,
                    map);
            }
        }

        /// <summary>
        /// Executes a statement and maps every record of its result asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the result of the map function.</typeparam>
        /// <param name="connection">The operation connection.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="traceKey">The tracking key of the statement.</param>
        /// <param name="map">The function that maps a record into the result.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="parameters">The parameters of the statement.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the mapped records.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "The parameter object passed to the RepoDB execute methods is either null or a Dictionary<string, object>, which is not reflected.")]
        private async Task<IList<T>> QueryAsync<T>(IDbConnection connection,
            IDbTransaction transaction,
            string sql,
            string traceKey,
            Func<IDataRecord, T> map,
            CancellationToken cancellationToken,
            params KeyValuePair<string, object>[] parameters)
        {
            using (var reader = await connection.ExecuteReaderAsync(sql,
                param: ToParameters(parameters),
                traceKey: SchemaReadContext.Current?.TraceKey ?? traceKey,
                commandTimeout: SchemaReadContext.Current?.CommandTimeout,
                trace: SchemaReadContext.Current?.Trace,
                transaction: transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                return await ReadRowsAsync(reader,
                    map,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        // Reading
        /// <summary>
        /// Reads everything that is needed to build the schema of the table (the sync version).
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="schema">The name of the schema.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The raw definition of the table.</returns>
        private AhtolaRawTable ReadTable(IDbConnection connection,
            IDbTransaction transaction,
            string schema,
            string table)
        {
            var quoted = Schema(schema);
            var arguments = new[]
            {
                Parameter("Table",
                    table)
            };
            var literal = AhtolaSchemaText.Literal(schema ?? AhtolaSchemaHelper.MainSchema);
            var raw = new AhtolaRawTable
            {
                Schema = schema,
                Table = table
            };
            var master = Query(connection,
                transaction,
                AhtolaSchemaText.TableSql(quoted),
                SchemaTraceKeys.GetColumns,
                r => (Name: Text(r,
                    "TableName"), Definition: Text(r,
                    "Definition")),
                arguments[0]).FirstOrDefault();
            if (master.Name == null)
            {
                return raw;
            }

            raw.Exists = true;
            raw.Table = master.Name;
            raw.Definition = master.Definition;
            raw.Columns = Query(connection,
                transaction,
                AhtolaSchemaText.ColumnsSql(literal),
                SchemaTraceKeys.GetColumns,
                MapColumn,
                arguments);
            raw.Indexes = Query(connection,
                transaction,
                AhtolaSchemaText.IndexListSql(literal),
                SchemaTraceKeys.GetIndexes,
                MapIndex,
                arguments);
            var definitions = Query(connection,
                transaction,
                AhtolaSchemaText.IndexDefinitionsSql(quoted),
                SchemaTraceKeys.GetIndexes,
                r => (Name: Text(r,
                    "IndexName"), Definition: Text(r,
                    "Definition")),
                arguments[0]);
            foreach (var index in raw.Indexes)
            {
                index.Definition = definitions.FirstOrDefault(d => string.Equals(d.Name,
                        index.Name,
                        StringComparison.OrdinalIgnoreCase)).Definition;
                index.Columns = Query(connection,
                    transaction,
                    AhtolaSchemaText.IndexColumnsSql(literal),
                    SchemaTraceKeys.GetIndexes,
                    MapIndexColumn,
                    Parameter("Index",
                        index.Name));
            }

            raw.ForeignKeys = Query(connection,
                transaction,
                AhtolaSchemaText.ForeignKeysSql(literal),
                SchemaTraceKeys.GetForeignKeys,
                MapForeignKey,
                arguments);
            foreach (var referenced in raw.ForeignKeys.Where(f => f.ReferencedColumn == null).Select(f => f.ReferencedTable).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                raw.ReferencedPrimaryKeys[referenced] = Query(connection,
                    transaction,
                    AhtolaSchemaText.PrimaryKeyColumnsSql(literal),
                    SchemaTraceKeys.GetPrimaryKey,
                    r => Text(r,
                        "ColumnName"),
                    Parameter("Table",
                        referenced)).ToList();
            }

            return raw;
        }

        /// <summary>
        /// Reads everything that is needed to build the schema of the table (the async version).
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="schema">The name of the schema.</param>
        /// <param name="table">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the raw definition of the table.</returns>
        private async Task<AhtolaRawTable> ReadTableAsync(IDbConnection connection,
            IDbTransaction transaction,
            string schema,
            string table,
            CancellationToken cancellationToken)
        {
            var quoted = Schema(schema);
            var arguments = new[]
            {
                Parameter("Table",
                    table)
            };
            var literal = AhtolaSchemaText.Literal(schema ?? AhtolaSchemaHelper.MainSchema);
            var raw = new AhtolaRawTable
            {
                Schema = schema,
                Table = table
            };
            var masters = await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.TableSql(quoted),
                SchemaTraceKeys.GetColumns,
                r => (Name: Text(r,
                    "TableName"), Definition: Text(r,
                    "Definition")),
                cancellationToken,
                arguments[0]).ConfigureAwait(false);
            var master = masters.FirstOrDefault();
            if (master.Name == null)
            {
                return raw;
            }

            raw.Exists = true;
            raw.Table = master.Name;
            raw.Definition = master.Definition;
            raw.Columns = await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.ColumnsSql(literal),
                SchemaTraceKeys.GetColumns,
                MapColumn,
                cancellationToken,
                arguments).ConfigureAwait(false);
            raw.Indexes = await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.IndexListSql(literal),
                SchemaTraceKeys.GetIndexes,
                MapIndex,
                cancellationToken,
                arguments).ConfigureAwait(false);
            var definitions = await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.IndexDefinitionsSql(quoted),
                SchemaTraceKeys.GetIndexes,
                r => (Name: Text(r,
                    "IndexName"), Definition: Text(r,
                    "Definition")),
                cancellationToken,
                arguments[0]).ConfigureAwait(false);
            foreach (var index in raw.Indexes)
            {
                index.Definition = definitions.FirstOrDefault(d => string.Equals(d.Name,
                        index.Name,
                        StringComparison.OrdinalIgnoreCase)).Definition;
                index.Columns = await QueryAsync(connection,
                    transaction,
                    AhtolaSchemaText.IndexColumnsSql(literal),
                    SchemaTraceKeys.GetIndexes,
                    MapIndexColumn,
                    cancellationToken,
                    Parameter("Index",
                        index.Name)).ConfigureAwait(false);
            }

            raw.ForeignKeys = await QueryAsync(connection,
                transaction,
                AhtolaSchemaText.ForeignKeysSql(literal),
                SchemaTraceKeys.GetForeignKeys,
                MapForeignKey,
                cancellationToken,
                arguments).ConfigureAwait(false);
            foreach (var referenced in raw.ForeignKeys.Where(f => f.ReferencedColumn == null).Select(f => f.ReferencedTable).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var columns = await QueryAsync(connection,
                    transaction,
                    AhtolaSchemaText.PrimaryKeyColumnsSql(literal),
                    SchemaTraceKeys.GetPrimaryKey,
                    r => Text(r,
                        "ColumnName"),
                    cancellationToken,
                    Parameter("Table",
                        referenced)).ConfigureAwait(false);
                raw.ReferencedPrimaryKeys[referenced] = columns.ToList();
            }

            return raw;
        }

        // Row mappers
        /// <summary>
        /// Reads a text value of the record.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value, or <c>null</c> if it is <c>DBNull</c>.</returns>
        private static string Text(IDataRecord r,
            string name)
        {
            var value = r[name];
            return value == DBNull.Value ? null : Convert.ToString(value);
        }

        /// <summary>
        /// Reads an integer value of the record.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value, or <c>0</c> if it is <c>DBNull</c>.</returns>
        private static int Int(IDataRecord r,
            string name)
        {
            var value = r[name];
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        /// <summary>
        /// Maps a record into a column of the table.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The column.</returns>
        private static AhtolaRawColumn MapColumn(IDataRecord r) => new AhtolaRawColumn
        {
            Name = Text(r,
                "Name"),
            DeclaredType = Text(r,
                "DeclaredType") ?? string.Empty,
            NotNull = Int(r,
                "IsNotNull") != 0,
            Default = Text(r,
                "DefaultDefinition"),
            PrimaryKeyPosition = Int(r,
                "PrimaryKeyPosition"),
            Hidden = Int(r,
                "Hidden")
        };

        /// <summary>
        /// Maps a record into an index of the table.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The index.</returns>
        private static AhtolaRawIndex MapIndex(IDataRecord r) => new AhtolaRawIndex
        {
            Name = Text(r,
                "IndexName"),
            IsUnique = Int(r,
                "IsUnique") != 0,
            Origin = Text(r,
                "Origin"),
            IsPartial = Int(r,
                "IsPartial") != 0
        };

        /// <summary>
        /// Maps a record into the row of a column of an index.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The row of the column of the index.</returns>
        private static AhtolaRawIndexColumn MapIndexColumn(IDataRecord r) => new AhtolaRawIndexColumn
        {
            Name = Text(r,
                "ColumnName"),
            IsDescending = Int(r,
                "IsDescending") != 0,
            IsKey = Int(r,
                "IsKey") != 0
        };

        /// <summary>
        /// Maps a record into a column of a foreign key.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The column of the foreign key.</returns>
        private static AhtolaRawForeignKey MapForeignKey(IDataRecord r) => new AhtolaRawForeignKey
        {
            Id = Int(r,
                "Id"),
            ReferencedTable = Text(r,
                "ReferencedTable"),
            Column = Text(r,
                "ColumnName"),
            ReferencedColumn = Text(r,
                "ReferencedColumn"),
            Update = Text(r,
                "UpdateAction"),
            Delete = Text(r,
                "DeleteAction")
        };
        // Building
        /// <summary>
        /// Builds the schema of the table from what was read.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="raw">The raw definition of the table.</param>
        /// <returns>The schema of the table.</returns>
        private TableSchema Build(IDbConnection connection,
            AhtolaRawTable raw)
        {
            var schema = new TableSchema(NewTable(connection,
                    raw.Table,
                    raw.Schema));
            if (!raw.Exists)
            {
                return schema;
            }

            var definition = AhtolaSchemaParser.ParseTable(raw.Definition);
            var primaryColumns = raw.Columns.Where(c => c.PrimaryKeyPosition > 0).OrderBy(c => c.PrimaryKeyPosition).Select(c => c.Name).ToList();
            schema.Columns = BuildColumns(connection,
                raw,
                definition,
                primaryColumns);
            schema.PrimaryKey = BuildPrimaryKey(raw,
                definition,
                primaryColumns);
            schema.Indexes = BuildIndexes(raw);
            schema.UniqueConstraints = BuildUniqueConstraints(raw,
                definition);
            schema.ForeignKeys = BuildForeignKeys(connection,
                raw,
                definition);
            schema.CheckConstraints = BuildCheckConstraints(raw,
                definition);
            return schema;
        }

        /// <summary>
        /// Builds the columns of the table out of its raw definition and the parsed statement that creates it.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="raw">The raw definition of the table.</param>
        /// <param name="definition">The parsed statement that creates the table.</param>
        /// <param name="primaryColumns">The names of the columns of the primary key.</param>
        /// <returns>The columns.</returns>
        private IList<ColumnInfo> BuildColumns(IDbConnection connection,
            AhtolaRawTable raw,
            AhtolaTableDefinition definition,
            IList<string> primaryColumns)
        {
            var columns = new List<ColumnInfo>();
            foreach (var column in raw.Columns)
            {
                definition.Columns.TryGetValue(column.Name,
                    out var parsed);
                var (typeName, arguments) = AhtolaSchemaParser.ParseType(column.DeclaredType);
                var isNumeric = IsNumeric(typeName);
                var isPrimary = column.PrimaryKeyPosition > 0;
                var isIdentity = isPrimary && primaryColumns.Count == 1 && string.Equals(typeName,
                    "integer",
                    StringComparison.Ordinal);
                var resolved = typeName.Length == 0 ? null : _typeResolver.Resolve(typeName);
                var clientType = resolved != null && resolved != typeof(object) ? resolved : ToClientType(typeName);
                var field = new DbField(column.Name,
                    isPrimary,
                    isIdentity,
                    !column.NotNull && !isPrimary,
                    clientType,
                    !isNumeric && arguments.Count > 0 ? arguments[0] : 0,
                    (byte)(isNumeric && arguments.Count > 0 ? arguments[0] : 0),
                    (byte)(isNumeric && arguments.Count > 1 ? arguments[1] : 0),
                    typeName,
                    column.Default != null,
                    GetProvider(connection));
                columns.Add(new ColumnInfo { Field = field, Ordinal = columns.Count + 1, DefaultExpression = column.Default, DefaultValue = SchemaDefaultValue.Classify(column.Default,
                        AhtolaSchemaDialect.Instance), ComputedExpression = column.Hidden >= 2 ? parsed?.GeneratedExpression : null, Collation = parsed?.Collation, NativeType = column.DeclaredType, NumericPrecision = isNumeric && arguments.Count > 0 ? (int? )arguments[0] : null, NumericScale = isNumeric && arguments.Count > 1 ? (int? )arguments[1] : null, IsUnicode = clientType == typeof(string) ? (bool? )true : null, LengthUnit = clientType == typeof(string) ? "characters" : null });
            }

            return columns;
        }

        /// <summary>
        /// Builds the primary key of the table.
        /// </summary>
        /// <param name="raw">The raw definition of the table.</param>
        /// <param name="definition">The parsed statement that creates the table.</param>
        /// <param name="primaryColumns">The names of the columns of the primary key.</param>
        /// <returns>The primary key, or <c>null</c> if the table has none.</returns>
        private static PrimaryKeyInfo BuildPrimaryKey(AhtolaRawTable raw,
            AhtolaTableDefinition definition,
            IList<string> primaryColumns)
        {
            if (primaryColumns.Count == 0)
            {
                return null;
            }

            var name=definition.Find(AhtolaConstraintKind.PrimaryKey,
                primaryColumns)?.Name ?? $"PK_{raw.Table}";
            return new PrimaryKeyInfo(name)
            {
                Columns = primaryColumns.ToList(),
                IsClustered = true
            };
        }

        /// <summary>
        /// Builds the indexes of the table (the ones that were created explicitly).
        /// </summary>
        /// <param name="raw">The raw definition of the table.</param>
        /// <returns>The indexes.</returns>
        private static IList<IndexInfo> BuildIndexes(AhtolaRawTable raw) => raw.Indexes.Where(index => string.Equals(index.Origin,
                "c",
                StringComparison.Ordinal)).OrderBy(index => index.Name,
            StringComparer.Ordinal).Select(index =>
        {
            var keys = index.Columns.Where(c => c.IsKey && c.Name != null).ToList();
            return new IndexInfo(index.Name)
            {
                IsUnique = index.IsUnique,
                IsClustered = false,
                Filter = index.IsPartial ? AhtolaSchemaParser.ParseIndexFilter(index.Definition) : null,
                Columns = keys.Select(c => c.Name).ToList(),
                DescendingColumns = keys.Where(c => c.IsDescending).Select(c => c.Name).ToList()
            };
        }).ToList();

        /// <summary>
        /// Builds the unique constraints of the table.
        /// </summary>
        /// <param name="raw">The raw definition of the table.</param>
        /// <param name="definition">The parsed statement that creates the table.</param>
        /// <returns>The unique constraints.</returns>
        private static IList<UniqueConstraintInfo> BuildUniqueConstraints(AhtolaRawTable raw,
            AhtolaTableDefinition definition) => raw.Indexes.Where(index => string.Equals(index.Origin,
                "u",
                StringComparison.Ordinal)).Select(index =>
        {
            var columns = index.Columns.Where(c => c.IsKey && c.Name != null).Select(c => c.Name).ToList();
            return new UniqueConstraintInfo(definition.Find(AhtolaConstraintKind.Unique,
                        columns)?.Name ?? index.Name)
            {
                Columns = columns
            };
        }).OrderBy(constraint => constraint.Name,
            StringComparer.Ordinal).ToList();

        /// <summary>
        /// Builds the foreign keys of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="raw">The raw definition of the table.</param>
        /// <param name="definition">The parsed statement that creates the table.</param>
        /// <returns>The foreign keys.</returns>
        private IList<ForeignKeyInfo> BuildForeignKeys(IDbConnection connection,
            AhtolaRawTable raw,
            AhtolaTableDefinition definition) => raw.ForeignKeys.GroupBy(f => f.Id).Select(group =>
        {
            var first = group.First();
            var columns = group.Select(f => f.Column).ToList();
            var referencedColumns = first.ReferencedColumn == null ? raw.ReferencedPrimaryKeys.TryGetValue(first.ReferencedTable,
                    out var primary) ? primary : new List<string>() : group.Select(f => f.ReferencedColumn).ToList();
            return new ForeignKeyInfo(definition.Find(AhtolaConstraintKind.ForeignKey,
                        columns)?.Name ?? $"FK_{raw.Table}_{string.Join("_",
                        columns)}")
            {
                Columns = columns,
                ReferencedTable = NewTable(connection,
                        first.ReferencedTable,
                        raw.Schema),
                ReferencedColumns = referencedColumns,
                UpdateRule = ToRule(first.Update),
                DeleteRule = ToRule(first.Delete)
            };
        }).OrderBy(foreignKey => foreignKey.Name,
            StringComparer.Ordinal).ToList();

        /// <summary>
        /// Builds the check constraints of the table.
        /// </summary>
        /// <param name="raw">The raw definition of the table.</param>
        /// <param name="definition">The parsed statement that creates the table.</param>
        /// <returns>The check constraints.</returns>
        private static IList<CheckConstraintInfo> BuildCheckConstraints(AhtolaRawTable raw,
            AhtolaTableDefinition definition)
        {
            var checks = definition.Constraints.Where(c => c.Kind == AhtolaConstraintKind.Check).ToList();
            return checks.Select((check,
                    i) => new CheckConstraintInfo(check.Name ?? $"CK_{raw.Table}_{i + 1}") { Expression = check.Expression }).ToList();
        }

        /// <summary>
        /// Checks whether the arguments of the type are a precision and a scale (and not a size).
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <returns><c>true</c> if the type is numeric; otherwise, <c>false</c>.</returns>
        private static bool IsNumeric(string typeName) => typeName == "decimal" || typeName == "numeric" || typeName == "dec" || typeName == "float" || typeName == "double" || typeName == "real";

        /// <summary>
        /// Gets the client type of a declared type, with the rules of the type affinity of SQLite.
        /// </summary>
        /// <param name="typeName">The declared type.</param>
        /// <returns>The client type.</returns>
        private static Type ToClientType(string typeName)
        {
            if (typeName.Contains("int"))
            {
                return typeof(long);
            }

            if (typeName.Contains("char") || typeName.Contains("clob") || typeName.Contains("text"))
            {
                return typeof(string);
            }

            if (typeName.Length == 0 || typeName.Contains("blob"))
            {
                return typeof(byte[]);
            }

            if (typeName.Contains("real") || typeName.Contains("floa") || typeName.Contains("doub"))
            {
                return typeof(double);
            }

            return typeof(object);
        }

        /// <summary>
        /// Converts the action of the database into the rule of a foreign key.
        /// </summary>
        /// <param name="action">The action that is read from the database.</param>
        /// <returns>The rule of the foreign key.</returns>
        private static ForeignKeyRule ToRule(string action)
        {
            switch (action?.ToUpperInvariant())
            {
                case "CASCADE":
                    return ForeignKeyRule.Cascade;
                case "SET NULL":
                    return ForeignKeyRule.SetNull;
                case "SET DEFAULT":
                    return ForeignKeyRule.SetDefault;
                case "RESTRICT":
                    return ForeignKeyRule.Restrict;
                default:
                    return ForeignKeyRule.NoAction;
            }
        }

        // Ordering
        /// <summary>
        /// Orders the tables so that a table always comes after the tables that it references (see <see cref="TableOrderer"/>).
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered relationships, one per table.</returns>
        internal static IList<RelationshipInfo> Order(IList<TableSchema> schemas) => TableOrderer.Order(schemas,
            schema => Key(schema.Table),
            Key);
        #endregion
    }
}
