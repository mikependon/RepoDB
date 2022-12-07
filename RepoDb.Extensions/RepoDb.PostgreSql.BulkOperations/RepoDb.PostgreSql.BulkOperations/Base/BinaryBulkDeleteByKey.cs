using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.BulkOperations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region Sync

        #region BinaryBulkDeleteByKeyBase<TPrimaryKey>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryBulkDeleteByKeyBase<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            var identityBehavior = BulkImportIdentityBehavior.Unspecified;
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primaryKey = dbFields.GetPrimary();
            var pseudoTableName = tableName;
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null;

            return PseudoBasedBinaryImport(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeleteByKeyPseudoTableName(tableName ?? ClassMappedNameCache.Get<TPrimaryKey>(), dbSetting),

                // getMappings
                () =>
                    mappings = new[]
                    {
                        new NpgsqlBulkInsertMapItem(primaryKey.Name, primaryKey.Name)
                    },

                // binaryImport
                (tableName) =>
                    connection.BinaryImport(tableName,
                        GetExpandoObjectData(primaryKeys, primaryKey.AsField()),
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteByKeyCommandText(pseudoTableName,
                        tableName,
                        dbFields.GetPrimary()?.AsField(),
                        dbSetting),

                // setIdentities
                null,

                null,
                false,
                identityBehavior,
                pseudoTableType,
                dbSetting,
                transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BinaryBulkDeleteByKeyBaseAsync<TPrimaryKey>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryBulkDeleteByKeyBaseAsync<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var identityBehavior = BulkImportIdentityBehavior.Unspecified;
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var primaryKey = dbFields.GetPrimary();
            var pseudoTableName = tableName;
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeleteByKeyPseudoTableName(tableName ?? ClassMappedNameCache.Get<TPrimaryKey>(), dbSetting),

                // getMappings
                () =>
                    mappings = new[]
                    {
                        new NpgsqlBulkInsertMapItem(primaryKey.Name, primaryKey.Name)
                    },

                // binaryImport
                async (tableName) =>
                    await connection.BinaryImportAsync(tableName,
                        GetExpandoObjectData(primaryKeys, primaryKey.AsField()),
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteByKeyCommandText(pseudoTableName,
                        tableName,
                        dbFields.GetPrimary()?.AsField(),
                        dbSetting),

                // setIdentities
                null,

                null,
                false,
                identityBehavior,
                pseudoTableType,
                dbSetting,
                transaction,
                cancellationToken);
        }

        #endregion

        #endregion
    }
}
