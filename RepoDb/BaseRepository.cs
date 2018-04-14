using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract class BaseRepository<TEntity, TDbConnection> : IBaseRepository<TEntity, TDbConnection>
        where TEntity : DataEntity
        where TDbConnection : DbConnection
    {
        private readonly DbRepository<TDbConnection> _dbRepository;

        public BaseRepository(string connectionString)
            : this(connectionString, null, null, null, null)
        {
        }

        public BaseRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null)
        {
        }

        public BaseRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null)
        {
        }

        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null)
        {
        }

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

        // ExecuteReader
        public IEnumerable<TEntity> ExecuteReader(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteReader<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteReaderAsync
        public Task<IEnumerable<TEntity>> ExecuteReaderAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteReaderAsync<TEntity>(commandText: commandText,
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