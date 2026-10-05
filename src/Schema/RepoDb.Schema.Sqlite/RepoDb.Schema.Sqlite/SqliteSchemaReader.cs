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
    /// An <see cref="ISchemaReader"/> that reads the schema of the tables of a SQLite database from the <c>sqlite_master</c> table and the <c>pragma</c> functions.
    /// The objects that SQLite does not expose through its functions (the names of the constraints, the check constraints, the expressions of the generated columns
    /// and the filters of the indexes) are read from the SQL text that created them.
    /// The connection (and the optional transaction) that is passed to the constructor is used by all the operations.
    /// A schema is an attached database (the tables of the main database have no schema).
    /// </summary>
    public class SqliteSchemaReader : ISchemaReader
    {
        #region Private Variables

        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly MdsSqLiteDbTypeNameToClientTypeResolver _typeResolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SqliteSchemaReader"/> class.
        /// </summary>
        /// <param name="connection">The connection to the SQLite database to be read.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        public SqliteSchemaReader(IDbConnection connection,
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
            return Build(ReadTable(schema, table));
        }

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public bool TableExists(string tableName)
        {
            var (schema, table) = Resolve(tableName);
            return Query(SqliteSchemaText.TableExistsSql(Schema(schema)), SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), Parameter("Table", table)).FirstOrDefault() > 0;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The name of the owning schema, or <c>null</c> if the table is in the main database.</returns>
        public string GetSchemaName(string tableName) =>
            Resolve(tableName).Schema;

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ColumnInfo"/> objects of the table.</returns>
        public IEnumerable<ColumnInfo> GetColumns(string tableName) =>
            GetTableSchema(tableName).Columns;

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public PrimaryKeyInfo GetPrimaryKey(string tableName) =>
            GetTableSchema(tableName).PrimaryKey;

        /// <summary>
        /// Gets the indexes of the table, except the ones that back the primary key and the unique constraints.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="IndexInfo"/> objects of the table.</returns>
        public IEnumerable<IndexInfo> GetIndexes(string tableName) =>
            GetTableSchema(tableName).Indexes;

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public IEnumerable<ForeignKeyInfo> GetForeignKeys(string tableName) =>
            GetTableSchema(tableName).ForeignKeys;

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<UniqueConstraintInfo> GetUniqueConstraints(string tableName) =>
            GetTableSchema(tableName).UniqueConstraints;

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<CheckConstraintInfo> GetCheckConstraints(string tableName) =>
            GetTableSchema(tableName).CheckConstraints;

        /// <summary>
        /// Gets the names of the tables of a schema (the tables of the engine are not included).
        /// </summary>
        /// <param name="schemaName">The name of the schema (an attached database) to be read. The default is <c>null</c>, which reads the main database.</param>
        /// <returns>The names of the tables.</returns>
        public IEnumerable<string> GetTables(string schemaName = null)
        {
            var schema = NormalizeSchema(schemaName);
            if (schema != null && !Query(SqliteSchemaText.DatabasesSql, SchemaTraceKeys.GetTables, r => Text(r, "SchemaName")).Contains(schema, StringComparer.OrdinalIgnoreCase))
            {
                return new List<string>();
            }
            return Query(SqliteSchemaText.TablesSql(Schema(schema)), SchemaTraceKeys.GetTables, r => Helper.Format(schema, Text(r, "TableName")));
        }

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
            var foreignKeys = new List<(TableInfo Child, TableInfo Parent)>();
            if (relationshipBehavior != CopySchemaRelationshipBehavior.TableOnly)
            {
                foreach (var schema in Query(SqliteSchemaText.DatabasesSql, SchemaTraceKeys.GetTables, r => NormalizeSchema(Text(r, "SchemaName"))))
                {
                    foreignKeys.AddRange(Query(SqliteSchemaText.RelationshipsSql(Schema(schema)),
                        SchemaTraceKeys.GetRelationships,
                        r => (new TableInfo(Text(r, "ChildTable"), schema), new TableInfo(Text(r, "ParentTable"), schema)),
                        Parameter("Schema", schema ?? Helper.MainSchema)));
                }
            }
            return CopySchemaRelationshipExpander.Expand(tables, foreignKeys, relationshipBehavior, Key)
                .Select(table => Helper.Format(table.Schema, table.Name))
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
            var (schema, table) = Resolve(tableName);
            return Build(await ReadTableAsync(schema, table, cancellationToken).ConfigureAwait(false));
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
            var (schema, table) = Resolve(tableName);
            var result = await QueryAsync(SqliteSchemaText.TableExistsSql(Schema(schema)), SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), cancellationToken, Parameter("Table", table)).ConfigureAwait(false);
            return result.FirstOrDefault() > 0;
        }

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the name of the owning schema, or <c>null</c> if the table is in the main database.</returns>
        public Task<string> GetSchemaNameAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Resolve(tableName).Schema);

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ColumnInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ColumnInfo>> GetColumnsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            (await GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).Columns;

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public async Task<PrimaryKeyInfo> GetPrimaryKeyAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            (await GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).PrimaryKey;

        /// <summary>
        /// Gets the indexes of the table, except the ones that back the primary key and the unique constraints.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="IndexInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<IndexInfo>> GetIndexesAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            (await GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).Indexes;

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ForeignKeyInfo>> GetForeignKeysAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            (await GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).ForeignKeys;

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<UniqueConstraintInfo>> GetUniqueConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            (await GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).UniqueConstraints;

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<CheckConstraintInfo>> GetCheckConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            (await GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).CheckConstraints;

        /// <summary>
        /// Gets the names of the tables of a schema (the tables of the engine are not included).
        /// </summary>
        /// <param name="schemaName">The name of the schema (an attached database) to be read. The default is <c>null</c>, which reads the main database.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the tables.</returns>
        public async Task<IEnumerable<string>> GetTablesAsync(string schemaName = null,
            CancellationToken cancellationToken = default)
        {
            var schema = NormalizeSchema(schemaName);
            if (schema != null)
            {
                var schemas = await QueryAsync(SqliteSchemaText.DatabasesSql, SchemaTraceKeys.GetTables, r => Text(r, "SchemaName"), cancellationToken).ConfigureAwait(false);
                if (!schemas.Contains(schema, StringComparer.OrdinalIgnoreCase))
                {
                    return new List<string>();
                }
            }
            return await QueryAsync(SqliteSchemaText.TablesSql(Schema(schema)), SchemaTraceKeys.GetTables, r => Helper.Format(schema, Text(r, "TableName")), cancellationToken).ConfigureAwait(false);
        }

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
            var tables = (tableNames ?? throw new ArgumentNullException(nameof(tableNames)))
                .Select(name =>
                {
                    var (schema, table) = Resolve(name);
                    return new TableInfo(table, schema);
                })
                .ToList();
            var foreignKeys = new List<(TableInfo Child, TableInfo Parent)>();
            if (relationshipBehavior != CopySchemaRelationshipBehavior.TableOnly)
            {
                var schemas = await QueryAsync(SqliteSchemaText.DatabasesSql, SchemaTraceKeys.GetTables, r => NormalizeSchema(Text(r, "SchemaName")), cancellationToken).ConfigureAwait(false);
                foreach (var schema in schemas)
                {
                    var rows = await QueryAsync(SqliteSchemaText.RelationshipsSql(Schema(schema)),
                        SchemaTraceKeys.GetRelationships,
                        r => (new TableInfo(Text(r, "ChildTable"), schema), new TableInfo(Text(r, "ParentTable"), schema)),
                        cancellationToken,
                        Parameter("Schema", schema ?? Helper.MainSchema)).ConfigureAwait(false);
                    foreignKeys.AddRange(rows);
                }
            }
            return CopySchemaRelationshipExpander.Expand(tables, foreignKeys, relationshipBehavior, Key)
                .Select(table => Helper.Format(table.Schema, table.Name))
                .ToList();
        }

        #endregion

        #endregion

        #region Helpers

        // Names

        /// <summary>
        /// Gets the key of the table: the names of SQLite are case insensitive.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string Key(TableInfo table) =>
            $"{table.Schema}\u0001{table.Name}".ToLowerInvariant();

        /// <summary>
        /// Gets the schema of a name: <c>null</c> for the main database.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string NormalizeSchema(string schema) =>
            string.IsNullOrWhiteSpace(schema) || string.Equals(schema, Helper.MainSchema, StringComparison.OrdinalIgnoreCase) ? null : schema;

        /// <summary>
        /// Gets the quoted name of the schema, to be used in the SQL text.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string Schema(string schema) =>
            Helper.Quote(schema ?? Helper.MainSchema);

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static (string Schema, string Table) Resolve(string tableName) =>
            Helper.Parse(tableName);

        // Parameters

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static KeyValuePair<string, object> Parameter(string name, object value) =>
            new KeyValuePair<string, object>(name, value);

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

        // Reading

        /// <summary>
        /// Reads everything that is needed to build the schema of the table (the sync version).
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private RawTable ReadTable(string schema,
            string table)
        {
            var quoted = Schema(schema);
            var arguments = new[] { Parameter("Table", table), Parameter("Schema", schema ?? Helper.MainSchema) };
            var raw = new RawTable { Schema = schema, Table = table };

            var master = Query(SqliteSchemaText.TableSql(quoted), SchemaTraceKeys.GetColumns, r => (Name: Text(r, "TableName"), Definition: Text(r, "Definition")), arguments[0]).FirstOrDefault();
            if (master.Name == null)
            {
                return raw;
            }
            raw.Exists = true;
            raw.Table = master.Name;
            raw.Definition = master.Definition;
            raw.Columns = Query(SqliteSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, arguments);
            raw.Indexes = Query(SqliteSchemaText.IndexListSql, SchemaTraceKeys.GetIndexes, MapIndex, arguments);
            var definitions = Query(SqliteSchemaText.IndexDefinitionsSql(quoted), SchemaTraceKeys.GetIndexes, r => (Name: Text(r, "IndexName"), Definition: Text(r, "Definition")), arguments[0]);
            foreach (var index in raw.Indexes)
            {
                index.Definition = definitions.FirstOrDefault(d => string.Equals(d.Name, index.Name, StringComparison.OrdinalIgnoreCase)).Definition;
                index.Columns = Query(SqliteSchemaText.IndexColumnsSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, Parameter("Index", index.Name), arguments[1]);
            }
            raw.ForeignKeys = Query(SqliteSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKey, arguments);
            foreach (var referenced in raw.ForeignKeys.Where(f => f.ReferencedColumn == null).Select(f => f.ReferencedTable).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                raw.ReferencedPrimaryKeys[referenced] = Query(SqliteSchemaText.PrimaryKeyColumnsSql, SchemaTraceKeys.GetPrimaryKey, r => Text(r, "ColumnName"), Parameter("Table", referenced), arguments[1]).ToList();
            }
            return raw;
        }

        /// <summary>
        /// Reads everything that is needed to build the schema of the table (the async version).
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<RawTable> ReadTableAsync(string schema,
            string table,
            CancellationToken cancellationToken)
        {
            var quoted = Schema(schema);
            var arguments = new[] { Parameter("Table", table), Parameter("Schema", schema ?? Helper.MainSchema) };
            var raw = new RawTable { Schema = schema, Table = table };

            var masters = await QueryAsync(SqliteSchemaText.TableSql(quoted), SchemaTraceKeys.GetColumns, r => (Name: Text(r, "TableName"), Definition: Text(r, "Definition")), cancellationToken, arguments[0]).ConfigureAwait(false);
            var master = masters.FirstOrDefault();
            if (master.Name == null)
            {
                return raw;
            }
            raw.Exists = true;
            raw.Table = master.Name;
            raw.Definition = master.Definition;
            raw.Columns = await QueryAsync(SqliteSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, cancellationToken, arguments).ConfigureAwait(false);
            raw.Indexes = await QueryAsync(SqliteSchemaText.IndexListSql, SchemaTraceKeys.GetIndexes, MapIndex, cancellationToken, arguments).ConfigureAwait(false);
            var definitions = await QueryAsync(SqliteSchemaText.IndexDefinitionsSql(quoted), SchemaTraceKeys.GetIndexes, r => (Name: Text(r, "IndexName"), Definition: Text(r, "Definition")), cancellationToken, arguments[0]).ConfigureAwait(false);
            foreach (var index in raw.Indexes)
            {
                index.Definition = definitions.FirstOrDefault(d => string.Equals(d.Name, index.Name, StringComparison.OrdinalIgnoreCase)).Definition;
                index.Columns = await QueryAsync(SqliteSchemaText.IndexColumnsSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, cancellationToken, Parameter("Index", index.Name), arguments[1]).ConfigureAwait(false);
            }
            raw.ForeignKeys = await QueryAsync(SqliteSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKey, cancellationToken, arguments).ConfigureAwait(false);
            foreach (var referenced in raw.ForeignKeys.Where(f => f.ReferencedColumn == null).Select(f => f.ReferencedTable).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var columns = await QueryAsync(SqliteSchemaText.PrimaryKeyColumnsSql, SchemaTraceKeys.GetPrimaryKey, r => Text(r, "ColumnName"), cancellationToken, Parameter("Table", referenced), arguments[1]).ConfigureAwait(false);
                raw.ReferencedPrimaryKeys[referenced] = columns.ToList();
            }
            return raw;
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
        /// <returns></returns>
        private static RawColumn MapColumn(IDataRecord r) =>
            new RawColumn
            {
                Name = Text(r, "Name"),
                DeclaredType = Text(r, "DeclaredType") ?? string.Empty,
                NotNull = Int(r, "IsNotNull") != 0,
                Default = Text(r, "DefaultDefinition"),
                PrimaryKeyPosition = Int(r, "PrimaryKeyPosition"),
                Hidden = Int(r, "Hidden")
            };

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static RawIndex MapIndex(IDataRecord r) =>
            new RawIndex
            {
                Name = Text(r, "IndexName"),
                IsUnique = Int(r, "IsUnique") != 0,
                Origin = Text(r, "Origin"),
                IsPartial = Int(r, "IsPartial") != 0
            };

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static RawIndexColumn MapIndexColumn(IDataRecord r) =>
            new RawIndexColumn
            {
                Name = Text(r, "ColumnName"),
                IsDescending = Int(r, "IsDescending") != 0,
                IsKey = Int(r, "IsKey") != 0
            };

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static RawForeignKey MapForeignKey(IDataRecord r) =>
            new RawForeignKey
            {
                Id = Int(r, "Id"),
                ReferencedTable = Text(r, "ReferencedTable"),
                Column = Text(r, "ColumnName"),
                ReferencedColumn = Text(r, "ReferencedColumn"),
                Update = Text(r, "UpdateAction"),
                Delete = Text(r, "DeleteAction")
            };

        // Building

        /// <summary>
        /// Builds the schema of the table from what was read.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private TableSchema Build(RawTable raw)
        {
            var schema = new TableSchema(raw.Table, raw.Schema);
            if (!raw.Exists)
            {
                return schema;
            }

            var definition = SqliteSchemaParser.ParseTable(raw.Definition);
            var primaryColumns = raw.Columns.Where(c => c.PrimaryKeyPosition > 0).OrderBy(c => c.PrimaryKeyPosition).Select(c => c.Name).ToList();

            schema.Columns = BuildColumns(raw, definition, primaryColumns);
            schema.PrimaryKey = BuildPrimaryKey(raw, definition, primaryColumns);
            schema.Indexes = BuildIndexes(raw);
            schema.UniqueConstraints = BuildUniqueConstraints(raw, definition);
            schema.ForeignKeys = BuildForeignKeys(raw, definition);
            schema.CheckConstraints = BuildCheckConstraints(raw, definition);
            return schema;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="definition"></param>
        /// <param name="primaryColumns"></param>
        /// <returns></returns>
        private IList<ColumnInfo> BuildColumns(RawTable raw,
            SqliteTableDefinition definition,
            IList<string> primaryColumns)
        {
            var columns = new List<ColumnInfo>();
            foreach (var column in raw.Columns)
            {
                definition.Columns.TryGetValue(column.Name, out var parsed);
                var (typeName, arguments) = SqliteSchemaParser.ParseType(column.DeclaredType);
                var isNumeric = IsNumeric(typeName);
                var isPrimary = column.PrimaryKeyPosition > 0;
                var isIdentity = isPrimary && primaryColumns.Count == 1 && string.Equals(typeName, "integer", StringComparison.Ordinal);
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
                    "SQLite");
                columns.Add(new ColumnInfo
                {
                    Field = field,
                    Ordinal = columns.Count + 1,
                    DefaultExpression = column.Default,
                    ComputedExpression = column.Hidden >= 2 ? parsed?.GeneratedExpression : null,
                    Collation = parsed?.Collation
                });
            }
            return columns;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="definition"></param>
        /// <param name="primaryColumns"></param>
        /// <returns></returns>
        private static PrimaryKeyInfo BuildPrimaryKey(RawTable raw,
            SqliteTableDefinition definition,
            IList<string> primaryColumns)
        {
            if (primaryColumns.Count == 0)
            {
                return null;
            }
            var name = definition.Find(SqliteConstraintKind.PrimaryKey, primaryColumns)?.Name ?? $"PK_{raw.Table}";
            return new PrimaryKeyInfo(name) { Columns = primaryColumns.ToList(), IsClustered = true };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private static IList<IndexInfo> BuildIndexes(RawTable raw) =>
            raw.Indexes
                .Where(index => string.Equals(index.Origin, "c", StringComparison.Ordinal))
                .OrderBy(index => index.Name, StringComparer.Ordinal)
                .Select(index =>
                {
                    var keys = index.Columns.Where(c => c.IsKey && c.Name != null).ToList();
                    return new IndexInfo(index.Name)
                    {
                        IsUnique = index.IsUnique,
                        IsClustered = false,
                        Filter = index.IsPartial ? SqliteSchemaParser.ParseIndexFilter(index.Definition) : null,
                        Columns = keys.Select(c => c.Name).ToList(),
                        DescendingColumns = keys.Where(c => c.IsDescending).Select(c => c.Name).ToList()
                    };
                })
                .ToList();

        /// <summary>
        ///
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="definition"></param>
        /// <returns></returns>
        private static IList<UniqueConstraintInfo> BuildUniqueConstraints(RawTable raw,
            SqliteTableDefinition definition) =>
            raw.Indexes
                .Where(index => string.Equals(index.Origin, "u", StringComparison.Ordinal))
                .Select(index =>
                {
                    var columns = index.Columns.Where(c => c.IsKey && c.Name != null).Select(c => c.Name).ToList();
                    return new UniqueConstraintInfo(definition.Find(SqliteConstraintKind.Unique, columns)?.Name ?? index.Name) { Columns = columns };
                })
                .OrderBy(constraint => constraint.Name, StringComparer.Ordinal)
                .ToList();

        /// <summary>
        ///
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="definition"></param>
        /// <returns></returns>
        private static IList<ForeignKeyInfo> BuildForeignKeys(RawTable raw,
            SqliteTableDefinition definition) =>
            raw.ForeignKeys
                .GroupBy(f => f.Id)
                .Select(group =>
                {
                    var first = group.First();
                    var columns = group.Select(f => f.Column).ToList();
                    var referencedColumns = first.ReferencedColumn == null
                        ? raw.ReferencedPrimaryKeys.TryGetValue(first.ReferencedTable, out var primary) ? primary : new List<string>()
                        : group.Select(f => f.ReferencedColumn).ToList();
                    return new ForeignKeyInfo(definition.Find(SqliteConstraintKind.ForeignKey, columns)?.Name ?? $"FK_{raw.Table}_{string.Join("_", columns)}")
                    {
                        Columns = columns,
                        ReferencedTable = new TableInfo(first.ReferencedTable, raw.Schema),
                        ReferencedColumns = referencedColumns,
                        UpdateRule = ToRule(first.Update),
                        DeleteRule = ToRule(first.Delete)
                    };
                })
                .OrderBy(foreignKey => foreignKey.Name, StringComparer.Ordinal)
                .ToList();

        /// <summary>
        ///
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="definition"></param>
        /// <returns></returns>
        private static IList<CheckConstraintInfo> BuildCheckConstraints(RawTable raw,
            SqliteTableDefinition definition)
        {
            var checks = definition.Constraints.Where(c => c.Kind == SqliteConstraintKind.Check).ToList();
            return checks
                .Select((check, i) => new CheckConstraintInfo(check.Name ?? $"CK_{raw.Table}_{i + 1}") { Expression = check.Expression })
                .ToList();
        }

        /// <summary>
        /// Checks whether the arguments of the type are a precision and a scale (and not a size).
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static bool IsNumeric(string typeName) =>
            typeName == "decimal" || typeName == "numeric" || typeName == "dec" || typeName == "float" || typeName == "double" || typeName == "real";

        /// <summary>
        /// Gets the client type of a declared type, with the rules of the type affinity of SQLite.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static CopySchemaForeignKeyRule ToRule(string action)
        {
            switch (action?.ToUpperInvariant())
            {
                case "CASCADE": return CopySchemaForeignKeyRule.Cascade;
                case "SET NULL": return CopySchemaForeignKeyRule.SetNull;
                case "SET DEFAULT": return CopySchemaForeignKeyRule.SetDefault;
                case "RESTRICT": return CopySchemaForeignKeyRule.Restrict;
                default: return CopySchemaForeignKeyRule.NoAction;
            }
        }

        // Ordering

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that it references (see <see cref="CopySchemaOrder"/>).
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered relationships, one per table.</returns>
        internal static IList<RelationshipInfo> Order(IList<TableSchema> schemas) =>
            CopySchemaOrder.Order(schemas, schema => Key(schema.Table), Key);

        #endregion

        #region Types

        /// <summary>
        /// What was read from the database about a table.
        /// </summary>
        private sealed class RawTable
        {
            public string Schema { get; set; }

            public string Table { get; set; }

            public bool Exists { get; set; }

            public string Definition { get; set; }

            public IList<RawColumn> Columns { get; set; } = new List<RawColumn>();

            public IList<RawIndex> Indexes { get; set; } = new List<RawIndex>();

            public IList<RawForeignKey> ForeignKeys { get; set; } = new List<RawForeignKey>();

            public IDictionary<string, List<string>> ReferencedPrimaryKeys { get; } = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        }

        private sealed class RawColumn
        {
            public string Name { get; set; }

            public string DeclaredType { get; set; }

            public bool NotNull { get; set; }

            public string Default { get; set; }

            public int PrimaryKeyPosition { get; set; }

            public int Hidden { get; set; }
        }

        private sealed class RawIndex
        {
            public string Name { get; set; }

            public bool IsUnique { get; set; }

            public string Origin { get; set; }

            public bool IsPartial { get; set; }

            public string Definition { get; set; }

            public IList<RawIndexColumn> Columns { get; set; } = new List<RawIndexColumn>();
        }

        private sealed class RawIndexColumn
        {
            public string Name { get; set; }

            public bool IsDescending { get; set; }

            public bool IsKey { get; set; }
        }

        private sealed class RawForeignKey
        {
            public int Id { get; set; }

            public string ReferencedTable { get; set; }

            public string Column { get; set; }

            public string ReferencedColumn { get; set; }

            public string Update { get; set; }

            public string Delete { get; set; }
        }

        #endregion
    }
}
