using System;
using System.Data;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb
{
    public static class CommandTypeCache
    {
        private static readonly IDictionary<Type, CommandType> _cache;

        static CommandTypeCache()
        {
            _cache = new Dictionary<Type, CommandType>();
        }

        public static CommandType Get<TEntity>()
            where TEntity : IDataEntity
        {
            var commandType = CommandType.Text;
            var type = typeof(TEntity);
            if (_cache.ContainsKey(type))
            {
                commandType = _cache[type];
            }
            else
            {
                commandType = DataEntityExtension.GetCommandType<TEntity>();
                _cache.Add(type, commandType);
            }
            return commandType;
        }
    }
}
