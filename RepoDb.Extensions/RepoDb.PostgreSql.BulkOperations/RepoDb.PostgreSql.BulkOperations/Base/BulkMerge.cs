using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region Sync

        #region BulkMergeBase<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="mergeCommandType"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportIdentityBehavior identityBehavior = default,
            PostgreSqlBulkImportMergeCommandType mergeCommandType = default,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkMerge,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            var entityType = entities?.First()?.GetType() ?? typeof(TEntity); // Solving the anonymous types
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var pseudoTableName = tableName;

            return PseudoBasedBinaryImport(connection,
                tableName,
                entities?.Count() ?? 0,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkMergePseudoTableName(tableName ?? ClassMappedNameCache.Get<TEntity>(), dbSetting),

                // getMappings
                () =>
                {
                    var includeIdentity = identityBehavior == PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
                    var includePrimary = true;

                    return mappings = mappings?.Any() == true ? mappings :
                        isDictionary ?
                        GetMappings(entities?.First() as IDictionary<string, object>,
                            dbFields,
                            includePrimary,
                            includeIdentity,
                            dbSetting) :
                        GetMappings(dbFields,
                            PropertyCache.Get(entityType),
                            includePrimary,
                            includeIdentity,
                            dbSetting);
                },

                // binaryImport
                (tableName) =>
                    connection.BinaryImportInternal<TEntity>(tableName,
                        entities,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction),

                // getMergeToPseudoCommandText
                () =>
                    GetMergeCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        identityBehavior,
                        mergeCommandType,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetIdentities(entityType, entities, dbFields, identityResults, dbSetting),

                qualifiers,
                false,
                identityBehavior,
                pseudoTableType,
                dbSetting,
                trace,
                traceKey,
                transaction);
        }

        #endregion

        #region BulkMergeBase<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="mergeCommandType"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportIdentityBehavior identityBehavior = default,
            PostgreSqlBulkImportMergeCommandType mergeCommandType = default,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkMerge,
            NpgsqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var pseudoTableName = tableName;

            return PseudoBasedBinaryImport(connection,
                tableName,
                table?.Rows.Count ?? 0,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkMergePseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                {
                    var includeIdentity = identityBehavior == PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
                    var includePrimary = true;

                    return mappings = mappings?.Any() == true ? mappings :
                        GetMappings(table,
                            dbFields,
                            includePrimary,
                            includeIdentity,
                            dbSetting);
                },

                // binaryImport
                (tableName) =>
                    connection.BinaryImportInternal(tableName,
                        table,
                        rowState,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction),

                // getMergeToPseudoCommandText
                () =>
                    GetMergeCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        identityBehavior,
                        mergeCommandType,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetDataTableIdentities(table, dbFields, identityResults, dbSetting),

                qualifiers,
                false,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                dbSetting,
                trace,
                traceKey,
                transaction: transaction);
        }

        #endregion

        #region BulkMergeBase<IDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="mergeCommandType"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase(this NpgsqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            PostgreSqlBulkImportIdentityBehavior identityBehavior = default,
            PostgreSqlBulkImportMergeCommandType mergeCommandType = default,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkMerge,
            NpgsqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var pseudoTableName = tableName;

            return PseudoBasedBinaryImport(connection,
                tableName,
                0, // row count is unknown for a forward-only reader
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkMergePseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                {
                    var includeIdentity = identityBehavior == PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
                    var includePrimary = true;

                    return mappings = mappings?.Any() == true ? mappings :
                        GetMappings(reader,
                            dbFields,
                            includePrimary,
                            includeIdentity,
                            dbSetting);
                },

                // binaryImport
                (tableName) =>
                    connection.BinaryImportInternal(tableName,
                        reader,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting,
                        transaction),

                // getMergeToPseudoCommandText
                () =>
                    GetMergeCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        identityBehavior,
                        mergeCommandType,
                        dbSetting),

                // setIdentities
                null,

                qualifiers,
                false,
                identityBehavior,
                pseudoTableType,
                dbSetting,
                trace,
                traceKey,
                transaction: transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeBaseAsync<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="mergeCommandType"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportIdentityBehavior identityBehavior = default,
            PostgreSqlBulkImportMergeCommandType mergeCommandType = default,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkMerge,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityType = entities?.First()?.GetType() ?? typeof(TEntity); // Solving the anonymous types
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var pseudoTableName = tableName;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                entities?.Count() ?? 0,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkMergePseudoTableName(tableName ?? ClassMappedNameCache.Get<TEntity>(), dbSetting),

                // getMappings
                () =>
                {
                    var includeIdentity = identityBehavior == PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
                    var includePrimary = true;

                    return mappings = mappings?.Any() == true ? mappings :
                        isDictionary ?
                        GetMappings(entities?.First() as IDictionary<string, object>,
                            dbFields,
                            includePrimary,
                            includeIdentity,
                            dbSetting) :
                        GetMappings(dbFields,
                            PropertyCache.Get(entityType),
                            includePrimary,
                            includeIdentity,
                            dbSetting);
                },

                // binaryImport
                async (tableName) =>
                    await connection.BinaryImportAsyncInternal<TEntity>(tableName,
                        entities,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // getMergeToPseudoCommandText
                () =>
                    GetMergeCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        identityBehavior,
                        mergeCommandType,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetIdentities(entityType, entities, dbFields, identityResults, dbSetting),

                qualifiers,
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

        #region BulkMergeBaseAsync<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="mergeCommandType"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportIdentityBehavior identityBehavior = default,
            PostgreSqlBulkImportMergeCommandType mergeCommandType = default,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkMerge,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var pseudoTableName = tableName;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                table?.Rows.Count ?? 0,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkMergePseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                {
                    var includeIdentity = identityBehavior == PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
                    var includePrimary = true;

                    return mappings = mappings?.Any() == true ? mappings :
                        GetMappings(table,
                            dbFields,
                            includePrimary,
                            includeIdentity,
                            dbSetting);
                },

                // binaryImport
                async (tableName) =>
                    await connection.BinaryImportAsyncInternal(tableName,
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

                // getMergeToPseudoCommandText
                () =>
                    GetMergeCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        identityBehavior,
                        mergeCommandType,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetDataTableIdentities(table, dbFields, identityResults, dbSetting),

                qualifiers,
                false,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                dbSetting,
                trace,
                traceKey,
                transaction: transaction,
                cancellationToken);
        }

        #endregion

        #region BulkMergeBaseAsync<IDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="mergeCommandType"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync(this NpgsqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            PostgreSqlBulkImportIdentityBehavior identityBehavior = default,
            PostgreSqlBulkImportMergeCommandType mergeCommandType = default,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkMerge,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var pseudoTableName = tableName;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                0, // row count is unknown for a forward-only reader
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkMergePseudoTableName(tableName, dbSetting),

                // getMappings
                () =>
                {
                    var includeIdentity = identityBehavior == PostgreSqlBulkImportIdentityBehavior.KeepIdentity;
                    var includePrimary = true;

                    return mappings = mappings?.Any() == true ? mappings :
                        GetMappings(reader,
                            dbFields,
                            includePrimary,
                            includeIdentity,
                            dbSetting);
                },

                // binaryImport
                async (tableName) =>
                    await connection.BinaryImportAsyncInternal(tableName,
                        reader,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // getMergeToPseudoCommandText
                () =>
                    GetMergeCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        identityBehavior,
                        mergeCommandType,
                        dbSetting),

                // setIdentities
                null,

                qualifiers,
                false,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                dbSetting,
                trace,
                traceKey,
                transaction: transaction,
                cancellationToken);
        }

        #endregion

        #endregion
    }
}
