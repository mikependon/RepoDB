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
            : this(connectionString, null)
        {
        }

        public BaseRepository(string connectionString, int? commandTimeout)
        {
            _dbRepository = new DbRepository<TDbConnection>(connectionString, commandTimeout);
        }

        public TDbConnection CreateConnection()
        {
            return DbRepository.CreateConnection();
        }

        public IDbRepository<TDbConnection> DbRepository => _dbRepository;

        // Query

        public IEnumerable<TEntity> Query(IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(transaction: transaction);
        }

        public IEnumerable<TEntity> Query(object where, IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction);
        }

        public IEnumerable<TEntity> Query(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction);
        }

        public IEnumerable<TEntity> Query(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction);
        }

        // Query

        public Task<IEnumerable<TEntity>> QueryAsync(IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(transaction: transaction);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction);
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

        // Update

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