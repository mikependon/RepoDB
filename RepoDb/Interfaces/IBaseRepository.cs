using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    public interface IBaseRepository<TEntity, TDbConnection>
        where TEntity : DataEntity
        where TDbConnection : DbConnection
    {
        // CreateConnection

        TDbConnection CreateConnection();

        // DbRepository

        IDbRepository<TDbConnection> DbRepository { get; }

        // Cache

        ICache Cache { get; }

        // Trace

        ITrace Trace { get; }

        // Trace

        IStatementBuilder StatementBuilder { get; }

        // Query

        IEnumerable<TEntity> Query(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        IEnumerable<TEntity> Query(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        IEnumerable<TEntity> Query(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        IEnumerable<TEntity> Query(IQueryGroup where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        // QueryAsync

        Task<IEnumerable<TEntity>> QueryAsync(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        Task<IEnumerable<TEntity>> QueryAsync(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        Task<IEnumerable<TEntity>> QueryAsync(IQueryGroup where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        // Insert

        object Insert(TEntity entity, IDbTransaction transaction = null);

        // InsertAsync

        Task<object> InsertAsync(TEntity entity, IDbTransaction transaction = null);

        // Update

        int Update(TEntity entity, IDbTransaction transaction = null);

        int Update(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        int Update(TEntity entity, object where, IDbTransaction transaction = null);

        int Update(TEntity entity, IQueryGroup where, IDbTransaction transaction = null);

        // UpdateAsync

        Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null);

        Task<int> UpdateAsync(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        Task<int> UpdateAsync(TEntity entity, object where, IDbTransaction transaction = null);

        Task<int> UpdateAsync(TEntity entity, IQueryGroup where, IDbTransaction transaction = null);

        // Delete

        int Delete(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        int Delete(object where, IDbTransaction transaction = null);

        int Delete(IQueryGroup where, IDbTransaction transaction = null);

        // DeleteAsync

        Task<int> DeleteAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        Task<int> DeleteAsync(object where, IDbTransaction transaction = null);

        Task<int> DeleteAsync(IQueryGroup where, IDbTransaction transaction = null);

        // Merge

        int Merge(TEntity entity, IDbTransaction transaction = null);

        int Merge(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null);

        // MergeAsync

        Task<int> MergeAsync(TEntity entity, IDbTransaction transaction = null);

        Task<int> MergeAsync(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null);

        // BulkInsert

        int BulkInsert(IEnumerable<TEntity> entities, IDbTransaction transaction = null);

        // BulkInsertAsync

        Task<int> BulkInsertAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null);

        // ExecuteReader

        IEnumerable<TEntity> ExecuteReader(string commandText, object entity = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteReaderAsync

        Task<IEnumerable<TEntity>> ExecuteReaderAsync(string commandText, object entity = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteNonQuery

        int ExecuteNonQuery(string commandText, object entity = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteNonQueryAsync

        Task<int> ExecuteNonQueryAsync(string commandText, object entity = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteScalar

        object ExecuteScalar(string commandText, object entity = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteScalarAsync

        Task<object> ExecuteScalarAsync(string commandText, object entity = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);
    }

}