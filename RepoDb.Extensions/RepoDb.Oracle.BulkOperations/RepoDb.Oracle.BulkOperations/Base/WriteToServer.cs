using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb;
using RepoDb.Enumerations.Oracle;
using RepoDb.Oracle.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class OracleConnectionExtension
    {
        #region WriteToServerInternal

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal<TEntity>(OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null)
            where TEntity : class
        {
            connection.EnsureOpen();
            using var reader = new DataEntityDataReader<TEntity>(entities);
            using var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, batchSize);
            bulkCopy.WriteToServer(reader);
            return entities != null ? entities.Count() : 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(OracleConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null)
        {
            connection.EnsureOpen();
            using var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout, batchSize);
            var rows = GetDataRows(table, rowState)?.ToArray();
            bulkCopy.WriteToServer(rows);
            return rows != null ? rows.Length : 0;
        }

        #endregion

        #region WriteToServerAsyncInternal

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal<TEntity>(OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            await connection.EnsureOpenAsync(cancellationToken);
            return await Task.Run(() => // No underlying 'Async' equivalent for 'WriteToServerInternal'
            {
                using var reader = new DataEntityDataReader<TEntity>(entities);
                using var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, batchSize);
                bulkCopy.WriteToServer(reader);
                return entities != null ? entities.Count() : 0;
            },
            cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(OracleConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            return await Task.Run(() => // No underlying 'Async' equivalent for 'WriteToServerInternal'
            {
                using var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout, batchSize);
                var rows = GetDataRows(table, rowState)?.ToArray();
                bulkCopy.WriteToServer(rows);
                return rows != null ? rows.Length : 0;
            },
            cancellationToken);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Resolves <see cref="OracleBulkImportPseudoTableType.Auto"/> to a concrete pseudo table type
        /// based on <paramref name="rowCount"/>: <see cref="OracleBulkImportPseudoTableType.Physical"/>
        /// when it is greater than or equal to <see cref="OracleConstants.RowCountThresholdForPhysicalTable"/>,
        /// otherwise <see cref="OracleBulkImportPseudoTableType.Memory"/>. Any value other than
        /// <see cref="OracleBulkImportPseudoTableType.Auto"/> is returned unchanged.
        /// </summary>
        private static OracleBulkImportPseudoTableType ResolvePseudoTableType(OracleBulkImportPseudoTableType pseudoTableType,
            int rowCount) =>
            pseudoTableType == OracleBulkImportPseudoTableType.Auto
                ? (rowCount >= OracleConstants.RowCountThresholdForPhysicalTable ? OracleBulkImportPseudoTableType.Physical : OracleBulkImportPseudoTableType.Memory)
                : pseudoTableType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        private static IEnumerable<DataRow> GetDataRows(DataTable dataTable,
            DataRowState? rowState = null)
        {
            var rows = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue == true)
            {
                rows = rows.Where(r => r.RowState == rowState);
            }
            foreach (var row in rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        private static OracleBulkCopy CreateBulkCopyForDataTable(OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize = null)
        {
            var bulkCopy = new OracleBulkCopy(connection)
            {
                DestinationTableName = tableName
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize.Value;
            }
            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    bulkCopy.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn);
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
            }
            return bulkCopy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        private static OracleBulkCopy CreateBulkCopyForDataReader(OracleConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize = null)
        {
            var bulkCopy = new OracleBulkCopy(connection)
            {
                DestinationTableName = tableName
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize.Value;
            }
            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    bulkCopy.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn);
                }
            }
            return bulkCopy;
        }

        #endregion
    }
}
