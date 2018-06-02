using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// An inherritable base object for all <b>Entity-Based Repositories</b>. This object is usually being inheritted
    /// if the derived class is meant for entity-based operations with corresponding <i>Data Transfer Object (DTO)</i>
    /// for data manipulations.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of <i>Data Transfer Object (DTO)</i> object to be mapped on this repository. This object must inherit
    /// the <i>RepoDb.DataEntity</i> object in order to be qualified as a repository entity.
    /// </typeparam>
    /// <typeparam name="TDbConnection">The type of the <i>System.Data.Common.DbConnection</i> object.</typeparam>
    public abstract class BaseRepository<TEntity, TDbConnection> : IBaseRepository<TEntity, TDbConnection>
        where TEntity : DataEntity
        where TDbConnection : DbConnection
    {
        private readonly DbRepository<TDbConnection> _dbRepository;

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        public BaseRepository(string connectionString)
            : this(connectionString, null, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations of this repository.</param>
        public BaseRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation of this repository.</param>
        /// <param name="cache">The </i>cache object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.Cache</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation of this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.ITrace</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation of this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.ITrace</i> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.ITrace</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder)
        {
            // Fields
            Cache = (cache ?? new MemoryCache());
            Trace = trace;
            StatementBuilder = (statementBuilder ??
                StatementBuilderMapper.Get(typeof(TDbConnection))?.StatementBuilder ??
                new SqlDbStatementBuilder());

            // Repository
            _dbRepository = new DbRepository<TDbConnection>(connectionString, commandTimeout,
                Cache, Trace, StatementBuilder);
        }

        // CreateConnection

        public TDbConnection CreateConnection()
        {
            return DbRepository.CreateConnection();
        }

        // DbRepository

        public IDbRepository<TDbConnection> DbRepository => _dbRepository;

        // DbCache

        public ICache Cache { get; }

        // Trace

        public ITrace Trace { get; }

        // StatementBuilder

        public IStatementBuilder StatementBuilder { get; }

        // Count

        public int Count(IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(transaction: transaction);
        }

        public int Count(object where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        public int Count(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        public int Count(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        // CountAsync

        public Task<int> CountAsync(IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(transaction: transaction);
        }

        public Task<int> CountAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<int> CountAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<int> CountAsync(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // CountBig

        public long CountBig(IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(transaction: transaction);
        }

        public long CountBig(object where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(where: where,
                transaction: transaction);
        }

        public long CountBig(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(where: where,
                transaction: transaction);
        }

        public long CountBig(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(where: where,
                transaction: transaction);
        }

        // CountBigAsync

        public Task<long> CountBigAsync(IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(transaction: transaction);
        }

        public Task<long> CountBigAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<long> CountBigAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<long> CountBigAsync(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // BatchQuery

        public IEnumerable<TEntity> BatchQuery(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public IEnumerable<TEntity> BatchQuery(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public IEnumerable<TEntity> BatchQuery(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public IEnumerable<TEntity> BatchQuery(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        // BatchQueryAsync

        public Task<IEnumerable<TEntity>> BatchQueryAsync(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public Task<IEnumerable<TEntity>> BatchQueryAsync(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public Task<IEnumerable<TEntity>> BatchQueryAsync(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public Task<IEnumerable<TEntity>> BatchQueryAsync(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        // Query

        public IEnumerable<TEntity> Query(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        // Query

        public Task<IEnumerable<TEntity>> QueryAsync(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        // Insert

        public object Insert(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity>(entity: entity,
                transaction: transaction);
        }

        // InsertAsync

        public Task<object> InsertAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        // InlineUpdate

        public int InlineUpdate(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        public int InlineUpdate(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        public int InlineUpdate(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // InlineUpdateAsync

        public Task<int> InlineUpdateAsync(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        public Task<int> InlineUpdateAsync(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        public Task<int> InlineUpdateAsync(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // Update

        public int Update(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                transaction: transaction);
        }

        public int Update(TEntity entity, object where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        public int Update(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        public int Update(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        // UpdateAsync

        public Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        public Task<int> UpdateAsync(TEntity entity, object where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        public Task<int> UpdateAsync(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        public Task<int> UpdateAsync(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        // Delete

        public int Delete(object where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        public int Delete(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        public int Delete(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        // DeleteAsync

        public Task<int> DeleteAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<int> DeleteAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<int> DeleteAsync(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // Merge
        public int Merge(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                transaction: transaction);
        }

        public int Merge(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        // Merge
        public Task<int> MergeAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        public Task<int> MergeAsync(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        // BulkInsert
        public int BulkInsert(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
        {
            return DbRepository.BulkInsert<TEntity>(entities: entities,
                transaction: transaction);
        }

        // BulkInsertAsync
        public Task<int> BulkInsertAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
        {
            return DbRepository.BulkInsertAsync<TEntity>(entities: entities,
                transaction: transaction);
        }

        // ExecuteQuery
        public IEnumerable<TEntity> ExecuteQuery(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQuery<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteQueryAsync
        public Task<IEnumerable<TEntity>> ExecuteQueryAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQueryAsync<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteNonQuery
        public int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null, int?
            commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteNonQueryAsync

        public Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null, int?
            commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteNonQueryAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteScalar

        public object ExecuteScalar(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteScalarAsync

        public Task<object> ExecuteScalarAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalarAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }
    }
}