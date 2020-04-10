using RepoDb.Extensions;
using RepoDb.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class used to extract the multiple resultsets of the query operation.
    /// </summary>
    public sealed class QueryMultipleExtractor : IDisposable
    {
        private static readonly ConcurrentDictionary<int, IEnumerable<DbField>> m_cache = new ConcurrentDictionary<int, IEnumerable<DbField>>();
        private DbDataReader m_reader = null;
        private IDbConnection m_connection = null;
        private IDbTransaction m_transaction = null;
        private string m_connectionString = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be extracted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The used <see cref="IDbTransaction"/> object.</param>
        internal QueryMultipleExtractor(DbDataReader reader,
            IDbConnection connection,
            IDbTransaction transaction)
            : this(reader,
                  connection,
                  transaction,
                  connection.ConnectionString)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be extracted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The used <see cref="IDbTransaction"/> object.</param>
        /// <param name="connectionString">The unaltered connetion string used by the connection object.</param>
        internal QueryMultipleExtractor(DbDataReader reader,
            IDbConnection connection,
            IDbTransaction transaction,
            string connectionString)
        {
            m_reader = reader;
            m_connection = connection;
            m_transaction = transaction;
            m_connectionString = connectionString;
            Position = 0;
        }

        /// <summary>
        /// Disposes the current instance of <see cref="QueryMultipleExtractor"/>.
        /// </summary>
        public void Dispose()
        {
            m_reader?.Dispose();
        }

        #region Properties

        /// <summary>
        /// Gets the position of the <see cref="DbDataReader"/>.
        /// </summary>
        public int Position { get; private set; }

        #endregion

        #region Methods

        // TODO: Revisits whether "without" creating a new instance of connection object is possible
        //       This line of code is a dead-end, I could not even refactor due to the feature of this class (multiple extracting).
        //       Reason why we need to recreate a new connection object is to let us open a new DbDataReader directly to the database
        //       where the current connection is connected. The variable m_reader is an already opened DbDataReader object which
        //       prevents us of doing so.


        /// <summary>
        /// Returns the usable cache key when calling the <see cref="DbField"/> operations.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to check.</typeparam>
        /// <returns>The key to the cache.</returns>
        private int GetDbFieldGetCallsCacheKey<TEntity>()
        {
            var key = m_connection.GetType().FullName.GetHashCode();

            // Add the entity type hash code
            key += typeof(TEntity).FullName.GetHashCode();

            // Add the connection string hashcode
            if (!string.IsNullOrEmpty(m_connectionString))
            {
                key += m_connectionString.GetHashCode();
            }

            // Return the reusable key
            return key;
        }

        /// <summary>
        /// Ensures that the <see cref="DbFieldCache.Get(IDbConnection, string, IDbTransaction)"/> method is being called one.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to check.</typeparam>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        private void EnsureSingleCallForDbFieldCacheGet<TEntity>(IDbTransaction transaction)
            where TEntity : class
        {
            var key = GetDbFieldGetCallsCacheKey<TEntity>();
            var dbFields = (IEnumerable<DbField>)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out dbFields) == false)
            {
                using (var connection = (IDbConnection)Activator.CreateInstance(m_connection.GetType(), new object[] { m_connectionString }))
                {
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
                    m_cache.TryAdd(key, dbFields);
                }
            }
        }

        /// <summary>
        /// Ensures that the <see cref="DbFieldCache.GetAsync(IDbConnection, string, IDbTransaction)"/> method is being called one.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to check.</typeparam>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        private async Task EnsureSingleCallForDbFieldCacheGeAsync<TEntity>(IDbTransaction transaction)
            where TEntity : class
        {
            var key = GetDbFieldGetCallsCacheKey<TEntity>();
            var dbFields = (IEnumerable<DbField>)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out dbFields) == false)
            {
                using (var connection = (IDbConnection)Activator.CreateInstance(m_connection.GetType(), new object[] { m_connectionString }))
                {
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
                    m_cache.TryAdd(key, dbFields);
                }
            }
        }

        #endregion

        #region Extract

        #region Extract<TEntity>

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
        /// <returns>An enumerable of extracted data entity.</returns>
        public IEnumerable<TEntity> Extract<TEntity>()
            where TEntity : class
        {
            // Call the cache first to avoid reusing multiple data readers
            EnsureSingleCallForDbFieldCacheGet<TEntity>(m_transaction);

            // Get the result
            var result = DataReader.ToEnumerable<TEntity>(m_reader, m_connection, m_transaction).AsList();

            // Move to next result
            NextResult();

            // Return the result
            return result;
        }

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
        /// <returns>An enumerable of extracted data entity.</returns>
        public async Task<IEnumerable<TEntity>> ExtractAsync<TEntity>()
            where TEntity : class
        {
            // Call the cache first to avoid reusing multiple data readers
            await EnsureSingleCallForDbFieldCacheGeAsync<TEntity>(m_transaction);

            // Get the result
            var result = await DataReader.ToEnumerableAsync<TEntity>(m_reader, m_connection, m_transaction);

            // Move to next result
            await NextResultAsync();

            // Return the result
            return result;
        }

        #endregion

        #region Extract<dynamic>

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of dynamic objects.
        /// </summary>
        /// <returns>An enumerable of extracted data entity.</returns>
        public IEnumerable<dynamic> Extract()
        {
            var result = DataReader.ToEnumerable(m_reader, null, m_connection, m_transaction).AsList();

            // Move to next result
            NextResult();

            // Return the result
            return result;
        }

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of dynamic objects in an asynchronous way.
        /// </summary>
        /// <returns>An enumerable of extracted data entity.</returns>
        public async Task<IEnumerable<dynamic>> ExtractAsync()
        {
            var result = await DataReader.ToEnumerableAsync(m_reader, null, m_connection, m_transaction);

            // Move to next result
            await NextResultAsync();

            // Return the result
            return result;
        }

        #endregion

        #endregion

        #region Scalar

        #region Scalar<TResult>

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>An instance of extracted object as value result.</returns>
        public TResult Scalar<TResult>()
        {
            var value = default(TResult);

            // Only if there are record
            if (m_reader.Read())
            {
                // TODO: This can be compiled expression using the 'Get<Type>()' method
                value = Converter.ToType<TResult>(m_reader[0]);
            }

            // Move to next result
            NextResult();

            // Return the result
            return value;
        }

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>An instance of extracted object as value result.</returns>
        public async Task<TResult> ScalarAsync<TResult>()
        {
            var value = default(TResult);

            // Only if there are record
            if (await m_reader.ReadAsync())
            {
                // TODO: This can be compiled expression using the 'Get<Type>()' method
                value = Converter.ToType<TResult>(m_reader[0]);
            }

            // Move to next result
            await NextResultAsync();

            // Return the result
            return value;
        }

        #endregion

        #region Scalar<object>

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object.
        /// </summary>
        /// <returns>An instance of extracted object as value result.</returns>
        public object Scalar()
        {
            var value = (object)null;

            // Only if there are record
            if (m_reader.Read())
            {
                value = Converter.DbNullToNull(m_reader.GetValue(0));
            }

            // Move to next result
            NextResult();

            // Return the result
            return value;
        }

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object in an asynchronous way.
        /// </summary>
        /// <returns>An instance of extracted object as value result.</returns>
        public async Task<object> ScalarAsync()
        {
            var value = (object)null;

            // Only if there are record
            if (await m_reader.ReadAsync())
            {
                value = Converter.DbNullToNull(m_reader.GetValue(0));
            }

            // Move to next result
            await NextResultAsync();

            // Return the result
            return value;
        }

        #endregion

        #endregion

        #region NextResult

        /// <summary>
        /// Advances the <see cref="DbDataReader"/> object to the next result.
        /// <returns>True if there are more result sets; otherwise false.</returns>
        /// </summary>
        public bool NextResult()
        {
            return (Position = m_reader.NextResult() ? Position + 1 : -1) >= 0;
        }

        /// <summary>
        /// Advances the <see cref="DbDataReader"/> object to the next result in an asynchronous way.
        /// <returns>True if there are more result sets; otherwise false.</returns>
        /// </summary>
        public async Task<bool> NextResultAsync()
        {
            return (Position = await m_reader.NextResultAsync() ? Position + 1 : -1) >= 0;
        }

        #endregion
    }
}
