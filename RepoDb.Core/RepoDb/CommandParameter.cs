using System;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to hold the definition of the <see cref="DbCommand"/> parameters.
    /// </summary>
    internal class CommandParameter
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandParameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="mappedToType">The parent type where this parameter is mapped.</param>
        public CommandParameter(string name,
            object value,
            Type mappedToType)
        {
            Name = name;
            Value = value;
            MappedToType = mappedToType;
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The parent type where this parameter is mapped.
        /// </summary>
        public Type MappedToType { get; set; }
    }
}
