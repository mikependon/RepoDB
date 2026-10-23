#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Helpers;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the helper methods of the <see cref="CopyDataToExtension"/> class: the tables whose rows are copied, the names of the tables and the copy of the rows.
    /// </summary>
    internal static class DbConnection
    {
        #region Methods

        /// <summary>
        /// Gets the tables whose rows are to be copied, in the order that they must be copied (a table comes after the tables that it references).
        /// If the schema is requested (see <see cref="IsSchemaRequested"/>), it is copied first, with <see cref="CopyTableSchemaToExtension.CopyTableSchemaTo(IDbConnection, IEnumerable{string}, IDbConnection, string, TableExistsBehavior, RelatedTableBehavior, Action{CopyTableSchemaResultItem}, Action{ExecutionError}, int?, string, ITrace, IDbTransaction)"/>.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="targetSchema">The schema of the target table, or <c>null</c>.</param>
        /// <param name="where">The filter of the requested table, or <c>null</c>.</param>
        /// <param name="relationshipBehavior">The behavior that defines which related tables are copied.</param>
        /// <param name="tableExistenceBehavior">The behavior that defines what happens to the tables that already exist.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="mappings">The column mappings of the requested table: only the mapped columns of the source table are created in the target table when the schema is copied.</param>
        /// <returns>The source table, the target table and the filter of each table.</returns>
        internal static IList<(string Source, string Target, QueryGroup Where)> GetTables(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            string targetSchema,
            QueryGroup where,
            CopyRelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            IList<CopyColumnMapping> mappings = null)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior, targetSchema))
            {
                return requested;
            }
            var results = new List<CopyTableSchemaResultItem>();
            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            var schemaMappings = ToSchemaMappings(mappings);
            if (schemaMappings == null)
            {
                connection.CopyTableSchemaTo(new[] { sourceTable },
                    destinationConnection,
                    targetSchema: targetSchema,
                    tableExistenceBehavior: tableExistenceBehavior,
                    relationshipBehavior: schemaBehavior,
                    createdCallback: results.Add,
                    commandTimeout: commandTimeout,
                    trace: trace,
                    transaction: transaction);
            }
            else
            {
                connection.CopyTableSchemaTo(new[] { sourceTable },
                    destinationConnection,
                    new[] { new TableMapping(sourceTable, schemaMappings) },
                    targetSchema: targetSchema,
                    tableExistenceBehavior: tableExistenceBehavior,
                    relationshipBehavior: schemaBehavior,
                    createdCallback: results.Add,
                    commandTimeout: commandTimeout,
                    trace: trace,
                    transaction: transaction);
            }
            if (relationshipBehavior == CopyRelationshipBehavior.TableOnly)
            {
                return new List<(string, string, QueryGroup)> { (sourceTable, GetName(destinationConnection, results.Single()), where) };
            }
            var reader = SchemaReaderMapper.Get(connection);
            var relationships = reader.GetDependencyOrder(reader.GetRelatedTables(new[] { sourceTable }, schemaBehavior));
            return ToTables(relationships, reader.GetTableSchema(sourceTable).Table, requested[0], results, connection, destinationConnection);
        }

        /// <summary>
        /// Gets the tables whose rows are to be copied (see <see cref="GetTables"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="targetSchema">The schema of the target table, or <c>null</c>.</param>
        /// <param name="where">The filter of the requested table, or <c>null</c>.</param>
        /// <param name="relationshipBehavior">The behavior that defines which related tables are copied.</param>
        /// <param name="tableExistenceBehavior">The behavior that defines what happens to the tables that already exist.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="mappings">The column mappings of the requested table: only the mapped columns of the source table are created in the target table when the schema is copied.</param>
        /// <returns>The source table, the target table and the filter of each table.</returns>
        internal static async Task<IList<(string Source, string Target, QueryGroup Where)>> GetTablesAsync(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            string targetSchema,
            QueryGroup where,
            CopyRelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken,
            IList<CopyColumnMapping> mappings = null)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior, targetSchema))
            {
                return requested;
            }
            var results = new List<CopyTableSchemaResultItem>();
            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            var schemaMappings = ToSchemaMappings(mappings);
            if (schemaMappings == null)
            {
                await connection.CopyTableSchemaToAsync(new[] { sourceTable },
                    destinationConnection,
                    targetSchema: targetSchema,
                    tableExistenceBehavior: tableExistenceBehavior,
                    relationshipBehavior: schemaBehavior,
                    createdCallback: results.Add,
                    commandTimeout: commandTimeout,
                    trace: trace,
                    transaction: transaction,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await connection.CopyTableSchemaToAsync(new[] { sourceTable },
                    destinationConnection,
                    new[] { new TableMapping(sourceTable, schemaMappings) },
                    targetSchema: targetSchema,
                    tableExistenceBehavior: tableExistenceBehavior,
                    relationshipBehavior: schemaBehavior,
                    createdCallback: results.Add,
                    commandTimeout: commandTimeout,
                    trace: trace,
                    transaction: transaction,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            if (relationshipBehavior == CopyRelationshipBehavior.TableOnly)
            {
                return new List<(string, string, QueryGroup)> { (sourceTable, GetName(destinationConnection, results.Single()), where) };
            }
            var reader = SchemaReaderMapper.Get(connection);
            var names = await reader.GetRelatedTablesAsync(new[] { sourceTable }, schemaBehavior, cancellationToken).ConfigureAwait(false);
            var relationships = await reader.GetDependencyOrderAsync(names, cancellationToken).ConfigureAwait(false);
            var table = (await reader.GetTableSchemaAsync(sourceTable, cancellationToken).ConfigureAwait(false)).Table;
            return ToTables(relationships, table, requested[0], results, connection, destinationConnection);
        }

        /// <summary>
        /// Gets the tables to be copied from the relationships: the requested table keeps its source name and its filter, and the related tables are copied as a whole.
        /// The rows are copied into the schema that the table was created in (see <see cref="CopyTableSchemaResultItem.Target"/>).
        /// </summary>
        /// <param name="relationships">The relationships of the tables, in the order that they are copied.</param>
        /// <param name="requested">The identity of the requested table.</param>
        /// <param name="requestedTable">The source table, the target table and the filter of the requested table.</param>
        /// <param name="results">The items of the tables that were created in the destination database.</param>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <returns>The source table, the target table and the filter of each table.</returns>
        internal static IList<(string Source, string Target, QueryGroup Where)> ToTables(IEnumerable<RelationshipInfo> relationships,
            TableInfo requested,
            (string Source, string Target, QueryGroup Where) requestedTable,
            IEnumerable<CopyTableSchemaResultItem> results,
            IDbConnection connection,
            IDbConnection destinationConnection) =>
            relationships
                .Select(relationship => relationship.Schema.Table)
                .Select(table =>
                {
                    var target = GetName(destinationConnection, results.First(result => result.Source.Table.Table.Name == table.Name && result.Source.Table.Table.Schema == table.Schema));
                    return table.Equals(requested)
                        ? (requestedTable.Source, target, requestedTable.Where)
                        : (GetName(connection, table.Schema, table.Name), target, (QueryGroup)null);
                })
                .ToList();

        /// <summary>
        /// Gets the name of the table that was created in the destination database, quoted for the database of the connection.
        /// </summary>
        /// <param name="connection">The connection of the destination database.</param>
        /// <param name="result">The item of the table that was created.</param>
        /// <returns>The quoted name of the table.</returns>
        internal static string GetName(IDbConnection connection,
            CopyTableSchemaResultItem result) =>
            GetName(connection, result.Target.Table.Table.Schema, result.Target.Table.Table.Name);

        /// <summary>
        /// Gets the name of the table, quoted for the database of the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="schema">The schema of the table, or <c>null</c>.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The quoted name of the table.</returns>
        internal static string GetName(IDbConnection connection,
            string schema,
            string table)
        {
            var setting = connection.GetDbSetting();
            var name = table.AsQuoted(false, true, setting);
            return string.IsNullOrWhiteSpace(schema) ? name : $"{schema.AsQuoted(false, true, setting)}.{name}";
        }

        /// <summary>
        /// Gets the schema of the target table (i.e.: <c>Sales</c> of <c>Sales.Person</c>), without the quotes.
        /// </summary>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <returns>The schema (<c>null</c> if the name of the table has no schema).</returns>
        internal static string GetTargetSchema(IDbConnection destinationConnection,
            string targetTable)
        {
            var setting = destinationConnection == null || string.IsNullOrWhiteSpace(targetTable) ? null : DbSettingMapper.Get(destinationConnection);
            if (setting == null || string.Equals(DataEntityExtension.GetTableName(targetTable, setting), targetTable, StringComparison.Ordinal))
            {
                return null;
            }
            return DataEntityExtension.GetSchema(targetTable, setting).AsUnquoted(setting);
        }

        /// <summary>
        /// Gets the name of the table without its schema and its quotes (i.e.: <c>Person</c> of <c>[Sales].[Person]</c>).
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The name of the table.</returns>
        internal static string GetTableName(IDbConnection connection,
            string table)
        {
            var setting = DbSettingMapper.Get(connection);
            return setting == null ? table : DataEntityExtension.GetTableName(table, setting).AsUnquoted(setting);
        }

        /// <summary>
        /// Checks whether the schema of the tables is to be copied before their rows: the default behaviors
        /// (<see cref="CopyRelationshipBehavior.TableOnly"/> and <see cref="TableExistsBehavior.Skip"/>) without a target schema do not copy it.
        /// </summary>
        /// <param name="relationshipBehavior">The behavior that defines which related tables are copied.</param>
        /// <param name="tableExistenceBehavior">The behavior that defines what happens to the tables that already exist.</param>
        /// <param name="targetSchema">The schema of the target table, or <c>null</c>.</param>
        /// <returns><c>true</c> if the schema has to be copied; otherwise, <c>false</c>.</returns>
        internal static bool IsSchemaRequested(CopyRelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            string targetSchema) =>
            relationshipBehavior != CopyRelationshipBehavior.TableOnly ||
            tableExistenceBehavior != TableExistsBehavior.Skip ||
            !string.IsNullOrWhiteSpace(targetSchema);

        /// <summary>
        /// Gets the relationship behavior of the schema copy that is equivalent to the one of the data copy.
        /// </summary>
        /// <param name="relationshipBehavior">The behavior of the copy of the data.</param>
        /// <returns>The behavior of the copy of the schema.</returns>
        internal static RelatedTableBehavior ToSchemaBehavior(CopyRelationshipBehavior relationshipBehavior)
        {
            switch (relationshipBehavior)
            {
                case CopyRelationshipBehavior.TableOnly:
                    return RelatedTableBehavior.TableOnly;
                case CopyRelationshipBehavior.Parents:
                    return RelatedTableBehavior.Parents;
                case CopyRelationshipBehavior.Children:
                    return RelatedTableBehavior.Children;
                case CopyRelationshipBehavior.ParentsAndChildren:
                    return RelatedTableBehavior.ParentsAndChildren;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relationshipBehavior));
            }
        }

        /// <summary>
        /// Copies the rows of the source table into the target table. The rows are read with a data reader that is fed to the bulk insert of the destination connection
        /// (the <c>BulkOperations</c> package of its provider). If the provider has no bulk insert, the rows of the reader are inserted in batches.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="where">The filter of the rows to be copied, or <c>null</c> to copy all the rows.</param>
        /// <param name="batchSize">The number of rows of a batch.</param>
        /// <param name="columnMappingsBehavior">The behavior that defines how the columns of the source table are aligned with the columns of the target table.</param>
        /// <param name="identityBehavior">The behavior that defines whether the values of the identity columns are copied as they are, or generated by the database of the target table.</param>
        /// <param name="mappings">The column mappings, or <c>null</c> to copy the columns that exist on both tables.</param>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="progressCallback">The callback that receives the progress after every batch, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        internal static void CopyRows(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int batchSize,
            CopyColumnMappingsBehavior columnMappingsBehavior,
            CopyIdentityBehavior identityBehavior,
            IList<CopyColumnMapping> mappings,
            CopyProgress options,
            Action<CopyProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            var item = Begin(options, connection, destinationConnection, sourceTable, targetTable);
            ValidateMappings(connection, destinationConnection, sourceTable, targetTable, mappings);
            var excluded = GetIdentityColumns(destinationConnection, targetTable, identityBehavior, transaction);
            var (commandText, param) = CreateSelect(connection, sourceTable, where, mappings);
            using (var reader = connection.ExecuteReader(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace))
            using (var buffer = new BatchBuffer(reader, batchSize, excluded))
            {
                if (IsBulkAvailable(destinationConnection, buffer, false))
                {
                    var before = CopyDataLogger.Count(destinationConnection, targetTable, transaction);
                    var total = 0;
                    while (buffer.Fill())
                    {
                        int inserted;
                        using (var batchReader = buffer.CreateReader())
                        {
                            inserted = BulkInsertInvoker.BulkInsert(destinationConnection, targetTable, batchReader, batchSize, columnMappingsBehavior, identityBehavior == CopyIdentityBehavior.KeepIdentity, commandTimeout, null, trace, transaction);
                        }
                        total += inserted;
                        buffer.Clear();
                        Report(options, inserted, progressCallback);
                    }
                    if (item.Batches.Count == 0)
                    {
                        Report(options, 0, progressCallback);
                    }
                    var after = CopyDataLogger.Count(destinationConnection, targetTable, transaction);
                    Record(item, CopyDataLogger.LogBulkInsert(options, destinationConnection, targetTable, total, before, after), after);
                }
                else
                {
                    var action = new Action<IList<object>>((IList<object> b) =>
                        Report(options, RowInserter.Insert(destinationConnection, targetTable, b, commandTimeout, traceKey, trace, transaction), progressCallback));
                    var batch = new List<object>(batchSize);
                    while (reader.Read())
                    {
                        batch.Add(ToRow(reader, excluded));
                        if (batch.Count == batchSize)
                        {
                            action(batch);
                            batch.Clear();
                        }
                    }
                    if (batch.Count > 0)
                    {
                        action(batch);
                    }
                }
            }
            Complete(options, item);
        }

        /// <summary>
        /// Copies the rows of the source table into the target table in an asynchronous way (see <see cref="CopyRows"/>).
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="where">The filter of the rows to be copied, or <c>null</c> to copy all the rows.</param>
        /// <param name="batchSize">The number of rows of a batch.</param>
        /// <param name="columnMappingsBehavior">The behavior that defines how the columns of the source table are aligned with the columns of the target table.</param>
        /// <param name="identityBehavior">The behavior that defines whether the values of the identity columns are copied as they are, or generated by the database of the target table.</param>
        /// <param name="mappings">The column mappings, or <c>null</c> to copy the columns that exist on both tables.</param>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="progressCallback">The callback that receives the progress after every batch, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal static async Task CopyRowsAsync(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int batchSize,
            CopyColumnMappingsBehavior columnMappingsBehavior,
            CopyIdentityBehavior identityBehavior,
            IList<CopyColumnMapping> mappings,
            CopyProgress options,
            Action<CopyProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var item = Begin(options, connection, destinationConnection, sourceTable, targetTable);
            ValidateMappings(connection, destinationConnection, sourceTable, targetTable, mappings);
            var excluded = GetIdentityColumns(destinationConnection, targetTable, identityBehavior, transaction);
            var (commandText, param) = CreateSelect(connection, sourceTable, where, mappings);
            using (var reader = await connection.ExecuteReaderAsync(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false))
            using (var buffer = new BatchBuffer(reader, batchSize, excluded))
            {
                if (IsBulkAvailable(destinationConnection, buffer, true))
                {
                    var before = await CopyDataLogger.CountAsync(destinationConnection, targetTable, transaction, cancellationToken).ConfigureAwait(false);
                    var total = 0;
                    while (await buffer.FillAsync(cancellationToken).ConfigureAwait(false))
                    {
                        int inserted;
                        using (var batchReader = buffer.CreateReader())
                        {
                            inserted = await BulkInsertInvoker.BulkInsertAsync(destinationConnection, targetTable, batchReader, batchSize, columnMappingsBehavior, identityBehavior == CopyIdentityBehavior.KeepIdentity, commandTimeout, null, trace, transaction, cancellationToken).ConfigureAwait(false);
                        }
                        total += inserted;
                        buffer.Clear();
                        Report(options, inserted, progressCallback);
                    }
                    if (item.Batches.Count == 0)
                    {
                        Report(options, 0, progressCallback);
                    }
                    var after = await CopyDataLogger.CountAsync(destinationConnection, targetTable, transaction, cancellationToken).ConfigureAwait(false);
                    Record(item, CopyDataLogger.LogBulkInsert(options, destinationConnection, targetTable, total, before, after), after);
                }
                else
                {
                    var action = new Func<IList<object>, Task>(async (IList<object> b) =>
                        Report(options, await RowInserter.InsertAsync(destinationConnection, targetTable, b, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false), progressCallback));
                    var batch = new List<object>(batchSize);
                    while (await ReadAsync(reader, cancellationToken).ConfigureAwait(false))
                    {
                        batch.Add(ToRow(reader, excluded));
                        if (batch.Count == batchSize)
                        {
                            await action(batch).ConfigureAwait(false);
                            batch.Clear();
                        }
                    }
                    if (batch.Count > 0)
                    {
                        await action(batch).ConfigureAwait(false);
                    }
                }
            }
            Complete(options, item);
        }

        /// <summary>
        /// Gets the mappings of the columns for the schema copy, so the schema of the target table is created with only the mapped columns, by their destination names.
        /// </summary>
        /// <param name="mappings">The column mappings of the copy of the data.</param>
        /// <returns>The mappings of the schema copy (<c>null</c> if there are no mappings, which creates all the columns). A source column that is mapped more than once is created with its first destination column.</returns>
        private static IList<ColumnMapping> ToSchemaMappings(IList<CopyColumnMapping> mappings)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return null;
            }
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return mappings
                .Where(mapping => seen.Add(mapping.SourceColumn))
                .Select(mapping => new ColumnMapping(mapping.SourceColumn, mapping.DestinationColumn))
                .ToList();
        }

        /// <summary>
        /// Composes the <c>SELECT</c> statement of the source table with the statement builder of the connection, and the parameters of its filter.
        /// If there are column mappings, only the mapped columns are selected, each one aliased with the name of its destination column.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="where">The filter of the rows to be copied, or <c>null</c>.</param>
        /// <param name="mappings">The column mappings, or <c>null</c> to read all the columns.</param>
        /// <returns>The statement and its parameters (<c>null</c> if there is no filter).</returns>
        private static (string CommandText, object Param) CreateSelect(IDbConnection connection,
            string sourceTable,
            QueryGroup where,
            IList<CopyColumnMapping> mappings)
        {
            var statementBuilder = StatementBuilderMapper.Get(connection) ?? throw new InvalidOperationException($"No statement builder is mapped to the connection '{connection.GetType().FullName}'.");
            var hasMappings = mappings?.Count > 0;
            var fields = hasMappings
                ? mappings.Select(mapping => mapping.SourceColumn).Distinct(StringComparer.OrdinalIgnoreCase).Select(column => new Field(column)).ToList()
                : DbFieldCache.Get(connection, sourceTable, null)?.GetAsFields();
            where?.Fix();
            var commandText = statementBuilder.CreateQuery(sourceTable, fields, where);
            if (hasMappings)
            {
                commandText = Alias(connection, commandText, mappings);
            }
            return (commandText, where == null ? null : ToParameters(where));
        }

        /// <summary>
        /// Replaces the list of the columns of the <c>SELECT</c> statement with the mapped columns, each one aliased with the name of its destination column.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="commandText">The <c>SELECT</c> statement.</param>
        /// <param name="mappings">The column mappings.</param>
        /// <returns>The statement.</returns>
        private static string Alias(IDbConnection connection,
            string commandText,
            IList<CopyColumnMapping> mappings)
        {
            const string select = "SELECT ";
            const string from = " FROM ";
            var dbSetting = DbSettingMapper.Get(connection);
            var start = commandText.IndexOf(select, StringComparison.OrdinalIgnoreCase);
            var end = commandText.IndexOf(from, StringComparison.OrdinalIgnoreCase);
            if (start < 0 || end < start)
            {
                throw new InvalidOperationException($"The column mappings cannot be applied to the statement '{commandText}'.");
            }
            var columns = string.Join(", ", mappings.Select(mapping => $"{mapping.SourceColumn.AsQuoted(dbSetting)} AS {mapping.DestinationColumn.AsQuoted(dbSetting)}"));
            return string.Concat(commandText.Substring(0, start + select.Length), columns, commandText.Substring(end));
        }

        /// <summary>
        /// Ensures that the source columns of the mappings exist on the source table and that the destination columns exist on the target table.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="mappings">The column mappings.</param>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidateMappings(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            IList<CopyColumnMapping> mappings)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return;
            }
            var sourceColumns = new HashSet<string>(DbFieldCache.Get(connection, sourceTable, null).GetAsFields().Select(field => field.Name), StringComparer.OrdinalIgnoreCase);
            var targetColumns = new HashSet<string>(DbFieldCache.Get(destinationConnection, targetTable, null).GetAsFields().Select(field => field.Name), StringComparer.OrdinalIgnoreCase);
            foreach (var mapping in mappings)
            {
                if (!sourceColumns.Contains(mapping.SourceColumn))
                {
                    throw new ArgumentException($"The source column '{mapping.SourceColumn}' does not exist on the source table '{sourceTable}'.", nameof(mappings));
                }
                if (!targetColumns.Contains(mapping.DestinationColumn))
                {
                    throw new ArgumentException($"The destination column '{mapping.DestinationColumn}' does not exist on the target table '{targetTable}'.", nameof(mappings));
                }
            }
        }

        /// <summary>
        /// Gets the parameters of the filter, by the names that the statement builder gives to them.
        /// </summary>
        /// <param name="where">The filter.</param>
        /// <returns>The parameters.</returns>
        private static IDictionary<string, object> ToParameters(QueryGroup where)
        {
            var parameters = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var field in where.GetFields(true))
            {
                var name = field.Parameter.Name;
                if (field.Operation == Operation.Between || field.Operation == Operation.NotBetween)
                {
                    var values = ToList(field.Parameter.Value);
                    parameters[$"{name}_Left"] = values.Count > 0 ? values[0] : null;
                    parameters[$"{name}_Right"] = values.Count > 1 ? values[1] : null;
                }
                else if (field.Operation == Operation.In || field.Operation == Operation.NotIn)
                {
                    var values = ToList(field.Parameter.Value);
                    for (var i = 0; i < values.Count; i++)
                    {
                        parameters[$"{name}_In_{i.ToString(System.Globalization.CultureInfo.InvariantCulture)}"] = values[i];
                    }
                }
                else
                {
                    parameters[name] = field.Parameter.Value;
                }
            }
            return parameters;
        }

        /// <summary>
        /// Converts the value into a list: a collection is converted to its items, any other value is the only item of the list.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The list of the values.</returns>
        private static IList<object> ToList(object value) =>
            value is System.Collections.IEnumerable enumerable && !(value is string)
                ? enumerable.Cast<object>().ToList()
                : new List<object> { value };

        /// <summary>
        /// Checks whether the bulk insert of the provider of the destination connection accepts the batches of the buffer (the data reader of the buffered rows).
        /// </summary>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="buffer">The buffer of the rows to be copied.</param>
        /// <param name="isAsync">A value that indicates whether the asynchronous bulk insert is searched.</param>
        /// <returns><c>true</c> if the provider has a bulk insert for the batches; otherwise, <c>false</c>.</returns>
        private static bool IsBulkAvailable(IDbConnection destinationConnection,
            BatchBuffer buffer,
            bool isAsync)
        {
            using (var probe = buffer.CreateReader())
            {
                return BulkInsertInvoker.IsAvailable(destinationConnection, probe, isAsync);
            }
        }

        /// <summary>
        /// Converts the current row of the reader into an object, to be inserted.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="excludedColumns">The names of the columns that are left out of the row, or <c>null</c> to have all of them.</param>
        /// <returns>The row, as an object that has a property for each column.</returns>
        private static object ToRow(IDataRecord record,
            ISet<string> excludedColumns)
        {
            var row = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                if (excludedColumns == null || !excludedColumns.Contains(name))
                {
                    row[name] = record.IsDBNull(i) ? null : record.GetValue(i);
                }
            }
            return row;
        }

        /// <summary>
        /// Gets the names of the columns of the target table that are not copied: its identity columns, if the database of the target table generates their values.
        /// </summary>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="identityBehavior">The behavior that defines whether the values of the identity columns are copied as they are, or generated by the database of the target table.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <returns>The names of the columns (without the case), or <c>null</c> if all the columns are copied.</returns>
        private static ISet<string> GetIdentityColumns(IDbConnection destinationConnection,
            string targetTable,
            CopyIdentityBehavior identityBehavior,
            IDbTransaction transaction)
        {
            if (identityBehavior != CopyIdentityBehavior.Ignore)
            {
                return null;
            }
            var dbSetting = destinationConnection.GetDbSetting();
            var columns = DbFieldCache.Get(destinationConnection, targetTable, transaction)?
                .GetItems().Where(field => field.IsIdentity)
                .Select(field => field.Name.AsUnquoted(true, dbSetting));
            return columns == null ? null : new HashSet<string>(columns, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Reads the next row of the reader asynchronously, if the reader supports it.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if there is another row; otherwise, <c>false</c>.</returns>
        private static Task<bool> ReadAsync(IDataReader reader,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return reader is System.Data.Common.DbDataReader dbReader
                ? dbReader.ReadAsync(cancellationToken)
                : Task.FromResult(reader.Read());
        }

        /// <summary>
        /// Starts the copy of a table: adds the item of the table (its source and target tables) to the progress, and makes it the current one.
        /// </summary>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <returns>The item of the table, or <c>null</c> if there is no progress.</returns>
        private static CopyProgressItem Begin(CopyProgress options,
            IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable)
        {
            if (options == null)
            {
                return null;
            }
            var now = DateTime.UtcNow;
            var item = new CopyProgressItem
            {
                StartTime = now,
                EndTime = now,
                NextBatchStart = now,
                Source = CreateEndpoint(connection, sourceTable),
                Target = CreateEndpoint(destinationConnection, targetTable)
            };
            item.Source.RowCount = 0;
            options.Items.Add(item);
            options.Current = item;
            options.EndTime = now;
            return item;
        }

        /// <summary>
        /// Creates the details of a table of a connection.
        /// </summary>
        /// <param name="connection">The connection of the database that the table belongs to.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The details of the table.</returns>
        private static CopyEndpoint CreateEndpoint(IDbConnection connection,
            string table) =>
            new CopyEndpoint
            {
                Server = (connection as System.Data.Common.DbConnection)?.DataSource,
                Database = connection.Database,
                DatabaseType = connection.GetType().Name,
                Table = new TableInfo(GetTableName(connection, table), GetTargetSchema(connection, table), connection.Database)
            };

        /// <summary>
        /// Records what is known about the rows of the target table after the bulk insert of the table.
        /// </summary>
        /// <param name="item">The item of the table.</param>
        /// <param name="skipped">The number of rows that the provider did not load, or <c>null</c> if it is not known.</param>
        /// <param name="rowCount">The number of rows of the target table after the bulk insert, or <c>null</c> if it is not known.</param>
        private static void Record(CopyProgressItem item,
            long? skipped,
            long? rowCount)
        {
            if (item == null)
            {
                return;
            }
            item.SkippedRowCount = skipped;
            item.Target.RowCount = rowCount;
        }

        /// <summary>
        /// Reports the progress of the copy: a batch of rows was copied.
        /// </summary>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="inserted">The number of rows that were inserted by the batch.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        private static void Report(CopyProgress options,
            int inserted,
            Action<CopyProgress> progressCallback)
        {
            var item = options.Current;
            var now = DateTime.UtcNow;
            item.Batches.Add(new CopyBatch
            {
                Number = item.Batches.Count + 1,
                RowCount = inserted,
                StartTime = item.NextBatchStart,
                EndTime = now
            });
            item.RowCount += inserted;
            item.Source.RowCount = (item.Source.RowCount ?? 0) + inserted;
            item.EndTime = now;
            options.BatchCount++;
            options.RowCount += inserted;
            options.EndTime = now;
            progressCallback?.Invoke(options);
            item.NextBatchStart = DateTime.UtcNow;
        }

        /// <summary>
        /// Completes the copy of a table: marks its end.
        /// </summary>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="item">The item of the table.</param>
        private static void Complete(CopyProgress options,
            CopyProgressItem item)
        {
            if (item == null)
            {
                return;
            }
            var now = DateTime.UtcNow;
            item.EndTime = now;
            options.EndTime = now;
            options.CompletedTableCount++;
        }

        /// <summary>
        /// Validates the arguments of the copy of the data.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the target table, or <c>null</c>.</param>
        /// <param name="batchSize">The number of rows of a batch.</param>
        /// <param name="relationshipBehavior">The behavior that defines which related tables are copied.</param>
        /// <param name="tableExistenceBehavior">The behavior that defines what happens to the tables that already exist.</param>
        /// <param name="columnMappingsBehavior">The behavior that defines how the columns are aligned.</param>
        /// <param name="identityBehavior">The behavior that defines whether the values of the identity columns are copied as they are, or generated by the database of the target table.</param>
        /// <param name="mappings">The column mappings, or <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static void Validate(IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            string targetSchema,
            int batchSize,
            CopyRelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            CopyColumnMappingsBehavior columnMappingsBehavior,
            CopyIdentityBehavior identityBehavior,
            IList<CopyColumnMapping> mappings)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(sourceTable))
            {
                throw new ArgumentNullException(nameof(sourceTable));
            }
            if (string.IsNullOrWhiteSpace(targetTable))
            {
                throw new ArgumentNullException(nameof(targetTable));
            }
            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "The batch size must be greater than zero.");
            }
            if (!Enum.IsDefined(typeof(CopyColumnMappingsBehavior), columnMappingsBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(columnMappingsBehavior), "The column mappings behavior is not defined.");
            }
            if (!Enum.IsDefined(typeof(CopyIdentityBehavior), identityBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(identityBehavior), "The identity behavior is not defined.");
            }
            if (identityBehavior == CopyIdentityBehavior.Ignore && relationshipBehavior != CopyRelationshipBehavior.TableOnly)
            {
                throw new ArgumentException("The identity values can only be generated again when only the table is copied, as the rows of the related tables would no longer reference the rows that they referenced.", nameof(identityBehavior));
            }
            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    if (mapping == null)
                    {
                        throw new ArgumentException("The column mappings must not contain a null item.", nameof(mappings));
                    }
                    if (string.IsNullOrWhiteSpace(mapping.SourceColumn) || string.IsNullOrWhiteSpace(mapping.DestinationColumn))
                    {
                        throw new ArgumentException("The source and the destination columns of a column mapping must not be empty.", nameof(mappings));
                    }
                }
                var duplicate = mappings.GroupBy(mapping => mapping.DestinationColumn, StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1);
                if (duplicate != null)
                {
                    throw new ArgumentException($"The destination column '{duplicate.Key}' is mapped more than once.", nameof(mappings));
                }
            }
            if (IsSchemaRequested(relationshipBehavior, tableExistenceBehavior, targetSchema) && !string.Equals(GetTableName(connection, sourceTable), GetTableName(destinationConnection, targetTable), StringComparison.Ordinal))
            {
                throw new ArgumentException("The schema is copied with the name of the source table, so the source and the target tables must have the same name (the schema of the target table can be different) when the relationship behavior is not 'TableOnly', the table existence behavior is not 'Skip' or the target table has a schema.", nameof(targetTable));
            }
        }

        #endregion
    }
}
