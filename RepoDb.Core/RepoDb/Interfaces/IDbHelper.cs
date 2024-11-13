#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class be a database helper object.
    /// </summary>
    public interface IDbHelper
    {
        /// <summary>
        /// Gets the type resolver used by this <see cref="IDbHelper"/> instance.
        /// </summary>
        IResolver<string, Type> DbTypeResolver { get; }

        #region GetFields

        /// <summary>
        /// Gets the list of <see cref="DbField"/> objects of the table.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        IEnumerable<DbField> GetFields(IDbConnection connection,
            string tableName,
            IDbTransaction? transaction = null);

        /// <summary>
        /// Gets the list of <see cref="DbField"/> objects of the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default);

        #endregion

        #region GetScopeIdentity

        /// <summary>
        /// Gets the newly generated identity from the database.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction? transaction = null);

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default);

        #endregion

        #region DynamicHandler

        /// <summary>
        /// A backdoor access from the core library used to handle an instance of an object to whatever purpose within the extended library.
        /// </summary>
        /// <typeparam name="TEventInstance">The type of the event instance to handle.</typeparam>
        /// <param name="instance">The instance of the event object to handle.</param>
        /// <param name="key">The key of the event to handle.</param>
        void DynamicHandler<TEventInstance>(TEventInstance instance,
            string key);

        #endregion
    }
}
