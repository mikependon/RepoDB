using RepoDb.Enumerations;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the duplicate mapping for data entity is found.
    /// </summary>
    public class DuplicateDataEntityMapException : DataEntityMapException
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.DuplicateDataEntityMapException</i> object.
        /// </summary>
        /// <param name="command">The command of the data entity mapping.</param>
        public DuplicateDataEntityMapException(Command command)
            : base(command) { }
    }
}
