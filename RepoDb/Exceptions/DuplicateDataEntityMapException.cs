using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Exceptions
{
    public class DuplicateDataEntityMapException<TEntity> : DataEntityMapException<TEntity>
        where TEntity : IDataEntity
    {
        public DuplicateDataEntityMapException(Command command)
            : base(command) { }
    }
}
