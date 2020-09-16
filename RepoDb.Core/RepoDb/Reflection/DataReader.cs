using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to manipulate the <see cref="DbDataReader"/> object.
    /// </summary>
    public static class DataReader
    {
        // TODO: Refactor the 'ToEnumerable' to only accepts the DbDataReader and IEnumerable<DbField>

        #region ToEnumerable<TResult>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> objects.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of the target result type.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        public static IEnumerable<TResult> ToEnumerable<TResult>(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null) =>
            ToEnumerableInternal<TResult>(reader, connection, null, transaction, true);

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The raw connection string.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>A list of the target result type.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        internal static IEnumerable<TResult> ToEnumerableInternal<TResult>(DbDataReader reader,
            IDbConnection connection = null,
            string connectionString = null,
            IDbTransaction transaction = null,
            bool enableValidation = true)
        {
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToTypeCompiledFunction<TResult>(reader,
                    connection,
                    connectionString,
                    transaction,
                    enableValidation);
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

        #endregion

        #region ToEnumerableAsync<TResult>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of the target result type.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        public static Task<IEnumerable<TResult>> ToEnumerableAsync<TResult>(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null) =>
            ToEnumerableInternalAsync<TResult>(reader, connection, null, transaction, true);

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The raw connection string.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>A list of the target result type.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        internal static async Task<IEnumerable<TResult>> ToEnumerableInternalAsync<TResult>(DbDataReader reader,
            IDbConnection connection = null,
            string connectionString = null,
            IDbTransaction transaction = null,
            bool enableValidation = true)
        {
            var list = new List<TResult>();
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var func = await FunctionCache.GetDataReaderToTypeCompiledFunctionAsync<TResult>(reader,
                    connection,
                    connectionString,
                    transaction,
                    enableValidation);
                while (await reader.ReadAsync())
                {
                    list.Add(func(reader));
                }
            }
            return list;
        }

        #endregion

        #region ToEnumerable<dynamic>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of dynamic objects.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null)
        {
            return ToEnumerable(reader, null, connection, transaction);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of dynamic objects.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        internal static IEnumerable<dynamic> ToEnumerable(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            if (reader != null && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(reader,
                    tableName,
                    connection,
                    transaction);
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

        #endregion

        #region ToEnumerableAsync<dynamic>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects in an asynchronous way.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of dynamic objects.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        public static Task<IEnumerable<dynamic>> ToEnumerableAsync(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null)
        {
            return ToEnumerableAsync(reader, null, connection, transaction);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of dynamic objects in an asynchronous way.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of dynamic objects.</returns>
        [Obsolete("This method is up for change and will soon to be refactored.")]
        internal static async Task<IEnumerable<dynamic>> ToEnumerableAsync(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            var list = new List<dynamic>();
            if (reader != null && reader.HasRows)
            {
                var func = await FunctionCache.GetDataReaderToExpandoObjectCompileFunctionAsync(reader,
                    tableName,
                    connection,
                    transaction);
                while (await reader.ReadAsync())
                {
                    list.Add(func(reader));
                }
            }
            return list;
        }

        #endregion
    }
}
