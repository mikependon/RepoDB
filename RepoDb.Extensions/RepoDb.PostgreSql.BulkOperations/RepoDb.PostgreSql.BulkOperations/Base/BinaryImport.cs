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

            // Open
            connection.EnsureOpen();

            // Actual Execution
            using (var importer = GetBinaryImporter<TEntity>(connection,
                tableName,
                ref mappings,
                bulkCopyTimeout,
                hasOrderingColumn,
                transaction))
            {
                var batches = entities.Split(batchSize.GetValueOrDefault());
                if (batches?.Any() == true)
                {
                    foreach (var batch in batches)
                    {
                        result += BinaryImport(importer, batch);
                    }
                }
                else
                {
                    result = BinaryImport(importer, entities);
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
        private static NpgsqlBinaryImporter GetBinaryImporter<TEntity>(NpgsqlConnection connection,
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
                mappings = AddOrderColumnMapping(mappings);
            }

            // Return
            return importer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            IEnumerable<TEntity> entities)
            where TEntity : class
        {
            importer.Complete();
            return entities.Count();
        }

        #endregion
    }
}
