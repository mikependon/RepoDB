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
    /// Inserts the rows into a table of a provider that has no bulk insert. The <c>InsertAll</c> of the library does not insert the identity (and the auto-increment) columns, so the
    /// values of the source would be generated again by the destination database (and the rows of the related tables would reference other rows): the statement is composed here to
    /// insert every column that the target table has.
    /// </summary>
    internal static class RowInserter
    {
        /// <summary>
        /// Inserts the rows (the dictionaries of the column names and the values) into the table.
        /// </summary>
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
        /// <returns>The number of inserted rows.</returns>
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

        private static IDbCommand CreateCommand(IDbConnection connection,
            string commandText,
            List<(string Column, string Parameter)> columns,
            int? commandTimeout,
            IDbTransaction transaction)
        {
            // The command is created here (instead of being executed by the library) as the library expands the parameters that are arrays (i.e.: the bytes of a binary column)
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
                parameter.ParameterName = parameterName;
                command.Parameters.Add(parameter);
            }
            return command;
        }

        private static void SetValues(IDbCommand command,
            List<(string Column, string Parameter)> columns,
            IDictionary<string, object> row)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                ((IDbDataParameter)command.Parameters[i]).Value = row.TryGetValue(columns[i].Column, out var value) && value != null ? value : DBNull.Value;
            }
        }
    }
}
