using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region DeleteAll<TEntity> (Delete<TEntity>)

        /// <summary>
        /// Delete the rows from the table. It uses the <see cref="DbConnectionExtension.Delete{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int DeleteAll(IEnumerable<TEntity> entities,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(entities: entities,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table. It uses the <see cref="DbConnectionExtension.Delete{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <param name="primaryKeys">The list of the primary keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int DeleteAll(IEnumerable<object> primaryKeys,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(primaryKeys: primaryKeys,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region DeleteAll<TEntity>

        /// <summary>
        /// Delete all the rows from the table.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int DeleteAll(string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(hints: hints,
                transaction: transaction);
        }

        #endregion

        #region DeleteAllAsync<TEntity> (DeleteAsync<TEntity>)

        /// <summary>
        /// Delete all the rows from the table in an asynchronous way. It uses the <see cref="DbConnectionExtension.Delete{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAllAsync(IEnumerable<TEntity> entities,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAllAsync<TEntity>(entities: entities,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table. It uses the <see cref="DbConnectionExtension.Delete{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <param name="primaryKeys">The list of the primary keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAllAsync(IEnumerable<object> primaryKeys,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAllAsync<TEntity>(primaryKeys: primaryKeys,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region DeleteAllAsync<TEntity>

        /// <summary>
        /// Delete all the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAllAsync(string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAllAsync<TEntity>();
        }

        #endregion
    }
}
