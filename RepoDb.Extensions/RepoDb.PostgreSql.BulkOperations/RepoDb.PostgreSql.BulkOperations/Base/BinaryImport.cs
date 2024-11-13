using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region Sync

        #region BinaryImport<TEntity/Anonymous/IDictionary<string,object>>

        /// <summary>
        /// Inserts a list of entities into the target table by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static int BinaryImport<TEntity>(this NpgsqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
            where TEntity : class =>
            BinaryImport<TEntity>(connection,
                ClassMappedNameCache.Get<TEntity>(),
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                keepIdentity,
                transaction);

        /// <summary>
        /// Inserts a list of entities into the target table by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database. If not specified, the entity mappings will be used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static int BinaryImport<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            tableName ??= ClassMappedNameCache.Get<TEntity>();

            return BinaryImport<TEntity>(connection,
                tableName,
                entities,
                mappings,
                DbFieldCache.Get(connection, tableName, transaction),
                bulkCopyTimeout,
                batchSize,
                (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                connection.GetDbSetting(),
                transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryImport<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            DbFieldCollection? dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            // Solving the anonymous types
            var entityType = (entities?.First()?.GetType() ?? typeof(TEntity));
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
            var isPrimaryAnIdentity = IsPrimaryAnIdentity(dbFields);
            var includePrimary = isPrimaryAnIdentity == false || (isPrimaryAnIdentity && includeIdentity);

            // Mappings
            mappings = mappings?.Any() == true ? mappings :
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

            // Execution
            int execute()
            {
                var result = 0;
                var batches = entities.Split(batchSize.GetValueOrDefault());

                foreach (var batch in batches)
                {
                    using (var importer = GetNpgsqlBinaryImporter(connection,
                        tableName,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting))
                    {
                        if (isDictionary)
                        {
                            result += BinaryImport(importer,
                                batch?.Select(entity => entity as IDictionary<string, object>),
                                mappings,
                                identityBehavior);
                        }
                        else
                        {
                            result += BinaryImport<TEntity>(importer,
                                tableName,
                                batch,
                                mappings,
                                entityType,
                                identityBehavior);
                        }
                    }
                }

                return result;
            }

            // Transactional
            return TransactionalExecute(connection, execute, transaction);
        }

        #endregion

        #region BinaryImport<DataTable>

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk. It uses the <see cref="DataTable.TableName"/> property 
        /// as the target table. Underneath this operation is a call directly to the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-inserted. If not specified, all the rows of the table will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the <see cref="DataTable"/> will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static int BinaryImport(this NpgsqlConnection connection,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null) =>
            BinaryImport(connection,
                table?.TableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                keepIdentity,
                transaction);

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database. If not specified, the <see cref="DataTable.TableName"/> property will be used.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-inserted. If not specified, all the rows of the table will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the <see cref="DataTable"/> will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static int BinaryImport(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
        {
            tableName ??= table?.TableName;

            return BinaryImport(connection,
                tableName ?? table?.TableName,
                table,
                rowState,
                mappings,
                DbFieldCache.Get(connection, tableName ?? table?.TableName, transaction),
                    bulkCopyTimeout,
                    batchSize,
                    (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                    connection.GetDbSetting(),
                    transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryImport(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            DbFieldCollection? dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
        {
            var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
            var isPrimaryAnIdentity = IsPrimaryAnIdentity(dbFields);
            var includePrimary = isPrimaryAnIdentity == false || (isPrimaryAnIdentity && includeIdentity);

            // Mappings
            mappings = mappings?.Any() == true ? mappings :
                GetMappings(table,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    dbSetting);

            // Execution
            int execute()
            {
                var result = 0;
                var rows = GetRows(table, rowState).ToList();
                var batches = rows.Split(batchSize.GetValueOrDefault());

                foreach (var batch in batches)
                {
                    using (var importer = GetNpgsqlBinaryImporter(connection,
                        tableName ?? table?.TableName,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting))
                    {
                        result += BinaryImport(importer,
                            batch,
                            mappings,
                            identityBehavior);
                    }
                }

                return result;
            }

            // Transactional
            return TransactionalExecute(connection, execute, transaction);
        }

        #endregion

        #region BinaryImport<DataReader>

        /// <summary>
        /// Inserts the rows of the <see cref="DbDataReader"/> into the target table by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="reader">The instance of <see cref="DbDataReader"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the <see cref="DbDataReader"/> will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static int BinaryImport(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null) =>
            BinaryImport(connection,
                tableName,
                reader,
                mappings,
                DbFieldCache.Get(connection, tableName, transaction),
                bulkCopyTimeout,
                (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                connection.GetDbSetting(),
                transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryImport(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            DbFieldCollection? dbFields = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
        {
            var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
            var isPrimaryAnIdentity = IsPrimaryAnIdentity(dbFields);
            var includePrimary = isPrimaryAnIdentity == false || (isPrimaryAnIdentity && includeIdentity);

            // Mappings
            mappings = mappings?.Any() == true ? mappings :
                GetMappings(reader,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    dbSetting);

            // Execution
            int execute()
            {
                var result = 0;

                using (var importer = GetNpgsqlBinaryImporter(connection,
                    tableName,
                    mappings,
                    bulkCopyTimeout,
                    identityBehavior,
                    dbSetting))
                {
                    result += BinaryImport(importer,
                        reader,
                        mappings,
                        identityBehavior);
                }

                return result;
            }

            // Transactional
            return TransactionalExecute(connection, execute, transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BinaryImportAsync<TEntity/Anonymous/IDictionary<string,object>>

        /// <summary>
        /// Inserts a list of entities into the target table by bulk in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static Task<int> BinaryImportAsync<TEntity>(this NpgsqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BinaryImportAsync<TEntity>(connection,
                ClassMappedNameCache.Get<TEntity>(),
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                keepIdentity,
                transaction,
                cancellationToken);

        /// <summary>
        /// Inserts a list of entities into the target table by bulk in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database. If not specified, the entity mappings will be used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static async Task<int> BinaryImportAsync<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            tableName ??= ClassMappedNameCache.Get<TEntity>();

            return await BinaryImportAsync<TEntity>(connection,
                tableName,
                entities,
                mappings,
                await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken),
                bulkCopyTimeout,
                batchSize,
                (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                connection.GetDbSetting(),
                transaction,
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportAsync<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            DbFieldCollection? dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Solving the anonymous types
            var entityType = (entities?.First()?.GetType() ?? typeof(TEntity));
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
            var isPrimaryAnIdentity = IsPrimaryAnIdentity(dbFields);
            var includePrimary = isPrimaryAnIdentity == false || (isPrimaryAnIdentity && includeIdentity);

            // Mappings
            mappings = mappings?.Any() == true ? mappings :
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

            // Execution
            async Task<int> executeAsync()
            {
                var result = 0;
                var batches = entities.Split(batchSize.GetValueOrDefault());

                foreach (var batch in batches)
                {
                    using (var importer = await GetNpgsqlBinaryImporterAsync(connection,
                        tableName,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting,
                        cancellationToken))
                    {
                        if (isDictionary)
                        {
                            result += await BinaryImportExplicitAsync(importer,
                                batch?.Select(entity => entity as IDictionary<string, object>),
                                mappings,
                                identityBehavior,
                                cancellationToken);
                        }
                        else
                        {
                            result += await BinaryImportAsync<TEntity>(importer,
                                tableName,
                                batch,
                                mappings,
                                entityType,
                                identityBehavior,
                                cancellationToken);
                        }
                    }
                }

                return result;
            }

            // Transactional
            return await TransactionalExecuteAsync<int>(connection, executeAsync, transaction, cancellationToken);
        }

        #endregion

        #region BinaryImportAsync<DataTable>

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk in an asynchronous way. It uses the <see cref="DataTable.TableName"/> property
        /// as the target table. Underneath this operation is a call directly to the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-inserted. If not specified, all the rows of the table will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the <see cref="DataTable"/> will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static Task<int> BinaryImportAsync(this NpgsqlConnection connection,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BinaryImportAsync(connection,
                table?.TableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                keepIdentity,
                transaction,
                cancellationToken);

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database. If not specified, the <see cref="DataTable.TableName"/> property will be used.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-inserted. If not specified, all the rows of the table will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the <see cref="DataTable"/> will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static async Task<int> BinaryImportAsync(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            tableName ??= table?.TableName;

            return await BinaryImportAsync(connection,
                tableName,
                table,
                rowState,
                mappings,
                await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken),
                bulkCopyTimeout,
                batchSize,
                (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                connection.GetDbSetting(),
                transaction,
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
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportAsync(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            DbFieldCollection? dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
            var isPrimaryAnIdentity = IsPrimaryAnIdentity(dbFields);
            var includePrimary = isPrimaryAnIdentity == false || (isPrimaryAnIdentity && includeIdentity);

            // Mappings
            mappings = mappings?.Any() == true ? mappings :
                GetMappings(table,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    dbSetting);

            // Execution
            async Task<int> executeAsync()
            {
                var result = 0;
                var rows = GetRows(table, rowState).ToList();
                var batches = rows.Split(batchSize.GetValueOrDefault());

                foreach (var batch in batches)
                {
                    using (var importer = await GetNpgsqlBinaryImporterAsync(connection,
                        tableName ?? table?.TableName,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        dbSetting,
                        cancellationToken))
                    {
                        result += await BinaryImportAsync(importer,
                            batch,
                            mappings,
                            identityBehavior,
                            cancellationToken);
                    }
                }

                return result;
            }

            // Transactional
            return await TransactionalExecuteAsync(connection, executeAsync, transaction, cancellationToken);
        }

        #endregion

        #region BinaryImportAsync<DataReader>

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="reader">The instance of <see cref="DbDataReader"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not an entity mapping)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the <see cref="DbDataReader"/> will be kept during the operation.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been inserted into the target table.</returns>
        public static async Task<int> BinaryImportAsync(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            await BinaryImportAsync(connection,
                tableName,
                reader,
                mappings,
                await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken),
                bulkCopyTimeout,
                (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                connection.GetDbSetting(),
                transaction,
                cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportAsync(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            DbFieldCollection? dbFields = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
            var isPrimaryAnIdentity = IsPrimaryAnIdentity(dbFields);
            var includePrimary = isPrimaryAnIdentity == false || (isPrimaryAnIdentity && includeIdentity);

            // Mappings
            mappings = mappings?.Any() == true ? mappings :
                GetMappings(reader,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    dbSetting).ToList();

            // Execution
            async Task<int> execute()
            {
                var result = 0;

                using (var importer = await GetNpgsqlBinaryImporterAsync(connection,
                    tableName,
                    mappings,
                    bulkCopyTimeout,
                    identityBehavior,
                    dbSetting,
                    cancellationToken))
                {
                    result += await BinaryImportAsync(importer,
                        reader,
                        mappings,
                        identityBehavior,
                        cancellationToken);
                }

                return result;
            }

            // Transactional
            return await TransactionalExecuteAsync(connection, execute, transaction, cancellationToken);
        }

        #endregion

        #endregion
    }
}
