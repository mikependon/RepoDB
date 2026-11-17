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
    /// Reads ClickHouse catalog metadata using operation connections.
    /// </summary>
    public class ClickHouseSchemaReader : ISchemaReader
    {
        #region Private Variables

        private readonly ClickHouseDbTypeNameToClientTypeResolver _typeResolver = new ClickHouseDbTypeNameToClientTypeResolver();
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
            var fullName = FullName(schema,
                table);
            return new TableSchema(NewTable(connection,
                    table,
                    schema))
            {
                Columns = Query(connection,
                    transaction,
                    ClickHouseSchemaText.ColumnsSql,
                    SchemaTraceKeys.GetColumns,
                    (value0) => MapColumn(connection,
                        value0),
                    FullNameParameter(fullName)),
                PrimaryKey = MapPrimaryKey(Query(connection,
                        transaction,
                        ClickHouseSchemaText.KeyConstraintSql,
                        SchemaTraceKeys.GetPrimaryKey,
                        MapKeyColumn,
                        FullNameParameter(fullName),
                        Parameter("Type",
                            "PK"))),
                Indexes = MapIndexes(Query(connection,
                        transaction,
                        ClickHouseSchemaText.IndexesSql,
                        SchemaTraceKeys.GetIndexes,
                        MapIndexColumn,
                        FullNameParameter(fullName))),
                ForeignKeys = MapForeignKeys(connection,
                    Query(connection,
                        transaction,
                        ClickHouseSchemaText.ForeignKeysSql,
                        SchemaTraceKeys.GetForeignKeys,
                        MapForeignKeyColumn,
                        FullNameParameter(fullName))),
                UniqueConstraints = MapUniqueConstraints(Query(connection,
                        transaction,
                        ClickHouseSchemaText.KeyConstraintSql,
                        SchemaTraceKeys.GetUniqueConstraints,
                        MapKeyColumn,
                        FullNameParameter(fullName),
                        Parameter("Type",
                            "UQ"))),
                CheckConstraints = ParseCheckConstraints(Query(connection,
                        transaction,
                        ClickHouseSchemaText.CheckConstraintsSql,
                        SchemaTraceKeys.GetCheckConstraints,
                        MapDefinition,
                        FullNameParameter(fullName)))
            };
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
            var (schema, table) = ParseTableName(tableName);
            return Query(connection,
                transaction,
                ClickHouseSchemaText.TableExistsSql,
                SchemaTraceKeys.TableExists,
                r => Convert.ToInt32(r[0]),
                FullNameParameter(FullName(schema,
                        table))).FirstOrDefault() == 1;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
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
            return Query(connection,
                transaction,
                ClickHouseSchemaText.ColumnsSql,
                SchemaTraceKeys.GetColumns,
                (value0) => MapColumn(connection,
                    value0),
                FullNameParameter(Resolve(tableName)));
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
            return MapPrimaryKey(Query(connection,
                    transaction,
                    ClickHouseSchemaText.KeyConstraintSql,
                    SchemaTraceKeys.GetPrimaryKey,
                    MapKeyColumn,
                    FullNameParameter(Resolve(tableName)),
                    Parameter("Type",
                        "PK")));
        }

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
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
            return MapIndexes(Query(connection,
                    transaction,
                    ClickHouseSchemaText.IndexesSql,
                    SchemaTraceKeys.GetIndexes,
                    MapIndexColumn,
                    FullNameParameter(Resolve(tableName))));
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
            return MapForeignKeys(connection,
                Query(connection,
                    transaction,
                    ClickHouseSchemaText.ForeignKeysSql,
                    SchemaTraceKeys.GetForeignKeys,
                    MapForeignKeyColumn,
                    FullNameParameter(Resolve(tableName))));
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
            return MapUniqueConstraints(Query(connection,
                    transaction,
                    ClickHouseSchemaText.KeyConstraintSql,
                    SchemaTraceKeys.GetUniqueConstraints,
                    MapKeyColumn,
                    FullNameParameter(Resolve(tableName)),
                    Parameter("Type",
                        "UQ")));
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
            return ParseCheckConstraints(Query(connection,
                    transaction,
                    ClickHouseSchemaText.CheckConstraintsSql,
                    SchemaTraceKeys.GetCheckConstraints,
                    MapDefinition,
                    FullNameParameter(Resolve(tableName))));
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
                ClickHouseSchemaText.SchemasSql,
                SchemaTraceKeys.GetSchemas,
                r => new SchemaInfo(Text(r,
                        "SchemaName")));
        }

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The names of the tables.</returns>
        public IEnumerable<string> GetTables(IDbConnection connection,
            string schemaName = null,
            IDbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return Query(connection,
                transaction,
                ClickHouseSchemaText.TablesSql,
                SchemaTraceKeys.GetTables,
                r => ClickHouseSchemaHelper.FormatTableName(Text(r,
                        "SchemaName"),
                    Text(r,
                        "TableName")),
                Parameter("SchemaName",
                    schemaName));
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
            var foreignKeys = relationshipBehavior == RelatedTableBehavior.TableOnly ? new List<(TableInfo Child, TableInfo Parent)>() : Query(connection,
                transaction,
                ClickHouseSchemaText.ForeignKeyRelationshipsSql,
                SchemaTraceKeys.GetRelationships,
                (value0) => MapRelationship(connection,
                    value0));
            return RelatedTableExpander.Expand(tables,
                foreignKeys,
                relationshipBehavior,
                Key).Select(table => ClickHouseSchemaHelper.FormatTableName(table.Schema,
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
            var (schema, table) = await ResolveAsync(tableName,
                cancellationToken).ConfigureAwait(false);
            var fullName = FullName(schema,
                table);
            return new TableSchema(NewTable(connection,
                    table,
                    schema))
            {
                Columns = await QueryAsync(connection,
                    transaction,
                    ClickHouseSchemaText.ColumnsSql,
                    SchemaTraceKeys.GetColumns,
                    (value0) => MapColumn(connection,
                        value0),
                    cancellationToken,
                    FullNameParameter(fullName)).ConfigureAwait(false),
                PrimaryKey = MapPrimaryKey(await QueryAsync(connection,
                        transaction,
                        ClickHouseSchemaText.KeyConstraintSql,
                        SchemaTraceKeys.GetPrimaryKey,
                        MapKeyColumn,
                        cancellationToken,
                        FullNameParameter(fullName),
                        Parameter("Type",
                            "PK")).ConfigureAwait(false)),
                Indexes = MapIndexes(await QueryAsync(connection,
                        transaction,
                        ClickHouseSchemaText.IndexesSql,
                        SchemaTraceKeys.GetIndexes,
                        MapIndexColumn,
                        cancellationToken,
                        FullNameParameter(fullName)).ConfigureAwait(false)),
                ForeignKeys = MapForeignKeys(connection,
                    await QueryAsync(connection,
                        transaction,
                        ClickHouseSchemaText.ForeignKeysSql,
                        SchemaTraceKeys.GetForeignKeys,
                        MapForeignKeyColumn,
                        cancellationToken,
                        FullNameParameter(fullName)).ConfigureAwait(false)),
                UniqueConstraints = MapUniqueConstraints(await QueryAsync(connection,
                        transaction,
                        ClickHouseSchemaText.KeyConstraintSql,
                        SchemaTraceKeys.GetUniqueConstraints,
                        MapKeyColumn,
                        cancellationToken,
                        FullNameParameter(fullName),
                        Parameter("Type",
                            "UQ")).ConfigureAwait(false)),
                CheckConstraints = ParseCheckConstraints(await QueryAsync(connection,
                        transaction,
                        ClickHouseSchemaText.CheckConstraintsSql,
                        SchemaTraceKeys.GetCheckConstraints,
                        MapDefinition,
                        cancellationToken,
                        FullNameParameter(fullName)).ConfigureAwait(false))
            };
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
            var (schema, table) = ParseTableName(tableName);
            var result = await QueryAsync(connection,
                transaction,
                ClickHouseSchemaText.TableExistsSql,
                SchemaTraceKeys.TableExists,
                r => Convert.ToInt32(r[0]),
                cancellationToken,
                FullNameParameter(FullName(schema,
                        table))).ConfigureAwait(false);
            return result.FirstOrDefault() == 1;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
        public async Task<string> GetSchemaNameAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return (await ResolveAsync(tableName,
                cancellationToken).ConfigureAwait(false)).Schema;
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
            return await QueryAsync(connection,
                transaction,
                ClickHouseSchemaText.ColumnsSql,
                SchemaTraceKeys.GetColumns,
                (value0) => MapColumn(connection,
                    value0),
                cancellationToken,
                FullNameParameter(await ResolveFullNameAsync(tableName,
                        cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);
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
            return MapPrimaryKey(await QueryAsync(connection,
                    transaction,
                    ClickHouseSchemaText.KeyConstraintSql,
                    SchemaTraceKeys.GetPrimaryKey,
                    MapKeyColumn,
                    cancellationToken,
                    FullNameParameter(await ResolveFullNameAsync(tableName,
                            cancellationToken).ConfigureAwait(false)),
                    Parameter("Type",
                        "PK")).ConfigureAwait(false));
        }

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
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
            return MapIndexes(await QueryAsync(connection,
                    transaction,
                    ClickHouseSchemaText.IndexesSql,
                    SchemaTraceKeys.GetIndexes,
                    MapIndexColumn,
                    cancellationToken,
                    FullNameParameter(await ResolveFullNameAsync(tableName,
                            cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));
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
            return MapForeignKeys(connection,
                await QueryAsync(connection,
                    transaction,
                    ClickHouseSchemaText.ForeignKeysSql,
                    SchemaTraceKeys.GetForeignKeys,
                    MapForeignKeyColumn,
                    cancellationToken,
                    FullNameParameter(await ResolveFullNameAsync(tableName,
                            cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));
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
            return MapUniqueConstraints(await QueryAsync(connection,
                    transaction,
                    ClickHouseSchemaText.KeyConstraintSql,
                    SchemaTraceKeys.GetUniqueConstraints,
                    MapKeyColumn,
                    cancellationToken,
                    FullNameParameter(await ResolveFullNameAsync(tableName,
                            cancellationToken).ConfigureAwait(false)),
                    Parameter("Type",
                        "UQ")).ConfigureAwait(false));
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
            return ParseCheckConstraints(await QueryAsync(connection,
                    transaction,
                    ClickHouseSchemaText.CheckConstraintsSql,
                    SchemaTraceKeys.GetCheckConstraints,
                    MapDefinition,
                    cancellationToken,
                    FullNameParameter(await ResolveFullNameAsync(tableName,
                            cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));
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
                ClickHouseSchemaText.SchemasSql,
                SchemaTraceKeys.GetSchemas,
                r => new SchemaInfo(Text(r,
                        "SchemaName")),
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
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
            return await QueryAsync(connection,
                transaction,
                ClickHouseSchemaText.TablesSql,
                SchemaTraceKeys.GetTables,
                r => ClickHouseSchemaHelper.FormatTableName(Text(r,
                        "SchemaName"),
                    Text(r,
                        "TableName")),
                cancellationToken,
                Parameter("SchemaName",
                    schemaName)).ConfigureAwait(false);
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
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            var tables = new List<TableInfo>();
            foreach (var name in names)
            {
                var (schema, table) = await ResolveAsync(name,
                    cancellationToken).ConfigureAwait(false);
                tables.Add(NewTable(connection,
                        table,
                        schema));
            }

            IEnumerable<(TableInfo Child, TableInfo Parent)> foreignKeys = new List<(TableInfo Child, TableInfo Parent)>();
            if (relationshipBehavior != RelatedTableBehavior.TableOnly)
            {
                foreignKeys = await QueryAsync(connection,
                    transaction,
                    ClickHouseSchemaText.ForeignKeyRelationshipsSql,
                    SchemaTraceKeys.GetRelationships,
                    (value0) => MapRelationship(connection,
                        value0),
                    cancellationToken).ConfigureAwait(false);
            }

            return RelatedTableExpander.Expand(tables,
                foreignKeys,
                relationshipBehavior,
                Key).Select(table => ClickHouseSchemaHelper.FormatTableName(table.Schema,
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

        /// <summary>
        /// Splits the name of a table into its schema and its name.
        /// </summary>
        /// <param name="tableName">The name of the table, with or without its schema.</param>
        /// <returns>The schema (<c>null</c> if the name has none) and the name of the table.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static (string Schema, string Table) ParseTableName(string tableName) => ClickHouseSchemaHelper.ParseSchemaAndTable(tableName);

        /// <summary>
        /// Gets the full name of the table out of its schema and its name, as it is passed to the statements.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The full name of the table.</returns>
        private static string FullName(string schema,
            string table) => $"{schema}\u0001{table}";

        /// <summary>
        /// Gets the key that identifies a table: its schema and its name.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The key of the table.</returns>
        private static string Key(TableInfo table) => $"{table.Schema}\u0001{table.Name}";

        /// <summary>
        /// Resolves the schema and the name of the table: the schema is the one of the name, or the one that the table is found in.
        /// </summary>
        /// <param name="tableName">The name of the table, with or without its schema.</param>
        /// <returns>The schema and the name of the table.</returns>
        private (string Schema, string Table) Resolve(string tableName)
        {
            return ParseTableName(tableName);
        }

        /// <summary>
        /// Resolves the schema and the name of the table asynchronously: the schema is the one of the name, or the one that the table is found in.
        /// </summary>
        /// <param name="tableName">The name of the table, with or without its schema.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the schema and the name of the table.</returns>
        private Task<(string Schema, string Table)> ResolveAsync(string tableName,
            CancellationToken cancellationToken) => Task.FromResult(ParseTableName(tableName));

        /// <summary>
        /// Resolves the full name of the table asynchronously.
        /// </summary>
        /// <param name="tableName">The name of the table, with or without its schema.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the full name of the table.</returns>
        private async Task<string> ResolveFullNameAsync(string tableName,
            CancellationToken cancellationToken)
        {
            var (schema, table) = await ResolveAsync(tableName,
                cancellationToken).ConfigureAwait(false);
            return FullName(schema,
                table);
        }

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

        /// <summary>
        /// Creates the parameter that holds the full name of a table.
        /// </summary>
        /// <param name="fullName">The full name of the table.</param>
        /// <returns>The parameter.</returns>
        private static KeyValuePair<string, object> FullNameParameter(string fullName) => Parameter("FullName",
            fullName);

        /// <summary>
        /// Creates the parameter that holds the full name of a table.
        /// </summary>
        /// <param name="name">The schema and the name of the table.</param>
        /// <returns>The parameter.</returns>
        private static KeyValuePair<string, object> FullNameParameter((string Schema, string Table) name) => FullNameParameter(FullName(name.Schema,
                name.Table));
        // Execution
        /// <summary>
        /// Converts the parameters into the parameter object that is passed to the statement.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The parameter object.</returns>
        private static IDictionary<string, object> ToParameters(KeyValuePair<string, object>[] parameters)
        {
            var result = new Dictionary<string, object>();
            foreach (var parameter in parameters)
            {
                if (parameter.Key == "FullName")
                {
                    var parts = ((string)parameter.Value).Split('\u0001');
                    result["Schema"] = string.IsNullOrEmpty(parts[0]) ? null : parts[0];
                    result["Table"] = parts[1];
                }
                else
                {
                    result[parameter.Key] = parameter.Value;
                }
            }

            return result;
        }

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
        /// Reads a nullable integer value of the record.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value, or <c>null</c> if it is <c>DBNull</c>.</returns>
        private static long? NullableLong(IDataRecord r,
            string name)
        {
            var value = r[name];
            return value == DBNull.Value ? (long? )null : Convert.ToInt64(value);
        }

        /// <summary>
        /// Reads a boolean value of the record.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value, or <c>false</c> if it is <c>DBNull</c>.</returns>
        private static bool Flag(IDataRecord r,
            string name)
        {
            var value = r[name];
            return value != DBNull.Value && Convert.ToBoolean(value);
        }

        /// <summary>
        /// Maps a record into a column of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="r">The record.</param>
        /// <returns>The column.</returns>
        private ColumnInfo MapColumn(IDbConnection connection,
            IDataRecord r)
        {
            var (type, isNullable) = Unwrap(Text(r,
                    "TypeName"));
            var kind = Text(r,
                "DefaultKind");
            var expression = Text(r,
                "DefaultDefinition");
            var isComputed = !string.IsNullOrEmpty(expression) && (kind == "MATERIALIZED" || kind == "ALIAS");
            var isDefault = !string.IsNullOrEmpty(expression) && kind == "DEFAULT";
            var baseName = BaseName(type);
            var arguments = Arguments(type);
            var field = new DbField(Text(r,
                    "Name"),
                Flag(r,
                    "IsPrimary"),
                false,
                isNullable,
                _typeResolver.Resolve(type) ?? typeof(object),
                baseName == "FixedString" && arguments.Length > 0 ? arguments[0] : 0,
                (byte)(baseName == "Decimal" && arguments.Length > 0 ? arguments[0] : 0),
                (byte)(baseName == "Decimal" && arguments.Length > 1 ? arguments[1] : baseName == "DateTime64" && arguments.Length > 0 ? arguments[0] : 0),
                IsWrapped(type) || !IsParameterized(baseName) ? type : baseName,
                isDefault,
                GetProvider(connection));
            return SchemaColumnMetadata.Apply(new ColumnInfo { Field = field, Ordinal = Int(r,
                    "Ordinal"), DefaultExpression = isDefault ? expression : null, DefaultValue = SchemaDefaultValue.Classify(isDefault ? expression : null,
                    ClickHouseSchemaDialect.Instance), ComputedExpression = isComputed ? expression : null },
                r);
        }

        /// <summary>
        /// Takes the <c>Nullable</c> wrapper off the type (it can be inside <c>LowCardinality</c>), and tells whether the type was nullable.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <returns>The name of the type, and a value that indicates whether the type was nullable.</returns>
        private static (string Type, bool IsNullable) Unwrap(string type)
        {
            type = type.Trim();
            if (type.StartsWith("Nullable(",
                StringComparison.Ordinal) && type.EndsWith(")",
                StringComparison.Ordinal))
            {
                return (type.Substring("Nullable(".Length,
                    type.Length - "Nullable(".Length - 1).Trim(), true);
            }

            if (type.StartsWith("LowCardinality(",
                StringComparison.Ordinal) && type.EndsWith(")",
                StringComparison.Ordinal))
            {
                var (inner, isNullable) = Unwrap(type.Substring("LowCardinality(".Length,
                        type.Length - "LowCardinality(".Length - 1));
                return ("LowCardinality(" + inner + ")", isNullable);
            }

            return (type, false);
        }

        /// <summary>
        /// Gets the name of the type without its arguments (i.e.: <c>Decimal</c> for <c>Decimal(10, 2)</c>).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The name of the type.</returns>
        private static string BaseName(string type)
        {
            var index = type.IndexOf('(');
            return index < 0 ? type : type.Substring(0,
                index).Trim();
        }

        /// <summary>
        /// Gets the numeric arguments of the type (i.e.: 10 and 2 for <c>Decimal(10, 2)</c>).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The arguments of the type, or an empty array if it has none.</returns>
        private static int[] Arguments(string type)
        {
            var start = type.IndexOf('(');
            if (start < 0 || !type.EndsWith(")",
                StringComparison.Ordinal))
            {
                return new int[0];
            }

            var values = new List<int>();
            foreach (var item in type.Substring(start + 1,
                type.Length - start - 2).Split(','))
            {
                if (!int.TryParse(item.Trim(),
                    System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var value))
                {
                    return new int[0];
                }

                values.Add(value);
            }

            return values.ToArray();
        }

        /// <summary>
        /// Checks whether the type wraps another type (i.e.: <c>LowCardinality(String)</c>), which is kept as it is.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <returns><c>true</c> if the type is wrapped; otherwise, <c>false</c>.</returns>
        private static bool IsWrapped(string type) => type.StartsWith("LowCardinality(",
            StringComparison.Ordinal);

        /// <summary>
        /// Checks whether the type takes its numbers from the fields of the column (they are not part of its database type).
        /// </summary>
        /// <param name="baseName">The name of the type, without its arguments.</param>
        /// <returns><c>true</c> if the type takes its numbers from the fields of the column; otherwise, <c>false</c>.</returns>
        private static bool IsParameterized(string baseName) => baseName == "Decimal" || baseName == "DateTime64" || baseName == "FixedString";

        /// <summary>
        /// Gets the text of the definition.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The text of the definition.</returns>
        private static string MapDefinition(IDataRecord r) => Text(r,
            "Definition");

        /// <summary>
        /// ClickHouse does not give the check constraints of a table in a system table, so they are parsed out of the statement that creates the table
        /// (i.e.: <c>CONSTRAINT ck_age CHECK Age &gt;= 0</c>).
        /// </summary>
        /// <param name="definitions">The statements that create the table.</param>
        /// <returns>The check constraints.</returns>
        private static IList<CheckConstraintInfo> ParseCheckConstraints(IList<string> definitions)
        {
            var list = new List<CheckConstraintInfo>();
            foreach (var definition in definitions)
            {
                foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(definition ?? string.Empty,
                    @"CONSTRAINT\s+(?:`((?:[^`]|``)+)`|(\w+))\s+CHECK\s+(.+?)(?=,\s*(?:CONSTRAINT|INDEX|PROJECTION)\s|\)\s*ENGINE\b|$)",
                    System.Text.RegularExpressions.RegexOptions.Singleline))
                {
                    var name=match.Groups[1].Success ? match.Groups[1].Value.Replace("``",
                        "`") : match.Groups[2].Value;
                    list.Add(new CheckConstraintInfo(name) { Expression = match.Groups[3].Value.Trim() });
                }
            }

            return list;
        }

        /// <summary>
        /// Maps a record into the row of a column of a key.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The row of the column of the key.</returns>
        private static (string Name, string Column, bool IsClustered) MapKeyColumn(IDataRecord r) => (Text(r,
            "ConstraintName"), Text(r,
            "ColumnName"), Flag(r,
            "IsClustered"));

        /// <summary>
        /// Maps the rows of the key columns into the primary key of the table.
        /// </summary>
        /// <param name="rows">The rows of the columns of the primary key.</param>
        /// <returns>The primary key, or <c>null</c> if the table has none.</returns>
        private static PrimaryKeyInfo MapPrimaryKey(IEnumerable<(string Name, string Column, bool IsClustered)> rows)
        {
            var list = rows.ToList();
            return list.Count == 0 ? null : new PrimaryKeyInfo(list[0].Name)
            {
                Columns = list.Select(x => x.Column).ToList(),
                IsClustered = list[0].IsClustered
            };
        }

        /// <summary>
        /// Maps the rows of the key columns into the unique constraints of the table.
        /// </summary>
        /// <param name="rows">The rows of the columns of the unique constraints.</param>
        /// <returns>The unique constraints.</returns>
        private static IList<UniqueConstraintInfo> MapUniqueConstraints(IEnumerable<(string Name, string Column, bool IsClustered)> rows) => rows.GroupBy(x => x.Name).Select(g => new UniqueConstraintInfo(g.Key) { Columns = g.Select(x => x.Column).ToList() }).ToList();

        /// <summary>
        /// Maps a record into the row of a column of an index.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The row of the column of the index.</returns>
        private static (string Name, bool IsUnique, bool IsClustered, string Filter, string Column, bool IsIncluded, bool IsDescending) MapIndexColumn(IDataRecord r) => (Text(r,
            "IndexName"), Flag(r,
            "IsUnique"), Flag(r,
            "IsClustered"), Text(r,
            "FilterDefinition"), Text(r,
            "ColumnName"), Flag(r,
            "IsIncluded"), Flag(r,
            "IsDescending"));

        /// <summary>
        /// Maps the rows of the index columns into the indexes of the table.
        /// </summary>
        /// <param name="rows">The rows of the columns of the indexes.</param>
        /// <returns>The indexes.</returns>
        private static IList<IndexInfo> MapIndexes(IEnumerable<(string Name, bool IsUnique, bool IsClustered, string Filter, string Column, bool IsIncluded, bool IsDescending)> rows) => rows.GroupBy(x => x.Name).Select(g => new IndexInfo(g.Key) { IsUnique = g.First().IsUnique, IsClustered = g.First().IsClustered, Filter = g.First().Filter, Columns = g.Where(x => !x.IsIncluded).Select(x => x.Column).ToList(), DescendingColumns = g.Where(x => !x.IsIncluded && x.IsDescending).Select(x => x.Column).ToList(), IncludedColumns = g.Where(x => x.IsIncluded).Select(x => x.Column).ToList() }).ToList();

        /// <summary>
        /// Maps a record into the row of a column of a foreign key.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The row of the column of the foreign key.</returns>
        private static (string Name, string Column, string RefSchema, string RefTable, string RefColumn, string Update, string Delete) MapForeignKeyColumn(IDataRecord r) => (Text(r,
            "ForeignKeyName"), Text(r,
            "ColumnName"), Text(r,
            "ReferencedSchema"), Text(r,
            "ReferencedTable"), Text(r,
            "ReferencedColumn"), Text(r,
            "UpdateAction"), Text(r,
            "DeleteAction"));

        /// <summary>
        /// Maps the rows of the foreign key columns into the foreign keys of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="rows">The rows of the columns of the foreign keys.</param>
        /// <returns>The foreign keys.</returns>
        private IList<ForeignKeyInfo> MapForeignKeys(IDbConnection connection,
            IEnumerable<(string Name, string Column, string RefSchema, string RefTable, string RefColumn, string Update, string Delete)> rows) => rows.GroupBy(x => x.Name).Select(g => new ForeignKeyInfo(g.Key) { Columns = g.Select(x => x.Column).ToList(), ReferencedTable = NewTable(connection,
                g.First().RefTable,
                g.First().RefSchema), ReferencedColumns = g.Select(x => x.RefColumn).ToList(), UpdateRule = ToRule(g.First().Update), DeleteRule = ToRule(g.First().Delete) }).ToList();

        /// <summary>
        /// Converts the action of the database into the rule of a foreign key.
        /// </summary>
        /// <param name="action">The action that is read from the database.</param>
        /// <returns>The rule of the foreign key.</returns>
        private static ForeignKeyRule ToRule(string action)
        {
            switch (action)
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

        /// <summary>
        /// Maps a record into a check constraint of the table.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The check constraint.</returns>
        private static CheckConstraintInfo MapCheckConstraint(IDataRecord r) => new CheckConstraintInfo(Text(r,
                "ConstraintName"))
        {
            Expression = Text(r,
                "Definition")
        };

        /// <summary>
        /// Maps a record into a relationship of two tables: the child table and the parent table that it references.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="r">The record.</param>
        /// <returns>The child table and the parent table.</returns>
        private (TableInfo Child, TableInfo Parent) MapRelationship(IDbConnection connection,
            IDataRecord r) => (NewTable(connection,
            Text(r,
                "ChildTable"),
            Text(r,
                "ChildSchema")), NewTable(connection,
            Text(r,
                "ParentTable"),
            Text(r,
                "ParentSchema")));
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
