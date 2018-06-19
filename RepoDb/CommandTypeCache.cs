using System;
using System.Data;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using RepoDb.Enumerations;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached <i>System.Data.CommandType</i> object that is mapped on a given <i>Data Entity</i> object.
    /// </summary>
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

        /// <summary>
        /// Gets the <i>System.Data.CommandType</i> object that is mapped on a given <i>Data Entity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the <i>System.Data.Command</i> object will be retrieved. This object must 
        /// implement the <i>RepoDb.Interfaces.DataEntity</i> interface.
        /// </typeparam>
        /// <param name="command">The target command where to get the mapped name of the data entity.</param>
        /// <returns>An instance of <i>System.Data.CommandType</i> object.</returns>
        public static CommandType Get<TEntity>(Command command)
            where TEntity : DataEntity
        {
            return Get(typeof(TEntity), command);
        }
    }
}
