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
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>An array of data entity objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader,
            IDbConnection connection = null)
            where TEntity : class
        {
            return ToEnumerable<TEntity>(reader, connection, connection?.ConnectionString, false);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The connection string that is currently in used.</param>
        /// <param name="basedOnFields">Check whether to create a delegate based on the data reader fields.</param>
        /// <returns>An array of data entity objects.</returns>
        internal static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            bool basedOnFields)
            where TEntity : class
        {
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToDataEntityFunction<TEntity>(reader,
                    connection,
                    connectionString,
                    basedOnFields);
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

        #endregion

        #region ToEnumerableAsync<TEntity>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>An array of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> ToEnumerableAsync<TEntity>(DbDataReader reader,
            IDbConnection connection = null)
            where TEntity : class
        {
            return ToEnumerableAsync<TEntity>(reader, connection, connection?.ConnectionString, false);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The connection string that is currently in used.</param>
        /// <param name="basedOnFields">Check whether to create a delegate based on the data reader fields.</param>
        /// <returns>An array of data entity objects.</returns>
        internal static async Task<IEnumerable<TEntity>> ToEnumerableAsync<TEntity>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            bool basedOnFields)
            where TEntity : class
        {
            var list = new List<TEntity>();
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToDataEntityFunction<TEntity>(reader,
                    connection,
                    connectionString,
                    basedOnFields);
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
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader,
            IDbConnection connection = null)
        {
            return ToEnumerable(reader, null, connection, connection?.ConnectionString);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The connection string that is currently in used.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        internal static IEnumerable<dynamic> ToEnumerable(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            string connectionString)
        {
            if (reader != null && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToExpandoObjectConverterFunction(reader,
                    tableName,
                    connection,
                    connectionString);
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

        #endregion

        #region ToEnumerableAsync<dynamic>

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> object in an asynchronous way.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        public static Task<IEnumerable<dynamic>> ToEnumerableAsync(DbDataReader reader,
            IDbConnection connection = null)
        {
            return ToEnumerableAsync(reader, null, connection, connection?.ConnectionString);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> object in an asynchronous way.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The connection string that is currently in used.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        internal static async Task<IEnumerable<dynamic>> ToEnumerableAsync(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            string connectionString)
        {
            var list = new List<dynamic>();
            if (reader != null && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToExpandoObjectConverterFunction(reader,
                    tableName,
                    connection,
                    connectionString);
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
