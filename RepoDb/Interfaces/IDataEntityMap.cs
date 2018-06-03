using System.Data;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// A an interface used implement to mark a class to be an object used to map a <i>RepoDb.Interfaces.IDataEntity</i> object into database object.
    /// </summary>
    public interface IDataEntityMap
    {
        /// <summary>
        /// Gets the name of database object being mapped.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of command used during execution.
        /// </summary>
        CommandType CommandType { get; }
    }
}