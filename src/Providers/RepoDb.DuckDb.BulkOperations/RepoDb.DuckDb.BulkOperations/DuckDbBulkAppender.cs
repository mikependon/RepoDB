#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using RepoDb.Extensions;

namespace RepoDb.DuckDb.BulkOperations
{
    /// <summary>
    /// Bulk writes rows into a DuckDB table using the native <see cref="DuckDBAppender"/>.
    /// </summary>
    public class DuckDbBulkAppender : IDisposable
    {
        private readonly DuckDBConnection connection;

        /// <summary>
        /// Creates a new instance bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection to write against.</param>
        public DuckDbBulkAppender(DuckDBConnection connection)
        {
            this.connection = connection;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the target table name.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Gets or sets the command timeout, in seconds. Zero uses the provider default.
        /// </summary>
        public int BulkCopyTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of rows appended before flushing. Zero flushes once at the end.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the transaction to write under.
        /// </summary>
        public DuckDBTransaction Transaction { get; set; }

        /// <summary>
        /// Gets the source-to-destination column mappings. Empty maps the source columns that match the destination columns by name.
        /// </summary>
        public DuckDbBulkAppenderColumnMappingCollection ColumnMappings { get; } = new();

        #endregion

        #region Methods

        /// <summary>
        /// Writes every row of <paramref name="reader"/> into <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="reader">The source rows.</param>
        /// <returns>The number of rows written.</returns>
        public int WriteToServer(IDataReader reader) =>
            Write(GetNames(reader), GetRows(reader), default);

        /// <summary>
        /// Writes the rows of <paramref name="table"/> into <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="table">The source rows.</param>
        /// <param name="rowState">When specified, only rows in this state are written.</param>
        /// <returns>The number of rows written.</returns>
        public int WriteToServer(DataTable table,
            DataRowState? rowState = null) =>
            Write(GetNames(table), GetRows(table, rowState), default);

        /// <summary>
        /// Writes every row of <paramref name="reader"/> into <see cref="DestinationTableName"/> in an asynchronous way.
        /// </summary>
        /// <param name="reader">The source rows.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows written.</returns>
        public Task<int> WriteToServerAsync(IDataReader reader,
            CancellationToken cancellationToken = default) =>
            AsTask(() => Write(GetNames(reader), GetRows(reader), cancellationToken));

        /// <summary>
        /// Writes the rows of <paramref name="table"/> into <see cref="DestinationTableName"/> in an asynchronous way.
        /// </summary>
        /// <param name="table">The source rows.</param>
        /// <param name="rowState">When specified, only rows in this state are written.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows written.</returns>
        public Task<int> WriteToServerAsync(DataTable table,
            DataRowState? rowState = null,
            CancellationToken cancellationToken = default) =>
            AsTask(() => Write(GetNames(table), GetRows(table, rowState), cancellationToken));

        /// <summary>
        /// Disposes the resources used by this instance.
        /// </summary>
        public void Dispose() =>
            GC.SuppressFinalize(this);

        #endregion

        #region Helpers

        private static Task<int> AsTask(Func<int> func)
        {
            try
            {
                return Task.FromResult(func());
            }
            catch (OperationCanceledException ex)
            {
                return Task.FromCanceled<int>(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                return Task.FromException<int>(ex);
            }
        }

        private static string[] GetNames(IDataReader reader) =>
            Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();

        private static string[] GetNames(DataTable table) =>
            table.Columns.OfType<DataColumn>().Select(column => column.ColumnName).ToArray();

        private static IEnumerable<object[]> GetRows(IDataReader reader)
        {
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                yield return values;
            }
        }

        private static IEnumerable<object[]> GetRows(DataTable table,
            DataRowState? rowState) =>
            table.Rows.OfType<DataRow>()
                .Where(row => rowState == null || row.RowState == rowState)
                .Select(row => row.ItemArray);

        private int Write(string[] names,
            IEnumerable<object[]> rows,
            CancellationToken cancellationToken)
        {
            var dbSetting = connection.GetDbSetting();
            var columns = GetColumns(DestinationTableName);
            var columnNames = columns.Select(column => column.Name).ToArray();
            var mappings = ColumnMappings.Count > 0 ?
                ColumnMappings.ToArray() :
                names.Where(name => IndexOf(columnNames, name) >= 0).Select(name => new DuckDbBulkInsertMapItem(name, name)).ToArray();
            if (mappings.Length == 0)
            {
                return 0;
            }

            // Resolve the source ordinals and the destination columns of the mappings
            var sources = mappings
                .Select(mapping => IndexOf(names, mapping.SourceColumn) is var index and >= 0 ? index :
                    throw new InvalidOperationException($"The source column '{mapping.SourceColumn}' is not found."))
                .ToArray();
            var targets = mappings
                .Select(mapping => IndexOf(columnNames, mapping.DestinationColumn.AsUnquoted(true, dbSetting)) is var index and >= 0 ? index :
                    throw new InvalidOperationException($"The destination column '{mapping.DestinationColumn}' is not found."))
                .ToArray();

            // Append directly when all columns are mapped, otherwise stage the mapped columns first
            var isDirect = targets.Distinct().Count() == columns.Length;
            var tableName = DestinationTableName;
            var order = Enumerable.Range(0, mappings.Length).ToArray();
            if (isDirect)
            {
                order = Enumerable.Range(0, columns.Length).Select(column => Array.IndexOf(targets, column)).ToArray();
            }
            else
            {
                tableName = $"__RepoDb_Appender_{Guid.NewGuid():N}";
                Execute($"CREATE TEMP TABLE {tableName.AsQuoted(true, dbSetting)} AS SELECT {GetColumnList(targets, columns, dbSetting)} FROM {DestinationTableName} LIMIT 0;");
            }

            try
            {
                var result = Append(tableName, rows, sources, order, targets, columns, cancellationToken);
                if (!isDirect)
                {
                    var columnList = GetColumnList(targets, columns, dbSetting);
                    Execute($"INSERT INTO {DestinationTableName} ({columnList}) SELECT {columnList} FROM {tableName.AsQuoted(true, dbSetting)} ORDER BY rowid;");
                }
                return result;
            }
            finally
            {
                if (!isDirect)
                {
                    Execute($"DROP TABLE IF EXISTS {tableName.AsQuoted(true, dbSetting)};");
                }
            }
        }

        private int Append(string tableName,
            IEnumerable<object[]> rows,
            int[] sources,
            int[] order,
            int[] targets,
            (string Name, Type Type)[] columns,
            CancellationToken cancellationToken)
        {
            var unquoted = tableName.AsUnquoted(true, connection.GetDbSetting());
            var dot = unquoted.IndexOf('.');
            var schema = dot < 0 ? null : unquoted[..dot];
            var table = dot < 0 ? unquoted : unquoted[(dot + 1)..];
            var appender = connection.CreateAppender(schema, table);
            var result = 0;

            try
            {
                foreach (var values in rows)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var row = appender.CreateRow();
                    foreach (var mapping in order)
                    {
                        AppendValue(row, Convert(values[sources[mapping]], columns[targets[mapping]].Type));
                    }
                    row.EndRow();
                    result++;

                    // Flush the batch
                    if (BatchSize > 0 && result % BatchSize == 0)
                    {
                        appender.Dispose();
                        appender = connection.CreateAppender(schema, table);
                    }
                }
            }
            finally
            {
                appender.Dispose();
            }

            return result;
        }

