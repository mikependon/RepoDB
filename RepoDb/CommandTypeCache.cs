using System;
using System.Data;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb
{
    public static class CommandTypeCache
    {
        private static readonly IDictionary<Type, CommandType> _commandTypes;

        static CommandTypeCache()
        {
            _commandTypes = new Dictionary<Type, CommandType>();
        }

        public static CommandType Get<TEntity>()
            where TEntity : IDataEntity
        {
            var commandType = CommandType.Text;
            var type = typeof(TEntity);
            if (_commandTypes.ContainsKey(type))
            {
                commandType = _commandTypes[type];
            }
            else
            {
                commandType = DataEntityExtension.GetCommandType<TEntity>();
                _commandTypes.Add(type, commandType);
            }
            return commandType;
        }
    }
}
