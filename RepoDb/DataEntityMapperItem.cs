using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// An object used by RepoDb.DataEntityMapper to map a RepoDb.Interfaces.IDataEntity object into database object.
    /// </summary>
    /// <typeparam name="TEntity">The type of RepoDb.Interfaces.IDataEntity object.</typeparam>
    public class DataEntityMapperItem<TEntity>
        where TEntity : IDataEntity
    {
        private readonly IDictionary<Command, IDataEntityMap> _cache;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Creates an instance of RepoDb.DataEntityMapperItem class.
        /// </summary>
        public DataEntityMapperItem()
        {
            _cache = new Dictionary<Command, IDataEntityMap>();
        }

        /// <summary>
        /// Set a mapping for the current defined entity.
        /// </summary>
        /// <param name="command">The type of command this mapping is used to.</param>
        /// <param name="name">The name of the object from the database.</param>
        /// <param name="commandType">The command type to be used during execution.</param>
        /// <returns></returns>
        public DataEntityMapperItem<TEntity> Set(Command command, string name, CommandType commandType)
        {
            var value = (IDataEntityMap)null;
            lock (_syncLock)
            {
                if (_cache.ContainsKey(command))
                {
                    throw new DuplicateDataEntityMapException<TEntity>(command);
                }
                else
                {
                    value = new DataEntityMap(name, commandType);
                    _cache.Add(command, value);
                }
            }
            return this;
        }
    }
}
