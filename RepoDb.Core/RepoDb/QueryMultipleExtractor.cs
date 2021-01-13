using RepoDb.Extensions;
using RepoDb.Reflection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to extract the multiple resultsets of the 'ExecuteQueryMultiple' operation.
    /// </summary>
    public sealed class QueryMultipleExtractor : IDisposable
    {
        /*
         * TODO: The extraction within this class does not use the DbFieldCache.Get() operation, therefore,
         *       we are not passing the values to the DataReader.ToEnumerable() method.
         */

        private DbConnection connection = null;
        private DbDataReader reader = null;
        private bool isDisposeConnection = false;
        private object param = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
        /// </summary>
        /// <param name="connection">The instance of the <see cref="DbConnection"/> object that is current in used.</param>
        /// <param name="reader">The instance of the <see cref="DbDataReader"/> object to be extracted.</param>
        /// <param name="isDisposeConnection">The flag that is used to define whether the associated <paramref name="connection"/> object will be disposed during the disposition process.</param>
        /// <param name="param">The parameter in used during the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        internal QueryMultipleExtractor(DbConnection connection,
            DbDataReader reader,
            bool isDisposeConnection = false,
            object param = null,
            CancellationToken cancellationToken = default)
        {
            this.connection = connection;
            this.reader = reader;
            this.isDisposeConnection = isDisposeConnection;
            this.param = param;
            Position = 0;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Disposes the current instance of <see cref="QueryMultipleExtractor"/>.
        /// </summary>
        public void Dispose()
        {
            // Reader
            reader?.Dispose();

            // Connection
            if (isDisposeConnection == true)
            {
                connection?.Dispose();
            }

            // Set the output parameters
            DbConnectionExtension.SetOutputParameters(param);
        }

        #region Properties

        /// <summary>
        /// Gets the position of the <see cref="DbDataReader"/>.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the instance of the <see cref="CancellationToken"/> currently in used.
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        #endregion

        #region Extract

        #region Extract<TEntity>

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An enumerable of extracted data entity.</returns>
        public IEnumerable<TEntity> Extract<TEntity>(bool isMoveToNextResult = true)
            where TEntity : class
        {
            var result = DataReader.ToEnumerable<TEntity>(reader).AsList();
            if (isMoveToNextResult)
            {
                NextResult();
            }
            return result;
        }

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An enumerable of extracted data entity.</returns>
        public async Task<IEnumerable<TEntity>> ExtractAsync<TEntity>(bool isMoveToNextResult = true)
            where TEntity : class
        {
            var result = (await DataReader.ToEnumerableAsync<TEntity>(reader, cancellationToken: CancellationToken)).AsList();
            if (isMoveToNextResult)
            {
                await NextResultAsync();
            }
            return result;
        }

        #endregion

        #region Extract<dynamic>

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of dynamic objects.
        /// </summary>
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An enumerable of extracted data entity.</returns>
        public IEnumerable<dynamic> Extract(bool isMoveToNextResult = true)
        {
            var result = DataReader.ToEnumerable(reader).AsList();
            if (isMoveToNextResult)
            {
                NextResult();
            }
            return result;
        }

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of dynamic objects in an asynchronous way.
        /// </summary>
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An enumerable of extracted data entity.</returns>
        public async Task<IEnumerable<dynamic>> ExtractAsync(bool isMoveToNextResult = true)
        {
            var result = (await DataReader.ToEnumerableAsync(reader, cancellationToken: CancellationToken)).AsList();
            if (isMoveToNextResult)
            {
                await NextResultAsync();
            }
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
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An instance of extracted object as value result.</returns>
        public TResult Scalar<TResult>(bool isMoveToNextResult = true)
        {
            var value = default(TResult);
            if (reader.Read())
            {
                value = Converter.ToType<TResult>(reader[0]);
            }
            if (isMoveToNextResult)
            {
                NextResult();
            }
            return value;
        }

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An instance of extracted object as value result.</returns>
        public async Task<TResult> ScalarAsync<TResult>(bool isMoveToNextResult = true)
        {
            var value = default(TResult);
            if (await reader.ReadAsync(CancellationToken))
            {
                value = Converter.ToType<TResult>(reader[0]);
            }
            if (isMoveToNextResult)
            {
                await NextResultAsync();
            }
            return value;
        }

        #endregion

        #region Scalar<object>

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object.
        /// </summary>
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An instance of extracted object as value result.</returns>
        public object Scalar(bool isMoveToNextResult = true) =>
            Scalar<object>(isMoveToNextResult);

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object in an asynchronous way.
        /// </summary>
        /// <param name="isMoveToNextResult">A flag to use whether the operation would call the <see cref="System.Data.IDataReader.NextResult()"/> method.</param>
        /// <returns>An instance of extracted object as value result.</returns>
        public Task<object> ScalarAsync(bool isMoveToNextResult = true) =>
            ScalarAsync<object>(isMoveToNextResult);

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
            (Position = await reader.NextResultAsync(CancellationToken) ? Position + 1 : -1) >= 0;

        #endregion
    }
}
