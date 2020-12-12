using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to handle the array value of the parameter.
    /// </summary>
    internal class CommandArrayParameter
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandArrayParameter"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        public CommandArrayParameter(string parameterName,
            IEnumerable<object> values)
        {
            ParameterName = parameterName;
            Values = values;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Gets the values of the parameter.
        /// </summary>
        public IEnumerable<object> Values { get; }
    }
}
