using RepoDb.Extensions;
using RepoDb.Reflection;
using System;
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
        private DbDataReader m_reader = null;
        private IDbConnection m_connection = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be extracted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        internal QueryMultipleExtractor(DbDataReader reader, IDbConnection connection)
        {
            m_reader = reader;
            m_connection = connection;
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

        #region Extract

        #region Extract<TEntity>

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
        /// <returns>An enumerable of extracted data entity.</returns>
        public IEnumerable<TEntity> Extract<TEntity>() where TEntity : class
        {
            var result = DataReader.ToEnumerable<TEntity>(m_reader, m_connection, false).AsList();

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
        public async Task<IEnumerable<TEntity>> ExtractAsync<TEntity>() where TEntity : class
        {
            var result = await DataReader.ToEnumerableAsync<TEntity>(m_reader, m_connection, false);

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
            var result = DataReader.ToEnumerable(m_reader).AsList();

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
            var result = await DataReader.ToEnumerableAsync(m_reader);

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
                value = ObjectConverter.ToType<TResult>(m_reader[0]);
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
                value = ObjectConverter.ToType<TResult>(m_reader[0]);
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
                value = ObjectConverter.DbNullToNull(m_reader.GetValue(0));
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
                value = ObjectConverter.DbNullToNull(m_reader.GetValue(0));
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
