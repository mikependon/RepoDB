using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.PostgreSql.BulkOperations;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        #region BinaryBulkUpdate<TEntity>

        /// <summary>
        /// Update the existing rows via entities by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImport' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of entities to be bulk-updated to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been updated into the target table.</returns>
        public static int BinaryBulkUpdate<TEntity>(this BaseRepository<TEntity, NpgsqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BinaryBulkUpdate<TEntity>(tableName: ClassMappedNameCache.Get<TEntity>(),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    keepIdentity: keepIdentity,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction);

        /// <summary>
        /// Update the existing rows via entities by bulk. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImport' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="entities">The list of entities to be bulk-updated to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been updated into the target table.</returns>
        public static int BinaryBulkUpdate<TEntity>(this BaseRepository<TEntity, NpgsqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BinaryBulkUpdate<TEntity>(tableName: (tableName ?? ClassMappedNameCache.Get<TEntity>()),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    keepIdentity: keepIdentity,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction);

        #endregion

        #endregion

        #region Async

        #region BinaryBulkUpdate<TEntity>

        /// <summary>
        /// Update the existing rows via entities by bulk in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImport' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of entities to be bulk-updated to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the entities will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been updated into the target table.</returns>
        public static async Task<int> BinaryBulkUpdateAsync<TEntity>(this BaseRepository<TEntity, NpgsqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.DbRepository.BinaryBulkUpdateAsync<TEntity>(tableName: ClassMappedNameCache.Get<TEntity>(),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    keepIdentity: keepIdentity,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction,
                    cancellationToken: cancellationToken);

        /// <summary>
        /// Update the existing rows via entities by bulk in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImport' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="entities">The list of entities to be bulk-updated to the target table.
        /// This can be an <see cref="IEnumerable{T}"/> of the following objects (<typeparamref name="TEntity"/> (as class/model), <see cref="ExpandoObject"/>,
        /// <see cref="IDictionary{TKey, TValue}"/> (of <see cref="string"/>/<see cref="object"/>) and Anonymous Types).</param>
        /// <param name="qualifiers">The list of qualifier fields to be used during the operation. Ensure to target the indexed columns to make the execution more performant. If not specified, the primary key will be used.</param>
        /// <param name="mappings">The list of mappings to be used. If not specified, only the matching properties/columns from the target table will be used. (This is not the entity mappings, but is working on top of it)</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the rows of the table will be sent together in one-go.</param>
        /// <param name="keepIdentity">A value that indicates whether the existing identity property values from the entities will be kept during the operation.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been updated into the target table.</returns>
        public static async Task<int> BinaryBulkUpdateAsync<TEntity>(this BaseRepository<TEntity, NpgsqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.DbRepository.BinaryBulkUpdateAsync<TEntity>(tableName: (tableName ?? ClassMappedNameCache.Get<TEntity>()),
                    entities: entities,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    keepIdentity: keepIdentity,
                    pseudoTableType: pseudoTableType,
                    transaction: transaction,
                    cancellationToken: cancellationToken);

        #endregion

        #endregion
    }
}
