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
        /// If the schema is requested (see <see cref="IsSchemaRequested"/>), it is copied first, with <see cref="CopySchemaToExtension.CopySchemaTo(IDbConnection, IEnumerable{string}, IDbConnection, CopySchemaExistsBehavior, CopySchemaRelationshipBehavior, Action{CopySchemaResult}, Action{CopySchemaError}, int?, string, ITrace, IDbTransaction)"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="targetSchema"></param>
        /// <param name="where"></param>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="mappings">The column mappings of the requested table: only the mapped columns of the source table are created in the target table when the schema is copied.</param>
        /// <returns>The source table, the target table and the filter of each table.</returns>
        internal static IList<(string Source, string Target, QueryGroup Where)> GetTables(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            string targetSchema,
            QueryGroup where,
            CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            IList<CopyDataColumnMapping> mappings = null)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior, targetSchema))
            {
                return requested;
            }
            var results = new List<CopySchemaResult>();
            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            connection.CopySchemaTo(new[] { sourceTable },
                destinationConnection,
                columns: ToColumns(mappings),
                targetSchema: targetSchema,
                tableExistenceBehavior: tableExistenceBehavior,
                relationshipBehavior: schemaBehavior,
                createdCallback: results.Add,
                commandTimeout: commandTimeout,
                trace: trace,
                transaction: transaction);
            if (relationshipBehavior == CopyDataRelationshipBehavior.TableOnly)
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
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="targetSchema"></param>
        /// <param name="where"></param>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="mappings">The column mappings of the requested table: only the mapped columns of the source table are created in the target table when the schema is copied.</param>
        /// <returns>The source table, the target table and the filter of each table.</returns>
        internal static async Task<IList<(string Source, string Target, QueryGroup Where)>> GetTablesAsync(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            string targetSchema,
            QueryGroup where,
            CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken,
            IList<CopyDataColumnMapping> mappings = null)
        {
            var requested = new List<(string, string, QueryGroup)> { (sourceTable, targetTable, where) };
            if (!IsSchemaRequested(relationshipBehavior, tableExistenceBehavior, targetSchema))
            {
                return requested;
            }
            var results = new List<CopySchemaResult>();
            var schemaBehavior = ToSchemaBehavior(relationshipBehavior);
            await connection.CopySchemaToAsync(new[] { sourceTable },
                destinationConnection,
                columns: ToColumns(mappings),
                targetSchema: targetSchema,
                tableExistenceBehavior: tableExistenceBehavior,
                relationshipBehavior: schemaBehavior,
                createdCallback: results.Add,
                commandTimeout: commandTimeout,
                trace: trace,
                transaction: transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            if (relationshipBehavior == CopyDataRelationshipBehavior.TableOnly)
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
        /// The rows are copied into the schema that the table was created in (see <see cref="CopySchemaResult.DestinationSchema"/>).
        /// </summary>
        /// <param name="relationships"></param>
        /// <param name="requested"></param>
        /// <param name="requestedTable"></param>
        /// <param name="results"></param>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <returns></returns>
        internal static IList<(string Source, string Target, QueryGroup Where)> ToTables(IEnumerable<RelationshipInfo> relationships,
            TableInfo requested,
            (string Source, string Target, QueryGroup Where) requestedTable,
            IEnumerable<CopySchemaResult> results,
            IDbConnection connection,
            IDbConnection destinationConnection) =>
            relationships
                .Select(relationship => relationship.Schema.Table)
                .Select(table =>
                {
                    var target = GetName(destinationConnection, results.First(result => result.TableName == table.Name && result.SourceSchema == table.Schema));
                    return table.Equals(requested)
                        ? (requestedTable.Source, target, requestedTable.Where)
                        : (GetName(connection, table.Schema, table.Name), target, (QueryGroup)null);
                })
                .ToList();

        /// <summary>
        /// Gets the name of the table that was created in the destination database, quoted for the database of the connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static string GetName(IDbConnection connection,
            CopySchemaResult result) =>
            GetName(connection, result.DestinationSchema, result.TableName);

        /// <summary>
        /// Gets the name of the table, quoted for the database of the connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <returns></returns>
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
        /// <param name="destinationConnection"></param>
        /// <param name="targetTable"></param>
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
        /// <param name="connection"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static string GetTableName(IDbConnection connection,
            string table)
        {
            var setting = DbSettingMapper.Get(connection);
            return setting == null ? table : DataEntityExtension.GetTableName(table, setting).AsUnquoted(setting);
        }

        /// <summary>
        /// Checks whether the schema of the tables is to be copied before their rows: the default behaviors
        /// (<see cref="CopyDataRelationshipBehavior.TableOnly"/> and <see cref="CopySchemaExistsBehavior.Skip"/>) without a target schema do not copy it.
        /// </summary>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="targetSchema"></param>
        /// <returns></returns>
        internal static bool IsSchemaRequested(CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior,
            string targetSchema) =>
            relationshipBehavior != CopyDataRelationshipBehavior.TableOnly ||
            tableExistenceBehavior != CopySchemaExistsBehavior.Skip ||
            !string.IsNullOrWhiteSpace(targetSchema);

        /// <summary>
        /// Gets the relationship behavior of the schema copy that is equivalent to the one of the data copy.
        /// </summary>
        /// <param name="relationshipBehavior"></param>
        /// <returns></returns>
        internal static CopySchemaRelationshipBehavior ToSchemaBehavior(CopyDataRelationshipBehavior relationshipBehavior)
        {
            switch (relationshipBehavior)
            {
                case CopyDataRelationshipBehavior.TableOnly:
                    return CopySchemaRelationshipBehavior.TableOnly;
                case CopyDataRelationshipBehavior.Parents:
                    return CopySchemaRelationshipBehavior.Parents;
                case CopyDataRelationshipBehavior.Children:
                    return CopySchemaRelationshipBehavior.Children;
                case CopyDataRelationshipBehavior.ParentsAndChildren:
                    return CopySchemaRelationshipBehavior.ParentsAndChildren;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relationshipBehavior));
            }
        }

        /// <summary>
        /// Copies the rows of the source table into the target table. The rows are read with a data reader that is fed to the bulk insert of the destination connection
        /// (the <c>BulkOperations</c> package of its provider). If the provider has no bulk insert, the rows of the reader are inserted in batches.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="where"></param>
        /// <param name="batchSize"></param>
        /// <param name="columnMappingsBehavior"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="progressCallback"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        internal static void CopyRows(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int batchSize,
            CopySchemaColumnMappingsBehavior columnMappingsBehavior,
            IList<CopyDataColumnMapping> mappings,
            CopyDataProgress options,
            Action<CopyDataProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            SetTables(options, connection, destinationConnection, sourceTable, targetTable);
            ValidateMappings(connection, destinationConnection, sourceTable, targetTable, mappings);
            var (commandText, param) = CreateSelect(connection, sourceTable, where, mappings);
            using (var reader = connection.ExecuteReader(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace))
            {
                if (BulkInsertInvoker.IsAvailable(destinationConnection, reader, false))
                {
                    var before = CopyDataLogger.Count(destinationConnection, targetTable, transaction);
                    var inserted = BulkInsertInvoker.BulkInsert(destinationConnection, targetTable, reader, batchSize, columnMappingsBehavior, commandTimeout, null, trace, transaction);
                    CopyDataLogger.LogBulkInsert(options, destinationConnection, targetTable, inserted, before, CopyDataLogger.Count(destinationConnection, targetTable, transaction));
                    Report(options, inserted, progressCallback);
                    return;
                }
                var action = new Action<IList<object>>((IList<object> b) =>
                    Report(options, RowInserter.Insert(destinationConnection, targetTable, b, commandTimeout, traceKey, trace, transaction), progressCallback));
                var batch = new List<object>(batchSize);
                while (reader.Read())
                {
                    batch.Add(ToRow(reader));
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

        /// <summary>
        /// Copies the rows of the source table into the target table in an asynchronous way (see <see cref="CopyRows"/>).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="where"></param>
        /// <param name="batchSize"></param>
        /// <param name="columnMappingsBehavior"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="progressCallback"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        internal static async Task CopyRowsAsync(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            QueryGroup where,
            int batchSize,
            CopySchemaColumnMappingsBehavior columnMappingsBehavior,
            IList<CopyDataColumnMapping> mappings,
            CopyDataProgress options,
            Action<CopyDataProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            SetTables(options, connection, destinationConnection, sourceTable, targetTable);
            ValidateMappings(connection, destinationConnection, sourceTable, targetTable, mappings);
            var (commandText, param) = CreateSelect(connection, sourceTable, where, mappings);
            using (var reader = await connection.ExecuteReaderAsync(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                if (BulkInsertInvoker.IsAvailable(destinationConnection, reader, true))
                {
                    var before = await CopyDataLogger.CountAsync(destinationConnection, targetTable, transaction, cancellationToken).ConfigureAwait(false);
                    var inserted = await BulkInsertInvoker.BulkInsertAsync(destinationConnection, targetTable, reader, batchSize, columnMappingsBehavior, commandTimeout, null, trace, transaction, cancellationToken).ConfigureAwait(false);
                    var after = await CopyDataLogger.CountAsync(destinationConnection, targetTable, transaction, cancellationToken).ConfigureAwait(false);
                    CopyDataLogger.LogBulkInsert(options, destinationConnection, targetTable, inserted, before, after);
                    Report(options, inserted, progressCallback);
                    return;
                }
                var action = new Func<IList<object>, Task>(async (IList<object> b) =>
                    Report(options, await RowInserter.InsertAsync(destinationConnection, targetTable, b, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false), progressCallback));
                var batch = new List<object>(batchSize);
                while (await ReadAsync(reader, cancellationToken).ConfigureAwait(false))
                {
                    batch.Add(ToRow(reader));
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

        /// <summary>
        /// Gets the names of the source columns of the mappings, so the schema of the target table is created with only the mapped columns.
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns>The distinct names of the source columns (<c>null</c> if there are no mappings, which creates all the columns).</returns>
        private static string[] ToColumns(IList<CopyDataColumnMapping> mappings) =>
            mappings == null || mappings.Count == 0
                ? null
                : mappings.Select(mapping => mapping.SourceColumn).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        /// <summary>
        /// Composes the <c>SELECT</c> statement of the source table with the statement builder of the connection, and the parameters of its filter.
        /// If there are column mappings, only the mapped columns are selected, each one aliased with the name of its destination column.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="where"></param>
        /// <param name="mappings"></param>
        /// <returns>The statement and its parameters (<c>null</c> if there is no filter).</returns>
        private static (string CommandText, object Param) CreateSelect(IDbConnection connection,
            string sourceTable,
            QueryGroup where,
            IList<CopyDataColumnMapping> mappings)
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
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="mappings"></param>
        /// <returns>The statement.</returns>
        private static string Alias(IDbConnection connection,
            string commandText,
            IList<CopyDataColumnMapping> mappings)
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
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="mappings"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidateMappings(IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable,
            IList<CopyDataColumnMapping> mappings)
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
        /// <param name="where"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IList<object> ToList(object value) =>
            value is System.Collections.IEnumerable enumerable && !(value is string)
                ? enumerable.Cast<object>().ToList()
                : new List<object> { value };

        /// <summary>
        /// Converts the current row of the reader into an object, to be inserted.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static object ToRow(IDataRecord record)
        {
            var row = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            for (var i = 0; i < record.FieldCount; i++)
            {
                row[record.GetName(i)] = record.IsDBNull(i) ? null : record.GetValue(i);
            }
            return row;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static Task<bool> ReadAsync(IDataReader reader,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return reader is System.Data.Common.DbDataReader dbReader
                ? dbReader.ReadAsync(cancellationToken)
                : Task.FromResult(reader.Read());
        }

        /// <summary>
        /// Sets the details of the tables (and the databases) that are being copied into the progress.
        /// </summary>
        private static void SetTables(CopyDataProgress options,
            IDbConnection connection,
            IDbConnection destinationConnection,
            string sourceTable,
            string targetTable)
        {
            if (options == null)
            {
                return;
            }
            options.SourceServer = (connection as System.Data.Common.DbConnection)?.DataSource;
            options.SourceDatabase = connection.Database;
            options.SourceTable = sourceTable;
            options.TargetServer = (destinationConnection as System.Data.Common.DbConnection)?.DataSource;
            options.TargetDatabase = destinationConnection.Database;
            options.TargetTable = targetTable;
            options.BatchStartTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Reports the progress of the copy: a batch of rows was copied.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="inserted"></param>
        /// <param name="progressCallback"></param>
        private static void Report(CopyDataProgress options,
            int inserted,
            Action<CopyDataProgress> progressCallback)
        {
            options.BatchNumber++;
            options.RowCount = inserted;
            options.BatchCopiedRowCount = inserted;
            options.TotalCopiedRowCount += inserted;
            options.BatchEndTime = DateTime.UtcNow;
            options.EndTime = options.BatchEndTime;
            progressCallback?.Invoke(options);
            options.BatchStartTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="targetSchema"></param>
        /// <param name="batchSize"></param>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="columnMappingsBehavior"></param>
        /// <param name="mappings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static void Validate(IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            string targetSchema,
            int batchSize,
            CopyDataRelationshipBehavior relationshipBehavior,
            CopySchemaExistsBehavior tableExistenceBehavior,
            CopySchemaColumnMappingsBehavior columnMappingsBehavior,
            IList<CopyDataColumnMapping> mappings)
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
            if (!Enum.IsDefined(typeof(CopySchemaColumnMappingsBehavior), columnMappingsBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(columnMappingsBehavior), "The column mappings behavior is not defined.");
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
