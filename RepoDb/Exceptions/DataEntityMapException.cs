using RepoDb.Enumerations;
using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown for any data entity mapping related exception.
    /// </summary>
    public class DataEntityMapException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.DataEntityMapException</i> object.
        /// </summary>
        /// <param name="command">The command of the data entity mapping.</param>
        public DataEntityMapException(Command command)
            : base(command.ToString().ToUpper()) { }
    }
}
