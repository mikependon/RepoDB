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
using ColumnMapping = RepoDb.Data.Models.ColumnMapping;
using Endpoint = RepoDb.Data.Models.Endpoint;

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
            RelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            IList<ColumnMapping> mappings = null)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior, targetSchema))
            {
                return requested;
            }
            var results = new List<CopyTableSchemaResultItem>();
            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            var schemaMappings = ToSchemaMappings(mappings);
            var renamedTable = GetRenamedTable(sourceTable, targetTable);
            if (schemaMappings == null)
            {
                connection.CopyTableSchemaTo(sourceTable,
                    destinationConnection,
                    targetSchema: targetSchema,
                    targetTable: renamedTable,
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
                    new[] { new TableMapping(sourceTable, renamedTable, schemaMappings) },
                    targetSchema: targetSchema,
                    tableExistenceBehavior: tableExistenceBehavior,
                    relationshipBehavior: schemaBehavior,
                    createdCallback: results.Add,
                    commandTimeout: commandTimeout,
                    trace: trace,
                    transaction: transaction);
            }
            if (relationshipBehavior == RelationshipBehavior.TableOnly)
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
            RelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken,
            IList<ColumnMapping> mappings = null)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior, targetSchema))
            {
                return requested;
            }
            var results = new List<CopyTableSchemaResultItem>();
            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            var schemaMappings = ToSchemaMappings(mappings);
            var renamedTable = GetRenamedTable(sourceTable, targetTable);
            if (schemaMappings == null)
            {
                await connection.CopyTableSchemaToAsync(sourceTable,
                    destinationConnection,
                    targetSchema: targetSchema,
                    targetTable: renamedTable,
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
                    new[] { new TableMapping(sourceTable, renamedTable, schemaMappings) },
                    targetSchema: targetSchema,
                    tableExistenceBehavior: tableExistenceBehavior,
                    relationshipBehavior: schemaBehavior,
                    createdCallback: results.Add,
                    commandTimeout: commandTimeout,
                    trace: trace,
                    transaction: transaction,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            if (relationshipBehavior == RelationshipBehavior.TableOnly)
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
        /// Gets the name that the table is created with in the destination database, if it is not the name of the source table.
        /// </summary>
        /// <param name="sourceTable">The name of the source table, with or without its schema.</param>
        /// <param name="targetTable">The name of the target table, with or without its schema.</param>
        /// <returns>The name of the target table without its schema and quotes, or <c>null</c> if it is the name of the source table.</returns>
        internal static string GetRenamedTable(string sourceTable,
            string targetTable)
        {
            static string Simple(string name) =>
                name.Split('.').Last().Trim().Trim('[', ']', '"', '`');

            var target = string.IsNullOrWhiteSpace(targetTable) ? null : Simple(targetTable);
            return target == null || string.Equals(target, Simple(sourceTable), StringComparison.OrdinalIgnoreCase) ? null : target;
        }

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
        /// (<see cref="RelationshipBehavior.TableOnly"/> and <see cref="TableExistsBehavior.Skip"/>) without a target schema do not copy it.
        /// </summary>
        /// <param name="relationshipBehavior">The behavior that defines which related tables are copied.</param>
        /// <param name="tableExistenceBehavior">The behavior that defines what happens to the tables that already exist.</param>
        /// <param name="targetSchema">The schema of the target table, or <c>null</c>.</param>
        /// <returns><c>true</c> if the schema has to be copied; otherwise, <c>false</c>.</returns>
        internal static bool IsSchemaRequested(RelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            string targetSchema) =>
            relationshipBehavior != RelationshipBehavior.TableOnly ||
            tableExistenceBehavior != TableExistsBehavior.Skip ||
            !string.IsNullOrWhiteSpace(targetSchema);

        /// <summary>
        /// Gets the relationship behavior of the schema copy that is equivalent to the one of the data copy.
        /// </summary>
        /// <param name="relationshipBehavior">The behavior of the copy of the data.</param>
        /// <returns>The behavior of the copy of the schema.</returns>
        internal static RelatedTableBehavior ToSchemaBehavior(RelationshipBehavior relationshipBehavior)
        {
            switch (relationshipBehavior)
            {
                case RelationshipBehavior.TableOnly:
                    return RelatedTableBehavior.TableOnly;
                case RelationshipBehavior.Parents:
                    return RelatedTableBehavior.Parents;
                case RelationshipBehavior.Children:
                    return RelatedTableBehavior.Children;
                case RelationshipBehavior.ParentsAndChildren:
                    return RelatedTableBehavior.ParentsAndChildren;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relationshipBehavior));
            }
        }

        /// <summary>
        /// Counts all selected source tables before copying. Each callback exposes the accumulating totals.
        /// </summary>
        internal static void CountSourceRows(IDbConnection connection,
            IList<(string Source, string Target, QueryGroup Where)> tables,
            CopyProgress progress, Action<CopyProgress> callback, int? commandTimeout, ITrace trace)
        {
            foreach (var table in tables)
            {
                var count = connection.Count(table.Source, table.Where, commandTimeout: commandTimeout, trace: trace);
                progress.SourceRowCounts[table.Source] = count;
                progress.TotalRowCount += count;
                progress.EndTime = DateTime.UtcNow;
                callback?.Invoke(progress);
            }
        }

        /// <summary>
        /// Counts the selected source tables asynchronously before copying.
        /// </summary>
        internal static async Task CountSourceRowsAsync(IDbConnection connection,
            IList<(string Source, string Target, QueryGroup Where)> tables,
            CopyProgress progress, Action<CopyProgress> callback, int? commandTimeout, ITrace trace,
            CancellationToken cancellationToken)
        {
            foreach (var table in tables)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var count = await connection.CountAsync(table.Source, table.Where, commandTimeout: commandTimeout,
                    trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                progress.SourceRowCounts[table.Source] = count;
                progress.TotalRowCount += count;
                progress.EndTime = DateTime.UtcNow;
                callback?.Invoke(progress);
            }
        }

        /// <summary>
        /// Copies the rows of the source table into the target table. The rows are read with a data reader that is fed to the bulk insert of the destination connection
        /// (the <c>BulkOperations</c> package of its provider), or to its bulk merge if the rows that already exist are bypassed. If the provider has no bulk insert, the rows of the reader are inserted in batches.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="sourceTable">The name of the source table.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="where">The filter of the rows to be copied, or <c>null</c> to copy all the rows.</param>
        /// <param name="bufferSize">The number of rows of a batch.</param>
        /// <param name="bypassExistingRows">A value that indicates whether the bulk merge is used instead of the bulk insert, with the primary key (or the identity) of the target table as the qualifiers.</param>
        /// <param name="qualifierNames">The names of the columns that identify the existing rows (used only if the existing rows are bypassed), or <c>null</c> to use the primary key (or the identity) of the target table.</param>
        /// <param name="columnMappingsBehavior">The behavior that defines how the columns of the source table are aligned with the columns of the target table.</param>
        /// <param name="identityBehavior">The behavior that defines whether the values of the identity columns are copied as they are, or generated by the database of the target table.</param>
        /// <param name="mappings">The column mappings, or <c>null</c> to copy the columns that exist on both tables.</param>
        /// <param name="dataInterceptors">The interceptors that every row of the source goes through before it is copied, in order, or <c>null</c>.</param>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="progressCallback">The callback that receives the progress after every batch, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <exception cref="InvalidOperationException">The existing rows are bypassed and the target table has no qualifiers.</exception>
        internal static void CopyRows(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int bufferSize,
            bool bypassExistingRows,
            IList<string> qualifierNames,
            ColumnMappingBehavior columnMappingsBehavior,
            IdentityBehavior identityBehavior,
            IList<ColumnMapping> mappings,
            IList<IDataInterceptor<Row>> dataInterceptors,
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
            var qualifiers = bypassExistingRows ? GetQualifiers(destinationConnection, targetTable, qualifierNames, excluded, transaction) : null;
            var (commandText, param) = CreateSelect(connection, sourceTable, where, mappings);
            var interceptors = RowInterception.Normalize(dataInterceptors);
            using (var reader = connection.ExecuteReader(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace))
            using (var buffer = new BatchBuffer(reader, bufferSize, excluded, interceptors))
            {
                if (IsBulkAvailable(destinationConnection, buffer, false, bypassExistingRows))
                {
                    var total = 0;
                    while (buffer.Fill())
                    {
                        EnsureQualifiers(buffer, qualifiers, interceptors != null && item.Batches.Count == 0);
                        using (var batchReader = buffer.CreateReader())
                        {
                            _ = bypassExistingRows
                                ? BulkInsertInvoker.BulkMerge(destinationConnection, targetTable, batchReader, qualifiers, bufferSize, columnMappingsBehavior, identityBehavior == IdentityBehavior.KeepIdentity, commandTimeout, null, trace, transaction)
                                : BulkInsertInvoker.BulkInsert(destinationConnection, targetTable, batchReader, bufferSize, columnMappingsBehavior, identityBehavior == IdentityBehavior.KeepIdentity, commandTimeout, null, trace, transaction);
                        }
                        var copied = buffer.Count;
                        var bytes = buffer.CopiedBytes;
                        total += copied;
                        buffer.Clear();
                        Report(options, copied, progressCallback, bytes);
                    }
                    if (item.Batches.Count == 0)
                    {
                        Report(options, 0, progressCallback);
                    }
                    RecordCopy(options, destinationConnection, targetTable, bypassExistingRows, total);
                }
                else
                {
                    var action = new Action<IList<object>>((IList<object> b) =>
                    {
                        _ = bypassExistingRows
                            ? RowInserter.Merge(destinationConnection, targetTable, b, qualifiers, commandTimeout, traceKey, trace, transaction)
                            : RowInserter.Insert(destinationConnection, targetTable, b, commandTimeout, traceKey, trace, transaction);
                        Report(options, b.Count, progressCallback, CopySize.OfRows(b));
                    });
                    var batch = new List<object>(bufferSize);
                    var columns = interceptors == null ? null : RowInterception.GetColumns(reader);
                    while (reader.Read())
                    {
                        batch.Add(ToRow(reader, excluded, interceptors, columns));
                        if (batch.Count == bufferSize)
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
        /// <param name="bufferSize">The number of rows of a batch.</param>
        /// <param name="bypassExistingRows">A value that indicates whether the bulk merge is used instead of the bulk insert, with the primary key (or the identity) of the target table as the qualifiers.</param>
        /// <param name="qualifierNames">The names of the columns that identify the existing rows (used only if the existing rows are bypassed), or <c>null</c> to use the primary key (or the identity) of the target table.</param>
        /// <param name="columnMappingsBehavior">The behavior that defines how the columns of the source table are aligned with the columns of the target table.</param>
        /// <param name="identityBehavior">The behavior that defines whether the values of the identity columns are copied as they are, or generated by the database of the target table.</param>
        /// <param name="mappings">The column mappings, or <c>null</c> to copy the columns that exist on both tables.</param>
        /// <param name="dataInterceptors">The interceptors that every row of the source goes through before it is copied, in order, or <c>null</c>.</param>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="progressCallback">The callback that receives the progress after every batch, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">The existing rows are bypassed and the target table has no qualifiers.</exception>
        internal static async Task CopyRowsAsync(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int bufferSize,
            bool bypassExistingRows,
            IList<string> qualifierNames,
            ColumnMappingBehavior columnMappingsBehavior,
            IdentityBehavior identityBehavior,
            IList<ColumnMapping> mappings,
            IList<IDataInterceptor<Row>> dataInterceptors,
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
            var qualifiers = bypassExistingRows ? GetQualifiers(destinationConnection, targetTable, qualifierNames, excluded, transaction) : null;
            var (commandText, param) = CreateSelect(connection, sourceTable, where, mappings);
            var interceptors = RowInterception.Normalize(dataInterceptors);
            using (var reader = await connection.ExecuteReaderAsync(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false))
            using (var buffer = new BatchBuffer(reader, bufferSize, excluded, interceptors))
            {
                if (IsBulkAvailable(destinationConnection, buffer, true, bypassExistingRows))
                {
                    var total = 0;
                    while (await buffer.FillAsync(cancellationToken).ConfigureAwait(false))
                    {
                        EnsureQualifiers(buffer, qualifiers, interceptors != null && item.Batches.Count == 0);
                        using (var batchReader = buffer.CreateReader())
                        {
                            _ = bypassExistingRows
                                ? await BulkInsertInvoker.BulkMergeAsync(destinationConnection, targetTable, batchReader, qualifiers, bufferSize, columnMappingsBehavior, identityBehavior == IdentityBehavior.KeepIdentity, commandTimeout, null, trace, transaction, cancellationToken).ConfigureAwait(false)
                                : await BulkInsertInvoker.BulkInsertAsync(destinationConnection, targetTable, batchReader, bufferSize, columnMappingsBehavior, identityBehavior == IdentityBehavior.KeepIdentity, commandTimeout, null, trace, transaction, cancellationToken).ConfigureAwait(false);
                        }
                        var copied = buffer.Count;
                        var bytes = buffer.CopiedBytes;
                        total += copied;
                        buffer.Clear();
                        Report(options, copied, progressCallback, bytes);
                    }
                    if (item.Batches.Count == 0)
                    {
                        Report(options, 0, progressCallback);
                    }
                    RecordCopy(options, destinationConnection, targetTable, bypassExistingRows, total);
                }
                else
                {
                    var action = new Func<IList<object>, Task>(async (IList<object> b) =>
                    {
                        _ = bypassExistingRows
                            ? await RowInserter.MergeAsync(destinationConnection, targetTable, b, qualifiers, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false)
                            : await RowInserter.InsertAsync(destinationConnection, targetTable, b, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
                        Report(options, b.Count, progressCallback, CopySize.OfRows(b));
                    });
                    var batch = new List<object>(bufferSize);
                    var columns = interceptors == null ? null : RowInterception.GetColumns(reader);
                    while (await ReadAsync(reader, cancellationToken).ConfigureAwait(false))
                    {
                        batch.Add(ToRow(reader, excluded, interceptors, columns));
                        if (batch.Count == bufferSize)
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

        /// <summary>Logs the in-memory row total without querying the destination.</summary>
        private static void RecordCopy(CopyProgress options,
            IDbConnection destinationConnection,
            string targetTable,
            bool bypassExistingRows,
            int total)
        {
            if (bypassExistingRows)
            {
                CopyDataLogger.LogBulkMerge(options, destinationConnection, targetTable, total);
            }
            else
            {
                CopyDataLogger.LogBulkInsert(options, destinationConnection, targetTable, total);
            }
        }

        /// <summary>
        /// Gets the mappings of the columns for the schema copy, so the schema of the target table is created with only the mapped columns, by their destination names.
        /// </summary>
        /// <param name="mappings">The column mappings of the copy of the data.</param>
        /// <returns>The mappings of the schema copy (<c>null</c> if there are no mappings, which creates all the columns). A source column that is mapped more than once is created with its first destination column.</returns>
        private static IList<RepoDb.Schema.Models.ColumnMapping> ToSchemaMappings(IList<ColumnMapping> mappings)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return null;
            }
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return mappings
                .Where(mapping => seen.Add(mapping.SourceColumn))
                .Select(mapping => new RepoDb.Schema.Models.ColumnMapping(mapping.SourceColumn, mapping.DestinationColumn))
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
        internal static (string CommandText, object Param) CreateSelect(IDbConnection connection,
            string sourceTable,
            QueryGroup where,
            IList<ColumnMapping> mappings)
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
            IList<ColumnMapping> mappings)
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
            IList<ColumnMapping> mappings)
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
        /// Checks whether the bulk insert (or the bulk merge, if the existing rows are bypassed) of the provider of the destination connection accepts the batches of the buffer
        /// (the data reader of the buffered rows).
        /// </summary>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="buffer">The buffer of the rows to be copied.</param>
        /// <param name="isAsync">A value that indicates whether the asynchronous bulk operation is searched.</param>
        /// <param name="bypassExistingRows">A value that indicates whether the bulk merge is searched instead of the bulk insert.</param>
        /// <returns><c>true</c> if the provider has the bulk operation for the batches; otherwise, <c>false</c>.</returns>
        private static bool IsBulkAvailable(IDbConnection destinationConnection,
            BatchBuffer buffer,
            bool isAsync,
            bool bypassExistingRows)
        {
            using (var probe = buffer.CreateReader())
            {
                return bypassExistingRows
                    ? BulkInsertInvoker.IsMergeAvailable(destinationConnection, probe, isAsync)
                    : BulkInsertInvoker.IsAvailable(destinationConnection, probe, isAsync);
            }
        }

        /// <summary>
        /// Gets the qualifiers of the merge: the columns that were given, or the primary key columns of the target table (its identity column if it has no primary key).
        /// </summary>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetTable">The name of the target table.</param>
        /// <param name="names">The names of the columns that were given, or <c>null</c>.</param>
        /// <param name="excludedColumns">The names of the columns that are left out of the rows (the identity columns that are generated by the database), or <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <returns>The qualifiers.</returns>
        /// <exception cref="ArgumentException">A given qualifier is empty, or is not a column of the target table.</exception>
        /// <exception cref="InvalidOperationException">No qualifier was given and the target table has no primary key and no identity column, or the qualifiers are not copied.</exception>
        internal static IList<Field> GetQualifiers(IDbConnection destinationConnection,
            string targetTable,
            IList<string> names,
            ISet<string> excludedColumns,
            IDbTransaction transaction)
        {
            var dbSetting = destinationConnection.GetDbSetting();
            var dbFields = DbFieldCache.Get(destinationConnection, targetTable, transaction);
            List<string> columns;
            if (names != null && names.Count > 0)
            {
                columns = new List<string>();
                foreach (var name in names)
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new ArgumentException("The qualifiers must not have an empty name.", "qualifiers");
                    }
                    var dbField = dbFields?.GetByUnquotedName(name.AsUnquoted(true, dbSetting));
                    if (dbField == null)
                    {
                        throw new ArgumentException($"The qualifier '{name}' is not a column of the table '{targetTable}'.", "qualifiers");
                    }
                    var unquoted = dbField.Name.AsUnquoted(true, dbSetting);
                    if (!columns.Contains(unquoted, StringComparer.OrdinalIgnoreCase))
                    {
                        columns.Add(unquoted);
                    }
                }
            }
            else
            {
                var items = dbFields?.GetItems();
                var qualifiers = items?.Where(field => field.IsPrimary).ToList();
                if (qualifiers == null || qualifiers.Count == 0)
                {
                    qualifiers = items?.Where(field => field.IsIdentity).ToList();
                }
                if (qualifiers == null || qualifiers.Count == 0)
                {
                    throw new InvalidOperationException($"The existing rows can only be bypassed if the target table '{targetTable}' has a primary key or an identity column, as they are the qualifiers of the merge.");
                }
                columns = qualifiers.Select(field => field.Name.AsUnquoted(true, dbSetting)).ToList();
            }
            if (excludedColumns != null && columns.Any(excludedColumns.Contains))
            {
                throw new InvalidOperationException($"The existing rows cannot be bypassed while the identity values are generated by the database, as a qualifier of the target table '{targetTable}' is its identity column.");
            }
            return columns.Select(name => new Field(name)).ToList();
        }

        /// <summary>
        /// Converts the current row of the reader into an object, to be inserted: the row goes through the interceptors first, if there are any.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="excludedColumns">The names of the columns that are left out of the row, or <c>null</c> to have all of them.</param>
        /// <param name="interceptors">The interceptors that the row goes through, in order, or <c>null</c>.</param>
        /// <param name="columns">The columns of the record (see <see cref="RowInterception.GetColumns"/>), when there are interceptors.</param>
        /// <returns>The row, as an object that has a property for each column.</returns>
        private static object ToRow(IDataRecord record,
            ISet<string> excludedColumns,
            IList<IDataInterceptor<Row>> interceptors,
            List<Column> columns)
        {
            var row = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            if (interceptors != null)
            {
                var intercepted = RowInterception.Apply(RowInterception.Read(record, columns), interceptors);
                for (var i = 0; i < intercepted.Count; i++)
                {
                    var name = intercepted.Columns[i].Name;
                    if (excludedColumns == null || !excludedColumns.Contains(name))
                    {
                        row[name] = intercepted[i];
                    }
                }
                return row;
            }
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
        /// Ensures that the columns of the batches have the qualifiers of the merge (the interceptors may have removed one of them).
        /// </summary>
        /// <param name="buffer">The buffer of the rows to be copied.</param>
        /// <param name="qualifiers">The qualifiers of the merge, or <c>null</c> if the rows are inserted.</param>
        /// <param name="check">A value that indicates whether the columns are checked (they are the same for every row).</param>
        /// <exception cref="InvalidOperationException">A qualifier is not a column of the batches.</exception>
        private static void EnsureQualifiers(BatchBuffer buffer,
            IList<Field> qualifiers,
            bool check)
        {
            if (!check || qualifiers == null)
            {
                return;
            }
            var missing = qualifiers.FirstOrDefault(qualifier => !buffer.HasColumn(qualifier.Name));
            if (missing != null)
            {
                throw new InvalidOperationException($"The existing rows cannot be bypassed as the qualifier '{missing.Name}' is not a column of the rows that the data interceptors give.");
            }
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
            IdentityBehavior identityBehavior,
            IDbTransaction transaction)
        {
            if (identityBehavior != IdentityBehavior.Ignore)
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
        internal static Task<bool> ReadAsync(IDataReader reader,
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
            item.TotalRowCount = options.SourceRowCounts.TryGetValue(sourceTable, out var count) ? count : 0;
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
        private static Endpoint CreateEndpoint(IDbConnection connection,
            string table) =>
            new Endpoint
            {
                Server = (connection as System.Data.Common.DbConnection)?.DataSource,
                Database = connection.Database,
                DatabaseType = connection.GetType().Name,
                Table = new TableInfo(GetTableName(connection, table), GetTargetSchema(connection, table), connection.Database)
            };

        /// <summary>
        /// Reports the progress of the copy: a batch of rows was copied.
        /// </summary>
        /// <param name="options">The progress of the copy.</param>
        /// <param name="inserted">The number of rows that were inserted by the batch.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        private static void Report(CopyProgress options,
            int inserted,
            Action<CopyProgress> progressCallback,
            long copiedBytes = 0)
        {
            var item = options.Current;
            var now = DateTime.UtcNow;
            item.Batches.Add(new CopyBatch
            {
                Number = item.Batches.Count + 1,
                RowCount = inserted,
                CopiedBytes = copiedBytes,
                StartTime = item.NextBatchStart,
                EndTime = now
            });
            item.RowCount += inserted;
            item.CopiedBytes += copiedBytes;
            options.CopiedBytes += copiedBytes;
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
        /// <param name="bufferSize">The number of rows of a batch.</param>
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
            int bufferSize,
            RelationshipBehavior relationshipBehavior,
            TableExistsBehavior tableExistenceBehavior,
            ColumnMappingBehavior columnMappingsBehavior,
            IdentityBehavior identityBehavior,
            IList<ColumnMapping> mappings)
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
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "The buffer size must be greater than zero.");
            }
            if (!Enum.IsDefined(typeof(ColumnMappingBehavior), columnMappingsBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(columnMappingsBehavior), "The column mappings behavior is not defined.");
            }
            if (!Enum.IsDefined(typeof(IdentityBehavior), identityBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(identityBehavior), "The identity behavior is not defined.");
            }
            if (identityBehavior == IdentityBehavior.Ignore && relationshipBehavior != RelationshipBehavior.TableOnly)
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
            if (relationshipBehavior != RelationshipBehavior.TableOnly && !string.Equals(GetTableName(connection, sourceTable), GetTableName(destinationConnection, targetTable), StringComparison.Ordinal))
            {
                throw new ArgumentException("The related tables are copied with the names of the source tables, so the source and the target tables must have the same name (the schema of the target table can be different) when the relationship behavior is not 'TableOnly'.", nameof(targetTable));
            }
        }

        #endregion
    }
}
