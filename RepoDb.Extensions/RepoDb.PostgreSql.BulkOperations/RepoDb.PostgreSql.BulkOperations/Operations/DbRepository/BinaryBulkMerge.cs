using Npgsql;
using RepoDb.Enumerations;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.PostgreSql.BulkOperations;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="DbRepository{TDbConnection}"/> object.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        #region BinaryBulkMerge<TEntity>

        /// <summary>
        /// Merges a list of entities into the target table by bulk. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsert' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="entities">The list of entities to be bulk-merged to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static int BinaryBulkMerge<TEntity>(this DbRepository<NpgsqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return connection.BinaryBulkMerge<TEntity>(tableName: ClassMappedNameCache.Get<TEntity>(),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Merges a list of entities into the target table by bulk. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsert' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="entities">The list of entities to be bulk-merged to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static int BinaryBulkMerge<TEntity>(this DbRepository<NpgsqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return connection.BinaryBulkMerge<TEntity>(tableName: (tableName ?? ClassMappedNameCache.Get<TEntity>()),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region BinaryBulkMerge<DataTable>

        /// <summary>
        /// Merges the rows of the <see cref="DataTable"/> into the target table by bulk. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsert' extended method.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-merged to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-merged. If not specified, all the rows of the table will be used.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static int BinaryBulkMerge(this DbRepository<NpgsqlConnection> repository,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return connection.BinaryBulkMerge(tableName: table?.TableName,
                    table: table,
                    rowState: rowState,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Merges the rows of the <see cref="DataTable"/> into the target table by bulk. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsert' extended method.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database. If not specified, the <see cref="DataTable.TableName"/> property will be used.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-merged to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-merged. If not specified, all the rows of the table will be used.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static int BinaryBulkMerge(this DbRepository<NpgsqlConnection> repository,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return connection.BinaryBulkMerge(tableName: (tableName ?? table?.TableName),
                    table: table,
                    rowState: rowState,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region BinaryBulkMerge<DbDataReader>

        /// <summary>
        /// Merges the rows of the <see cref="DbDataReader"/> into the target table by bulk. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsert' extended method.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="reader">The instance of <see cref="DbDataReader"/> object that contains the rows to be bulk-merged to the target table.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static int BinaryBulkMerge(this DbRepository<NpgsqlConnection> repository,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return connection.BinaryBulkMerge(tableName: tableName,
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Async

        #region BinaryBulkMerge<TEntity>

        /// <summary>
        /// Merges a list of entities into the target table by bulk in an asynchronous way. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsertAsync' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="entities">The list of entities to be bulk-merged to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static async Task<int> BinaryBulkMergeAsync<TEntity>(this DbRepository<NpgsqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return await connection.BinaryBulkMergeAsync<TEntity>(tableName: ClassMappedNameCache.Get<TEntity>(),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Merges a list of entities into the target table by bulk in an asynchronous way. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsertAsync' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="entities">The list of entities to be bulk-merged to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static async Task<int> BinaryBulkMergeAsync<TEntity>(this DbRepository<NpgsqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return await connection.BinaryBulkMergeAsync<TEntity>(tableName: (tableName ?? ClassMappedNameCache.Get<TEntity>()),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region BinaryBulkMerge<DataTable>

        /// <summary>
        /// Merges the rows of the <see cref="DataTable"/> into the target table by bulk in an asynchronous way. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsertAsync' extended method.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-merged to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-merged. If not specified, all the rows of the table will be used.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static async Task<int> BinaryBulkMergeAsync(this DbRepository<NpgsqlConnection> repository,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return await connection.BinaryBulkMergeAsync(tableName: table?.TableName,
                    table: table,
                    rowState: rowState,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Merges the rows of the <see cref="DataTable"/> into the target table by bulk in an asynchronous way. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsertAsync' extended method.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database. If not specified, the <see cref="DataTable.TableName"/> property will be used.</param>
        /// <param name="table">The source <see cref="DataTable"/> object that contains the rows to be bulk-merged to the target table.</param>
        /// <param name="rowState">The state of the rows to be bulk-merged. If not specified, all the rows of the table will be used.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static async Task<int> BinaryBulkMergeAsync(this DbRepository<NpgsqlConnection> repository,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return await connection.BinaryBulkMergeAsync(tableName: (tableName ?? table?.TableName),
                    table: table,
                    rowState: rowState,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region BinaryBulkMerge<DbDataReader>

        /// <summary>
        /// Merges the rows of the <see cref="DbDataReader"/> into the target table by bulk in an asynchronous way. This operation is inserting a row (if not present), and updating an existing 
        /// row (if present), based on the given qualifiers. It uses either of the 'INSERT/UPDATE' and 'ON CONFLICT DO UPDATE' commands of the 
        /// PostgreSQL based on the value passed to the '<paramref name="mergeCommandType"/>' argument. Underneath this operation is a call directly to 
        /// the existing <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the customized 'BinaryBulkInsertAsync' extended method.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="reader">The instance of <see cref="DbDataReader"/> object that contains the rows to be bulk-merged to the target table.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="mergeCommandType">The value that defines the type of command to be used during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been merged into the target table.</returns>
        public static async Task<int> BinaryBulkMergeAsync(this DbRepository<NpgsqlConnection> repository,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportMergeCommandType mergeCommandType = default,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? repository.CreateConnection());

            try
            {
                // Call the method
                return await connection.BinaryBulkMergeAsync(tableName: tableName,
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    identityBehavior: identityBehavior,
                    mergeCommandType: mergeCommandType,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                if (repository.ConnectionPersistency == ConnectionPersistency.PerCall)
                {
                    if (transaction == null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
