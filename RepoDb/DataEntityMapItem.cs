using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// An object used by RepoDb.DataEntityMapper to map a RepoDb.Interfaces.IDataEntity object into database object.
    /// </summary>
    public class DataEntityMapItem
    {
        private readonly IDictionary<Command, IDataEntityMap> _cache;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Creates an instance of RepoDb.DataEntityMapItem class.
        /// </summary>
        public DataEntityMapItem()
        {
            _cache = new Dictionary<Command, IDataEntityMap>();
        }

        /// <summary>
        /// Validates the entry of the mapping.
        /// </summary>
        /// <param name="command">The type of command this mapping is used to.</param>
        /// <param name="map">The mapping to be used before execution.</param>
        private void Validate(Command command, IDataEntityMap map)
        {
            if (map == null)
            {
                throw new NullReferenceException("Map");
            }
            var error = false;
            switch (command)
            {
                case Command.BatchQuery:
                case Command.Count:
                case Command.CountBig:
                case Command.InlineUpdate:
                case Command.Merge:
                    error = map.CommandType != CommandType.Text;
                    break;
                case Command.BulkInsert:
                    error = map.CommandType == CommandType.StoredProcedure;
                    break;
                case Command.Delete:
                case Command.Insert:
                case Command.Query:
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
        /// <returns>The current instance of RepoDb.DataEntityMapItem that holds the mapping.</returns>
        public DataEntityMapItem On(Command command, string name, CommandType commandType = CommandType.Text)
        {
            return Set(command, new DataEntityMap(name, commandType));
        }

        /// <summary>
        /// Set a mapping for the current defined entity.
        /// </summary>
        /// <param name="command">The type of command this mapping is used to.</param>
        /// <param name="map">The mapping to be used before execution.</param>
        /// <returns>The current instance of RepoDb.DataEntityMapItem that holds the mapping.</returns>
        public DataEntityMapItem Set(Command command, IDataEntityMap map)
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
        /// Gets the instance of RepoDb.DataEntityMap object based on the command mapping.
        /// </summary>
        /// <param name="command">The command specified on this mapping.</param>
        /// <returns>An instance of RepoDb.IDataEntityMap that holds the mapping.</returns>
        public IDataEntityMap Get(Command command)
        {
            var result = (IDataEntityMap)null;
            if (_cache.ContainsKey(command))
            {
                result = _cache[command];
            }
            return result;
        }
    }
}
