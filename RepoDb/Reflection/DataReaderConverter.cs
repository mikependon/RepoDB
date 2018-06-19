using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to convert the <i>System.Data.Common.DbDataReader</i> into <i>RepoDb.Interfaces.DataEntity</i> object.
    /// </summary>
    public static class DataReaderConverter
    {
        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into <i>RepoDb.Interfaces.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.Interfaces.DataEntity</i> type to convert.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An instance <i>RepoDb.Interfaces.DataEntity</i> object.</returns>
        public static TEntity AsEntity<TEntity>(DbDataReader reader)
            where TEntity : DataEntity
        {
            var @delegate = DelegateCache.GetDataReaderToDataEntityDelegate<TEntity>(reader);
            return @delegate(reader);
        }

        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into an enumerable of <i>RepoDb.Interfaces.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.Interfaces.DataEntity</i> type to convert.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An array of <i>RepoDb.Interfaces.DataEntity</i> objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader)
            where TEntity : DataEntity
        {
            var @delegate = DelegateCache.GetDataReaderToDataEntityDelegate<TEntity>(reader);
            var list = new List<TEntity>();
            while (reader.Read())
            {
                var entity = @delegate(reader);
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// Converts the <i>System.Data.Common.DbDataReader</i> into an enumerable of <i>System.Dynamic.ExpandoObject</i> object.
        /// </summary>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An array of <i>System.Dynamic.ExpandoObject</i> objects.</returns>
        public static IEnumerable<object> ToEnumerable(DbDataReader reader)
        {
            var @delegate = DelegateFactory.GetDataReaderToExpandoObjectDelegate(reader);
            var list = new List<object>();
            while (reader.Read())
            {
                var expandoObject = @delegate(reader);
                list.Add(expandoObject);
            }
            return list;
        }
    }
}
