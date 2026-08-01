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

#pragma warning disable CS0618 // Bridges the new PostgreSql-prefixed enums to the legacy internal Extensions APIs, which still use the deprecated enums.

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region Sync

        #region BulkDeleteBase<TEntity>

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
        /// <param name="keepIdentity"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = true,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            var entityType = entities?.First()?.GetType() ?? typeof(TEntity); // Solving the anonymous types
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var pseudoTableName = tableName;
            var identityBehavior = keepIdentity ? PostgreSqlBulkImportIdentityBehavior.KeepIdentity : PostgreSqlBulkImportIdentityBehavior.Unspecified;

            return PseudoBasedBinaryImport(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeletePseudoTableName(tableName ?? ClassMappedNameCache.Get<TEntity>(), dbSetting),

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
                    connection.BinaryImport<TEntity>(tableName,
                        entities,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting,
                        transaction),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetIdentities(entityType, entities, dbFields, identityResults, dbSetting),

                qualifiers,
                false,
                (BulkImportIdentityBehavior)identityBehavior,
                (BulkImportPseudoTableType)pseudoTableType,
                dbSetting,
                transaction);
        }

        #endregion

        #region BulkDeleteBase<DataTable>

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
        /// <param name="keepIdentity"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = true,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var pseudoTableName = tableName;
            var identityBehavior = keepIdentity ? PostgreSqlBulkImportIdentityBehavior.KeepIdentity : PostgreSqlBulkImportIdentityBehavior.Unspecified;

            return PseudoBasedBinaryImport(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeletePseudoTableName(tableName, dbSetting),

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
                    connection.BinaryImport(tableName,
                        table,
                        rowState,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting,
                        transaction),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetDataTableIdentities(table, dbFields, identityResults, dbSetting),

                qualifiers,
                false,
                identityBehavior: (BulkImportIdentityBehavior)identityBehavior,
                pseudoTableType: (BulkImportPseudoTableType)pseudoTableType,
                dbSetting,
                transaction: transaction);
        }

        #endregion

        #region BulkDeleteBase<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = true,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var pseudoTableName = tableName;
            var identityBehavior = keepIdentity ? PostgreSqlBulkImportIdentityBehavior.KeepIdentity : PostgreSqlBulkImportIdentityBehavior.Unspecified;

            return PseudoBasedBinaryImport(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeletePseudoTableName(tableName, dbSetting),

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
                    connection.BinaryImport(tableName,
                        reader,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting,
                        transaction),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting),

                // setIdentities
                null,

                qualifiers,
                false,
                (BulkImportIdentityBehavior)identityBehavior,
                (BulkImportPseudoTableType)pseudoTableType,
                dbSetting,
                transaction: transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkDeleteBaseAsync<TEntity>

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
        /// <param name="keepIdentity"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = true,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityType = entities?.First()?.GetType() ?? typeof(TEntity); // Solving the anonymous types
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var pseudoTableName = tableName;
            var identityBehavior = keepIdentity ? PostgreSqlBulkImportIdentityBehavior.KeepIdentity : PostgreSqlBulkImportIdentityBehavior.Unspecified;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeletePseudoTableName(tableName ?? ClassMappedNameCache.Get<TEntity>(), dbSetting),

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
                    await connection.BinaryImportAsync<TEntity>(tableName,
                        entities,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetIdentities(entityType, entities, dbFields, identityResults, dbSetting),

                qualifiers,
                false,
                (BulkImportIdentityBehavior)identityBehavior,
                (BulkImportPseudoTableType)pseudoTableType,
                dbSetting,
                transaction,
                cancellationToken);
        }

        #endregion

        #region BulkDeleteBaseAsync<DataTable>

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
        /// <param name="keepIdentity"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = true,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var pseudoTableName = tableName;
            var identityBehavior = keepIdentity ? PostgreSqlBulkImportIdentityBehavior.KeepIdentity : PostgreSqlBulkImportIdentityBehavior.Unspecified;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeletePseudoTableName(tableName, dbSetting),

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
                    await connection.BinaryImportAsync(tableName,
                        table,
                        rowState,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        batchSize,
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting),

                // setIdentities
                (identityResults) =>
                    SetDataTableIdentities(table, dbFields, identityResults, dbSetting),

                qualifiers,
                false,
                identityBehavior: (BulkImportIdentityBehavior)identityBehavior,
                pseudoTableType: (BulkImportPseudoTableType)pseudoTableType,
                dbSetting,
                transaction: transaction,
                cancellationToken);
        }

        #endregion

        #region BulkDeleteBaseAsync<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = true,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var pseudoTableName = tableName;
            var identityBehavior = keepIdentity ? PostgreSqlBulkImportIdentityBehavior.KeepIdentity : PostgreSqlBulkImportIdentityBehavior.Unspecified;

            return await PseudoBasedBinaryImportAsync(connection,
                tableName,
                bulkCopyTimeout,
                dbFields,

                // getPseudoTableName
                () =>
                    pseudoTableName = GetBinaryBulkDeletePseudoTableName(tableName, dbSetting),

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
                    await connection.BinaryImportAsync(tableName,
                        reader,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting,
                        transaction,
                        cancellationToken),

                // getDeleteToPseudoCommandText
                () =>
                    GetDeleteCommandText(pseudoTableName,
                        tableName,
                        mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                        qualifiers,
                        dbFields.GetPrimary()?.AsField(),
                        dbFields.GetIdentity()?.AsField(),
                        (BulkImportIdentityBehavior)identityBehavior,
                        dbSetting),

                // setIdentities
                null,

                qualifiers,
                false,
                identityBehavior: (BulkImportIdentityBehavior)identityBehavior,
                pseudoTableType: (BulkImportPseudoTableType)pseudoTableType,
                dbSetting,
                transaction: transaction,
                cancellationToken);
        }

        #endregion

        #endregion
    }
#pragma warning restore CS0618
}
