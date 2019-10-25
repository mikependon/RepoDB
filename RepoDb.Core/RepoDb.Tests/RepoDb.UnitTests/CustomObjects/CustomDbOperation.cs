using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbOperation : IDbOperation
    {
        public int BulkInsert<TEntity>(IDbConnection connection, IEnumerable<TEntity> entities, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
        {
            return 1;
        }

        public int BulkInsert<TEntity>(IDbConnection connection, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
        {
            return 1;
        }

        public int BulkInsert(IDbConnection connection, string tableName, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
        {
            return 1;
        }

        public int BulkInsert<TEntity>(IDbConnection connection, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
        {
            return 1;
        }

        public int BulkInsert(IDbConnection connection, string tableName, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
        {
            return 1;
        }

        public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection, IEnumerable<TEntity> entities, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
        {
            return Task.FromResult(1);
        }

        public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
        {
            return Task.FromResult(1);
        }

        public Task<int> BulkInsertAsync(IDbConnection connection, string tableName, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
        {
            return Task.FromResult(1);
        }

        public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
        {
            return Task.FromResult(1);
        }

        public Task<int> BulkInsertAsync(IDbConnection connection, string tableName, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
        {
            return Task.FromResult(1);
        }
    }

}
