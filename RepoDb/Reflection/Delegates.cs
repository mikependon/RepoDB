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

    /// <summary>
    /// A delegate used to convert the <i>RepoDb.DataEntity</i> object into <i>System.Data.DataRow</i> object.
    /// </summary>
    /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> type to be converted.</typeparam>
    /// <param name="entity">The <i>RepoDb.DataEntity</i> object to be converted.</param>
    /// <param name="dataTable">The <i>System.Data.DataTable</i> object that will contain the converted row.</param>
    /// <returns>An instance of <i>System.Data.DataRow</i> containing the converted values.</returns>
    public delegate DataRow DataEntityToDataRowDelegate<TEntity>(TEntity entity, DataTable dataTable) where TEntity : DataEntity;
}
