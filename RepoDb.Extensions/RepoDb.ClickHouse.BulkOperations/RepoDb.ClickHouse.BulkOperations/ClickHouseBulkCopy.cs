using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DriverBulkCopy = ClickHouse.Driver.Copy.ClickHouseBulkCopy;

namespace RepoDb.ClickHouse.BulkOperations
{
    /// <summary>
    /// An official bulk loader class used by RepoDB for its bulk operations against ClickHouse. Unlike the
    /// MySQL provider's <c>MySqlBulkCopy</c> (a hand-rolled <c>LOAD DATA LOCAL INFILE</c> temp-file writer
    /// built on top of <c>MySql.Data</c>'s low-level API), ClickHouse already ships a full streaming
    /// bulk-copy implementation - <c>ClickHouse.Driver</c>'s own <see cref="DriverBulkCopy"/> - so this class
    /// is a thin adapter that reshapes the <see cref="ClickHouseBulkCopyColumnMapping"/>-based API the rest of
    /// this library's <c>Base/*.cs</c> orchestration code already expects (mirroring the sibling providers'
    /// surface) onto that driver type.
    /// </summary>
    internal sealed class ClickHouseBulkCopy
    {
        private readonly ClickHouseConnection connection;

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseBulkCopy"/> bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection the underlying <see cref="DriverBulkCopy"/> will load through.</param>
        public ClickHouseBulkCopy(ClickHouseConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Gets or sets the destination table name. Expected to be the raw, unquoted table name (every call
        /// site in <c>Base/WriteToServer.cs</c> sets this from <c>tableName.AsUnquoted(dbSetting)</c>) -
        /// <see cref="DriverBulkCopy"/> resolves the destination column types itself via a
        /// <c>system.columns</c>-style lookup keyed off this value, so it must not be back-tick quoted.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Gets or sets the bulk-load timeout, in seconds. <see cref="DriverBulkCopy"/> has no per-copy
        /// timeout knob of its own (command timeouts are governed by the connection/command instead), so this
        /// is accepted for API parity with the other bulk-operations providers but is otherwise unused.
        /// </summary>
        public int BulkCopyTimeout { get; set; } = 30;

        /// <summary>
        /// Gets or sets the number of rows written to the server per underlying batch. Passed straight through
        /// to <see cref="DriverBulkCopy.BatchSize"/> when set; otherwise the driver's own default (100,000) is
        /// used.
        /// </summary>
        public int? BatchSize { get; set; }

        /// <summary>
        /// Gets the source-ordinal-to-destination-column mappings that determine both which columns get
        /// written and the column order presented to <see cref="DriverBulkCopy"/>.
        /// </summary>
        public List<ClickHouseBulkCopyColumnMapping> ColumnMappings { get; } = new();

        #region WriteToServer

        /// <summary>
        /// Writes every remaining row of <paramref name="reader"/> to <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="reader">
        /// The source reader. Read positionally via each mapping's <see cref="ClickHouseBulkCopyColumnMapping.SourceOrdinal"/>.
        /// </param>
        /// <returns>The number of rows reported as written.</returns>
        public int WriteToServer(IDataReader reader) =>
            WriteToServerAsync(reader, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous counterpart of <see cref="WriteToServer(IDataReader)"/>.
        /// </summary>
        public async Task<int> WriteToServerAsync(IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            var projectedReader = new ColumnFilteredDataReader(reader,
                ColumnMappings.Select(m => m.SourceOrdinal).ToArray());

            using var bulkCopy = CreateInnerBulkCopy();
            await bulkCopy.InitAsync();
            await bulkCopy.WriteToServerAsync(projectedReader, cancellationToken);
            return checked((int)bulkCopy.RowsWritten);
        }

        /// <summary>
        /// Writes <paramref name="rows"/> to <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="rows">The source rows, read positionally via each mapping's <see cref="ClickHouseBulkCopyColumnMapping.SourceOrdinal"/> - i.e. the real <see cref="DataTable"/> column index, not the mapping's position in <see cref="ColumnMappings"/>.</param>
        /// <param name="columnCount">Unused beyond a defensive bounds check - <see cref="ColumnMappings"/> alone already determines exactly which columns get written.</param>
        /// <returns>The number of rows reported as written.</returns>
        public int WriteToServer(DataRow[] rows,
            int columnCount) =>
            WriteToServerAsync(rows, columnCount, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous counterpart of <see cref="WriteToServer(DataRow[], int)"/>.
        /// </summary>
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

            using var bulkCopy = CreateInnerBulkCopy();
            await bulkCopy.InitAsync();
            await bulkCopy.WriteToServerAsync(rowValues, cancellationToken);
            return checked((int)bulkCopy.RowsWritten);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Builds a fresh, correctly-configured instance of the underlying driver's bulk-copy type - one per
        /// write, matching <see cref="DriverBulkCopy"/>'s own single-use design (it owns/tracks its own
        /// per-copy state, such as <see cref="DriverBulkCopy.RowsWritten"/>).
        /// </summary>
        private DriverBulkCopy CreateInnerBulkCopy()
        {
            var bulkCopy = new DriverBulkCopy(connection)
            {
                DestinationTableName = DestinationTableName,
                ColumnNames = ColumnMappings.Select(m => m.DestinationColumn).ToArray(),
            };
            if (BatchSize.HasValue)
            {
                bulkCopy.BatchSize = BatchSize.Value;
            }
            return bulkCopy;
        }

        #endregion
    }
}
