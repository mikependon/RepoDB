using Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
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
        /// <param name="keepIdentity">A value that indicates whether the identity property/column will be kept and used.</param>
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
                // Solving the anonymous types
                var entityType = (entities?.First()?.GetType() ?? typeof(TEntity));
                var isDictionary = entityType.IsDictionaryStringObject();

                // Mappings (Dictionary<string, object>)
                mappings = isDictionary && mappings?.Any() != true ?
                    GetMappings(entities?.First() as IDictionary<string, object>) : mappings;

                // Each batch requires an importer
                var batches = entities.Split(batchSize.GetValueOrDefault());
                foreach (var batch in batches)
                {
                    // Actual Execution
                    using (var importer = GetNpgsqlBinaryImporter(connection,
                        tableName,
                        mappings,
                        entityType,
                        bulkCopyTimeout,
                        keepIdentity,
                        transaction))
                    {
                        if (isDictionary)
                        {
                            result += BinaryImportExplicit(importer,
                                entities?.Select(entity => entity as IDictionary<string, object>),
                                mappings);
                        }
                        else
                        {
                            result += BinaryImport<TEntity>(connection,
                                importer,
                                tableName,
                                batch,
                                mappings,
                                entityType,
                                keepIdentity,
                                transaction);
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
        /// <param name="keepIdentity">A value that indicates whether the identity property/column will be kept and used.</param>
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
                mappings = mappings?.Any() != true ? GetMappings(table).ToList() : mappings;

                // Rows
                var rows = GetRows(table, rowState).ToList();

                // Each batch requires an importer
                var batches = rows.Split(batchSize.GetValueOrDefault());
                foreach (var batch in batches)
                {
                    using (var importer = GetNpgsqlBinaryImporter(connection,
                        tableName ?? table?.TableName,
                        mappings,
                        null,
                        bulkCopyTimeout,
                        keepIdentity,
                        transaction))
                    {
                        result += BinaryImportExplicit(importer,
                            batch,
                            mappings);
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
        /// <param name="keepIdentity">A value that indicates whether the identity property/column will be kept and used.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int BinaryImport(this NpgsqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = false,
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
                mappings = mappings?.Any() != true ? GetMappings(reader).ToList() : mappings;

                // BinaryImport
                using (var importer = GetNpgsqlBinaryImporter(connection,
                    tableName,
                    mappings,
                    null,
                    bulkCopyTimeout,
                    keepIdentity,
                    transaction))
                {
                    result += BinaryImportExplicit(importer,
                        reader,
                        mappings);
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
        /// <param name="keepIdentity">A value that indicates whether the identity property/column will be kept and used.</param>
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
                // Solving the anonymous types
                var entityType = (entities?.First()?.GetType() ?? typeof(TEntity));
                var isDictionary = entityType.IsDictionaryStringObject();

                // Mappings (Dictionary<string, object>)
                mappings = isDictionary && mappings?.Any() != true ?
                    GetMappings(entities?.First() as IDictionary<string, object>) : mappings;

                // Each batch requires an importer
                var batches = entities.Split(batchSize.GetValueOrDefault());
                foreach (var batch in batches)
                {
                    // Actual Execution
                    using (var importer = await GetNpgsqlBinaryImporterAsync(connection,
                        tableName,
                        mappings,
                        entityType,
                        bulkCopyTimeout,
                        keepIdentity,
                        transaction))
                    {
                        if (isDictionary)
                        {
                            result += await BinaryImportExplicitAsync(importer,
                                entities?.Select(entity => entity as IDictionary<string, object>),
                                mappings,
                                cancellationToken);
                        }
                        else
                        {
                            result += await BinaryImportAsync<TEntity>(connection,
                                importer,
                                tableName,
                                batch,
                                mappings,
                                entityType,
                                keepIdentity,
                                transaction,
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
        /// <param name="keepIdentity">A value that indicates whether the identity property/column will be kept and used.</param>
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
                mappings = mappings?.Any() != true ? GetMappings(table).ToList() : mappings;

                // Rows
                var rows = GetRows(table, rowState).ToList();

                // Each batch requires an importer
                var batches = rows.Split(batchSize.GetValueOrDefault());
                foreach (var batch in batches)
                {
                    using (var importer = await GetNpgsqlBinaryImporterAsync(connection,
                        tableName ?? table?.TableName,
                        mappings,
                        null,
                        bulkCopyTimeout,
                        keepIdentity,
                        transaction,
                        cancellationToken))
                    {
                        result += await BinaryImportExplicitAsync(importer,
                            batch,
                            mappings,
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
        /// <param name="keepIdentity">A value that indicates whether the identity property/column will be kept and used.</param>
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
                mappings = mappings?.Any() != true ? GetMappings(reader).ToList() : mappings;

                // BinaryImport
                using (var importer = await GetNpgsqlBinaryImporterAsync(connection,
                    tableName,
                    mappings,
                    null,
                    bulkCopyTimeout,
                    keepIdentity,
                    transaction,
                    cancellationToken))
                {
                    result += await BinaryImportExplicitAsync(importer,
                        reader,
                        mappings,
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

        #region Other Methods

        #region Sync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static NpgsqlBinaryImporter GetNpgsqlBinaryImporter(NpgsqlConnection connection,
            string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
        {
            var copyCommand = GetBinaryImportCopyCommand(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction);
            var importer = connection.BeginBinaryImport(copyCommand);

            // Timeout
            if (bulkCopyTimeout.HasValue)
            {
                importer.Timeout = TimeSpan.FromSeconds(bulkCopyTimeout.Value);
            }

            // Return
            return importer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryImport<TEntity>(NpgsqlConnection connection,
            NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            if (mappings?.Any() == true)
            {
                return BinaryImport<TEntity>(importer,
                    tableName,
                    entities,
                    mappings,
                    entityType);
            }
            else
            {
                var dbFields = DbFieldCache.Get(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction);
                var properties = PropertyCache.Get(entityType);

                return BinaryImport<TEntity>(importer,
                    tableName,
                    entities,
                    dbFields,
                    properties,
                    entityType,
                    keepIdentity,
                    connection.GetDbSetting());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteFunc<TEntity>(tableName,
                mappings,
                entityType);
            var enumerator = entities.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (entity) => func(importer, entity));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="dbSetting"></param>
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            Type entityType,
            bool keepIdentity,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteFunc<TEntity>(tableName,
                dbFields,
                properties,
                entityType,
                keepIdentity,
                dbSetting);
            var enumerator = entities.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (entity) => func(importer, entity));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        private static int BinaryImportExplicit(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var enumerator = dictionaries.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (dictionary) =>
                {
                    foreach (var mapping in mappings)
                    {
                        importer.Write(dictionary[mapping.SourceColumn]);
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        private static int BinaryImportExplicit(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var enumerator = rows.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (row) =>
                {
                    foreach (var mapping in mappings)
                    {
                        importer.Write(row[mapping.SourceColumn]);
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        private static int BinaryImportExplicit(NpgsqlBinaryImporter importer,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            return BinaryImportWrite(importer,
                () => reader.Read(),
                () => reader,
                (current) =>
                {
                    foreach (var mapping in mappings)
                    {
                        importer.Write(current[mapping.SourceColumn]);
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="moveNext"></param>
        /// <param name="getCurrent"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        private static int BinaryImportWrite<TEntity>(NpgsqlBinaryImporter importer,
            Func<bool> moveNext,
            Func<TEntity> getCurrent,
            Action<TEntity> write)
            where TEntity : class
        {
            var result = 0;

            while (moveNext())
            {
                importer.StartRow();
                write(getCurrent());
                result++;
            }

            importer.Complete();
            return result;
        }

        #endregion

        #region Async

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<NpgsqlBinaryImporter> GetNpgsqlBinaryImporterAsync(NpgsqlConnection connection,
            string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var copyCommand = await GetBinaryImportCopyCommandAsync(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction,
                cancellationToken);
#if NET6_0
            var importer = await connection.BeginBinaryImportAsync(copyCommand);
#else
            var importer = connection.BeginBinaryImport(copyCommand);
#endif

            // Timeout
            if (bulkCopyTimeout.HasValue)
            {
                importer.Timeout = TimeSpan.FromSeconds(bulkCopyTimeout.Value);
            }

            // Return
            return importer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportAsync<TEntity>(NpgsqlConnection connection,
            NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (mappings?.Any() == true)
            {
                return await BinaryImportAsync<TEntity>(importer,
                    tableName,
                    entities,
                    mappings,
                    entityType,
                    cancellationToken);
            }
            else
            {
                var dbFields = await DbFieldCache.GetAsync(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction,
                    cancellationToken);
                var properties = PropertyCache.Get(entityType);

                return await BinaryImportAsync<TEntity>(importer,
                    tableName,
                    entities,
                    dbFields,
                    properties,
                    entityType,
                    keepIdentity,
                    connection.GetDbSetting(),
                    cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportAsync<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(tableName,
                mappings,
                entityType);
            var enumerator = entities.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (entity) => await func(importer, entity, cancellationToken),
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportAsync<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            Type entityType,
            bool keepIdentity,
            IDbSetting dbSetting,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(tableName,
                dbFields,
                properties,
                entityType,
                keepIdentity,
                dbSetting);
            var enumerator = entities.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (entity) => await func(importer, entity, cancellationToken),
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            CancellationToken cancellationToken = default)
        {
            var enumerator = dictionaries.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (dictionary) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await importer.WriteAsync(dictionary[mapping.SourceColumn], cancellationToken);
                    }
                },
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            CancellationToken cancellationToken = default)
        {
            var enumerator = rows.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (row) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await importer.WriteAsync(row[mapping.SourceColumn], cancellationToken);
                    }
                },
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            CancellationToken cancellationToken = default)
        {
            return await BinaryImportWriteAsync(importer,
                async () => await reader.ReadAsync(cancellationToken),
                async () => await Task.FromResult(reader),
                async (current) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await importer.WriteAsync(current[mapping.SourceColumn], cancellationToken);
                    }
                },
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="moveNextAsync"></param>
        /// <param name="getCurrentAsync"></param>
        /// <param name="writeAsync"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportWriteAsync<TEntity>(NpgsqlBinaryImporter importer,
            Func<Task<bool>> moveNextAsync,
            Func<Task<TEntity>> getCurrentAsync,
            Func<TEntity, Task> writeAsync,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = 0;

            while (await moveNextAsync())
            {
                await importer.StartRowAsync(cancellationToken);
                await writeAsync(await getCurrentAsync());
                result++;
            }

            await importer.CompleteAsync(cancellationToken);
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IDictionary<string, object> dictionary) =>
            dictionary?.Keys.Select(key => new NpgsqlBulkInsertMapItem(key, key, dictionary[key]?.GetType().GetUnderlyingType()));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                yield return new NpgsqlBulkInsertMapItem(column.ColumnName, column.ColumnName, column.DataType.GetUnderlyingType());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                yield return new NpgsqlBulkInsertMapItem(name, name, reader.GetFieldType(i).GetUnderlyingType());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        private static IEnumerable<DataRow> GetRows(DataTable table,
            DataRowState? rowState)
        {
            foreach (DataRow row in table.Rows)
            {
                if (rowState == null || row.RowState == rowState)
                {
                    yield return row;
                }
            }
        }

        #endregion

        #endregion
    }
}
