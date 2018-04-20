using RepoDb.Interfaces;
using System.Data.Common;

namespace RepoDb.Reflection
{
    public delegate TEntity DataReaderToEntityDelegate<TEntity>(DbDataReader dataReader) where TEntity : IDataEntity;
}
