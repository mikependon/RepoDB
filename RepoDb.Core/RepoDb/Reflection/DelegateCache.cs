using RepoDb.Reflection;
using RepoDb.Reflection.Delegates;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the dynamic delegate.
    /// </summary>
    public static class DelegateCache
    {
        /// <summary>
        /// Gets a delegate that is used to convert the <i>System.Data.Common.DbDataReader</i> object into <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> object to convert to.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An instance of <i>RepoDb.DataEntity</i> object.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>(DbDataReader reader)
            where TEntity : class
        {
            return DataReaderToDataEntityDelegateCache<TEntity>.Get(reader);
        }

        /// <summary>
        /// Gets a delegate that is used to convert the <i>System.Data.Common.DbDataReader</i> object into <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An instance of <i>RepoDb.DataEntity</i> object.</returns>
        public static DataReaderToExpandoObjectDelegate GetDataReaderToExpandoObjectDelegate(DbDataReader reader)
        {
            return DataReaderToExpandoObjectDelegateCache.Get(reader);
        }

        #region DataReaderToDataEntityDelegateCache

        private static class DataReaderToDataEntityDelegateCache<TEntity>
            where TEntity : class
        {
            private static DataReaderToDataEntityDelegate<TEntity> m_delegate;

            public static DataReaderToDataEntityDelegate<TEntity> Get(DbDataReader reader)
            {
                if (m_delegate == null)
                {
                    m_delegate = DelegateFactory.GetDataReaderToDataEntityDelegate<TEntity>(reader);
                }
                return m_delegate;
            }
        }

        #endregion

        #region DataReaderToExpandoObjectDelegateCache

        private static class DataReaderToExpandoObjectDelegateCache
        {
            private static DataReaderToExpandoObjectDelegate m_delegate;

            public static DataReaderToExpandoObjectDelegate Get(DbDataReader reader)
            {
                if (m_delegate == null)
                {
                    m_delegate = DelegateFactory.GetDataReaderToExpandoObjectDelegate(reader);
                }
                return m_delegate;
            }
        }

        #endregion
    }
}
