using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
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
        #region Sync

        #region BinaryBulkInsertBase<TEntity>

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
        private static int BinaryBulkInsertBase<TEntity>(this NpgsqlConnection connection,
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
            var entityType = entities?.First()?.GetType() ?? typeof(TEntity); // Solving the anonymous types
            var isDictionary = entityType.IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);

            return PseudoBasedBinaryImport(connection,
                tableName,
                bulkCopyTimeout,

                // getPseudoTableName
                () =>
                    GetBinaryInsertPseudoTableName(tableName ?? ClassMappedNameCache.Get<TEntity>(), dbSetting),

                // getMappings
                () =>
                    mappings = mappings?.Any() == true ? mappings :
                        isDictionary ?
                        GetMappings(entities?.First() as IDictionary<string, object>,
                            dbFields,
                            (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                            dbSetting) :
                        GetMappings(dbFields,
                            PropertyCache.Get(entityType),
                            (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                            dbSetting),

                dbFields,

                // binaryImport
                (tableName) =>
                    connection.BinaryImport<TEntity>(tableName,
                        entities,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction),

                // setIdentities
                (identities) =>
                    SetIdentities(entityType, entities, dbFields, identities, dbSetting),

                identityBehavior,
                pseudoTableType,
                dbSetting,
                transaction);
        }

        #endregion

        #region BinaryBulkInsertBase<DataTable>

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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryBulkInsertBase(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);

            return PseudoBasedBinaryImport(connection,
                tableName,
                bulkCopyTimeout,

                // getPseudoTableName
                () =>
                    GetBinaryInsertPseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                    mappings?.Any() == true ? mappings : GetMappings(table, dbFields,
                        (identityBehavior == BulkImportIdentityBehavior.KeepIdentity), dbSetting),

                dbFields,

                // binaryImport
                (tableName) =>
                    connection.BinaryImport(tableName,
                        table,
                        rowState,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction),

                // setIdentities
                null,

                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                dbSetting,
                transaction: transaction);
        }

        #endregion

        #region BinaryBulkInsertBase<DbDataReader>

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
        private static int BinaryBulkInsertBase(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);

            return PseudoBasedBinaryImport(connection,
                tableName,
                bulkCopyTimeout,

                // getPseudoTableName
                () =>
                    GetBinaryInsertPseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                    mappings?.Any() == true ? mappings : GetMappings(reader, dbFields,
                        (identityBehavior == BulkImportIdentityBehavior.KeepIdentity), dbSetting),

                dbFields,

                // binaryImport
                (tableName) =>
                    connection.BinaryImport(tableName,
                        reader,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting,
                        transaction),

                // setIdentities
                null,

                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                dbSetting,
                transaction: transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BinaryBulkInsertBaseAsync<TEntity>

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryBulkInsertBaseAsync<TEntity>(this NpgsqlConnection connection,
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
            var entityType = entities?.First()?.GetType() ?? typeof(TEntity); // Solving the anonymous types
            var isDictionary = entityType.IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                bulkCopyTimeout,

                // getPseudoTableName
                () =>
                    GetBinaryInsertPseudoTableName(tableName ?? ClassMappedNameCache.Get<TEntity>(), dbSetting),

                // getMappings
                () =>
                    mappings = mappings?.Any() == true ? mappings :
                        isDictionary ?
                        GetMappings(entities?.First() as IDictionary<string, object>,
                            dbFields,
                            (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                            dbSetting) :
                        GetMappings(dbFields,
                            PropertyCache.Get(entityType),
                            (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                            dbSetting),

                dbFields,

                // binaryImport
                async (tableName) =>
                    await connection.BinaryImportAsync<TEntity>(tableName,
                        entities,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // setIdentities
                (identities) =>
                    SetIdentities(entityType, entities, dbFields, identities, dbSetting),

                identityBehavior,
                pseudoTableType,
                dbSetting,
                transaction,
                cancellationToken);
        }

        #endregion

        #region BinaryBulkInsertBaseAsync<DataTable>

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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryBulkInsertBaseAsync(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                bulkCopyTimeout,

                // getPseudoTableName
                () =>
                    GetBinaryInsertPseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                    mappings?.Any() == true ? mappings : GetMappings(table, dbFields,
                        (identityBehavior == BulkImportIdentityBehavior.KeepIdentity), dbSetting),

                dbFields,

                // binaryImport
                async (tableName) =>
                    await connection.BinaryImportAsync(tableName,
                        table,
                        rowState,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // setIdentities
                null,

                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                dbSetting,
                transaction: transaction,
                cancellationToken);
        }

        #endregion

        #region BinaryBulkInsertBaseAsync<DbDataReader>

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryBulkInsertBaseAsync(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                bulkCopyTimeout,

                // getPseudoTableName
                () =>
                    GetBinaryInsertPseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                    mappings?.Any() == true ? mappings : GetMappings(reader, dbFields,
                        (identityBehavior == BulkImportIdentityBehavior.KeepIdentity), dbSetting),

                dbFields,

                // binaryImport
                async (tableName) =>
                    await connection.BinaryImportAsync(tableName,
                        reader,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // setIdentities
                null,

                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                dbSetting,
                transaction: transaction,
                cancellationToken);
        }

        #endregion

        #endregion
    }
}
