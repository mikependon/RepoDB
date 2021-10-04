using Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region BinaryImport<TEntity>

        /// <summary>
        /// Inserts a list of entities into the target table by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The target table.</param>
        /// <param name="entities">The list of entities.</param>
        /// <param name="mappings">The list of mappings.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation.</param>
        /// <param name="batchSize">The size per batch to be sent to the database.</param>
        /// <param name="transaction">The current transaction object in used. If not provided, an implicit transaction will be created.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int BinaryImport<TEntity>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
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

                // ExpandoObject/Dictionary
                if (isDictionary && mappings.Any() != true)
                {
                    mappings = GetMappingsFromDictionary(entities?.First() as IDictionary<string, object>);
                }

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
                        transaction))
                    {
                        if (isDictionary)
                        {
                            result += BinaryImportDictionary(importer,
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

        #region BinaryImportAsync<TEntity>

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappingsFromDictionary(IDictionary<string, object> dictionary) =>
            dictionary?.Keys.Select(key => new NpgsqlBulkInsertMapItem(key, key, dictionary[key]?.GetType()));

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
            NpgsqlTransaction transaction = null)
        {
            var copyCommand = GetBinaryImportCopyCommand(connection, tableName, entityType, mappings, transaction);
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
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryImport<TEntity>(NpgsqlConnection connection,
            NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
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
                var dbFields = DbFieldCache.Get(connection, tableName ?? ClassMappedNameCache.Get(entityType), transaction);
                var properties = PropertyCache.Get(entityType);

                return BinaryImport<TEntity>(importer,
                    tableName,
                    entities,
                    dbFields,
                    properties,
                    entityType,
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
            var result = 0;

            foreach (var entity in entities)
            {
                importer.StartRow();
                func(importer, entity);
                result++;
            }

            importer.Complete();
            return result;
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
        /// <param name="dbSetting"></param>
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            Type entityType,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteFunc<TEntity>(tableName,
                dbFields,
                properties,
                entityType,
                dbSetting);
            var result = 0;

            foreach (var entity in entities)
            {
                importer.StartRow();
                func(importer, entity);
                result++;
            }

            importer.Complete();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        private static int BinaryImportDictionary(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var result = 0;

            foreach (var entity in entities)
            {
                importer.StartRow();

                foreach (var mapping in mappings)
                {
                    var data = entity[mapping.SourceColumn];
                    importer.Write(data);
                }

                result++;
            }

            importer.Complete();
            return result;
        }

        #endregion
    }
}
