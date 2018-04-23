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

        // Count

        int Count(IDbTransaction transaction = null);

        int Count(object where, IDbTransaction transaction = null);

        int Count(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        int Count(IQueryGroup where, IDbTransaction transaction = null);

        // CountAsync

        Task<int> CountAsync(IDbTransaction transaction = null);

        Task<int> CountAsync(object where, IDbTransaction transaction = null);

        Task<int> CountAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        Task<int> CountAsync(IQueryGroup where, IDbTransaction transaction = null);

        // CountBig

        long CountBig(IDbTransaction transaction = null);

        long CountBig(object where, IDbTransaction transaction = null);

        long CountBig(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        long CountBig(IQueryGroup where, IDbTransaction transaction = null);

        // CountBigAsync

        Task<long> CountBigAsync(IDbTransaction transaction = null);

        Task<long> CountBigAsync(object where, IDbTransaction transaction = null);

        Task<long> CountBigAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        Task<long> CountBigAsync(IQueryGroup where, IDbTransaction transaction = null);

        // BatchQuery

        IEnumerable<TEntity> BatchQuery(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        IEnumerable<TEntity> BatchQuery(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        IEnumerable<TEntity> BatchQuery(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        IEnumerable<TEntity> BatchQuery(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        // BatchQueryAsync

        Task<IEnumerable<TEntity>> BatchQueryAsync(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        Task<IEnumerable<TEntity>> BatchQueryAsync(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        Task<IEnumerable<TEntity>> BatchQueryAsync(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        Task<IEnumerable<TEntity>> BatchQueryAsync(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

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

        // InlineUpdate

        int InlineUpdate(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        int InlineUpdate(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        int InlineUpdate(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        // InlineUpdateAsync

        Task<int> InlineUpdateAsync(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        Task<int> InlineUpdateAsync(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        Task<int> InlineUpdateAsync(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null);

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

        // ExecuteQuery

        IEnumerable<TEntity> ExecuteQuery(string commandText, object entity = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteQueryAsync

        Task<IEnumerable<TEntity>> ExecuteQueryAsync(string commandText, object entity = null, CommandType? commandType = null,
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