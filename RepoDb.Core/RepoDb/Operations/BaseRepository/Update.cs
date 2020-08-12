using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region Update<TEntity>(TableName)

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(string tableName,
            TEntity entity,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(tableName: tableName,
                entity: entity,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(string tableName,
            TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(tableName: tableName,
                entity: entity,
                whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(string tableName,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(string tableName,
            TEntity entity,
            QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(string tableName,
            TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(string tableName,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region Update<TEntity>

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(TEntity entity,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(TEntity entity,
            QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public int Update(TEntity entity,
            QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region UpdateAsync<TEntity>(TableName)

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(string tableName,
            TEntity entity,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(tableName: tableName,
                entity: entity,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(string tableName,
            TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(tableName: tableName,
                entity: entity,
                whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(string tableName,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(string tableName,
            TEntity entity,
            QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(string tableName,
            TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(string tableName,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region UpdateAsync<TEntity>

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(TEntity entity,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(TEntity entity,
            QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public Task<int> UpdateAsync(TEntity entity,
            QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
