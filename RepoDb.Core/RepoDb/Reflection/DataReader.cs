using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to manipulate the <see cref="DbDataReader"/> object.
    /// </summary>
    public static class DataReader
    {
        #region ToEnumerable<TResult>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <returns>A list of the target result type.</returns>
        public static IEnumerable<TResult> ToEnumerable<TResult>(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null)
        {
            if (reader?.IsClosed == false && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToTypeCompiledFunction<TResult>(reader,
                    dbFields,
                    dbSetting);
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

        #endregion

        #region ToEnumerableAsync<TResult>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects in asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A list of the target result type.</returns>
        public static async IAsyncEnumerable<TResult> ToEnumerableAsync<TResult>(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (reader?.IsClosed != false || !reader.HasRows) yield break;
            
            var func = FunctionCache.GetDataReaderToTypeCompiledFunction<TResult>(reader,
                dbFields,
                dbSetting);
            
            while (await reader.ReadAsync(cancellationToken))
            {
                yield return func(reader);
            }
        }

        #endregion

        #region ToEnumerable<dynamic>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <returns>An array of dynamic objects.</returns>
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null)
        {
            if (reader?.IsClosed == false && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(reader,
                    dbFields,
                    dbSetting);
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

        #endregion

        #region ToEnumerableAsync<dynamic>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An array of dynamic objects.</returns>
        public static async IAsyncEnumerable<dynamic> ToEnumerableAsync(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (reader?.IsClosed != false || !reader.HasRows) yield break;
            
            var func = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(reader,
                dbFields,
                dbSetting);
            
            while (await reader.ReadAsync(cancellationToken))
            {
                yield return func(reader);
            }
        }

        #endregion
    }
}
