using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to handle the command text of the array value of the parameter.
    /// </summary>
    internal class CommandArrayParametersText
    {
        /// <summary>
        /// Gets the actual command string to be executed (derived from array parameters).
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets the database type of the parameter.
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// Gets the list of the command array parameters.
        /// </summary>
        public IList<CommandArrayParameter> CommandArrayParameters { get; } = new List<CommandArrayParameter>();
    }
}
