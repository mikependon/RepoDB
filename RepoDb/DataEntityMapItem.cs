using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// An object used by <i>RepoDb.DataEntityMapper</i> to map a <i>RepoDb.DataEntity</i> object into database object.
    /// </summary>
    public class DataEntityMapItem
    {
        private readonly IDictionary<Command, DataEntityMap> _cache;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Creates an instance of <i>RepoDb.DataEntityMapItem</i> class.
        /// </summary>
        public DataEntityMapItem()
        {
            _cache = new Dictionary<Command, DataEntityMap>();
        }

        /// <summary>
        /// Validates the entry of the mapping.
        /// </summary>
        /// <param name="command">The type of command this mapping is used to.</param>
        /// <param name="map">The mapping to be used before execution.</param>
        private void Validate(Command command, DataEntityMap map)
        {
            if (map == null)
            {
                throw new NullReferenceException("Map");
            }
            var error = false;
            switch (command)
            {
                case Command.InlineInsert:
                case Command.InlineMerge:
                case Command.InlineUpdate:
                    error = map.CommandType == CommandType.StoredProcedure;
                    break;
                case Command.BatchQuery:
                case Command.BulkInsert:
                case Command.Count:
                case Command.Delete:
                case Command.DeleteAll:
                case Command.Insert:
                case Command.Merge:
                case Command.Query:
                case Command.Truncate:
                case Command.Update:
                    error = false;
                    break;
            }
            if (error)
            {
                throw new InvalidOperationException($"The command type '{map.CommandType.ToString()}' is not yet supported for '{command.ToString()}'.");
            }
        }

        /// <summary>
        /// Set a mapping for the current defined entity.
        /// </summary>
        /// <param name="command">The type of command this mapping is used to.</param>
        /// <param name="name">The name of the object from the database.</param>
        /// <param name="commandType">The command type to be used during execution.</param>
        /// <returns>The current instance of <i>RepoDb.DataEntityMapItem</i> that holds the mapping.</returns>
        public DataEntityMapItem On(Command command, string name, CommandType commandType = CommandType.Text)
        {
            return Set(command, new DataEntityMap(name, commandType));
        }

        /// <summary>
        /// Set a mapping for the current defined entity.
        /// </summary>
        /// <param name="command">The type of command this mapping is used to.</param>
        /// <param name="map">The mapping to be used before execution.</param>
        /// <returns>The current instance of <i>RepoDb.DataEntityMapItem</i> that holds the mapping.</returns>
        public DataEntityMapItem Set(Command command, DataEntityMap map)
        {
            // Validate
            Validate(command, map);

            // Check and Add
            lock (_syncLock)
            {
                if (_cache.ContainsKey(command))
                {
                    throw new DuplicateDataEntityMapException(command);
                }
                else
                {
                    _cache.Add(command, map);
                }
            }

            // Return
            return this;
        }

        /// <summary>
        /// Gets the instance of <i>RepoDb.DataEntityMap</i> object based on the command mapping.
        /// </summary>
        /// <param name="command">The command specified on this mapping.</param>
        /// <returns>An instance of <i>RepoDb.DataEntityMap</i> that holds the mapping.</returns>
        public DataEntityMap Get(Command command)
        {
            var result = (DataEntityMap)null;
            if (_cache.ContainsKey(command))
            {
                result = _cache[command];
            }
            return result;
        }
    }
}
