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
        private static readonly IDictionary<string, CommandType> _cache = new Dictionary<string, CommandType>();

        internal static CommandType Get(Type type, Command command)
        {
            var value = CommandType.Text;
            var key = $"{type.FullName}.{command.ToString()}".ToLower();
            if (_cache.ContainsKey(key))
            {
                value = _cache[key];
            }
            else
            {
                value = DataEntityExtension.GetCommandType(type, command);
                _cache.Add(key, value);
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
