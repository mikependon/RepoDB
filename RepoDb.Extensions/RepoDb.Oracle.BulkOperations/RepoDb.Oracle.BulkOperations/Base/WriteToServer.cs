using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb;
using RepoDb.Enumerations.Oracle;
using RepoDb.Extensions;
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
        /// Resolves <paramref name="pseudoTableType"/> to the pseudo table type actually used for a bulk
        /// operation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Temporarily forced to <see cref="OracleBulkImportPseudoTableType.Physical"/> for every input</b>
        /// (including an explicit <see cref="OracleBulkImportPseudoTableType.Memory"/> request, and regardless
        /// of <paramref name="rowCount"/>) - <see cref="OracleBulkCopy.WriteToServer(System.Data.DataRow[])"/>
        /// always performs a direct-path load internally (ODP.NET's <c>OracleBulkCopyOptions</c> has no
        /// conventional-path alternative), and Oracle's direct-path engine cannot write into a Global
        /// Temporary Table at all - confirmed live via <c>ORA-39826: Direct path load of view or synonym
        /// (...) could not be resolved</c>, Oracle's generic error for an unsupported direct-path destination
        /// object type. Since every pseudo table is bulk-written to via <see cref="OracleBulkCopy"/>
        /// (see <see cref="WriteToServerInternal{TEntity}"/>/<see cref="WriteToServerInternal(OracleConnection, string, System.Data.DataTable, System.Data.DataRowState?, IEnumerable{OracleBulkInsertMapItem}, int?, int?)"/>),
        /// a <c>Memory</c> (GTT) pseudo table can never actually be used as a bulk-copy destination as
        /// currently built - so <see cref="OracleBulkImportPseudoTableType.Auto"/>'s row-count threshold
        /// logic is a no-op for now too, until a working strategy for a session-isolated staging table
        /// (e.g. writing to a GTT via array-bound <c>INSERT</c>s instead of <see cref="OracleBulkCopy"/>)
        /// is designed and implemented.
        /// </para>
        /// </remarks>
        private static OracleBulkImportPseudoTableType ResolvePseudoTableType(OracleBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            pseudoTableType == OracleBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= OracleConstants.RowCountThresholdForPhysicalTable ?
                OracleBulkImportPseudoTableType.Physical :
                    OracleBulkImportPseudoTableType.Physical; // pseudoTableType; // TODO: ODP.NET Limitation, force to Physical for now

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
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new OracleBulkCopy(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
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
                    bulkCopy.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn.AsQuoted(true, dbSetting));
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName.AsQuoted(true, dbSetting));
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
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new OracleBulkCopy(connection)
            {
                // See the remarks in CreateBulkCopyForDataTable - same quoting requirement applies here.
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize.Value;
            }
            foreach (var mapping in mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader))
            {
                bulkCopy.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn.AsQuoted(true, dbSetting));
            }
            return bulkCopy;
        }

        /// <summary>
        /// Builds a default source-to-destination column mapping for <paramref name="reader"/> when the
        /// caller did not supply an explicit one - by intersecting the reader's own columns (every public
        /// property of the source <c>TEntity</c>, per <see cref="DataEntityDataReader{TEntity}"/>) against
        /// <paramref name="tableName"/>'s real columns.
        /// </summary>
        /// <remarks>
        /// Needed because a <c>TEntity</c> can carry "extra" properties that have no corresponding column
        /// at all (e.g. a computed/joined field, or a navigation collection - see the <c>...WithExtraFields</c>
        /// integration test entities). Left unfiltered, <see cref="OracleBulkCopy"/> falls back to
        /// ordinal column mapping when <see cref="OracleBulkCopy.ColumnMappings"/> is left empty, and a
        /// column-count/type mismatch between the reader and the destination table fails with
        /// <c>ORA-50029: Column mapping is invalid</c>. Extra reader columns with no matching destination
        /// field are silently skipped (never written), exactly like the explicit-mappings path already does
        /// for a caller-supplied mapping that omits a column.
        /// </remarks>
        private static IEnumerable<OracleBulkInsertMapItem> GetDefaultMappingsForDataReader(OracleConnection connection,
            string tableName,
            IDataReader reader)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, null);
            var dbSetting = connection.GetDbSetting();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var dbField = dbFields.GetByUnquotedName(columnName.AsUnquoted(true, dbSetting));
                if (dbField != null)
                {
                    yield return new OracleBulkInsertMapItem(columnName, dbField.Name);
                }
            }
        }

        #endregion
    }
}
