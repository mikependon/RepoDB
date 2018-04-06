using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    public interface IDbRepository<TDbConnection>
        where TDbConnection : DbConnection
    {
        // CreateConnection

        TDbConnection CreateConnection();

        // Cache

        ICache Cache { get; }

        // Trace

        ITrace Trace { get; }

        // Query

        IEnumerable<TEntity> Query<TEntity>(IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        IEnumerable<TEntity> Query<TEntity>(object where, IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        IEnumerable<TEntity> Query<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        IEnumerable<TEntity> Query<TEntity>(IQueryGroup where, IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        // QueryAsync

        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(object where, IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null, string cacheKey = null)
            where TEntity : DataEntity;

        // Insert

        object Insert<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // InsertAsync

        Task<object> InsertAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // Update

        int Update<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        int Update<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        int Update<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        int Update<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // Update

        Task<int> UpdateAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        Task<int> UpdateAsync<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        Task<int> UpdateAsync<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // Delete

        int Delete<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        int Delete<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        int Delete<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // DeleteAsync

        Task<int> DeleteAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        Task<int> DeleteAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        Task<int> DeleteAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // Merge

        int Merge<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        int Merge<TEntity>(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // MergeAsync

        Task<int> MergeAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        Task<int> MergeAsync<TEntity>(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // BulkInsert

        int BulkInsert<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // BulkInsertAsync

        Task<int> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // ExecuteReader

        IEnumerable<TEntity> ExecuteReader<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // ExecuteReaderAsync

        Task<IEnumerable<TEntity>> ExecuteReaderAsync<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // ExecuteNonQuery

        int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteNonQueryAsync

        Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteScalar

        object ExecuteScalar(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteScalarAsync

        Task<object> ExecuteScalarAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);
    }
}