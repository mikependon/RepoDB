using System.Data;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// A contracts object used to map a RepoDb.Interfaces.IDataEntity object into database object.
    /// </summary>
    public interface IDataEntityMap
    {
        /// <summary>
        /// Gets the name of database object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of command used during execution.
        /// </summary>
        CommandType CommandType { get; }
    }
}