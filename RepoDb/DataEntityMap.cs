using RepoDb.Interfaces;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// An object used to map a RepoDb.Interfaces.IDataEntity object into database object.
    /// </summary>
    public class DataEntityMap : IDataEntityMap
    {
        /// <summary>
        /// Creates an instance of RepoDb.ObjectMap class.
        /// </summary>
        /// <param name="name">The name of database object.</param>
        /// <param name="commandType">The type of command used during execution.</param>
        public DataEntityMap(string name, CommandType commandType)
        {
            Name = name;
            CommandType = commandType;
        }

        /// <summary>
        /// Gets the name of database object being mapped.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of command used during execution.
        /// </summary>
        public CommandType CommandType { get; }
    }
}
