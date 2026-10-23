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
    /// An <see cref="ISchemaReader"/> that reads the schema of the tables of a DuckDb database from its <c>duckdb_*()</c> catalog functions.
    /// DuckDB does not keep the names of the constraints, so it generates them. The connection (and the optional transaction) that is passed to the constructor is used by all the operations.
    /// </summary>
    public class DuckDbSchemaReader : ISchemaReader
    {
        #region Private Variables

        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly DuckDbTypeNameToClientTypeResolver _typeResolver = new DuckDbTypeNameToClientTypeResolver();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="DuckDbSchemaReader"/> class.
        /// </summary>
        /// <param name="connection">The connection to the DuckDb database to be read.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        public DuckDbSchemaReader(IDbConnection connection,
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

            return new TableSchema(NewTable(table, schema))
            {
                Columns = MapComputedColumns(Query(DuckDbSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, FullNameParameter(fullName))),
                PrimaryKey = MapPrimaryKey(Query(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, FullNameParameter(fullName), Parameter("KeyType", "PRIMARY KEY"))),
                Indexes = MapIndexes(Query(DuckDbSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, FullNameParameter(fullName))),
                ForeignKeys = MapForeignKeys(Query(DuckDbSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, FullNameParameter(fullName))),
                UniqueConstraints = MapUniqueConstraints(Query(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, FullNameParameter(fullName), Parameter("KeyType", "UNIQUE"))),
                CheckConstraints = Query(DuckDbSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, FullNameParameter(fullName))
            };
        }

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public bool TableExists(string tableName)
        {
            return Query(DuckDbSchemaText.TableExistsSql, SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), FullNameParameter(Resolve(tableName))).FirstOrDefault() == 1;
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
            MapComputedColumns(Query(DuckDbSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public PrimaryKeyInfo GetPrimaryKey(string tableName) =>
            MapPrimaryKey(Query(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, FullNameParameter(Resolve(tableName)), Parameter("KeyType", "PRIMARY KEY")));

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="IndexInfo"/> objects of the table.</returns>
        public IEnumerable<IndexInfo> GetIndexes(string tableName) =>
            MapIndexes(Query(DuckDbSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public IEnumerable<ForeignKeyInfo> GetForeignKeys(string tableName) =>
            MapForeignKeys(Query(DuckDbSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, FullNameParameter(Resolve(tableName))));

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<UniqueConstraintInfo> GetUniqueConstraints(string tableName) =>
            MapUniqueConstraints(Query(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, FullNameParameter(Resolve(tableName)), Parameter("KeyType", "UNIQUE")));

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public IEnumerable<CheckConstraintInfo> GetCheckConstraints(string tableName) =>
            Query(DuckDbSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, FullNameParameter(Resolve(tableName)));

        /// <inheritdoc />
        public IEnumerable<SchemaInfo> GetSchemas() =>
            Query(DuckDbSchemaText.SchemasSql, SchemaTraceKeys.GetSchemas, r => new SchemaInfo(Text(r, "SchemaName")));

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads the current schema of the connection.</param>
        /// <returns>The names of the tables.</returns>
        public IEnumerable<string> GetTables(string schemaName = null) =>
            Query(DuckDbSchemaText.TablesSql, SchemaTraceKeys.GetTables, r => DuckDbSchemaHelper.FormatTableName(Text(r, "SchemaName"), Text(r, "TableName")), Parameter("SchemaName", schemaName));

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
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="RelatedTableBehavior.TableOnly"/> returns only the given tables.</param>
        /// <returns>The names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        public IEnumerable<string> GetRelatedTables(IEnumerable<string> tableNames,
            RelatedTableBehavior relationshipBehavior)
        {
            var tables = (tableNames ?? throw new ArgumentNullException(nameof(tableNames)))
                .Select(name =>
                {
                    var (schema, table) = Resolve(name);
                    return NewTable(table, schema);
                })
                .ToList();
            var foreignKeys = relationshipBehavior == RelatedTableBehavior.TableOnly
                ? new List<(TableInfo Child, TableInfo Parent)>()
                : Query(DuckDbSchemaText.ForeignKeyRelationshipsSql, SchemaTraceKeys.GetRelationships, MapRelationship);
            return RelatedTableExpander.Expand(tables, foreignKeys, relationshipBehavior, Key)
                .Select(table => DuckDbSchemaHelper.FormatTableName(table.Schema, table.Name))
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

            return new TableSchema(NewTable(table, schema))
            {
                Columns = MapComputedColumns(await QueryAsync(DuckDbSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)),
                PrimaryKey = MapPrimaryKey(await QueryAsync(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, cancellationToken, FullNameParameter(fullName), Parameter("KeyType", "PRIMARY KEY")).ConfigureAwait(false)),
                Indexes = MapIndexes(await QueryAsync(DuckDbSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)),
                ForeignKeys = MapForeignKeys(await QueryAsync(DuckDbSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)),
                UniqueConstraints = MapUniqueConstraints(await QueryAsync(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, cancellationToken, FullNameParameter(fullName), Parameter("KeyType", "UNIQUE")).ConfigureAwait(false)),
                CheckConstraints = await QueryAsync(DuckDbSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, cancellationToken, FullNameParameter(fullName)).ConfigureAwait(false)
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
            var result = await QueryAsync(DuckDbSchemaText.TableExistsSql, SchemaTraceKeys.TableExists, r => Convert.ToInt32(r[0]), cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);
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
            MapComputedColumns(await QueryAsync(DuckDbSchemaText.ColumnsSql, SchemaTraceKeys.GetColumns, MapColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        public async Task<PrimaryKeyInfo> GetPrimaryKeyAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapPrimaryKey(await QueryAsync(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetPrimaryKey, MapKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false)), Parameter("KeyType", "PRIMARY KEY")).ConfigureAwait(false));

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="IndexInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<IndexInfo>> GetIndexesAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapIndexes(await QueryAsync(DuckDbSchemaText.IndexesSql, SchemaTraceKeys.GetIndexes, MapIndexColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<ForeignKeyInfo>> GetForeignKeysAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapForeignKeys(await QueryAsync(DuckDbSchemaText.ForeignKeysSql, SchemaTraceKeys.GetForeignKeys, MapForeignKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false));

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<UniqueConstraintInfo>> GetUniqueConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            MapUniqueConstraints(await QueryAsync(DuckDbSchemaText.KeyConstraintSql, SchemaTraceKeys.GetUniqueConstraints, MapKeyColumn, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false)), Parameter("KeyType", "UNIQUE")).ConfigureAwait(false));

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        public async Task<IEnumerable<CheckConstraintInfo>> GetCheckConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default) =>
            await QueryAsync(DuckDbSchemaText.CheckConstraintsSql, SchemaTraceKeys.GetCheckConstraints, MapCheckConstraint, cancellationToken, FullNameParameter(await ResolveFullNameAsync(tableName, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IEnumerable<SchemaInfo>> GetSchemasAsync(CancellationToken cancellationToken = default) =>
            await QueryAsync(DuckDbSchemaText.SchemasSql, SchemaTraceKeys.GetSchemas, r => new SchemaInfo(Text(r, "SchemaName")), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads the current schema of the connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the tables.</returns>
        public async Task<IEnumerable<string>> GetTablesAsync(string schemaName = null,
            CancellationToken cancellationToken = default) =>
            await QueryAsync(DuckDbSchemaText.TablesSql, SchemaTraceKeys.GetTables, r => DuckDbSchemaHelper.FormatTableName(Text(r, "SchemaName"), Text(r, "TableName")), cancellationToken, Parameter("SchemaName", schemaName)).ConfigureAwait(false);

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
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="RelatedTableBehavior.TableOnly"/> returns only the given tables.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        public async Task<IEnumerable<string>> GetRelatedTablesAsync(IEnumerable<string> tableNames,
            RelatedTableBehavior relationshipBehavior,
            CancellationToken cancellationToken = default)
        {
            var names = (tableNames ?? throw new ArgumentNullException(nameof(tableNames))).ToList();
            var tables = new List<TableInfo>();
            foreach (var name in names)
            {
                var (schema, table) = await ResolveAsync(name, cancellationToken).ConfigureAwait(false);
                tables.Add(NewTable(table, schema));
            }
            IEnumerable<(TableInfo Child, TableInfo Parent)> foreignKeys = new List<(TableInfo Child, TableInfo Parent)>();
            if (relationshipBehavior != RelatedTableBehavior.TableOnly)
            {
                foreignKeys = await QueryAsync(DuckDbSchemaText.ForeignKeyRelationshipsSql, SchemaTraceKeys.GetRelationships, MapRelationship, cancellationToken).ConfigureAwait(false);
            }
            return RelatedTableExpander.Expand(tables, foreignKeys, relationshipBehavior, Key)
                .Select(table => DuckDbSchemaHelper.FormatTableName(table.Schema, table.Name))
                .ToList();
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the provider of the setting of the connection (see <see cref="RepoDb.Interfaces.IDbSetting.Provider"/>): it is the provider of the tables and of the columns that are read.
        /// It is <c>null</c> if the connection has no setting.
        /// </summary>
        private string Provider =>
            DbSettingMapper.Get(_connection)?.Provider;

        /// <summary>
        /// Creates the identity of a table that is read from the connection: its name, its schema, the database of the connection and the type of the provider.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The name of the schema that owns the table.</param>
        /// <returns>The identity of the table.</returns>
        private TableInfo NewTable(string name,
            string schema) =>
            new TableInfo(name, schema, _connection.Database, Provider);

        /// <summary>
        /// Splits the name of a table into its schema and its name.
        /// </summary>
        /// <param name="tableName">The name of the table, with or without its schema.</param>
        /// <returns>The schema (<c>null</c> if the name has none) and the name of the table.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static (string Schema, string Table) ParseTableName(string tableName) =>
            DuckDbSchemaHelper.ParseSchemaAndTable(tableName);

        /// <summary>
        /// Gets the full name of the table out of its schema and its name, as it is passed to the statements.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The full name of the table.</returns>
        private static string FullName(string schema,
            string table) =>
            $"{schema}\u0001{table}";

        /// <summary>
        /// Gets the key that identifies a table: its schema and its name.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The key of the table.</returns>
        private static string Key(TableInfo table) =>
            $"{table.Schema}\u0001{table.Name}";

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
            CancellationToken cancellationToken) =>
            Task.FromResult(ParseTableName(tableName));

        /// <summary>
        /// Gets the current schema of the connection.
        /// </summary>
        /// <returns>The name of the current schema.</returns>
        private string CurrentSchema() =>
            Query(DuckDbSchemaText.CurrentSchemaSql, SchemaTraceKeys.ResolveSchemaName, r => Convert.ToString(r[0])).First();

        /// <summary>
        /// Gets the current schema of the connection.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the name of the current schema.</returns>
        private async Task<string> CurrentSchemaAsync(CancellationToken cancellationToken) =>
            (await QueryAsync(DuckDbSchemaText.CurrentSchemaSql, SchemaTraceKeys.ResolveSchemaName, r => Convert.ToString(r[0]), cancellationToken).ConfigureAwait(false)).First();

        /// <summary>
        /// Resolves the full name of the table asynchronously.
        /// </summary>
        /// <param name="tableName">The name of the table, with or without its schema.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the full name of the table.</returns>
        private async Task<string> ResolveFullNameAsync(string tableName,
            CancellationToken cancellationToken)
        {
            var (schema, table) = await ResolveAsync(tableName, cancellationToken).ConfigureAwait(false);
            return FullName(schema, table);
        }

        // Parameters

        /// <summary>
        /// Creates a named parameter of a statement.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The parameter.</returns>
        private static KeyValuePair<string, object> Parameter(string name,
            object value) =>
            new KeyValuePair<string, object>(name, value);

        /// <summary>
        /// Creates the parameter that holds the full name of a table.
        /// </summary>
        /// <param name="fullName">The full name of the table.</param>
        /// <returns>The parameter.</returns>
        private static KeyValuePair<string, object> FullNameParameter(string fullName) =>
            Parameter("FullName", fullName);

        /// <summary>
        /// Creates the parameter that holds the full name of a table.
        /// </summary>
        /// <param name="name">The schema and the name of the table.</param>
        /// <returns>The parameter.</returns>
        private static KeyValuePair<string, object> FullNameParameter((string Schema, string Table) name) =>
            FullNameParameter(FullName(name.Schema, name.Table));

        // Execution

        /// <summary>
        /// Checks whether any of the parameters refers to the current schema of the connection (a table without a schema).
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if a parameter refers to the current schema; otherwise, <c>false</c>.</returns>
        private static bool RefersToCurrentSchema(KeyValuePair<string, object>[] parameters) =>
            parameters.Any(parameter =>
                (parameter.Key == "FullName" && string.IsNullOrEmpty(((string)parameter.Value).Split('\u0001')[0])) ||
                (parameter.Key == "SchemaName" && parameter.Value == null));

        /// <summary>
        /// Converts the parameters into the parameter object that is passed to the statement. A parameter that refers to the current schema is replaced by its name.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="currentSchema">The current schema of the connection, or <c>null</c> if no parameter refers to it.</param>
        /// <returns>The parameter object.</returns>
        private static IDictionary<string, object> ToParameters(KeyValuePair<string, object>[] parameters,
            string currentSchema)
        {
            var result = new Dictionary<string, object>();
            foreach (var parameter in parameters)
            {
                if (parameter.Key == "FullName")
                {
                    var parts = ((string)parameter.Value).Split('\u0001');
                    result["SchemaName"] = string.IsNullOrEmpty(parts[0]) ? currentSchema : parts[0];
                    result["TableName"] = parts[1];
                }
                else
                {
                    result[parameter.Key] = parameter.Key == "SchemaName" && parameter.Value == null ? currentSchema : parameter.Value;
                }
            }
            return result;
        }

        /// <summary>
        /// Executes a statement and maps every record of its result.
        /// </summary>
        /// <typeparam name="T">The type of the result of the map function.</typeparam>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="traceKey">The tracking key of the statement.</param>
        /// <param name="map">The function that maps a record into the result.</param>
        /// <param name="parameters">The parameters of the statement.</param>
        /// <returns>The mapped records.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "The parameter object passed to the RepoDB execute methods is either null or a Dictionary<string, object>, which is not reflected.")]
        private IList<T> Query<T>(string sql,
            string traceKey,
            Func<IDataRecord, T> map,
            params KeyValuePair<string, object>[] parameters)
        {
            var currentSchema = RefersToCurrentSchema(parameters) ? CurrentSchema() : null;
            using (var reader = _connection.ExecuteReader(sql,
                param: ToParameters(parameters, currentSchema),
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
        /// Executes a statement and maps every record of its result asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the result of the map function.</typeparam>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="traceKey">The tracking key of the statement.</param>
        /// <param name="map">The function that maps a record into the result.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="parameters">The parameters of the statement.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the mapped records.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "The parameter object passed to the RepoDB execute methods is either null or a Dictionary<string, object>, which is not reflected.")]
        private async Task<IList<T>> QueryAsync<T>(string sql,
            string traceKey,
            Func<IDataRecord, T> map,
            CancellationToken cancellationToken,
            params KeyValuePair<string, object>[] parameters)
        {
            var currentSchema = RefersToCurrentSchema(parameters) ? await CurrentSchemaAsync(cancellationToken).ConfigureAwait(false) : null;
            using (var reader = await _connection.ExecuteReaderAsync(sql,
                param: ToParameters(parameters, currentSchema),
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
            return value == DBNull.Value ? (long?)null : Convert.ToInt64(value);
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
        /// <param name="r">The record.</param>
        /// <returns>The column.</returns>
        private ColumnInfo MapColumn(IDataRecord r)
        {
            var dataType = Text(r, "TypeName").Trim();
            var typeName = ToTypeName(dataType);
            var defaultDefinition = Text(r, "DefaultDefinition")?.Trim();
            var field = new DbField(Text(r, "Name"),
                Flag(r, "IsPrimary"),
                Flag(r, "IsIdentity"),
                Flag(r, "IsNullable"),
                _typeResolver.Resolve(BaseName(typeName)) ?? typeof(object),
                0,
                (byte)(typeName == "decimal" ? Int(r, "PrecisionValue") : 0),
                (byte)(typeName == "decimal" ? Int(r, "ScaleValue") : 0),
                typeName == "decimal" ? typeName : IsSimple(typeName) ? typeName : dataType,
                !string.IsNullOrWhiteSpace(defaultDefinition),
                Provider);

            return new ColumnInfo
            {
                Field = field,
                Ordinal = Int(r, "Ordinal"),
                IdentitySeed = NullableLong(r, "IdentitySeed"),
                IdentityIncrement = NullableLong(r, "IdentityIncrement"),
                DefaultExpression = string.IsNullOrWhiteSpace(defaultDefinition) ? null : defaultDefinition,
                DefaultValue = SchemaDefaultValue.Classify(string.IsNullOrWhiteSpace(defaultDefinition) ? null : defaultDefinition)
            };
        }

        /// <summary>
        /// Gets the name of the type in lower case, without its precision and scale (i.e.: <c>decimal(10,2)</c> is <c>decimal</c>).
        /// </summary>
        /// <param name="dataType">The data type of the column, as the database reports it.</param>
        /// <returns>The name of the type.</returns>
        private static string ToTypeName(string dataType)
        {
            var name = dataType.ToLowerInvariant();
            return name.StartsWith("decimal(", StringComparison.Ordinal) ? "decimal" : name;
        }

        /// <summary>
        /// Gets the name of the type without the other types that it contains (i.e.: <c>integer[]</c> is <c>list</c>, and <c>enum('a')</c> is <c>enum</c>).
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <returns>The name of the type.</returns>
        private static string BaseName(string typeName)
        {
            if (typeName.EndsWith("]", StringComparison.Ordinal))
            {
                return "list";
            }
            var index = typeName.IndexOf('(');
            return index < 0 ? typeName : typeName.Substring(0, index).Trim();
        }

        /// <summary>
        /// Checks whether the type is a simple one (its name has no other type or value in it).
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <returns><c>true</c> if the type is simple; otherwise, <c>false</c>.</returns>
        private static bool IsSimple(string typeName) =>
            typeName.IndexOf('(') < 0 && typeName.IndexOf('[') < 0;

        /// <summary>
        /// DuckDB has no flag for a generated column: it keeps the expression of the column as its default. A default that is not an identity and that refers to another column
        /// of the table can only be the expression of a generated column (a default can not refer to a column).
        /// </summary>
        /// <param name="columns">The columns of the table.</param>
        /// <returns>The columns of the table.</returns>
        private static IList<ColumnInfo> MapComputedColumns(IList<ColumnInfo> columns)
        {
            foreach (var column in columns.Where(c => c.DefaultExpression != null))
            {
                var references = columns.Any(other => !ReferenceEquals(other, column) &&
                    System.Text.RegularExpressions.Regex.IsMatch(column.DefaultExpression,
                        "(^|[^A-Za-z0-9_])\"?" + System.Text.RegularExpressions.Regex.Escape(other.Field.Name) + "\"?([^A-Za-z0-9_(]|$)",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                if (references)
                {
                    // DuckDB wraps the expression in a cast to the type of the column
                    var cast = System.Text.RegularExpressions.Regex.Match(column.DefaultExpression,
                        @"^CAST\((.*)\s+AS\s+" + System.Text.RegularExpressions.Regex.Escape(column.Field.DatabaseType) + @"\)$",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                    column.ComputedExpression = cast.Success ? cast.Groups[1].Value : column.DefaultExpression;
                    column.DefaultExpression = null;
                    column.DefaultValue = Enumerations.ColumnDefaultValue.None;
                    column.Field = new DbField(column.Field.Name, column.Field.IsPrimary, column.Field.IsIdentity, column.Field.IsNullable, column.Field.Type,
                        column.Field.Size, column.Field.Precision, column.Field.Scale, column.Field.DatabaseType, false, column.Field.Provider);
                }
            }
            return columns;
        }

        /// <summary>
        /// Gets the expression of a check constraint out of its text (i.e.: <c>CHECK((Age &gt;= 0))</c>).
        /// </summary>
        /// <param name="text">The source of the check constraint.</param>
        /// <returns>The expression.</returns>
        private static string ToCheckExpression(string text) =>
            RemoveOuterParentheses(System.Text.RegularExpressions.Regex.Replace(text?.Trim() ?? string.Empty, @"^CHECK\s*", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase));

        /// <summary>
        /// Removes the parentheses that wrap the whole expression, so the expression that is composed back is not wrapped again and again.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The expression without the parentheses that wrap it.</returns>
        private static string RemoveOuterParentheses(string expression)
        {
            while (expression.Length >= 2 && expression[0] == '(' && expression[expression.Length - 1] == ')' && IsWrapped(expression))
            {
                expression = expression.Substring(1, expression.Length - 2).Trim();
            }
            return expression;
        }

        /// <summary>
        /// Checks whether the first parenthesis of the expression is closed by the last character of the expression.
        /// </summary>
        /// <param name="expression">The expression that starts with an opening parenthesis and ends with a closing one.</param>
        /// <returns><c>true</c> if the first parenthesis is closed by the last one; otherwise, <c>false</c>.</returns>
        private static bool IsWrapped(string expression)
        {
            var depth = 0;
            var quote = (char)0;
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                if (quote != 0)
                {
                    quote = c == quote ? (char)0 : quote;
                }
                else if (c == (char)39 || c == (char)34)
                {
                    quote = c;
                }
                else if (c == '(')
                {
                    depth++;
                }
                else if (c == ')' && --depth == 0)
                {
                    return i == expression.Length - 1;
                }
            }
            return false;
        }

        /// <summary>
        /// Maps a record into the row of a column of a key.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The row of the column of the key.</returns>
        private static (string Name, string Column, bool IsClustered) MapKeyColumn(IDataRecord r) =>
            (Text(r, "ConstraintName"), Text(r, "ColumnName"), Flag(r, "IsClustered"));

        /// <summary>
        /// Maps the rows of the key columns into the primary key of the table.
        /// </summary>
        /// <param name="rows">The rows of the columns of the primary key.</param>
        /// <returns>The primary key, or <c>null</c> if the table has none.</returns>
        private static PrimaryKeyInfo MapPrimaryKey(IEnumerable<(string Name, string Column, bool IsClustered)> rows)
        {
            var list = rows.ToList();
            return list.Count == 0
                ? null
                : new PrimaryKeyInfo(list[0].Name) { Columns = list.Select(x => x.Column).ToList(), IsClustered = list[0].IsClustered };
        }

        /// <summary>
        /// Maps the rows of the key columns into the unique constraints of the table.
        /// </summary>
        /// <param name="rows">The rows of the columns of the unique constraints.</param>
        /// <returns>The unique constraints.</returns>
        private static IList<UniqueConstraintInfo> MapUniqueConstraints(IEnumerable<(string Name, string Column, bool IsClustered)> rows) =>
            rows.GroupBy(x => x.Name)
                .Select(g => new UniqueConstraintInfo(g.Key) { Columns = g.Select(x => x.Column).ToList() })
                .ToList();

        /// <summary>
        /// Maps a record into the row of a column of an index.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The row of the column of the index.</returns>
        private static (string Name, bool IsUnique, string Expressions) MapIndexColumn(IDataRecord r) =>
            (Text(r, "IndexName"), Flag(r, "IsUnique"), Text(r, "Expressions"));

        /// <summary>
        /// Maps the indexes. DuckDB gives the expressions of an index as a text (i.e.: <c>['"Name"', Age]</c>), and it does not keep the direction of the keys.
        /// An index of an expression (not of the columns) is not read.
        /// </summary>
        /// <param name="rows">The rows of the columns of the indexes.</param>
        /// <returns>The indexes.</returns>
        private static IList<IndexInfo> MapIndexes(IEnumerable<(string Name, bool IsUnique, string Expressions)> rows) =>
            rows.Select(x => (x.Name, x.IsUnique, Columns: ToIndexColumns(x.Expressions)))
                .Where(x => x.Columns != null)
                .Select(x => new IndexInfo(x.Name) { IsUnique = x.IsUnique, Columns = x.Columns })
                .ToList();

        /// <summary>
        /// Gets the columns out of the expressions of an index, or <c>null</c> if the index is not of the columns only.
        /// </summary>
        /// <param name="expressions">The expressions of the index, as a text.</param>
        /// <returns>The names of the columns.</returns>
        private static List<string> ToIndexColumns(string expressions)
        {
            var columns = new List<string>();
            foreach (var item in (expressions ?? string.Empty).Trim().TrimStart('[').TrimEnd(']').Split(','))
            {
                var column = item.Trim().Trim('\'').Trim();
                if (column.Length >= 2 && column[0] == '"' && column[column.Length - 1] == '"')
                {
                    column = column.Substring(1, column.Length - 2).Replace("\"\"", "\"");
                }
                if (column.Length == 0 || column.IndexOfAny(new[] { '(', ' ', '"' }) >= 0)
                {
                    return null;
                }
                columns.Add(column);
            }
            return columns;
        }

        /// <summary>
        /// Maps a record into the row of a column of a foreign key.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The row of the column of the foreign key.</returns>
        private static (string Name, string Column, string RefSchema, string RefTable, string RefColumn, string Update, string Delete) MapForeignKeyColumn(IDataRecord r) =>
            (Text(r, "ForeignKeyName"), Text(r, "ColumnName"), Text(r, "ReferencedSchema"), Text(r, "ReferencedTable"), Text(r, "ReferencedColumn"), Text(r, "UpdateAction"), Text(r, "DeleteAction"));

        /// <summary>
        /// Maps the rows of the foreign key columns into the foreign keys of the table.
        /// </summary>
        /// <param name="rows">The rows of the columns of the foreign keys.</param>
        /// <returns>The foreign keys.</returns>
        private IList<ForeignKeyInfo> MapForeignKeys(IEnumerable<(string Name, string Column, string RefSchema, string RefTable, string RefColumn, string Update, string Delete)> rows) =>
            rows.GroupBy(x => x.Name)
                .Select(g => new ForeignKeyInfo(g.Key)
                {
                    Columns = g.Select(x => x.Column).ToList(),
                    ReferencedTable = NewTable(g.First().RefTable, g.First().RefSchema),
                    ReferencedColumns = g.Select(x => x.RefColumn).ToList(),
                    UpdateRule = ToRule(g.First().Update),
                    DeleteRule = ToRule(g.First().Delete)
                })
                .ToList();

        /// <summary>
        /// Converts the action of the database into the rule of a foreign key.
        /// </summary>
        /// <param name="action">The action that is read from the database.</param>
        /// <returns>The rule of the foreign key.</returns>
        private static ForeignKeyRule ToRule(string action)
        {
            switch (action)
            {
                case "CASCADE": return ForeignKeyRule.Cascade;
                case "SET NULL": return ForeignKeyRule.SetNull;
                case "SET DEFAULT": return ForeignKeyRule.SetDefault;
                case "RESTRICT": return ForeignKeyRule.Restrict;
                default: return ForeignKeyRule.NoAction;
            }
        }

        /// <summary>
        /// Maps a record into a check constraint of the table.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The check constraint.</returns>
        private static CheckConstraintInfo MapCheckConstraint(IDataRecord r) =>
            new CheckConstraintInfo(Text(r, "ConstraintName")) { Expression = ToCheckExpression(Text(r, "Definition")) };

        /// <summary>
        /// Maps a record into a relationship of two tables: the child table and the parent table that it references.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <returns>The child table and the parent table.</returns>
        private (TableInfo Child, TableInfo Parent) MapRelationship(IDataRecord r) =>
            (NewTable(Text(r, "ChildTable"), Text(r, "ChildSchema")), NewTable(Text(r, "ParentTable"), Text(r, "ParentSchema")));

        // Ordering

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that it references (see <see cref="TableOrderer"/>).
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered relationships, one per table.</returns>
        internal static IList<RelationshipInfo> Order(IList<TableSchema> schemas) =>
            TableOrderer.Order(schemas, schema => Key(schema.Table), Key);

        #endregion
    }
}
