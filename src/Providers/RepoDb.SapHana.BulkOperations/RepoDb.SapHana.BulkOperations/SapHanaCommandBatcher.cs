#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sap.Data.Hana;
using RepoDb.Extensions;

namespace RepoDb.SapHana.BulkOperations
{
    /// <summary>
    /// A <see cref="HanaCommand"/>-based row-by-row batch-insert class, following the same
    /// conventions as <see cref="HanaBulkCopy"/>. This class is present to support the same API
    /// as <see cref="HanaBulkCopy"/> but with a genuine async implementation.
    /// </summary>
    public class SapHanaCommandBatcher : IDisposable
    {
        private readonly HanaConnection connection;

        /// <summary>
        /// Creates a new instance bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection to batch inserts against.</param>
        public SapHanaCommandBatcher(HanaConnection connection)
        {
            this.connection = connection;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the target table name, already quoted as <see cref="HanaBulkCopy.DestinationTableName"/>
        /// would expect.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Gets or sets the target table name, unquoted, as required for the <see cref="DbFieldCache"/>
        /// lookup <see cref="AddParameters"/> uses to pre-declare each parameter's <see cref="HanaDbType"/>
        /// from its destination column's actual type - see the remarks there for why.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the command timeout, in seconds. Zero uses the provider default.
        /// </summary>
        public int BulkCopyTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of rows executed against a single prepared <see cref="HanaCommand"/>
        /// before it is disposed and a fresh one is prepared.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the transaction each row is executed under.
        /// </summary>
        public HanaTransaction Transaction { get; set; }

        /// <summary>
        /// Gets the source-to-destination column mappings. Empty maps every source column to itself.
        /// </summary>
        public SapHanaCommandBatcherColumnMappingCollection ColumnMappings { get; } = new();

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private IReadOnlyList<SapHanaBulkInsertMapItem> ResolveMappings(IDataReader reader)
        {
            if (ColumnMappings.Count == 0)
            {
                var identityMappings = new SapHanaBulkInsertMapItem[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    identityMappings[i] = new SapHanaBulkInsertMapItem(name, name);
                }
                return identityMappings;
            }

            var mappings = new SapHanaBulkInsertMapItem[ColumnMappings.Count];
            for (var i = 0; i < ColumnMappings.Count; i++)
            {
                var mapping = ColumnMappings[i];
                var ordinal = reader.GetOrdinal(mapping.SourceColumn);
                mappings[i] = new SapHanaBulkInsertMapItem(reader.GetName(ordinal), mapping.DestinationColumn, mapping.HanaDbType);
            }
            return mappings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private IReadOnlyList<SapHanaBulkInsertMapItem> ResolveMappings(DataRow[] rows)
        {
            if (ColumnMappings.Count > 0)
            {
                return ColumnMappings.ToArray();
            }

            if (rows == null || rows.Length == 0)
            {
                return Array.Empty<SapHanaBulkInsertMapItem>();
            }

            return rows[0].Table.Columns
                .OfType<DataColumn>()
                .Select(column => new SapHanaBulkInsertMapItem(column.ColumnName, column.ColumnName))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private string BuildInsertStatement(IReadOnlyList<SapHanaBulkInsertMapItem> mappings)
        {
            var dbSetting = connection.GetDbSetting();
            var columnList = string.Join(", ", mappings.Select(m => m.DestinationColumn.AsQuoted(true, dbSetting)));
            var parameterList = string.Join(", ", mappings.Select((_, i) => dbSetting.SqlTextParameterPrefix + ParameterName(i)));
            return $"INSERT INTO {DestinationTableName} ({columnList}) VALUES ({parameterList})";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string ParameterName(int index) => "p" + index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private HanaCommand CreateCommand(string commandText) =>
            (HanaCommand)connection.CreateCommand(commandText, CommandType.Text, BulkCopyTimeout > 0 ? BulkCopyTimeout : null, Transaction);

        /// <summary>
        /// Creates and binds one <see cref="HanaParameter"/> per mapping onto <paramref name="command"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private HanaParameter[] AddParameters(HanaCommand command,
            IReadOnlyList<SapHanaBulkInsertMapItem> mappings)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = TableName != null ? DbFieldCache.Get(connection, TableName, Transaction) : null;
            var parameters = new HanaParameter[mappings.Count];

            for (var i = 0; i < mappings.Count; i++)
            {
                var mapping = mappings[i];
                HanaParameter parameter;

                if (mapping.HanaDbType.HasValue)
                {
                    parameter = new HanaParameter(ParameterName(i), mapping.HanaDbType.Value);
                }
                else
                {
                    var destinationField = dbFields?.GetByUnquotedName(mapping.DestinationColumn.AsUnquoted(true, dbSetting));
                    var destinationType = destinationField?.Type != null
                        ? Nullable.GetUnderlyingType(destinationField.Type) ?? destinationField.Type
                        : null;
                    parameter = destinationType == typeof(decimal)
                        ? new HanaParameter(ParameterName(i), HanaDbType.Decimal)
                        : new HanaParameter(ParameterName(i), DBNull.Value);
                }

                command.Parameters.Add(parameter);
                parameters[i] = parameter;
            }

            return parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object NormalizeParameterValue(object value) =>
            value is decimal decimalValue ? new HanaDecimal(decimalValue) : value ?? DBNull.Value;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IDisposable EnterInvariantCulture() => new InvariantCultureScope();

        /// <summary>
        /// 
        /// </summary>
        private sealed class InvariantCultureScope : IDisposable
        {
            private readonly CultureInfo originalCulture = CultureInfo.CurrentCulture;
            private readonly CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;

            public InvariantCultureScope()
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            }

            public void Dispose()
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }

        #endregion

        #region Sync

        /// <summary>
        /// Executes every row of <paramref name="reader"/> as its own <c>INSERT</c> round trip against
        /// <see cref="DestinationTableName"/>, streaming rows as they are read rather than materializing
        /// them into memory first.
        /// </summary>
        /// <param name="reader">The source rows to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int WriteToServer(IDataReader reader)
        {
            if (reader.FieldCount == 0)
            {
                return 0;
            }

            using var cultureScope = EnterInvariantCulture();
            var mappings = ResolveMappings(reader);
            var sourceOrdinals = mappings.Select(m => reader.GetOrdinal(m.SourceColumn)).ToArray();
            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : int.MaxValue;
            var affectedRows = 0;
            var command = CreateCommand(commandText);
            var parameters = AddParameters(command, mappings);
            var pendingRows = 0;
            
            command.Prepare();

            try
            {
                while (reader.Read())
                {
                    for (var i = 0; i < sourceOrdinals.Length; i++)
                    {
                        parameters[i].Value = NormalizeParameterValue(reader.GetValue(sourceOrdinals[i]));
                    }
                    affectedRows += command.ExecuteNonQuery();
                    pendingRows++;

                    if (pendingRows >= effectiveBatchSize)
                    {
                        command.Dispose();
                        command = CreateCommand(commandText);
                        parameters = AddParameters(command, mappings);
                        command.Prepare();
                        pendingRows = 0;
                    }
                }
            }
            finally
            {
                command.Dispose();
            }

            return affectedRows;
        }

        /// <summary>
        /// Executes every row of <paramref name="rows"/> as its own <c>INSERT</c> round trip against
        /// <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="rows">The source rows to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int WriteToServer(DataRow[] rows)
        {
            var mappings = ResolveMappings(rows);

            if (rows == null || rows.Length == 0 || mappings.Count == 0)
            {
                return 0;
            }

            using var cultureScope = EnterInvariantCulture();
            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : rows.Length;
            var affectedRows = 0;

            for (var offset = 0; offset < rows.Length; offset += effectiveBatchSize)
            {
                var count = Math.Min(effectiveBatchSize, rows.Length - offset);
                using var command = CreateCommand(commandText);
                var parameters = AddParameters(command, mappings);
                command.Prepare();

                for (var rowIndex = 0; rowIndex < count; rowIndex++)
                {
                    var row = rows[offset + rowIndex];
                    for (var i = 0; i < mappings.Count; i++)
                    {
                        parameters[i].Value = NormalizeParameterValue(row[mappings[i].SourceColumn]);
                    }
                    affectedRows += command.ExecuteNonQuery();
                }
            }

            return affectedRows;
        }

        #endregion

        #region Async

        /// <summary>
        /// Executes every row of <paramref name="reader"/> as its own <c>INSERT</c> round trip against
        /// <see cref="DestinationTableName"/>, streaming rows as they are read rather than materializing
        /// them into memory first.
        /// </summary>
        /// <param name="reader">The source rows to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> WriteToServerAsync(IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            if (reader.FieldCount == 0)
            {
                return 0;
            }

            using var cultureScope = EnterInvariantCulture();
            var mappings = ResolveMappings(reader);
            var sourceOrdinals = mappings.Select(m => reader.GetOrdinal(m.SourceColumn)).ToArray();
            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : int.MaxValue;
            var affectedRows = 0;
            var dbReader = reader as DbDataReader;
            var command = CreateCommand(commandText);
            var parameters = AddParameters(command, mappings);
            var pendingRows = 0;
            
            await command.PrepareAsync(cancellationToken);

            try
            {
                while (dbReader != null ? await dbReader.ReadAsync(cancellationToken) : reader.Read())
                {
                    for (var i = 0; i < sourceOrdinals.Length; i++)
                    {
                        parameters[i].Value = NormalizeParameterValue(reader.GetValue(sourceOrdinals[i]));
                    }
                    affectedRows += await command.ExecuteNonQueryAsync(cancellationToken);
                    pendingRows++;

                    if (pendingRows >= effectiveBatchSize)
                    {
                        await command.DisposeAsync();
                        command = CreateCommand(commandText);
                        parameters = AddParameters(command, mappings);
                        await command.PrepareAsync(cancellationToken);
                        pendingRows = 0;
                    }
                }
            }
            finally
            {
                await command.DisposeAsync();
            }

            return affectedRows;
        }

        /// <summary>
        /// Executes every row of <paramref name="rows"/> as its own <c>INSERT</c> round trip against
        /// <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="rows">The source rows to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> WriteToServerAsync(DataRow[] rows,
            CancellationToken cancellationToken = default)
        {
            var mappings = ResolveMappings(rows);

            if (rows == null || rows.Length == 0 || mappings.Count == 0)
            {
                return 0;
            }

            using var cultureScope = EnterInvariantCulture();
            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : rows.Length;
            var affectedRows = 0;

            for (var offset = 0; offset < rows.Length; offset += effectiveBatchSize)
            {
                var count = Math.Min(effectiveBatchSize, rows.Length - offset);
                await using var command = CreateCommand(commandText);
                var parameters = AddParameters(command, mappings);
                await command.PrepareAsync(cancellationToken);

                for (var rowIndex = 0; rowIndex < count; rowIndex++)
                {
                    var row = rows[offset + rowIndex];
                    for (var i = 0; i < mappings.Count; i++)
                    {
                        parameters[i].Value = NormalizeParameterValue(row[mappings[i].SourceColumn]);
                    }
                    affectedRows += await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            return affectedRows;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the underlying resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            // Does nothing for now; this instance owns no unmanaged resources of its own - each
            // HanaCommand it creates is scoped to (and disposed within) a single WriteToServer call.
        }

        #endregion
    }
}
