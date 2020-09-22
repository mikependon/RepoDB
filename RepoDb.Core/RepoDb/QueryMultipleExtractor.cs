using RepoDb.Extensions;
using RepoDb.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to extract the multiple resultsets of the 'ExecuteQueryMultiple' operation.
    /// </summary>
    public sealed class QueryMultipleExtractor : IDisposable
    {
        /*
         * TODO: The extraction within this class does not use the DbFieldCache.Get() operation, therefore,
         *       we are not passing the values to the DataReader.ToEnumerable() method.
         */

        private DbDataReader reader = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be extracted.</param>
        internal QueryMultipleExtractor(DbDataReader reader)
        {
            this.reader = reader;
            Position = 0;
        }

        /// <summary>
        /// Disposes the current instance of <see cref="QueryMultipleExtractor"/>.
        /// </summary>
        public void Dispose() =>
            reader?.Dispose();

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
        public IEnumerable<TEntity> Extract<TEntity>()
            where TEntity : class
        {
            var result = DataReader.ToEnumerable<TEntity>(reader).AsList();
            NextResult();
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
            var result = DataReader.ToEnumerable<TEntity>(reader).AsList();
            await NextResultAsync();
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
            var result = DataReader.ToEnumerable(reader).AsList();
            NextResult();
            return result;
        }

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of dynamic objects in an asynchronous way.
        /// </summary>
        /// <returns>An enumerable of extracted data entity.</returns>
        public async Task<IEnumerable<dynamic>> ExtractAsync()
        {
            var result = DataReader.ToEnumerable(reader).AsList();
            await NextResultAsync();
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
            if (reader.Read())
            {
                value = Converter.ToType<TResult>(reader[0]);
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
            if (await reader.ReadAsync())
            {
                value = Converter.ToType<TResult>(reader[0]);
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
        public object Scalar() =>
            Scalar<object>();

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object in an asynchronous way.
        /// </summary>
        /// <returns>An instance of extracted object as value result.</returns>
        public Task<object> ScalarAsync() =>
            ScalarAsync<object>();

        #endregion

        #endregion

        #region NextResult

        /// <summary>
        /// Advances the <see cref="DbDataReader"/> object to the next result.
        /// <returns>True if there are more result sets; otherwise false.</returns>
        /// </summary>
        public bool NextResult() =>
            (Position = reader.NextResult() ? Position + 1 : -1) >= 0;

        /// <summary>
        /// Advances the <see cref="DbDataReader"/> object to the next result in an asynchronous way.
        /// <returns>True if there are more result sets; otherwise false.</returns>
        /// </summary>
        public async Task<bool> NextResultAsync() =>
            (Position = await reader.NextResultAsync() ? Position + 1 : -1) >= 0;

        #endregion
    }
}
