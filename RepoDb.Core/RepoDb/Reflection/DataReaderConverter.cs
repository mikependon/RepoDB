using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to convert the <i>System.Data.Common.DbDataReader</i> into <i>RepoDb.DataEntity</i> object.
    /// </summary>
    public static class DataReaderConverter
    {
        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into an enumerable of <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> type to convert.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An array of <i>RepoDb.DataEntity</i> objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader)
            where TEntity : class
        {
            return ToEnumerable<TEntity>(reader, null);
        }

        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into an enumerable of <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> type to convert.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <param name="cacheKey">The customized cache key.</param>
        /// <returns>An array of <i>RepoDb.DataEntity</i> objects.</returns>
        internal static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader, string cacheKey)
            where TEntity : class
        {
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var @delegate = DelegateCache.GetDataReaderToDataEntityDelegate<TEntity>(reader, cacheKey);
                while (reader.Read())
                {
                    yield return @delegate(reader);
                }
            }
        }

        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into an enumerable of <i>System.Dynamic.ExpandoObject</i> object.
        /// </summary>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An array of <i>System.Dynamic.ExpandoObject</i> objects.</returns>
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader)
        {
            return ToEnumerable(reader, null);
        }

        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into an enumerable of <i>System.Dynamic.ExpandoObject</i> object.
        /// </summary>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <param name="cacheKey">The customized cache key.</param>
        /// <returns>An array of <i>System.Dynamic.ExpandoObject</i> objects.</returns>
        internal static IEnumerable<dynamic> ToEnumerable(DbDataReader reader, string cacheKey)
        {
            if (reader != null && reader.HasRows)
            {
                var @delegate = DelegateCache.GetDataReaderToExpandoObjectDelegate(reader);
                while (reader.Read())
                {
                    yield return @delegate(reader);
                }
            }
        }
    }
}
