using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Collections.Concurrent;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the command type used for the entity.
    /// </summary>
    public static class CommandTypeCache
    {
        private static readonly ConcurrentDictionary<string, CommandType> m_cache = new ConcurrentDictionary<string, CommandType>();

        /// <summary>
        /// Gets the cached command type for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The cached command type of the entity.</returns>
        public static CommandType Get<TEntity>(Command command = Command.None)
            where TEntity : class
        {
            var type = typeof(TEntity);
            var key = $"{type.FullName}{command.ToString()}";
            var commandType = CommandType.Text;
            if (m_cache.TryGetValue(key, out commandType) == false)
            {
                commandType = DataEntityExtension.GetCommandType<TEntity>(command);
                m_cache.TryAdd(key, commandType);
            }
            return commandType;
        }
    }
}
