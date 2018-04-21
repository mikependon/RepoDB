using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Exceptions
{
    public class DataEntityMapException<TEntity> : Exception
        where TEntity : IDataEntity
    {
        public DataEntityMapException(Command command)
            : base($"{typeof(TEntity).FullName} ({command.ToString().ToUpper()})") { }
    }
}
