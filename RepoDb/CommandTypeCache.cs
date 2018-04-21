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

        internal static CommandType Get(Type type, Command command)
        {
            var value = CommandType.Text;
            if (_cache.ContainsKey(type))
            {
                value = _cache[type];
            }
            else
            {
                value = DataEntityExtension.GetCommandType(type, command);
                _cache.Add(type, value);
            }
            return value;
        }

        public static CommandType Get<TEntity>(Command command)
            where TEntity : IDataEntity
        {
            return Get(typeof(TEntity), command);
        }
    }
}