        private (string Name, Type Type)[] GetColumns(string tableName)
        {
            using var command = CreateCommand($"SELECT * FROM {tableName} LIMIT 0;");
            using var reader = command.ExecuteReader();
            return Enumerable.Range(0, reader.FieldCount)
                .Select(index => (reader.GetName(index), reader.GetFieldType(index)))
                .ToArray();
        }

        private void Execute(string commandText)
        {
            using var command = CreateCommand(commandText);
            command.ExecuteNonQuery();
        }

        private DuckDBCommand CreateCommand(string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = Transaction;
            if (BulkCopyTimeout > 0)
            {
                command.CommandTimeout = BulkCopyTimeout;
            }
            return command;
        }

        private static string GetColumnList(int[] targets,
            (string Name, Type Type)[] columns,
            RepoDb.Interfaces.IDbSetting dbSetting) =>
            targets.Select(index => columns[index].Name.AsQuoted(true, dbSetting)).Join(", ");

        private static int IndexOf(string[] names,
            string name) =>
            Array.FindIndex(names, n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));

        private static object Convert(object value,
            Type type)
        {
            if (value is null or DBNull)
            {
                return null;
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            return value switch
            {
                _ when value.GetType() == type => value,
                Enum => type == typeof(string) ? value.ToString() : System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture),
                DateTime dateTime when type == typeof(DateOnly) => DateOnly.FromDateTime(dateTime),
                TimeSpan timeSpan when type == typeof(TimeOnly) => TimeOnly.FromTimeSpan(timeSpan),
                string text when type == typeof(Guid) => Guid.Parse(text),
                Guid guid when type == typeof(string) => guid.ToString(),
                IConvertible when typeof(IConvertible).IsAssignableFrom(type) && type != typeof(object) => System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture),
                _ => value
            };
        }

        private static void AppendValue(IDuckDBAppenderRow row,
            object value)
        {
            _ = value switch
            {
                null => row.AppendNullValue(),
                bool v => row.AppendValue(v),
                byte[] v => row.AppendValue(v),
                string v => row.AppendValue(v),
                decimal v => row.AppendValue(v),
                Guid v => row.AppendValue(v),
                BigInteger v => row.AppendValue(v),
                sbyte v => row.AppendValue(v),
                short v => row.AppendValue(v),
                int v => row.AppendValue(v),
                long v => row.AppendValue(v),
                byte v => row.AppendValue(v),
                ushort v => row.AppendValue(v),
                uint v => row.AppendValue(v),
                ulong v => row.AppendValue(v),
                float v => row.AppendValue(v),
                double v => row.AppendValue(v),
                DateOnly v => row.AppendValue(v),
                TimeOnly v => row.AppendValue(v),
                DateTime v => row.AppendValue(v),
                DateTimeOffset v => row.AppendValue(v),
                TimeSpan v => row.AppendValue(v),
                char v => row.AppendValue(v.ToString()),
                _ => throw new NotSupportedException($"The type '{value.GetType()}' is not supported by the DuckDB appender.")
            };
        }

        #endregion
    }
}
