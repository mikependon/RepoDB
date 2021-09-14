using Npgsql;

namespace RepoDb.Attributes.Parameter.Npgsql
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="NpgsqlParameter.ConvertedValue"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class ConvertedValueAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConvertedValueAttribute"/> class.
        /// </summary>
        /// <param name="convertedValue">The converted value.</param>
        public ConvertedValueAttribute(object convertedValue)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.ConvertedValue), convertedValue)
        { }

        /// <summary>
        /// Gets the mapped converted value of the parameter.
        /// </summary>
        public object ConvertedValue => Value;
    }
}