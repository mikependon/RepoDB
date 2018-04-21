using System;
using System.Data;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using RepoDb.Enumerations;

namespace RepoDb
{
    public static class CommandTypeCache
    {
        private static readonly IDictionary<Type, CommandType> _cache = new Dictionary<Type, CommandType>();

        public static CommandType Get<TEntity>(Command command)
            where TEntity : IDataEntity
        {
            var value = CommandType.Text;
            var key = typeof(TEntity);
            if (_cache.ContainsKey(key))
            {
                value = _cache[key];
            }
            else
            {
                value = DataEntityExtension.GetCommandType<TEntity>(command);
                _cache.Add(key, value);
            }
            return value;
        }
    }
}
