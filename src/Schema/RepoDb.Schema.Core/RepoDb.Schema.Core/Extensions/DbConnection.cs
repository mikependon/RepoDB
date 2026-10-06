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
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains the helper methods of the <see cref="CopySchemaToExtension"/> class: the checks of the destination tables, the reports of the results and the validation of the arguments.
    /// </summary>
    internal static class DbConnection
    {
        #region Methods

        /// <summary>
        /// Finds the result of the table that was requested among the results of the tables that were copied (the requested table and its related tables).
        /// </summary>
        /// <param name="results">The results of the copied tables.</param>
        /// <param name="table">The identity of the table that was requested.</param>
        /// <returns>The result of the requested table.</returns>
        internal static CopySchemaResult FindResult(IList<CopySchemaResult> results,
            TableInfo table) =>
            results.FirstOrDefault(r => string.Equals(r.TableName, table.Name, StringComparison.Ordinal) &&
                string.Equals(r.SourceSchema, table.Schema, StringComparison.Ordinal));

        /// <summary>
        /// Gets the schema of the destination database that the tables are created in: the given one, or the default schema of the setting of the destination connection (see <see cref="IDbSetting.DefaultSchema"/>).
        /// </summary>
        /// <param name="targetSchema"></param>
        /// <param name="destinationConnection"></param>
        /// <returns>The schema (<c>null</c> if it is not given, and the setting has no default schema).</returns>
        internal static string GetTargetSchema(string targetSchema,
            IDbConnection destinationConnection)
        {
            var schema = string.IsNullOrWhiteSpace(targetSchema)
                ? destinationConnection.GetDbSetting().DefaultSchema
                : targetSchema;
            return string.IsNullOrWhiteSpace(schema) ? null : schema;
        }

        /// <summary>
        /// Reads the state of the tables in the destination database: whether each one exists and, when the existing tables are aligned,
        /// the columns and the indexes that they are missing. A statement that the composer does not compose leaves the state unknown.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="behavior"></param>
        /// <param name="composer"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        internal static void ProbeTables(IEnumerable<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            IDbConnection destinationConnection,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction)
        {
            bool? Probe(string statement) =>
                string.IsNullOrWhiteSpace(statement)
                    ? (bool?)null
                    : ToBoolean(destinationConnection.ExecuteScalar(statement, commandTimeout: commandTimeout, transaction: transaction, trace: trace));

            foreach (var table in tables)
            {
                table.Exists = Probe(composer.ComposeTableExists(table.Name));
                if (table.Exists == true && behavior == CopySchemaExistsBehavior.Align)
                {
                    table.MissingColumns = table.Schema.Columns.Where(column => Probe(composer.ComposeColumnExists(table.Name, column.Field.Name)) == false).ToList();
                    table.MissingIndexes = table.Schema.Indexes.Where(index => index.Name != null && Probe(composer.ComposeIndexExists(table.Name, index.Name)) == false).ToList();
                }
            }
        }

        /// <summary>
        /// Reads the state of the tables in the destination database (see <see cref="ProbeTables"/>).
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="behavior"></param>
        /// <param name="composer"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        internal static async Task ProbeTablesAsync(IEnumerable<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            IDbConnection destinationConnection,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            async Task<bool?> Probe(string statement) =>
                string.IsNullOrWhiteSpace(statement)
                    ? (bool?)null
                    : ToBoolean(await destinationConnection.ExecuteScalarAsync(statement, commandTimeout: commandTimeout, transaction: transaction, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false));

            foreach (var table in tables)
            {
                table.Exists = await Probe(composer.ComposeTableExists(table.Name)).ConfigureAwait(false);
                if (table.Exists == true && behavior == CopySchemaExistsBehavior.Align)
                {
                    table.MissingColumns = new List<ColumnInfo>();
                    foreach (var column in table.Schema.Columns)
                    {
                        if (await Probe(composer.ComposeColumnExists(table.Name, column.Field.Name)).ConfigureAwait(false) == false)
                        {
                            table.MissingColumns.Add(column);
                        }
                    }
                    table.MissingIndexes = new List<IndexInfo>();
                    foreach (var index in table.Schema.Indexes.Where(index => index.Name != null))
                    {
                        if (await Probe(composer.ComposeIndexExists(table.Name, index.Name)).ConfigureAwait(false) == false)
                        {
                            table.MissingIndexes.Add(index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts the result of an existence statement (<c>1</c> if it exists).
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static bool ToBoolean(object result) =>
            result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;

        /// <summary>
        /// Creates the error of a statement that has failed.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="step"></param>
        /// <param name="statementIndex"></param>
        /// <returns></returns>
        internal static CopySchemaError CreateError(Exception exception,
            CopySchemaStep step,
            int statementIndex) =>
            new CopySchemaError
            {
                Exception = exception,
                Statement = step.Statement,
                StatementIndex = statementIndex,
                TableName = step.Owner?.Schema.Table?.Name,
                SchemaName = step.Owner?.Schema.Table?.Schema
            };

        /// <summary>
        /// Gets the script that was (or is going to be) executed for the table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="schemaComposer"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GetScript(CopySchemaTable table,
            ISchemaComposer schemaComposer)
        {
            switch (table.Outcome)
            {
                case CopySchemaOutcome.Skipped:
                    return Enumerable.Empty<string>();
                case CopySchemaOutcome.Aligned:
                    return table.AlignStatements;
                default:
                    var create = schemaComposer.ComposeSchema(table.Schema) ?? Enumerable.Empty<string>();
                    return table.DropStatement == null ? create : new[] { table.DropStatement }.Concat(create);
            }
        }

        /// <summary>
        /// Calls the callback with the result of each table that is completed.
        /// </summary>
        /// <param name="createdCallback"></param>
        /// <param name="tables"></param>
        /// <param name="errors"></param>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="schemaComposer"></param>
        /// <param name="startTime"></param>
        internal static void Report(Action<CopySchemaResult> createdCallback,
            IEnumerable<CopySchemaTable> tables,
            IList<CopySchemaError> errors,
            IDbConnection connection,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior,
            ISchemaComposer schemaComposer,
            DateTime startTime)
        {
            if (createdCallback == null)
            {
                return;
            }

            foreach (var table in tables)
            {
                var schema = table.Schema;
                var aligned = table.Outcome == CopySchemaOutcome.Aligned;
                var schemaErrors = errors
                    .Where(error => error.TableName == null ||
                        (string.Equals(error.TableName, schema.Table.Name, StringComparison.Ordinal) &&
                        string.Equals(error.SchemaName, schema.Table.Schema, StringComparison.Ordinal)))
                    .ToList();

                createdCallback(new CopySchemaResult
                {
                    Action = tableExistenceBehavior,
                    Outcome = schemaErrors.Count > 0 ? CopySchemaOutcome.Failed : table.Outcome,
                    TableExisted = table.Exists,
                    AddedColumns = aligned ? table.MissingColumns.Select(column => column.Field.Name).ToList() : new List<string>(),
                    AddedIndexes = aligned ? table.MissingIndexes.Select(index => index.Name).ToList() : new List<string>(),
                    Errors = schemaErrors,
                    TableName = schema.Table.Name,
                    SourceSchema = table.Source.Table.Schema,
                    DestinationSchema = schema.Table.Schema,
                    SourceDatabase = connection.Database,
                    SourceServer = (connection as System.Data.Common.DbConnection)?.DataSource,
                    SourceDatabaseType = connection.GetType().Name,
                    DestinationDatabase = destinationConnection.Database,
                    DestinationServer = (destinationConnection as System.Data.Common.DbConnection)?.DataSource,
                    DestinationDatabaseType = destinationConnection.GetType().Name,
                    StartTime = startTime,
                    EndTime = DateTime.UtcNow,
                    Script = string.Join(Environment.NewLine, GetScript(table, schemaComposer)),
                    ColumnCount = schema.Columns.Count,
                    IndexCount = schema.Indexes.Count,
                    ForeignKeyCount = schema.ForeignKeys.Count,
                    UniqueConstraintCount = schema.UniqueConstraints.Count,
                    CheckConstraintCount = schema.CheckConstraints.Count
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="destinationConnection"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void Validate(IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableNames"></param>
        /// <param name="destinationConnection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static IList<string> Validate(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (tableNames == null)
            {
                throw new ArgumentNullException(nameof(tableNames));
            }
            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
            var names = tableNames.ToList();
            if (names.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("The table names cannot contain a null or a white-space name.", nameof(tableNames));
            }
            return names;
        }

        #endregion
    }
}
