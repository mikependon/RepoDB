using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;
using RepoDb;
using RepoDb.Enumerations.MySqlConnector;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.MySqlConnector.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class MySqlConnectorConnectionExtension
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
        internal static int WriteToServerInternal<TEntity>(MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null)
            where TEntity : class
        {
            connection.EnsureOpen();
            using var reader = new DataEntityDataReader<TEntity>(entities);
            var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout);
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
        internal static int WriteToServerInternal(MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null)
        {
            connection.EnsureOpen();
            var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout);
            var rows = GetDataRows(table, rowState)?.ToArray();
            bulkCopy.WriteToServer(rows, table.Columns.Count);
            return rows != null ? rows.Length : 0;
        }

        /// <summary>
        /// Streams <paramref name="reader"/> directly into <paramref name="tableName"/> via <see cref="MySqlBulkCopy"/>
        /// without ever materializing it into a list or <see cref="DataTable"/> first - the whole point of the
        /// <see cref="DbDataReader"/> overloads is to let a source query keep streaming into the destination
        /// as it's read, instead of buffering every row in memory up front (as the <c>TEntity</c>/<see cref="DataTable"/>
        /// overloads do). Since <paramref name="reader"/> is forward-only and single-pass, there's no way to know
        /// the row count ahead of time the way the other overloads do (<c>entities.Count()</c>/<c>rows.Length</c>) -
        /// so this wraps it in <see cref="CountingDataReader"/>, which tallies exactly how many rows
        /// <see cref="MySqlBulkCopy"/> actually pulled through <see cref="IDataReader.Read"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null)
        {
            connection.EnsureOpen();
            var countingReader = new CountingDataReader(reader);
            var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, countingReader, mappings, bulkCopyTimeout);
            bulkCopy.WriteToServer(countingReader);
            return countingReader.Count;
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
        internal static async Task<int> WriteToServerAsyncInternal<TEntity>(MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            await connection.EnsureOpenAsync(cancellationToken);
            return await Task.Run(() => // No underlying 'Async' equivalent for 'WriteToServerInternal'
            {
                using var reader = new DataEntityDataReader<TEntity>(entities);
                var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout);
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
        internal static async Task<int> WriteToServerAsyncInternal(MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout);
            var rows = GetDataRows(table, rowState)?.ToArray();
            await bulkCopy.WriteToServerAsync(rows, table.Columns.Count, cancellationToken);
            return rows != null ? rows.Length : 0;
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="WriteToServerInternal(MySqlConnection, string, DbDataReader, IEnumerable{MySqlConnectorBulkInsertMapItem}, int?, int?)"/> -
        /// see its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            return await Task.Run(() => // No underlying 'Async' equivalent for 'WriteToServerInternal'
            {
                var countingReader = new CountingDataReader(reader);
                var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, countingReader, mappings, bulkCopyTimeout);
                bulkCopy.WriteToServer(countingReader);
                return countingReader.Count;
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
        /// <b>Temporarily forced to <see cref="MySqlConnectorBulkImportPseudoTableType.Physical"/> for every input</b>
        /// (including an explicit <see cref="MySqlConnectorBulkImportPseudoTableType.Memory"/> request, and regardless
        /// of <paramref name="rowCount"/>) - <see cref="MySqlBulkCopy.WriteToServer(System.Data.DataRow[])"/>
        /// always performs a direct-path load internally (ODP.NET's <c>MySqlConnectorBulkCopyOptions</c> has no
        /// conventional-path alternative), and MySqlConnector's direct-path engine cannot write into a Global
        /// Temporary Table at all - confirmed live via <c>ORA-39826: Direct path load of view or synonym
        /// (...) could not be resolved</c>, MySqlConnector's generic error for an unsupported direct-path destination
        /// object type. Since every pseudo table is bulk-written to via <see cref="MySqlBulkCopy"/>
        /// (see <see cref="WriteToServerInternal{TEntity}"/>/<see cref="WriteToServerInternal(MySqlConnection, string, System.Data.DataTable, System.Data.DataRowState?, IEnumerable{MySqlConnectorBulkInsertMapItem}, int?, int?)"/>),
        /// a <c>Memory</c> (GTT) pseudo table can never actually be used as a bulk-copy destination as
        /// currently built - so <see cref="MySqlConnectorBulkImportPseudoTableType.Auto"/>'s row-count threshold
        /// logic is a no-op for now too, until a working strategy for a session-isolated staging table
        /// (e.g. writing to a GTT via array-bound <c>INSERT</c>s instead of <see cref="MySqlBulkCopy"/>)
        /// is designed and implemented.
        /// </para>
        /// </remarks>
        private static MySqlConnectorBulkImportPseudoTableType ResolvePseudoTableType(MySqlConnectorBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            pseudoTableType == MySqlConnectorBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= MySqlConnectorConstants.RowCountThresholdForPhysicalTable ?
                MySqlConnectorBulkImportPseudoTableType.Physical :
                    MySqlConnectorBulkImportPseudoTableType.Physical; // pseudoTableType; // TODO: ODP.NET Limitation, force to Physical for now

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
            if (!rows.Any())
            {
                throw new System.InvalidOperationException($"No rows found from data table where the state is '{rowState.ToString()}'.");
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
        private static MySqlBulkCopy CreateBulkCopyForDataTable(MySqlConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings,
            int? bulkCopyTimeout)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new MySqlBulkCopy(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (mappings != null)
            {
                var columnMappings = mappings.AsList();
                foreach (var mapping in columnMappings)
                {
                    bulkCopy.ColumnMappings.Add(
                        new MySqlBulkCopyColumnMapping(columnMappings.IndexOf(mapping), mapping.DestinationColumn.AsQuoted(true, dbSetting)));
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(
                        new MySqlBulkCopyColumnMapping(table.Columns.IndexOf(column), column.ColumnName.AsQuoted(true, dbSetting)));
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
        /// <returns></returns>
        private static MySqlBulkCopy CreateBulkCopyForDataReader(MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings,
            int? bulkCopyTimeout)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new MySqlBulkCopy(connection)
            {
                // See the remarks in CreateBulkCopyForDataTable - same quoting requirement applies here.
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            var columnMappings = mappings?.AsList() ?? GetDefaultMappingsForDataReader(connection, tableName, reader).AsList();
            foreach (var mapping in columnMappings)
            {
                bulkCopy.ColumnMappings.Add(
                    new MySqlBulkCopyColumnMapping(columnMappings.IndexOf(mapping), mapping.DestinationColumn.AsQuoted(true, dbSetting)));
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
        /// integration test entities). Left unfiltered, <see cref="MySqlBulkCopy"/> falls back to
        /// ordinal column mapping when <see cref="MySqlBulkCopy.ColumnMappings"/> is left empty, and a
        /// column-count/type mismatch between the reader and the destination table fails with
        /// <c>ORA-50029: Column mapping is invalid</c>. Extra reader columns with no matching destination
        /// field are silently skipped (never written), exactly like the explicit-mappings path already does
        /// for a caller-supplied mapping that omits a column.
        /// </remarks>
        private static IEnumerable<MySqlConnectorBulkInsertMapItem> GetDefaultMappingsForDataReader(MySqlConnection connection,
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
                    yield return new MySqlConnectorBulkInsertMapItem(columnName, dbField.Name);
                }
            }
        }

        /// <summary>
        /// Resolves the field(s) used to match an existing row during the <c>MERGE</c> (the <c>ON</c> clause).
        /// Falls back to the table's primary key, then its identity key, when <paramref name="qualifiers"/>
        /// is not provided.
        /// </summary>
        /// <exception cref="PrimaryFieldNotFoundException">
        /// No <paramref name="qualifiers"/> were given, and the table has neither a primary nor an identity key.
        /// </exception>
        private static IEnumerable<Field> GetQualifierFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Field> qualifiers = null)
        {
            if (qualifiers?.Any() == true)
            {
                return qualifiers;
            }

            var primaryOrIdentity = dbFields?.GetPrimary() ?? dbFields?.GetIdentity();

            if (primaryOrIdentity == null)
            {
                throw new PrimaryFieldNotFoundException(
                    $"No primary or identity key found for table '{tableName}'. Provide explicit 'qualifiers' instead.");
            }

            return new[] { primaryOrIdentity.AsField() };
        }

        /// <summary>
        /// Builds a lightweight <see cref="MySqlConnectorCommand"/> that is never executed - it exists purely to
        /// carry a descriptive <c>CommandText</c> into <see cref="Tracer.InvokeBeforeExecution"/>/
        /// <see cref="Tracer.InvokeBeforeExecutionAsync"/> (see the <c>Base/BulkInsert.cs</c>,
        /// <c>Base/BulkMerge.cs</c>, <c>Base/BulkUpdate.cs</c> and <c>Base/BulkDelete.cs</c> leaf
        /// execution methods that call this). A bulk operation's actual data movement goes through
        /// <see cref="MySqlBulkCopy"/> (plus, for BulkMerge/BulkUpdate/BulkDelete, a handful of staging
        /// table DDL/DML statements) rather than a single <see cref="DbCommand"/> - so unlike
        /// <see cref="RepoDb.Extensions.DbConnectionExtension.ExecuteNonQuery"/> and friends, there is no
        /// command for the trace machinery to pick up for free; this synthesizes one.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText">A human-readable description of the bulk operation being traced (not executed).</param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static DbCommand CreateTraceCommand(MySqlConnection connection,
            string commandText,
            int? commandTimeout = null,
            MySqlTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
