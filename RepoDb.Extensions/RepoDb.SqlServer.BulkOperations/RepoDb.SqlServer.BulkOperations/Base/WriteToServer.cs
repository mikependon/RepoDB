using RepoDb.Exceptions;
using RepoDb.SqlServer.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region WriteToServerInternal

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
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int WriteToServerInternal<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
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
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            var result = default(int);

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int WriteToServerInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            TSqlTransaction transaction = null)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            var result = default(int);

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

                // Add the mappings
                AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);


                // Open the connection and do the operation
                connection.EnsureOpen();
                var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DbDataReader) });
                writeToServerMethod(sqlBulkCopy, new[] { reader });

                // Set the return value
                var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                    Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : reader.RecordsAffected;
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int WriteToServerInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            TSqlTransaction transaction = null)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            var result = default(int);

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
                    AddOrderColumn(dataTable);
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                connection.EnsureOpen();
                if (rowState.HasValue == true)
                {
                    var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DataTable), typeof(DataRowState) });
                    writeToServerMethod(sqlBulkCopy, new object[] { dataTable, rowState.Value });
                }
                else
                {
                    var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DataTable) });
                    writeToServerMethod(sqlBulkCopy, new[] { dataTable });
                }

                // Set the result
                result = rowState == null ? dataTable.Rows.Count : GetDataRows(dataTable, rowState).Count();
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
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> WriteToServerAsyncInternal<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
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
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            var result = default(int);

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
                await connection.EnsureOpenAsync(cancellationToken: cancellationToken);
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> WriteToServerAsyncInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            TSqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            var result = default(int);

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

                // Add the mappings
                AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                await connection.EnsureOpenAsync(cancellationToken);
                var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DbDataReader), typeof(CancellationToken) });
                await writeToServerMethod(sqlBulkCopy, new object[] { reader, cancellationToken });

                // Set the return value
                var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                    Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : reader.RecordsAffected;
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> WriteToServerAsyncInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            TSqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            var result = default(int);

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
                    AddOrderColumn(dataTable);
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                await connection.EnsureOpenAsync(cancellationToken);
                if (rowState.HasValue == true)
                {
                    var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DataTable), typeof(DataRowState), typeof(CancellationToken) });
                    await writeToServerMethod(sqlBulkCopy, new object[] { dataTable, rowState.Value, cancellationToken });
                }
                else
                {
                    var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DataTable), typeof(CancellationToken) });
                    await writeToServerMethod(sqlBulkCopy, new object[] { dataTable, cancellationToken });
                }

                // Set the result
                result = rowState == null ? dataTable.Rows.Count : GetDataRows(dataTable, rowState).Count();
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
