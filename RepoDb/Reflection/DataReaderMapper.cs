using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A mapper used to convert the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.
    /// </summary>
    public static class DataReaderMapper
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
            var @delegate = DelegateCache.GetDataReaderToEntity<TEntity>();
            return @delegate(reader);
        }

        /// <summary>
        /// Converts the System.Data.Common.DbDataReader to an enumerable RepoDb.Interfaces.IDataEntity.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        /// <returns>An array of RepoDb.Interfaces.IDataEntity objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader)
            where TEntity : IDataEntity
        {
            var @delegate = DelegateCache.GetDataReaderToEntity<TEntity>();
            var list = new List<TEntity>();
            while (reader.Read())
            {
                list.Add(@delegate(reader));
            }
            return list;
        }
    }
}
