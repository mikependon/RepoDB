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
        /// Converts the <i>System.Data.Common.DbDataReader</i> into <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> type to convert.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An instance <i>RepoDb.DataEntity</i> object.</returns>
        public static TEntity AsEntity<TEntity>(DbDataReader reader)
            where TEntity : DataEntity
        {
            if (reader == null)
            {
                return null;
            }
            var @delegate = DelegateFactory.GetDataReaderToDataEntityDelegate<TEntity>(reader);
            return @delegate(reader);
        }

        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into an enumerable of <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> type to convert.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An array of <i>RepoDb.DataEntity</i> objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader)
            where TEntity : DataEntity
        {
            if (reader != null && reader.HasRows)
            {
                var @delegate = DelegateFactory.GetDataReaderToDataEntityDelegate<TEntity>(reader);
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
            if (reader != null && reader.HasRows)
            {
                var @delegate = DelegateFactory.GetDataReaderToExpandoObjectDelegate(reader);
                while (reader.Read())
                {
                    yield return @delegate(reader);
                }
            }
        }
    }
}
