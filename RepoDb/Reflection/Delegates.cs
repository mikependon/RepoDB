using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RepoDb.Reflection.Delegates
{
    /// <summary>
    /// A delegate used to map the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity object.
    /// </summary>
    /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity object to map.</typeparam>
    /// <param name="dataReader">An instance of System.Data.Common.DbDataReader to be mapped.</param>
    /// <returns>An instance of RepoDb.Interfaces.IDataEntity object containing the values from the System.Data.Common.DbDataReader object.</returns>
    public delegate TEntity DataReaderToDataEntityDelegate<TEntity>(DbDataReader dataReader) where TEntity : IDataEntity;

    /// <summary>
    /// A delegate used to convert the RepoDb.Interfaces.IDataEntity object into System.Data.DataRow object.
    /// </summary>
    /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to be converted.</typeparam>
    /// <param name="entity">The RepoDb.Interfaces.IDataEntity object to be converted.</param>
    /// <param name="dataTable">The System.Data.DataTable object that will contain the converted row.</param>
    /// <returns>An instance of System.Data.DataRow containing the converted values.</returns>
    public delegate DataRow DataEntityToDataRowDelegate<TEntity>(TEntity entity, DataTable dataTable) where TEntity : IDataEntity;
}
