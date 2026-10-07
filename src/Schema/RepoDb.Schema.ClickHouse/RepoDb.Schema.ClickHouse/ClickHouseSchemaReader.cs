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
    /// An <see cref="ISchemaReader"/> that reads the schema of the tables of a ClickHouse database from its <c>system</c> tables.
    /// The connection (and the optional transaction) that is passed to the constructor is used by all the operations.
    /// </summary>
    public class ClickHouseSchemaReader : ISchemaReader
    {
        #region Private Variables

        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly ClickHouseDbTypeNameToClientTypeResolver _typeResolver = new ClickHouseDbTypeNameToClientTypeResolver();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseSchemaReader"/> class.
        /// </summary>
        /// <param name="connection">The connection to the ClickHouse database to be read.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        public ClickHouseSchemaReader(IDbConnection connection,
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

            return new TableSchema(table, schema)
            {
                Columns = Query(ClickHouseSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, FullNameParameter(fullName)),
                PrimaryKey = MapPrimaryKey(Query(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, FullNameParameter(fullName), Parameter("Type", "PK"))),
                Indexes = MapIndexes(Query(ClickHouseSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, FullNameParameter(fullName))),
                ForeignKeys = MapForeignKeys(Query(ClickHouseSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, FullNameParameter(fullName))),
                UniqueConstraints = MapUniqueConstraints(Query(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, FullNameParameter(fullName), Parameter("Type", "UQ"))),
                CheckConstraints = ParseCheckConstraints(Query(ClickHouseSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapDefinition, FullNameParameter(fullName)))
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
            return Query(ClickHouseSchemaText.TableExistsSql, SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), FullNameParameter(FullName(schema, table))).FirstOrDefault() == 1;
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
            Query(ClickHouseSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, FullNameParameter(Resolve(tableName)));

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public PrimaryKeyInfo GetPrimaryKey(string tableName) =>
            MapPrimaryKey(Query(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, FullNameParameter(Resolve(tableName)), Parameter("Type", "PK")));

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="IndexInfo"/> objects of the table.</returns>
        public IEnumerable<IndexInfo> GetIndexes(string tableName) =>
            MapIndexes(Query(ClickHouseSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public IEnumerable<ForeignKeyInfo> GetForeignKeys(string tableName) =>
            MapForeignKeys(Query(ClickHouseSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<UniqueConstraintInfo> GetUniqueConstraints(string tableName) =>
            MapUniqueConstraints(Query(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, FullNameParameter(Resolve(tableName)), Parameter("Type", "UQ")));

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<CheckConstraintInfo> GetCheckConstraints(string tableName) =>
            ParseCheckConstraints(Query(ClickHouseSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapDefinition, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <returns>The names of the tables.</returns>
        public IEnumerable<string> GetTables(string schemaName = null) =>
            Query(ClickHouseSchemaText.TablesSql, SchemaTraceKeys.GetTables, r => ClickHouseSchemaHelper.FormatTableName(Text(r, "SchemaName"), Text(r, "TableName")), Parameter("SchemaName", schemaName));

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that its foreign keys reference. Use the order to create the tables, and the reverse of it to drop them.
        /// </summary>
        /// <param name="tableNames">The names of the tables to be ordered.</param>
        /// <returns>The ordered names of the tables.</returns>
        public IEnumerable<RelationshipInfo> GetDependencyOrder(IEnumerable<string> tableNames)
        {
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            return Order(names.Select(GetTableSchema).ToList());
        }

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="CopySchemaRelationshipBehavior.TableOnly"/> returns only the given tables.</param>
        /// <returns>The names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        public IEnumerable<string> GetRelatedTables(IEnumerable<string> tableNames,
            CopySchemaRelationshipBehavior relationshipBehavior)
        {
            var tables = (tableNames ?? throw new ArgumentNullException(nameof(tableNames)))
                .Select(name =>
                {
                    var (schema, table) = Resolve(name);
                    return new TableInfo(table, schema);
                })
                .ToList();
            var foreignKeys = relationshipBehavior == CopySchemaRelationshipBehavior.TableOnly
                ? new List<(TableInfo Child, TableInfo Parent)>()
                : Query(ClickHouseSchemaText.ForeignKeyRelationshipsSql, SchemaTraceKeys.GetRelationships, MapRelationship);
            return CopySchemaRelationshipExpander.Expand(tables, foreignKeys, relationshipBehavior, Key)
                .Select(table => ClickHouseSchemaHelper.FormatTableName(table.Schema, table.Name))
                .ToList();
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

            return new TableSchema(table, schema)
            {
                Columns = await QueryAsync(ClickHouseSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false),
                PrimaryKey = MapPrimaryKey(await QueryAsync(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, cancellationToken, FullNameParameter(fullName), Parameter("Type", "PK")).ConfigureAwait(false)),
                Indexes = MapIndexes(await QueryAsync(ClickHouseSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)),
                ForeignKeys = MapForeignKeys(await QueryAsync(ClickHouseSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)),
                UniqueConstraints = MapUniqueConstraints(await QueryAsync(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, cancellationToken, FullNameParameter(fullName), Parameter("Type", "UQ")).ConfigureAwait(false)),
                CheckConstraints = ParseCheckConstraints(await QueryAsync(ClickHouseSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapDefinition, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false))
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
            var result = await QueryAsync(ClickHouseSchemaText.TableExistsSql, SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), cancellationToken, FullNameParameter(FullName(schema, table))).ConfigureAwait(false);
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
            await QueryAsync(ClickHouseSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public async Task<PrimaryKeyInfo> GetPrimaryKeyAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapPrimaryKey(await QueryAsync(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false)), Parameter("Type", "PK")).ConfigureAwait(false));

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="IndexInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<IndexInfo>> GetIndexesAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapIndexes(await QueryAsync(ClickHouseSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ForeignKeyInfo>> GetForeignKeysAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapForeignKeys(await QueryAsync(ClickHouseSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<UniqueConstraintInfo>> GetUniqueConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapUniqueConstraints(await QueryAsync(ClickHouseSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false)), Parameter("Type", "UQ")).ConfigureAwait(false));

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<CheckConstraintInfo>> GetCheckConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            ParseCheckConstraints(await QueryAsync(ClickHouseSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapDefinition, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the tables.</returns>
        public async Task<IEnumerable<string>> GetTablesAsync(string schemaName = null,
            CancellationToken cancellationToken = default) =>
            await QueryAsync(ClickHouseSchemaText.TablesSql, SchemaTraceKeys.GetTables, r => ClickHouseSchemaHelper.FormatTableName(Text(r, "SchemaName"), Text(r, "TableName")), cancellationToken, Parameter("SchemaName", schemaName)).ConfigureAwait(false);

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that its foreign keys reference. Use the order to create the tables, and the reverse of it to drop them.
        /// </summary>
        /// <param name="tableNames">The names of the tables to be ordered.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the ordered names of the tables.</returns>
        public async Task<IEnumerable<RelationshipInfo>> GetDependencyOrderAsync(IEnumerable<string> tableNames,
            CancellationToken cancellationToken = default)
        {
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            var schemas = new List<TableSchema>();
            foreach (var name in names)
            {
                schemas.Add(await GetTableSchemaAsync(name, cancellationToken).ConfigureAwait(false));
            }
            return Order(schemas);
        }

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="CopySchemaRelationshipBehavior.TableOnly"/> returns only the given tables.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        public async Task<IEnumerable<string>> GetRelatedTablesAsync(IEnumerable<string> tableNames,
            CopySchemaRelationshipBehavior relationshipBehavior,
            CancellationToken cancellationToken = default)
        {
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            var tables = new List<TableInfo>();
            foreach (var name in names)
            {
                var (schema, table) = await ResolveAsync(name, cancellationToken).ConfigureAwait(false);
                tables.Add(new TableInfo(table, schema));
            }
            IEnumerable<(TableInfo Child, TableInfo Parent)> foreignKeys = new List<(TableInfo Child, TableInfo Parent)>();
            if (relationshipBehavior != CopySchemaRelationshipBehavior.TableOnly)
            {
                foreignKeys = await QueryAsync(ClickHouseSchemaText.ForeignKeyRelationshipsSql, SchemaTraceKeys.GetRelationships, MapRelationship, cancellationToken).ConfigureAwait(false);
            }
            return CopySchemaRelationshipExpander.Expand(tables, foreignKeys, relationshipBehavior, Key)
                .Select(table => ClickHouseSchemaHelper.FormatTableName(table.Schema, table.Name))
                .ToList();
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
        private static (string Schema, string Table) ParseTableName(string tableName) =>
            ClickHouseSchemaHelper.ParseSchemaAndTable(tableName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string FullName(string schema, string table) =>
            $"{schema}\u0001{table}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static string Key(TableInfo table) =>
            $"{table.Schema}\u0001{table.Name}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private (string Schema, string Table) Resolve(string tableName)
        {
            return ParseTableName(tableName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task<(string Schema, string Table)> ResolveAsync(string tableName,
            CancellationToken cancellationToken) =>
            Task.FromResult(ParseTableName(tableName));

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
            var (type, isNullable) = Unwrap(Text(r, "TypeName"));
            var kind = Text(r, "DefaultKind");
            var expression = Text(r, "DefaultDefinition");
            var isComputed = !string.IsNullOrEmpty(expression) && (kind == "MATERIALIZED" || kind == "ALIAS");
            var isDefault = !string.IsNullOrEmpty(expression) && kind == "DEFAULT";
            var baseName = BaseName(type);
            var arguments = Arguments(type);
            var field = new DbField(Text(r, "Name"),
                Flag(r, "IsPrimary"),
                false,
                isNullable,
                _typeResolver.Resolve(type) ?? typeof(object),
                baseName == "FixedString" && arguments.Length > 0 ? arguments[0] : 0,
                (byte)(baseName == "Decimal" && arguments.Length > 0 ? arguments[0] : 0),
                (byte)(baseName == "Decimal" && arguments.Length > 1 ? arguments[1] : baseName == "DateTime64" && arguments.Length > 0 ? arguments[0] : 0),
                IsWrapped(type) || !IsParameterized(baseName) ? type : baseName,
                isDefault,
                "ClickHouse");

            return new ColumnInfo
            {
                Field = field,
                Ordinal = Int(r, "Ordinal"),
                DefaultExpression = isDefault ? expression : null,
                ComputedExpression = isComputed ? expression : null
            };
        }

        /// <summary>
        /// Takes the <c>Nullable</c> wrapper off the type (it can be inside <c>LowCardinality</c>), and tells whether the type was nullable.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static (string Type, bool IsNullable) Unwrap(string type)
        {
            type = type.Trim();
            if (type.StartsWith("Nullable(", StringComparison.Ordinal) && type.EndsWith(")", StringComparison.Ordinal))
            {
                return (type.Substring("Nullable(".Length, type.Length - "Nullable(".Length - 1).Trim(), true);
            }
            if (type.StartsWith("LowCardinality(", StringComparison.Ordinal) && type.EndsWith(")", StringComparison.Ordinal))
            {
                var (inner, isNullable) = Unwrap(type.Substring("LowCardinality(".Length, type.Length - "LowCardinality(".Length - 1));
                return ("LowCardinality(" + inner + ")", isNullable);
            }
            return (type, false);
        }

        /// <summary>
        /// Gets the name of the type without its arguments (i.e.: <c>Decimal</c> for <c>Decimal(10, 2)</c>).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string BaseName(string type)
        {
            var index = type.IndexOf('(');
            return index < 0 ? type : type.Substring(0, index).Trim();
        }

        /// <summary>
        /// Gets the numeric arguments of the type (i.e.: 10 and 2 for <c>Decimal(10, 2)</c>).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int[] Arguments(string type)
        {
            var start = type.IndexOf('(');
            if (start < 0 || !type.EndsWith(")", StringComparison.Ordinal))
            {
                return new int[0];
            }
            var values = new List<int>();
            foreach (var item in type.Substring(start + 1, type.Length - start - 2).Split(','))
            {
                if (!int.TryParse(item.Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var value))
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
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsWrapped(string type) =>
            type.StartsWith("LowCardinality(", StringComparison.Ordinal);

        /// <summary>
        /// Checks whether the type takes its numbers from the fields of the column (they are not part of its database type).
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        private static bool IsParameterized(string baseName) =>
            baseName == "Decimal" || baseName == "DateTime64" || baseName == "FixedString";

        /// <summary>
        /// Gets the text of the definition.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static string MapDefinition(IDataRecord r) =>
            Text(r, "Definition");

        /// <summary>
        /// ClickHouse does not give the check constraints of a table in a system table, so they are parsed out of the statement that creates the table
        /// (i.e.: <c>CONSTRAINT ck_age CHECK Age &gt;= 0</c>).
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        private static IList<CheckConstraintInfo> ParseCheckConstraints(IList<string> definitions)
        {
            var list = new List<CheckConstraintInfo>();
            foreach (var definition in definitions)
            {
                foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(definition ?? string.Empty,
                    @"CONSTRAINT\s+(?:`((?:[^`]|``)+)`|(\w+))\s+CHECK\s+(.+?)(?=,\s*(?:CONSTRAINT|INDEX|PROJECTION)\s|\)\s*ENGINE\b|$)",
                    System.Text.RegularExpressions.RegexOptions.Singleline))
                {
                    var name = match.Groups[1].Success ? match.Groups[1].Value.Replace("``", "`") : match.Groups[2].Value;
                    list.Add(new CheckConstraintInfo(name) { Expression = match.Groups[3].Value.Trim() });
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static (string Name, string Column, bool IsClustered) MapKeyColumn(IDataRecord r) =>
            (Text(r, "ConstraintName"), Text(r, "ColumnName"), Flag(r, "IsClustered"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static PrimaryKeyInfo MapPrimaryKey(IEnumerable<(string Name, string Column, bool IsClustered)> rows)
        {
            var list = rows.ToList();
            return list.Count == 0
                ? null
                : new PrimaryKeyInfo(list[0].Name) { Columns = list.Select(x => x.Column).ToList(), IsClustered = list[0].IsClustered };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static IList<UniqueConstraintInfo> MapUniqueConstraints(IEnumerable<(string Name, string Column, bool IsClustered)> rows) =>
            rows.GroupBy(x => x.Name)
                .Select(g => new UniqueConstraintInfo(g.Key) { Columns = g.Select(x => x.Column).ToList() })
                .ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static (string Name, bool IsUnique, bool IsClustered, string Filter, string Column, bool IsIncluded, bool IsDescending) MapIndexColumn(IDataRecord r) =>
            (Text(r, "IndexName"), Flag(r, "IsUnique"), Flag(r, "IsClustered"), Text(r, "FilterDefinition"), Text(r, "ColumnName"), Flag(r, "IsIncluded"), Flag(r, "IsDescending"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static IList<IndexInfo> MapIndexes(IEnumerable<(string Name, bool IsUnique, bool IsClustered, string Filter, string Column, bool IsIncluded, bool IsDescending)> rows) =>
            rows.GroupBy(x => x.Name)
                .Select(g => new IndexInfo(g.Key)
                {
                    IsUnique = g.First().IsUnique,
                    IsClustered = g.First().IsClustered,
                    Filter = g.First().Filter,
                    Columns = g.Where(x => !x.IsIncluded).Select(x => x.Column).ToList(),
                    DescendingColumns = g.Where(x => !x.IsIncluded && x.IsDescending).Select(x => x.Column).ToList(),
                    IncludedColumns = g.Where(x => x.IsIncluded).Select(x => x.Column).ToList()
                })
                .ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static (string Name, string Column, string RefSchema, string RefTable, string RefColumn, string Update, string Delete) MapForeignKeyColumn(IDataRecord r) =>
            (Text(r, "ForeignKeyName"), Text(r, "ColumnName"), Text(r, "ReferencedSchema"), Text(r, "ReferencedTable"), Text(r, "ReferencedColumn"), Text(r, "UpdateAction"), Text(r, "DeleteAction"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static IList<ForeignKeyInfo> MapForeignKeys(IEnumerable<(string Name, string Column, string RefSchema, string RefTable, string RefColumn, string Update, string Delete)> rows) =>
            rows.GroupBy(x => x.Name)
                .Select(g => new ForeignKeyInfo(g.Key)
                {
                    Columns = g.Select(x => x.Column).ToList(),
                    ReferencedTable = new TableInfo(g.First().RefTable, g.First().RefSchema),
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
        private static CopySchemaForeignKeyRule ToRule(string action)
        {
            switch (action)
            {
                case "CASCADE": return CopySchemaForeignKeyRule.Cascade;
                case "SET NULL": return CopySchemaForeignKeyRule.SetNull;
                case "SET DEFAULT": return CopySchemaForeignKeyRule.SetDefault;
                case "RESTRICT": return CopySchemaForeignKeyRule.Restrict;
                default: return CopySchemaForeignKeyRule.NoAction;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static CheckConstraintInfo MapCheckConstraint(IDataRecord r) =>
            new CheckConstraintInfo(Text(r, "ConstraintName")) { Expression = Text(r, "Definition") };

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static (TableInfo Child, TableInfo Parent) MapRelationship(IDataRecord r) =>
            (new TableInfo(Text(r, "ChildTable"), Text(r, "ChildSchema")), new TableInfo(Text(r, "ParentTable"), Text(r, "ParentSchema")));

        // Ordering

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that it references (see <see cref="CopySchemaOrder"/>).
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered relationships, one per table.</returns>
        internal static IList<RelationshipInfo> Order(IList<TableSchema> schemas) =>
            CopySchemaOrder.Order(schemas, schema => Key(schema.Table), Key);

        #endregion
    }
}
