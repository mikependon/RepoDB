using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A mapper used to convert the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.
    /// </summary>
    public static class DataReaderConverter
    {
        /// <summary>
        /// Converts the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        /// <returns>An instance RepoDb.Interfaces.IDataEntity object.</returns>
        public static TEntity AsEntity<TEntity>(DbDataReader reader)
            where TEntity : IDataEntity
        {
            var @delegate = DelegateCache.GetDataReaderToDataEntityDelegate<TEntity>(reader);
            return @delegate(reader);
        }

        /// <summary>
        /// Converts the System.Data.Common.DbDataReader to an enumerable of RepoDb.Interfaces.IDataEntity.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        /// <returns>An array of RepoDb.Interfaces.IDataEntity objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader)
            where TEntity : IDataEntity
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
        /// Converts the System.Data.Common.DbDataReader to an enumerable of System.Dynamic.ExpandoObject.
        /// </summary>
        /// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        /// <returns>An array of System.Dynamic.ExpandoObject objects.</returns>
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
