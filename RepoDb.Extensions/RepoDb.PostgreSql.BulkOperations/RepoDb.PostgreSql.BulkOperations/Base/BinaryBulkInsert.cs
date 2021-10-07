using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.PostgreSql.BulkOperations;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region BinaryBulkInsert

        // TODO: Make this one private

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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int BinaryBulkInsertBase<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            string pseudoTableName = null;

            try
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);

                mappings = mappings?.Any() == true ? mappings :
                    GetMappings(dbFields,
                        PropertyCache.Get<TEntity>(),
                        (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                        dbSetting);

                CreatePseudoTable(connection,
                    tableName,
                    () => (pseudoTableName = GetBinaryInsertPseudoTableName<TEntity>(tableName, connection.GetDbSetting())),
                    mappings,
                    bulkCopyTimeout,
                    identityBehavior,
                    pseudoTableType,
                    dbSetting,
                    transaction);

                return connection.BinaryImport<TEntity>((pseudoTableName ?? tableName),
                    entities,
                    mappings,
                    dbFields,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    dbSetting,
                    transaction);

                // TODO: INSERT INTO tableName SELECT * FROM pseudoTableName;

                // TODO: Return the identity
            }
            finally
            {
                DropPseudoTable(connection, pseudoTableName, bulkCopyTimeout, transaction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BinaryBulkInsertBase(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            return connection.BinaryImport(tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                transaction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BinaryBulkInsertBase(this NpgsqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            return connection.BinaryImport(tableName,
                dataTable,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                transaction);
        }

        #endregion

        #region BinaryBulkInsertAsyncBase

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryBulkInsertAsyncBase<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await connection.BinaryImportAsync(tableName,
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                transaction,
                cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BinaryBulkInsertAsyncBase(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return await connection.BinaryImportAsync(tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                transaction,
                cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BinaryBulkInsertAsyncBase(this NpgsqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return await connection.BinaryImportAsync(tableName,
                dataTable,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                transaction,
                cancellationToken);
        }

        #endregion
    }
}
