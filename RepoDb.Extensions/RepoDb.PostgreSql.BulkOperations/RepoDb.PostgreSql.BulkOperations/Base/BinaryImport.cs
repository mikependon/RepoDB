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
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="entities">The list of entities to be bulk-inserted to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together.</param>
        /// <param name="keepIdentity">A value that indicates whether the newly generated identity value from the target table will be set back to the entity.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int BinaryImport<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
            where TEntity : class =>
            BinaryImport<TEntity>(connection,
                tableName,
                entities,
                mappings,
                DbFieldCache.Get(connection, tableName, transaction),
                bulkCopyTimeout,
                batchSize,
                (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                connection.GetDbSetting(),
                transaction);

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
            IEnumerable<DbField> dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            // Variables
            var result = default(int);
            var hasTransaction = transaction != null;

            // Open
            connection.EnsureOpen();

            // Ensure transaction
            if (hasTransaction == false)
            {
                transaction = connection.BeginTransaction();
            }

            try
            {
                // Variables
                var entityType = (entities?.First()?.GetType() ?? typeof(TEntity)); // Solving the anonymous types
                var isDictionary = entityType.IsDictionaryStringObject();
                var properties = PropertyCache.Get(entityType);

                // Mappings (Dictionary<string, object>)
                mappings = mappings?.Any() == true ? mappings :
                    isDictionary ?
                    GetMappings(entities?.First() as IDictionary<string, object>, dbFields, dbSetting) :
                    GetMappings(dbFields,
                        properties,
                        (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                        dbSetting);

                // Each batch requires an importer
                var batches = entities.Split(batchSize.GetValueOrDefault());
                foreach (var batch in batches)
                {
                    // Actual Execution
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
                                entities?.Select(entity => entity as IDictionary<string, object>),
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

                // Commit
                if (hasTransaction == false)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    transaction.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    transaction.Dispose();
                }
            }

            // Return
            return result;
        }

        #endregion

        #region BinaryImport<DataTable>

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-inserted. If not specified, all the rows of the table will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together.</param>
        /// <param name="keepIdentity">A value that indicates whether the newly generated identity value from the target table will be set back to the entity.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int BinaryImport(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null) =>
            BinaryImport(connection,
                tableName,
                table,
                rowState,
                mappings,
                DbFieldCache.Get(connection, tableName, transaction),
                bulkCopyTimeout,
                batchSize,
                (keepIdentity ? BulkImportIdentityBehavior.KeepIdentity : default),
                connection.GetDbSetting(),
                transaction);

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
            IEnumerable<DbField> dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
        {
            // Variables
            var result = default(int);
            var hasTransaction = transaction != null;

            // Open
            connection.EnsureOpen();

            // Ensure transaction
            if (hasTransaction == false)
            {
                transaction = connection.BeginTransaction();
            }

            try
            {
                // Mappings (DataTable)
                mappings = mappings?.Any() == true ? mappings :
                    GetMappings(table, dbFields, dbSetting).ToList();

                // Rows
                var rows = GetRows(table, rowState).ToList();

                // Each batch requires an importer
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

                // Commit
                if (hasTransaction == false)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    transaction.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    transaction.Dispose();
                }
            }

            // Return
            return result;
        }

        #endregion

        #region BinaryImport<DataReader>

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="reader">The instance of <see cref="DbDataReader"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="keepIdentity">A value that indicates whether the newly generated identity value from the target table will be set back to the entity.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
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
            IEnumerable<DbField> dbFields = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
        {
            // Variables
            var result = default(int);
            var hasTransaction = transaction != null;

            // Open
            connection.EnsureOpen();

            // Ensure transaction
            if (hasTransaction == false)
            {
                transaction = connection.BeginTransaction();
            }

            try
            {
                // Mappings (DataTable)
                mappings = mappings?.Any() == true ? mappings :
                    GetMappings(reader, dbFields, dbSetting).ToList();

                // BinaryImport
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

                // Commit
                if (hasTransaction == false)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    transaction.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    transaction.Dispose();
                }
            }

            // Return
            return result;
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
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="entities">The list of entities to be bulk-inserted to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together.</param>
        /// <param name="keepIdentity">A value that indicates whether the newly generated identity value from the target table will be set back to the entity.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static async Task<int> BinaryImportAsync<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await BinaryImportAsync<TEntity>(connection,
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
            IEnumerable<DbField> dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables
            var result = default(int);
            var hasTransaction = transaction != null;

            // Open
            await connection.EnsureOpenAsync(cancellationToken);

            // Ensure transaction
            if (hasTransaction == false)
            {
#if NET5_0 
                transaction = await connection.BeginTransactionAsync(cancellationToken);
#else
                transaction = connection.BeginTransaction();
#endif
            }

            try
            {
                // Variables
                var entityType = (entities?.First()?.GetType() ?? typeof(TEntity)); // Solving the anonymous types
                var isDictionary = entityType.IsDictionaryStringObject();
                var properties = PropertyCache.Get(entityType);

                // Mappings (Dictionary<string, object>)
                mappings = mappings?.Any() == true ? mappings :
                    isDictionary ?
                    GetMappings(entities?.First() as IDictionary<string, object>, dbFields, dbSetting) :
                    GetMappings(dbFields,
                        properties,
                        (identityBehavior == BulkImportIdentityBehavior.KeepIdentity),
                        dbSetting);

                // Each batch requires an importer
                var batches = entities.Split(batchSize.GetValueOrDefault());
                foreach (var batch in batches)
                {
                    // Actual Execution
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
                                entities?.Select(entity => entity as IDictionary<string, object>),
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

                // Commit
                if (hasTransaction == false)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    await transaction.DisposeAsync();
                }
            }

            // Return
            return result;
        }

        #endregion

        #region BinaryImportAsync<DataTable>

        /// <summary>
        /// Inserts the rows of the <see cref="DataTable"/> into the target table by bulk in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-inserted to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-inserted. If not specified, all the rows of the table will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together.</param>
        /// <param name="keepIdentity">A value that indicates whether the newly generated identity value from the target table will be set back to the entity.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static async Task<int> BinaryImportAsync(this NpgsqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            await BinaryImportAsync(connection,
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
            IEnumerable<DbField> dbFields = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var result = default(int);
            var hasTransaction = transaction != null;

            // Open
            await connection.EnsureOpenAsync(cancellationToken);

            // Ensure transaction
            if (hasTransaction == false)
            {
#if NET5_0 
                transaction = await connection.BeginTransactionAsync(cancellationToken);
#else
                transaction = connection.BeginTransaction();
#endif
            }

            try
            {
                // Mappings (DataTable)
                mappings = mappings?.Any() == true ? mappings :
                    GetMappings(table, dbFields, dbSetting).ToList();

                // Rows
                var rows = GetRows(table, rowState).ToList();

                // Each batch requires an importer
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

                // Commit
                if (hasTransaction == false)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    await transaction.DisposeAsync();
                }
            }

            // Return
            return result;
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
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="keepIdentity">A value that indicates whether the newly generated identity value from the target table will be set back to the entity.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
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
            IEnumerable<DbField> dbFields = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var result = default(int);
            var hasTransaction = transaction != null;

            // Open
            await connection.EnsureOpenAsync(cancellationToken);

            // Ensure transaction
            if (hasTransaction == false)
            {
#if NET5_0 
                transaction = await connection.BeginTransactionAsync(cancellationToken);
#else
                transaction = connection.BeginTransaction();
#endif
            }

            try
            {
                // Mappings (DataTable)
                mappings = mappings?.Any() == true ? mappings :
                    GetMappings(reader, dbFields, dbSetting).ToList();

                // BinaryImport
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

                // Commit
                if (hasTransaction == false)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    await transaction.DisposeAsync();
                }
            }

            // Return
            return result;
        }

        #endregion

        #endregion
    }
}
