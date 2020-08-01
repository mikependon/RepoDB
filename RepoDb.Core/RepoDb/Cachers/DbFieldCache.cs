using RepoDb.Exceptions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to cache the list of <see cref="DbField"/> of the database table.
    /// </summary>
    public static class DbFieldCache
    {
        private static readonly ConcurrentDictionary<long, IEnumerable<DbField>> cache = new ConcurrentDictionary<long, IEnumerable<DbField>>();

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached enumerable of <see cref="DbField"/> objects.
        /// </summary>
        public static void Flush()
        {
            cache.Clear();
        }

        /// <summary>
        /// Throws an exception of any of the validation needed is failing.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="dbFields">The list of <see cref="DbField"/> objects to be validated.</param>
        private static void ValidateDbFields(string tableName,
            IEnumerable<DbField> dbFields)
        {
            if (dbFields?.Any() != true)
            {
                throw new MissingFieldsException($"There are no database fields found for table '{tableName}'. Make sure that the target table '{tableName}' is present in the database and/or atleast a single field is available.");
            }
        }

        #endregion

        #region Methods

        #region Sync

        /// <summary>
        /// Gets the cached list of <see cref="DbField"/> of the database table based on the data entity mapped name.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static IEnumerable<DbField> Get(IDbConnection connection,
            string tableName,
            IDbTransaction transaction) =>
            Get(connection, tableName, transaction, true);

        /// <summary>
        /// Gets the cached list of <see cref="DbField"/> of the database table based on the data entity mapped name.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static IEnumerable<DbField> Get(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            bool enableValidation) =>
            GetInternal(connection, tableName, transaction, enableValidation);

        /// <summary>
        /// Gets the cached field definitions of the entity.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        internal static IEnumerable<DbField> GetInternal<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            bool enableValidation)
            where TDbConnection : IDbConnection
        {
            var type = connection.GetType();
            var key = (long)type.FullName.GetHashCode();
            var result = (IEnumerable<DbField>)null;

            // Note: For SqlConnection, the ConnectionString is changing if the (Integrated Security=False). Actually for this isolation, the database name is enough.
            if (!string.IsNullOrEmpty(connection?.Database))
            {
                key += connection.Database.GetHashCode();
            }

            // Add the hashcode of the table name
            if (string.IsNullOrEmpty(tableName) == false)
            {
                key += tableName.GetHashCode();
            }

            // Try get the value
            if (cache.TryGetValue(key, out result) == false)
            {
                // Get from DB
                var dbHelper = DbHelperMapper.Get(type);
                result = dbHelper?.GetFields(connection, tableName, transaction);

                // Validate
                if (enableValidation)
                {
                    ValidateDbFields(tableName, result);
                }

                // Add to cache
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Async

        /// <summary>
        /// Gets the cached list of <see cref="DbField"/> of the database table based on the data entity mapped name in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static Task<IEnumerable<DbField>> GetAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction) =>
            GetAsync(connection, tableName, transaction, true);

        /// <summary>
        /// Gets the cached list of <see cref="DbField"/> of the database table based on the data entity mapped name in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static Task<IEnumerable<DbField>> GetAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            bool enableValidation) =>
            GetInternalAsync(connection, tableName, transaction, enableValidation);

        /// <summary>
        /// Gets the cached field definitions of the entity in an asychronous way.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        internal static async Task<IEnumerable<DbField>> GetInternalAsync<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            bool enableValidation)
            where TDbConnection : IDbConnection
        {
            var type = connection.GetType();
            var key = (long)type.FullName.GetHashCode();
            var result = (IEnumerable<DbField>)null;

            // Note: For SqlConnection, the ConnectionString is changing if the (Integrated Security=False). Actually for this isolation, the database name is enough.
            if (!string.IsNullOrEmpty(connection?.Database))
            {
                key += connection.Database.GetHashCode();
            }

            // Add the hashcode of the table name
            if (string.IsNullOrEmpty(tableName) == false)
            {
                key += tableName.GetHashCode();
            }

            // Try get the value
            if (cache.TryGetValue(key, out result) == false)
            {
                // Get from DB
                var dbHelper = DbHelperMapper.Get(type);
                result = await dbHelper?.GetFieldsAsync(connection, tableName, transaction);

                // Validate
                if (enableValidation)
                {
                    ValidateDbFields(tableName, result);
                }

                // Add to cache
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #endregion
    }
}
