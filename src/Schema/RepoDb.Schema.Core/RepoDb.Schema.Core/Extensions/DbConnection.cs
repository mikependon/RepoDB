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
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains the helper methods of the <see cref="CopyTableSchemaToExtension"/> class: the checks of the destination tables, the reports of the results and the validation of the arguments.
    /// </summary>
    internal static class DbConnection
    {
        #region Methods

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
        /// the columns, the indexes and the foreign keys that they are missing. A statement that the composer does not compose leaves the state unknown.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="behavior"></param>
        /// <param name="composer"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static void ProbeTables(IEnumerable<PlanTable> tables,
            CopyTableSchemaExistsBehavior behavior,
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

            // A foreign key is added to a table that exists only if the table that it references is copied, or already exists (a table is not created to be referenced)
            bool CanBeReferenced(ForeignKeyInfo foreignKey)
            {
                if (foreignKey.ReferencedTable == null)
                {
                    return true;
                }
                var name = composer.ComposeName(foreignKey.ReferencedTable);
                return tables.Any(item => string.Equals(item.Name, name, StringComparison.Ordinal)) || Probe(composer.ComposeTableExists(name)) == true;
            }

            foreach (var table in tables)
            {
                table.Exists = Probe(composer.ComposeTableExists(table.Name));
                if (table.Exists == true && behavior == CopyTableSchemaExistsBehavior.Align)
                {
                    table.MissingColumns = table.Schema.Columns.Where(column => Probe(composer.ComposeColumnExists(table.Name, column.Field.Name)) == false).ToList();
                    table.MissingIndexes = table.Schema.Indexes.Where(index => index.Name != null && Probe(composer.ComposeIndexExists(table.Name, index.Name)) == false).ToList();
                    table.MissingForeignKeys = table.Schema.ForeignKeys.Where(foreignKey => foreignKey.Name != null && Probe(composer.ComposeForeignKeyExists(table.Name, foreignKey.Name)) == false && CanBeReferenced(foreignKey)).ToList();
                }
            }

            // The tables that are created are created without what the destination refuses: the foreign keys that reference a table that is not copied and does not exist,
            // and the check constraints that the composer does not accept
            foreach (var table in tables.Where(table => table.Exists != true || behavior == CopyTableSchemaExistsBehavior.Drop).ToList())
            {
                SkipUnsupported(table, table.Schema.ForeignKeys.Where(foreignKey => !CanBeReferenced(foreignKey)).ToList(), composer);
            }
            IndexNameResolver.Resolve(tables, behavior, composer, destinationConnection.GetDbSetting()?.AreIndexNamesUniquePerSchema == true, Probe);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static async Task ProbeTablesAsync(IEnumerable<PlanTable> tables,
            CopyTableSchemaExistsBehavior behavior,
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

            // A foreign key is added to a table that exists only if the table that it references is copied, or already exists (a table is not created to be referenced)
            async Task<bool> CanBeReferenced(ForeignKeyInfo foreignKey)
            {
                if (foreignKey.ReferencedTable == null)
                {
                    return true;
                }
                var name = composer.ComposeName(foreignKey.ReferencedTable);
                return tables.Any(item => string.Equals(item.Name, name, StringComparison.Ordinal)) || await Probe(composer.ComposeTableExists(name)).ConfigureAwait(false) == true;
            }

            foreach (var table in tables)
            {
                table.Exists = await Probe(composer.ComposeTableExists(table.Name)).ConfigureAwait(false);
                if (table.Exists == true && behavior == CopyTableSchemaExistsBehavior.Align)
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
                    table.MissingForeignKeys = new List<ForeignKeyInfo>();
                    foreach (var foreignKey in table.Schema.ForeignKeys.Where(foreignKey => foreignKey.Name != null))
                    {
                        if (await Probe(composer.ComposeForeignKeyExists(table.Name, foreignKey.Name)).ConfigureAwait(false) == false && await CanBeReferenced(foreignKey).ConfigureAwait(false))
                        {
                            table.MissingForeignKeys.Add(foreignKey);
                        }
                    }
                }
            }

            foreach (var table in tables.Where(table => table.Exists != true || behavior == CopyTableSchemaExistsBehavior.Drop).ToList())
            {
                var unreferenced = new List<ForeignKeyInfo>();
                foreach (var foreignKey in table.Schema.ForeignKeys)
                {
                    if (!await CanBeReferenced(foreignKey).ConfigureAwait(false))
                    {
                        unreferenced.Add(foreignKey);
                    }
                }
                SkipUnsupported(table, unreferenced, composer);
            }
            await IndexNameResolver.ResolveAsync(tables, behavior, composer, destinationConnection.GetDbSetting()?.AreIndexNamesUniquePerSchema == true, Probe).ConfigureAwait(false);
        }

        /// <summary>
        /// Leaves out of the schema of a table that is created the objects that the destination database refuses, and reports each one as a warning of the table:
        /// the foreign keys that reference a table that is not copied and does not exist in the destination, and the check constraints that the composer does not accept
        /// (see <see cref="ICheckConstraintSupport"/>). The default values that the composer does not create (see <see cref="IColumnDefaultSupport"/>) are reported too.
        /// </summary>
        /// <param name="table">The state of the table.</param>
        /// <param name="foreignKeys">The foreign keys of the table that reference a table that is not copied and does not exist in the destination.</param>
        /// <param name="composer">The composer of the destination database.</param>
        internal static void SkipUnsupported(PlanTable table,
            IList<ForeignKeyInfo> foreignKeys,
            ISchemaComposer composer)
        {
            // The default values that the destination can not create are not left out here (the composer does not write them): they are reported
            if (composer is IColumnDefaultSupport defaultSupport)
            {
                foreach (var column in table.Schema.Columns)
                {
                    var defaultReason = defaultSupport.GetUnsupportedDefaultReason(column);
                    if (defaultReason != null)
                    {
                        table.Warnings.Add($"Skipped the default value of the column '{column.Field?.Name}': {defaultReason}.");
                    }
                }
            }

            var support = composer as ICheckConstraintSupport;
            var checkConstraints = support == null
                ? new List<KeyValuePair<CheckConstraintInfo, string>>()
                : table.Schema.CheckConstraints
                    .Select(checkConstraint => new KeyValuePair<CheckConstraintInfo, string>(checkConstraint, support.GetUnsupportedReason(checkConstraint)))
                    .Where(pair => pair.Value != null)
                    .ToList();
            if (foreignKeys.Count == 0 && checkConstraints.Count == 0)
            {
                return;
            }

            var schema = table.Schema;
            table.Schema = new TableSchema(schema.Table)
            {
                Columns = schema.Columns,
                PrimaryKey = schema.PrimaryKey,
                Indexes = schema.Indexes,
                UniqueConstraints = schema.UniqueConstraints,
                CheckConstraints = schema.CheckConstraints.Where(checkConstraint => checkConstraints.All(pair => !ReferenceEquals(pair.Key, checkConstraint))).ToList(),
                ForeignKeys = schema.ForeignKeys.Where(foreignKey => !foreignKeys.Contains(foreignKey)).ToList()
            };
            foreach (var foreignKey in foreignKeys)
            {
                table.Warnings.Add($"Skipped the foreign key '{foreignKey.Name}': the table '{foreignKey.ReferencedTable.Name}' is not part of the copy and does not exist in the destination.");
            }
            foreach (var pair in checkConstraints)
            {
                table.Warnings.Add($"Skipped the check constraint '{pair.Key.Name}': {pair.Value}.");
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
        internal static ExecutionError CreateError(Exception exception,
            PlanStep step,
            int statementIndex) =>
            new ExecutionError
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
        internal static IEnumerable<string> GetScript(PlanTable table,
            ISchemaComposer schemaComposer)
        {
            switch (table.Outcome)
            {
                case CopyTableSchemaOutcome.Skipped:
                    return Enumerable.Empty<string>();
                case CopyTableSchemaOutcome.Aligned:
                    return table.AlignStatements;
                default:
                    var create = schemaComposer.ComposeSchema(table.Schema) ?? Enumerable.Empty<string>();
                    return table.DropStatement == null ? create : new[] { table.DropStatement }.Concat(create);
            }
        }

        /// <summary>
        /// Adds the item of each table that is completed to the items, and calls the callback with it.
        /// </summary>
        /// <param name="createdCallback"></param>
        /// <param name="items"></param>
        /// <param name="tables"></param>
        /// <param name="errors"></param>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="schemaComposer"></param>
        internal static void Report(Action<CopyTableSchemaResultItem> createdCallback,
            IList<CopyTableSchemaResultItem> items,
            IEnumerable<PlanTable> tables,
            IList<ExecutionError> errors,
            IDbConnection connection,
            IDbConnection destinationConnection,
            CopyTableSchemaExistsBehavior tableExistenceBehavior,
            ISchemaComposer schemaComposer)
        {
            foreach (var table in tables)
            {
                var schema = table.Schema;
                var aligned = table.Outcome == CopyTableSchemaOutcome.Aligned;
                var schemaErrors = errors
                    .Where(error => error.TableName == null ||
                        (string.Equals(error.TableName, schema.Table.Name, StringComparison.Ordinal) &&
                        string.Equals(error.SchemaName, schema.Table.Schema, StringComparison.Ordinal)))
                    .ToList();

                var item = new CopyTableSchemaResultItem
                {
                    Action = tableExistenceBehavior,
                    Outcome = schemaErrors.Count > 0 ? CopyTableSchemaOutcome.Failed : table.Outcome,
                    TableExisted = table.Exists,
                    AddedColumns = aligned ? table.MissingColumns.Select(column => column.Field.Name).ToList() : new List<string>(),
                    AddedIndexes = aligned ? table.MissingIndexes.Select(index => index.Name).ToList() : new List<string>(),
                    AddedForeignKeys = aligned ? table.MissingForeignKeys.Select(foreignKey => foreignKey.Name).ToList() : new List<string>(),
                    Errors = schemaErrors,
                    Warnings = table.Warnings.ToList(),
                    Source = new Endpoint
                    {
                        Table = table.Original,
                        Database = connection.Database,
                        Server = (connection as System.Data.Common.DbConnection)?.DataSource,
                        DatabaseType = connection.GetType().Name
                    },
                    Target = new Endpoint
                    {
                        Table = schema,
                        Database = destinationConnection.Database,
                        Server = (destinationConnection as System.Data.Common.DbConnection)?.DataSource,
                        DatabaseType = destinationConnection.GetType().Name
                    },
                    Script = string.Join(Environment.NewLine, GetScript(table, schemaComposer)),
                    ColumnCount = schema.Columns.Count,
                    IndexCount = schema.Indexes.Count,
                    ForeignKeyCount = schema.ForeignKeys.Count,
                    UniqueConstraintCount = schema.UniqueConstraints.Count,
                    CheckConstraintCount = schema.CheckConstraints.Count
                };
                items.Add(item);
                createdCallback?.Invoke(item);
            }
        }

        /// <summary>
        /// Creates the result of the schema copy operation from the items of the tables that were copied.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="items"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        internal static CopyTableSchemaResult CreateResult(DateTime startTime,
            IList<CopyTableSchemaResultItem> items,
            IList<ExecutionError> errors) =>
            new CopyTableSchemaResult
            {
                StartTime = startTime,
                EndTime = DateTime.UtcNow,
                Items = items,
                Errors = errors,
                Warnings = items.SelectMany(item => item.Warnings).ToList()
            };

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
