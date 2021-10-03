using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region BinaryImport

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
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int BinaryImport<TEntity>(NpgsqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
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
                // Each batch requires an importer
                var batches = entities.Split(batchSize.GetValueOrDefault());
                foreach (var batch in batches)
                {
                    // Actual Execution
                    using (var importer = GetNpgsqlBinaryImporter<TEntity>(connection,
                        tableName,
                        ref mappings,
                        bulkCopyTimeout,
                        hasOrderingColumn,
                        transaction))
                    {
                        result += BinaryImport(connection, importer, tableName, batch, mappings, transaction);
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
                // Commit
                if (hasTransaction == false)
                {
                    transaction.Rollback();
                }
            }

            // Return
            return result;
        }

        #endregion

        #region WriteToServerAsync

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static NpgsqlBinaryImporter GetNpgsqlBinaryImporter<TEntity>(NpgsqlConnection connection,
            string tableName,
            ref IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            int? bulkCopyTimeout = null,
            bool hasOrderingColumn = false,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            var copyCommand = GetBinaryImportCopyCommand<TEntity>(connection, tableName, mappings, transaction);
            var importer = connection.BeginBinaryImport(copyCommand);

            // Timeout
            if (bulkCopyTimeout.HasValue)
            {
                importer.Timeout = TimeSpan.FromSeconds(bulkCopyTimeout.Value);
            }

            // Order column
            if (hasOrderingColumn)
            {
                //mappings = AddOrderColumnMapping(mappings);
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
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryImport<TEntity>(NpgsqlConnection connection,
            NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (mappings?.Any() == true)
            {
                return BinaryImport(importer, tableName, entities, mappings);
            }
            else
            {
                var dbFields = DbFieldCache.Get(connection, tableName ?? ClassMappedNameCache.Get(entityType), transaction);
                var fields = FieldCache.Get(entityType);
                return BinaryImport(importer, tableName, entities, dbFields, fields);
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
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteFunc<TEntity>(tableName, mappings);
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
        /// <param name="fields"></param>
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<Field> fields)
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
