using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
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

        #region BulkDeleteByKeyBase<TPrimaryKey>

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
        private static int BulkDeleteByKeyBase<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkDeleteByKey,
            NpgsqlTransaction transaction = null)
        {
            var identityBehavior = PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primaryKey = dbFields.GetPrimary();
            var pseudoTableName = tableName;
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null;

            return PseudoBasedBinaryImport(connection,
                tableName,
                primaryKeys?.Count() ?? 0,
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
                    connection.BinaryImportInternal(tableName,
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
                trace,
                traceKey,
                transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkDeleteByKeyBaseAsync<TPrimaryKey>

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
        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkDeleteByKey,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var identityBehavior = PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var primaryKey = dbFields.GetPrimary();
            var pseudoTableName = tableName;
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                primaryKeys?.Count() ?? 0,
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
                    await connection.BinaryImportAsyncInternal(tableName,
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
                trace,
                traceKey,
                transaction,
                cancellationToken);
        }

        #endregion

        #endregion
    }
}
