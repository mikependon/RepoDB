using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;

namespace RepoDb.ClickHouse.BulkOperations
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ClickHouseBulkCopy
    {
        private readonly ClickHouseConnection connection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public ClickHouseBulkCopy(ClickHouseConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BulkCopyTimeout { get; set; } = 30;

        /// <summary>
        /// 
        /// </summary>
        public int? BatchSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ClickHouseBulkCopyColumnMapping> ColumnMappings { get; } = new();

        #region WriteToServer

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public int WriteToServer(IDataReader reader) =>
            WriteToServerAsync(reader, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> WriteToServerAsync(IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            var projectedReader = new ColumnFilteredDataReader(reader,
                ColumnMappings.Select(m => m.SourceOrdinal).ToArray());

            var rowsWritten = await InsertBinaryAsync(EnumerateRows(projectedReader), cancellationToken);
            return checked((int)rowsWritten);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        public int WriteToServer(DataRow[] rows,
            int columnCount) =>
            WriteToServerAsync(rows, columnCount, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columnCount"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> WriteToServerAsync(DataRow[] rows,
            int columnCount,
            CancellationToken cancellationToken = default)
        {
            var rowValues = (rows ?? Array.Empty<DataRow>())
                .Select(row => ColumnMappings
                    .Select(mapping => mapping.SourceOrdinal < columnCount && mapping.SourceOrdinal < row.Table.Columns.Count && !row.IsNull(mapping.SourceOrdinal)
                        ? row[mapping.SourceOrdinal]
                        : null)
                    .ToArray());

            var rowsWritten = await InsertBinaryAsync(rowValues, cancellationToken);
            return checked((int)rowsWritten);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<long> InsertBinaryAsync(IEnumerable<object[]> rows,
            CancellationToken cancellationToken)
        {
            using var client = new ClickHouseClient(connection.ConnectionString);
            var options = new InsertOptions
            {
                BatchSize = BatchSize ?? 100_000,
                MaxDegreeOfParallelism = 4,
                MaxExecutionTime = TimeSpan.FromSeconds(BulkCopyTimeout),
                CustomSettings = new Dictionary<string, object>
                {
                    ["optimize_on_insert"] = 0,
                },
            };

            return await client.InsertBinaryAsync(
                DestinationTableName,
                ColumnMappings.Select(m => m.DestinationColumn).ToArray(),
                rows,
                options,
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<object[]> EnumerateRows(IDataReader reader)
        {
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                yield return values;
            }
        }

        #endregion
    }
}
