using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region WriteToServerInternal

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="hasOrderingColumn"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static int WriteToServerInternal<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null)
            where TEntity : class
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            int result;

            // Actual Execution
            using (var sqlBulkCopy = (SqlBulkCopy)Activator.CreateInstance(typeof(SqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the order column
                if (hasOrderingColumn)
                {
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                connection.EnsureOpen();
                using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction, hasOrderingColumn))
                {
                    var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", new[] { typeof(DbDataReader) });
                    writeToServerMethod(sqlBulkCopy, new[] { reader });
                    result = reader.RecordsAffected;
                }

                // Ensure the result
                if (result <= 0)
                {
                    // Set the return value
                    var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                        Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
                    result = (int)rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy);
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int WriteToServerInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction? transaction = null)
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            int result;

            // Actual Execution
            using (var sqlBulkCopy = (SqlBulkCopy)Activator.CreateInstance(typeof(SqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the mappings
                AddMappings(sqlBulkCopy, mappings);


                // Open the connection and do the operation
                connection.EnsureOpen();
                var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", new[] { typeof(DbDataReader) });
                writeToServerMethod(sqlBulkCopy, new[] { reader });

                // Set the return value
                var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                    Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
                result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : reader.RecordsAffected;
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
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
        private static int WriteToServerInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            SqlTransaction? transaction = null)
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            int result;

            // Actual Execution
            using (var sqlBulkCopy = (SqlBulkCopy)Activator.CreateInstance(typeof(SqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the order column
                if (hasOrderingColumn)
                {
                    AddOrderColumn(dataTable);
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                connection.EnsureOpen();
                if (rowState.HasValue == true)
                {
                    var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", new[] { typeof(DataTable), typeof(DataRowState) });
                    writeToServerMethod(sqlBulkCopy, new object[] { dataTable, rowState.Value });
                }
                else
                {
                    var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<SqlBulkCopy>("WriteToServer", new[] { typeof(DataTable) });
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
        private static async Task<int> WriteToServerAsyncInternal<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            int result;

            // Actual Execution
            using (var sqlBulkCopy = (SqlBulkCopy)Activator.CreateInstance(typeof(SqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the order column
                if (hasOrderingColumn)
                {
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                await connection.EnsureOpenAsync(cancellationToken: cancellationToken);
                using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction, hasOrderingColumn))
                {
                    var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DbDataReader), typeof(CancellationToken) });
                    await writeToServerMethod(sqlBulkCopy, new object[] { reader, cancellationToken });
                    result = reader.RecordsAffected;
                }

                // Ensure the result
                if (result <= 0)
                {
                    // Set the return value
                    var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                        Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
                    result = (int)rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy);
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
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
        private static async Task<int> WriteToServerAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            int result;

            // Actual Execution
            using (var sqlBulkCopy = (SqlBulkCopy)Activator.CreateInstance(typeof(SqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the mappings
                AddMappings(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                await connection.EnsureOpenAsync(cancellationToken);
                var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DbDataReader), typeof(CancellationToken) });
                await writeToServerMethod(sqlBulkCopy, new object[] { reader, cancellationToken });

                // Set the return value
                var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
                    Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
                result = rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy) ?? reader.RecordsAffected;
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
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
        private static async Task<int> WriteToServerAsyncInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool hasOrderingColumn = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Throw an error if there are no mappings
            if (mappings?.Any() != true)
            {
                throw new MissingMappingException("There are no mapping(s) found for this operation.");
            }

            // Variables needed
            int result;

            // Actual Execution
            using (var sqlBulkCopy = (SqlBulkCopy)Activator.CreateInstance(typeof(SqlBulkCopy), connection, options, transaction))
            {
                // Set the destinationtable
                Compiler.SetProperty(sqlBulkCopy, "DestinationTableName", tableName);

                // Set the timeout
                if (bulkCopyTimeout.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                }

                // Set the batch size
                if (batchSize.HasValue)
                {
                    Compiler.SetProperty(sqlBulkCopy, "BatchSize", batchSize.Value);
                }

                // Add the order column
                if (hasOrderingColumn)
                {
                    AddOrderColumn(dataTable);
                    mappings = AddOrderColumnMapping(mappings);
                }

                // Add the mappings
                AddMappings(sqlBulkCopy, mappings);

                // Open the connection and do the operation
                await connection.EnsureOpenAsync(cancellationToken);
                if (rowState.HasValue == true)
                {
                    var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DataTable), typeof(DataRowState), typeof(CancellationToken) });
                    await writeToServerMethod(sqlBulkCopy, new object[] { dataTable, rowState.Value, cancellationToken });
                }
                else
                {
                    var writeToServerMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DataTable), typeof(CancellationToken) });
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
