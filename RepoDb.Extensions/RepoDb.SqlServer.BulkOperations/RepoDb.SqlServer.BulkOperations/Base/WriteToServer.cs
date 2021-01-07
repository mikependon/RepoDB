using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region WriteToServer

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int WriteToServer<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            TSqlTransaction transaction = null)
            where TEntity : class
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            var result = default(int);

            // Mappings
            if (mappings?.Any() != true)
            {
                // Ensure the DB Fields
                dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = entityType.IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities?.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data entity)
                if (entityFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var field = entityFields?.FirstOrDefault(ef =>
                            string.Equals(ef.Name, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(field.Name ?? f.Name, f.Name);
                    });
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }
            }

            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Actual Execution
            using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the order column
                if (hasOrderingColumn)
                {
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                connection.EnsureOpen();
                using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction, hasOrderingColumn))
                {
                    var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DbDataReader) });
                    writeToServerMethod(sqlBulkCopy, new[] { reader });
                    result = reader.RecordsAffected;
                }

                // Ensure the result
                if (result <= 0)
                {
                    // Set the return value
                    var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                        Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                    result = (int)rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy);
                }
            }

            // Return the result
            return result;
        }

        #endregion

        #region WriteToServerAsync

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> WriteToServerAsync<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            TSqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            var result = default(int);

            // Mappings
            if (mappings?.Any() != true)
            {
                // Ensure the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = entityType.IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities?.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data entity)
                if (entityFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var field = entityFields?.FirstOrDefault(ef =>
                            string.Equals(ef.Name, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(field.Name ?? f.Name, f.Name);
                    });
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }
            }

            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Actual Execution
            using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the order column
                if (hasOrderingColumn)
                {
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                await connection.EnsureOpenAsync();
                using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction, hasOrderingColumn))
                {
                    var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DbDataReader), typeof(CancellationToken) });
                    await writeToServerMethod(sqlBulkCopy, new object[] { reader, cancellationToken });
                    result = reader.RecordsAffected;
                }

                // Ensure the result
                if (result <= 0)
                {
                    // Set the return value
                    var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                        Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                    result = (int)rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy);
                }
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
