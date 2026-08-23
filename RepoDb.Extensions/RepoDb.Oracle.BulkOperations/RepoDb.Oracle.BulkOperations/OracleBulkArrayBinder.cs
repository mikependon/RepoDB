using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Resolvers;


namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// An async, array-bind based alternative to <see cref="OracleBulkCopy"/> - there is no true async
    /// equivalent of <see cref="OracleBulkCopy.WriteToServer(IDataReader)"/>, so this issues batched
    /// <c>INSERT ... VALUES (:p0, :p1, ...)</c> statements with <see cref="OracleCommand.ArrayBindCount"/>
    /// set, which ODP.NET can execute via <see cref="OracleCommand.ExecuteNonQueryAsync(CancellationToken)"/>.
    /// </summary>
    internal class OracleBulkArrayBinder : IDisposable
    {
        private const int MaxBindableParametersCount = 65_535;
        private static readonly TypeToOracleDbTypeResolver dbTypeResolver = new();
        private readonly OracleConnection connection;

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        public OracleBulkArrayBinder(OracleConnection connection)
        {
            this.connection = connection;
        }

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int BulkCopyTimeout { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        ///
        /// </summary>
        public OracleTransaction Transaction { get; set; }

        /// <summary>
        ///
        /// </summary>
        public OracleBulkArrayBinderColumnMappingCollection ColumnMappings { get; } = new();

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        private int GetEffectiveBatchSize(
            int columnCount)
        {
            return BatchSize > 0 ? BatchSize : Math.Min(1000, MaxBindableParametersCount / columnCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<DataTable> ToDataTableAsync(
            IDataReader reader,
            CancellationToken cancellationToken)
        {
            var dataTable = new DataTable();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i) ?? typeof(object));
            }

            var values = new object[reader.FieldCount];

            if (reader is DbDataReader dbReader)
            {
                while (await dbReader.ReadAsync(cancellationToken))
                {
                    dbReader.GetValues(values);
                    dataTable.Rows.Add(values);
                }
            }
            else
            {
                while (reader.Read())
                {
                    reader.GetValues(values);
                    dataTable.Rows.Add(values);
                }
            }

            return dataTable;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columnName"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static object[] GetBoundValues(
            IReadOnlyList<DataRow> rows,
            string columnName,
            int offset,
            int count)
        {
            var values = new object[count];
            for (var rowIndex = 0; rowIndex < count; rowIndex++)
            {
                values[rowIndex] = rows[offset + rowIndex][columnName] ?? DBNull.Value;
            }
            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IReadOnlyList<OracleBulkInsertMapItem> ResolveMappings(
            DataTable dataTable)
        {
            if (ColumnMappings.Count > 0)
            {
                return ColumnMappings.ToArray();
            }

            return dataTable.Columns
                .OfType<DataColumn>()
                .Select(column => new OracleBulkInsertMapItem(column.ColumnName, column.ColumnName))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private string BuildInsertStatement(
            IReadOnlyList<OracleBulkInsertMapItem> mappings)
        {
            var columnList = string.Join(", ", mappings.Select(mapping => mapping.DestinationColumn));
            var parameterList = string.Join(", ", Enumerable.Range(0, mappings.Count).Select(i => ":p" + i));
            return $"INSERT INTO {DestinationTableName} ({columnList}) VALUES ({parameterList})";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rows"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<int> BindArrayCoreAsync(
            DataTable dataTable,
            IReadOnlyList<DataRow> rows,
            CancellationToken cancellationToken)
        {
            if (dataTable.Columns.Count == 0 || rows.Count == 0)
            {
                return 0;
            }

            var mappings = ResolveMappings(dataTable);
            var batchSize = GetEffectiveBatchSize(mappings.Count);
            var commandText = BuildInsertStatement(mappings);
            var affectedRows = 0;

            for (var offset = 0; offset < rows.Count; offset += batchSize)
            {
                var count = Math.Min(batchSize, rows.Count - offset);

                await using var command = connection.CreateCommand();

                command.Transaction = Transaction;
                command.BindByName = true;
                command.ArrayBindCount = count;
                command.CommandText = commandText;
                if (BulkCopyTimeout > 0)
                {
                    command.CommandTimeout = BulkCopyTimeout;
                }

                for (var i = 0; i < mappings.Count; i++)
                {
                    var mapping = mappings[i];
                    var parameter = new OracleParameter
                    {
                        ParameterName = "p" + i,
                        OracleDbType = mapping.OracleDbType ?? dbTypeResolver.Resolve(dataTable.Columns[mapping.SourceColumn].DataType),
                        Direction = ParameterDirection.Input,
                        Value = GetBoundValues(rows, mapping.SourceColumn, offset, count)
                    };

                    command.Parameters.Add(parameter);
                }

                affectedRows += await command.ExecuteNonQueryAsync(
                    cancellationToken);
            }

            return affectedRows;
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> BindArrayAsync(
            IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            if (reader.FieldCount == 0)
            {
                return 0;
            }

            var dataTable = await ToDataTableAsync(reader, cancellationToken);
            var rows = dataTable.Rows.OfType<DataRow>().ToArray();

            return await BindArrayCoreAsync(dataTable, rows, cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> BindArrayAsync(
            DataTable dataTable,
            DataRowState? rowState = null,
            CancellationToken cancellationToken = default)
        {
            var rowsQuery = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue)
            {
                rowsQuery = rowsQuery.Where(row => row.RowState == rowState);
            }

            return BindArrayCoreAsync(dataTable, rowsQuery.ToArray(), cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
        }
    }
}
