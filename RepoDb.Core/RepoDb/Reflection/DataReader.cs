using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Threading.Tasks;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to manipulate the <see cref="DbDataReader"/> object.
    /// </summary>
    public static class DataReader
    {
        #region ToEnumerable<TEntity>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> objects.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of data entity objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            ToEnumerableInternal<TEntity>(reader, connection, null, transaction, true);

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The raw connection string.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>An array of data entity objects.</returns>
        internal static IEnumerable<TEntity> ToEnumerableInternal<TEntity>(DbDataReader reader,
            IDbConnection connection = null,
            string connectionString = null,
            IDbTransaction transaction = null,
            bool enableValidation = true)
            where TEntity : class
        {
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToDataEntityCompiledFunction<TEntity>(reader,
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

        #region ToEnumerableAsync<TEntity>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> ToEnumerableAsync<TEntity>(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            ToEnumerableInternalAsync<TEntity>(reader, connection, null, transaction, true);

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity objects in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The raw connection string.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>An array of data entity objects.</returns>
        internal static async Task<IEnumerable<TEntity>> ToEnumerableInternalAsync<TEntity>(DbDataReader reader,
            IDbConnection connection = null,
            string connectionString = null,
            IDbTransaction transaction = null,
            bool enableValidation = true)
            where TEntity : class
        {
            var list = new List<TEntity>();
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var func = await FunctionCache.GetDataReaderToDataEntityCompiledFunctionAsync<TEntity>(reader,
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
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null)
        {
            return ToEnumerable(reader, null, connection, transaction);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
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
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> objects in an asynchronous way.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        public static Task<IEnumerable<dynamic>> ToEnumerableAsync(DbDataReader reader,
            IDbConnection connection = null,
            IDbTransaction transaction = null)
        {
            return ToEnumerableAsync(reader, null, connection, transaction);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> objects in an asynchronous way.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
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
