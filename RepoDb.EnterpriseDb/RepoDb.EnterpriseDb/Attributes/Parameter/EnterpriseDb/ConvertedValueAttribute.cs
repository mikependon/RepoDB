using System;
using EnterpriseDB.EDBClient;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter"/> for ConvertedValue
    /// property via an entity property before the actual execution.
    /// </summary>
    [Obsolete("Obsoleted by EnterpriseDB.EDBClient.")]
    public class ConvertedValueAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConvertedValueAttribute"/> class.
        /// </summary>
        /// <param name="convertedValue">The converted value.</param>
        public ConvertedValueAttribute(object convertedValue)
            : base(typeof(EDBParameter),  "ConvertedValue", convertedValue)
        { }

        /// <summary>
        /// Gets the mapped converted value of the parameter.
        /// </summary>
        public object ConvertedValue => Value;
    }
}
