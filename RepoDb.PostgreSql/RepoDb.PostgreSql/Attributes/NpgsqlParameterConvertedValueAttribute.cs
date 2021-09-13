using Npgsql;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="NpgsqlParameter.ConvertedValue"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class NpgsqlParameterConvertedValueAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlParameterConvertedValueAttribute"/> class.
        /// </summary>
        /// <param name="convertedValue">The converted value.</param>
        public NpgsqlParameterConvertedValueAttribute(object convertedValue)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.ConvertedValue), convertedValue)
        { }

        /// <summary>
        /// Gets the mapped converted value of the parameter.
        /// </summary>
        public object ConvertedValue => Value;
    }
}