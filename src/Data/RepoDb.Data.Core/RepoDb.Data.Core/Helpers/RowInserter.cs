#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Inserts (or merges) the rows into a table of a provider that has no bulk insert (or no bulk merge). The <c>InsertAll</c> and the <c>MergeAll</c> of the library do not insert the
    /// identity (and the auto-increment) columns, so the values of the source would be generated again by the destination database (and the rows of the related tables would reference
    /// other rows): the statements are composed here to write every column that the target table has. A row is merged with an <c>UPDATE</c> by the qualifiers, and an <c>INSERT</c>
    /// if no row was updated, so it works with any provider.
    /// </summary>
    internal static class RowInserter
    {
        #region Public Methods

        /// <summary>
        /// Inserts the rows (the dictionaries of the column names and the values) into the table.
        /// </summary>
        /// <param name="connection">The destination connection.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="rows">The rows to be inserted: the dictionaries of the column names and the values.</param>
        /// <param name="commandTimeout">The command timeout, in seconds.</param>
        /// <param name="traceKey">The trace key of the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction of the destination connection. The rows are inserted in a transaction of their own if it is <c>null</c>.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Insert(IDbConnection connection,
            string tableName,
            IList<object> rows,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            if (rows.Count == 0)
            {
                return 0;
            }
            var (commandText, columns) = Compose(connection, tableName, rows, transaction);
            var ownTransaction = transaction == null ? connection.EnsureOpen().BeginTransaction() : null;
            try
            {
                var inserted = 0;
                using (var command = CreateCommand(connection, commandText, columns, commandTimeout, transaction ?? ownTransaction))
                {
                    foreach (IDictionary<string, object> row in rows)
                    {
                        SetValues(command, columns, row);
                        inserted += command.ExecuteNonQuery();
                    }
                }
                ownTransaction?.Commit();
                return inserted;
            }
            catch
            {
                ownTransaction?.Rollback();
                throw;
            }
            finally
            {
                ownTransaction?.Dispose();
            }
        }

        /// <summary>
        /// Inserts the rows (the dictionaries of the column names and the values) into the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The destination connection.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="rows">The rows to be inserted: the dictionaries of the column names and the values.</param>
        /// <param name="commandTimeout">The command timeout, in seconds.</param>
        /// <param name="traceKey">The trace key of the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction of the destination connection. The rows are inserted in a transaction of their own if it is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the number of inserted rows.</returns>
        public static async Task<int> InsertAsync(IDbConnection connection,
            string tableName,
            IList<object> rows,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            if (rows.Count == 0)
            {
                return 0;
            }
            var (commandText, columns) = Compose(connection, tableName, rows, transaction);
            var ownTransaction = transaction == null ? (await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false)).BeginTransaction() : null;
            try
            {
                var inserted = 0;
                using (var command = CreateCommand(connection, commandText, columns, commandTimeout, transaction ?? ownTransaction))
                {
                    foreach (IDictionary<string, object> row in rows)
                    {
                        SetValues(command, columns, row);
                        inserted += command is System.Data.Common.DbCommand dbCommand
                            ? await dbCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false)
                            : command.ExecuteNonQuery();
                    }
                }
                ownTransaction?.Commit();
                return inserted;
            }
            catch
            {
                ownTransaction?.Rollback();
                throw;
            }
            finally
            {
                ownTransaction?.Dispose();
            }
        }

        /// <summary>
        /// Merges the rows (the dictionaries of the column names and the values) into the table: a row updates the row that has its qualifiers, or it is inserted if there is none.
        /// </summary>
        /// <param name="connection">The destination connection.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="rows">The rows to be merged: the dictionaries of the column names and the values.</param>
        /// <param name="qualifiers">The fields that identify the existing rows.</param>
        /// <param name="commandTimeout">The command timeout, in seconds.</param>
        /// <param name="traceKey">The trace key of the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction of the destination connection. The rows are merged in a transaction of their own if it is <c>null</c>.</param>
        /// <returns>The number of merged rows.</returns>
        /// <exception cref="InvalidOperationException">A qualifier is not a column of the rows.</exception>
        public static int Merge(IDbConnection connection,
            string tableName,
            IList<object> rows,
            IList<Field> qualifiers,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            if (rows.Count == 0)
            {
                return 0;
            }
            var plan = ComposeMerge(connection, tableName, rows, qualifiers, transaction);
            var ownTransaction = transaction == null ? connection.EnsureOpen().BeginTransaction() : null;
            try
            {
                var merged = 0;
                using (var update = CreateCommand(connection, plan.UpdateText, plan.UpdateColumns, commandTimeout, transaction ?? ownTransaction))
                using (var insert = CreateCommand(connection, plan.InsertText, plan.InsertColumns, commandTimeout, transaction ?? ownTransaction))
                {
                    foreach (IDictionary<string, object> row in rows)
                    {
                        SetValues(update, plan.UpdateColumns, row);
                        if (update.ExecuteNonQuery() <= 0)
                        {
                            SetValues(insert, plan.InsertColumns, row);
                            insert.ExecuteNonQuery();
                        }
                        merged++;
                    }
                }
                ownTransaction?.Commit();
                return merged;
            }
            catch
            {
                ownTransaction?.Rollback();
                throw;
            }
            finally
            {
                ownTransaction?.Dispose();
            }
        }

        /// <summary>
        /// Merges the rows (the dictionaries of the column names and the values) into the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The destination connection.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="rows">The rows to be merged: the dictionaries of the column names and the values.</param>
        /// <param name="qualifiers">The fields that identify the existing rows.</param>
        /// <param name="commandTimeout">The command timeout, in seconds.</param>
        /// <param name="traceKey">The trace key of the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction of the destination connection. The rows are merged in a transaction of their own if it is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the number of merged rows.</returns>
        /// <exception cref="InvalidOperationException">A qualifier is not a column of the rows.</exception>
        public static async Task<int> MergeAsync(IDbConnection connection,
            string tableName,
            IList<object> rows,
            IList<Field> qualifiers,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            if (rows.Count == 0)
            {
                return 0;
            }
            var plan = ComposeMerge(connection, tableName, rows, qualifiers, transaction);
            var ownTransaction = transaction == null ? (await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false)).BeginTransaction() : null;
            try
            {
                var merged = 0;
                using (var update = CreateCommand(connection, plan.UpdateText, plan.UpdateColumns, commandTimeout, transaction ?? ownTransaction))
                using (var insert = CreateCommand(connection, plan.InsertText, plan.InsertColumns, commandTimeout, transaction ?? ownTransaction))
                {
                    foreach (IDictionary<string, object> row in rows)
                    {
                        SetValues(update, plan.UpdateColumns, row);
                        var updated = update is System.Data.Common.DbCommand dbUpdate
                            ? await dbUpdate.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false)
                            : update.ExecuteNonQuery();
                        if (updated <= 0)
                        {
                            SetValues(insert, plan.InsertColumns, row);
                            if (insert is System.Data.Common.DbCommand dbInsert)
                            {
                                await dbInsert.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                            }
                            else
                            {
                                insert.ExecuteNonQuery();
                            }
                        }
                        merged++;
                    }
                }
                ownTransaction?.Commit();
                return merged;
            }
            catch
            {
                ownTransaction?.Rollback();
                throw;
            }
            finally
            {
                ownTransaction?.Dispose();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Composes the <c>INSERT</c> statement for the columns that both the first row and the target table have.
        /// </summary>
        /// <param name="connection">The destination connection.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="rows">The rows to be inserted (the first row defines the columns).</param>
        /// <param name="transaction">The transaction of the destination connection.</param>
        /// <returns>The statement, and the columns of the statement with the name of their parameter.</returns>
        /// <exception cref="InvalidOperationException">No column of the rows matches a column of the target table.</exception>
        private static (string CommandText, List<(string Column, string Parameter)> Columns) Compose(IDbConnection connection,
            string tableName,
            IList<object> rows,
            IDbTransaction transaction)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var first = (IDictionary<string, object>)rows[0];

            // The columns that both the row and the table have (the others are ignored)
            var columns = first.Keys
                .Where(key => dbFields.GetByUnquotedName(key.AsUnquoted(true, dbSetting)) != null)
                .Select((key, index) => (Column: key, Parameter: dbSetting.ParameterPrefix + "p" + index.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                .ToList();
            if (columns.Count == 0)
            {
                throw new InvalidOperationException($"No column of the source matches a column of the table '{tableName}'.");
            }

            var commandText = $"INSERT INTO {tableName.AsQuoted(true, dbSetting)} ({string.Join(", ", columns.Select(c => c.Column.AsQuoted(true, dbSetting)))}) " +
                $"VALUES ({string.Join(", ", columns.Select(c => c.Parameter))})";
            return (commandText, columns);
        }

        /// <summary>
        /// Composes the statements that merge a row: the <c>INSERT</c> of every column, and the <c>UPDATE</c> of the columns that are not qualifiers by the qualifiers
        /// (if every column is a qualifier, the <c>UPDATE</c> sets a qualifier to itself, so it still tells whether the row exists).
        /// </summary>
        /// <param name="connection">The destination connection.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="rows">The rows to be merged (the first row defines the columns).</param>
        /// <param name="qualifiers">The fields that identify the existing rows.</param>
        /// <param name="transaction">The transaction of the destination connection.</param>
        /// <returns>The statements, and the columns of each one with the name of their parameter.</returns>
        /// <exception cref="InvalidOperationException">A qualifier is not a column of the rows.</exception>
        private static (string InsertText, List<(string Column, string Parameter)> InsertColumns, string UpdateText, List<(string Column, string Parameter)> UpdateColumns) ComposeMerge(IDbConnection connection,
            string tableName,
            IList<object> rows,
            IList<Field> qualifiers,
            IDbTransaction transaction)
        {
            var dbSetting = connection.GetDbSetting();
            var (insertText, insertColumns) = Compose(connection, tableName, rows, transaction);
            var keys = new List<string>();
            foreach (var qualifier in qualifiers)
            {
                var column = insertColumns.FirstOrDefault(c => string.Equals(c.Column.AsUnquoted(true, dbSetting), qualifier.Name, StringComparison.OrdinalIgnoreCase));
                if (column.Column == null)
                {
                    throw new InvalidOperationException($"The qualifier '{qualifier.Name}' is not a column of the rows to be merged into the table '{tableName}'.");
                }
                keys.Add(column.Column);
            }
            var sets = insertColumns.Where(c => !keys.Contains(c.Column)).Select(c => c.Column).ToList();
            if (sets.Count == 0)
            {
                sets = keys.ToList();
            }

            var updateColumns = new List<(string Column, string Parameter)>();
            var assignments = new List<string>();
            foreach (var column in sets)
            {
                var parameter = dbSetting.ParameterPrefix + "u" + updateColumns.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
                updateColumns.Add((column, parameter));
                assignments.Add($"{column.AsQuoted(true, dbSetting)} = {parameter}");
            }
            var conditions = new List<string>();
            foreach (var column in keys)
            {
                var parameter = dbSetting.ParameterPrefix + "u" + updateColumns.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
                updateColumns.Add((column, parameter));
                conditions.Add($"{column.AsQuoted(true, dbSetting)} = {parameter}");
            }
            var updateText = $"UPDATE {tableName.AsQuoted(true, dbSetting)} SET {string.Join(", ", assignments)} WHERE {string.Join(" AND ", conditions)}";
            return (insertText, insertColumns, updateText, updateColumns);
        }

        /// <summary>
        /// Creates the command (with the parameters) that writes a row (the insert, or the update of the merge).
        /// </summary>
        /// <param name="connection">The destination connection.</param>
        /// <param name="commandText">The <c>INSERT</c> statement.</param>
        /// <param name="columns">The columns of the statement with the name of their parameter.</param>
        /// <param name="commandTimeout">The command timeout, in seconds.</param>
        /// <param name="transaction">The transaction of the destination connection.</param>
        /// <returns>The command.</returns>
        private static IDbCommand CreateCommand(IDbConnection connection,
            string commandText,
            List<(string Column, string Parameter)> columns,
            int? commandTimeout,
            IDbTransaction transaction)
        {
            // The command is created here (instead of being executed by the library) as the library expands the parameters that are arrays (i.e.: the bytes of a binary column)
            // The name of a parameter is set without the prefix "$" (the one of DuckDB), as the driver binds the values by the name that follows it (the statement keeps the prefix)
            var command = connection.EnsureOpen().CreateCommand();
            command.CommandText = commandText;
            command.Transaction = transaction;
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }
            foreach (var (_, parameterName) in columns)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName.StartsWith("$", StringComparison.Ordinal) ? parameterName.Substring(1) : parameterName;
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// Sets the values of a row to the parameters of the command (a missing value is set as <see cref="DBNull"/>).
        /// </summary>
        /// <param name="command">The command that inserts a row.</param>
        /// <param name="columns">The columns of the statement with the name of their parameter.</param>
        /// <param name="row">The row: the dictionary of the column names and the values.</param>
        private static void SetValues(IDbCommand command,
            List<(string Column, string Parameter)> columns,
            IDictionary<string, object> row)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                ((IDbDataParameter)command.Parameters[i]).Value = row.TryGetValue(columns[i].Column, out var value) && value != null ? value : DBNull.Value;
            }
        }

        #endregion
    }
}
