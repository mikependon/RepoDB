using RepoDb.Interfaces;
using System.Data.Common;

namespace RepoDb.Reflection.Delegates
{
    /// <summary>
    /// A delegate used to map the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity object.
    /// </summary>
    /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity object to map.</typeparam>
    /// <param name="dataReader">An instance of System.Data.Common.DbDataReader to be mapped.</param>
    /// <returns>An instance of RepoDb.Interfaces.IDataEntity object containing the values from the System.Data.Common.DbDataReader object.</returns>
    public delegate TEntity DataReaderToEntityMapperDelegate<TEntity>(DbDataReader dataReader) where TEntity : IDataEntity;
}
