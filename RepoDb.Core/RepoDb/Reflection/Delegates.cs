using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace RepoDb.Reflection.Delegates
{
    /// <summary>
    /// A delegate used to map the <i>System.Data.Common.DbDataReader</i> into <i>RepoDb.DataEntity</i> object.
    /// </summary>
    /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> object to map.</typeparam>
    /// <param name="dataReader">An instance of <i>System.Data.Common.DbDataReader</i> to be mapped.</param>
    /// <returns>An instance of <i>RepoDb.DataEntity</i> object containing the values from the <i>System.Data.Common.DbDataReader</i> object.</returns>
    public delegate TEntity DataReaderToDataEntityDelegate<TEntity>(DbDataReader dataReader) where TEntity : DataEntity;

    /// <summary>
    /// A delegate used to map the <i>System.Data.Common.DbDataReader</i> into <i>System.Dynamic.ExpandoObject</i> object.
    /// </summary>
    /// <param name="dataReader">An instance of <i>System.Data.Common.DbDataReader</i> to be mapped.</param>
    /// <returns>An instance of <i>System.Dynamic.ExpandoObject</i> object containing the values from the <i>System.Data.Common.DbDataReader</i> object.</returns>
    public delegate ExpandoObject DataReaderToExpandoObjectDelegate(DbDataReader dataReader);
}
